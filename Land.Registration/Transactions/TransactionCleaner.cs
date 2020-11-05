/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Filing                                       Component : Filing Workflow                       *
*  Assembly : Empiria.Land.Registration.dll                Pattern   : Static methods library                *
*  Type     : TransactionCleaner                           License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Provides methods to clean out of date transactions.                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

using Empiria.Land.Data;

namespace Empiria.Land.Registration.Transactions {

  /// <summary>Provides methods to clean out of date transactions.</summary>
  public class TransactionCleaner {

    #region Fields

    static private readonly TransactionCleaner instance = new TransactionCleaner();  // singleton element

    #endregion Fields

    #region Constructors and parsers

    private TransactionCleaner() {
      // Singleton pattern needs private constructor
    }


    static public TransactionCleaner GetInstance() {
      return instance;
    }


    #endregion Constructors and parsers

    #region Public methods


    public string UID {
      get;
    } = Guid.NewGuid().ToString();


    public bool IsRunning {
      get;
      private set;
    } = false;


    public void Clean() {
      if (IsRunning) {
        return;
      }

      try {
        IsRunning = true;
        // CleanWorkflowTransactions();

        // CleanWorkflowTransactionsWithOpenDocument();

        //CleanUndelivered();

        SendTransactionsToESignSystem();

        AutoUpdateWorkflowForESignedTransactions();

      } finally {
        IsRunning = false;
      }
    }


    #endregion Public methods

    #region Private methods


    static private void AutoUpdateWorkflowForESignedTransactions() {
      FixedList<LRSTransaction> transactions = GetSignedTransactionsInOnSignStatus();
      var deliveryDesk = Contacts.Contact.Parse(98);
      var archive = Contacts.Contact.Parse(97);


      foreach (var transaction in transactions) {
        if ((!transaction.Document.IsEmptyInstance && transaction.Document.Security.Unsigned())
            || transaction.GetIssuedCertificates().Exists((x) => x.Unsigned())) {
          continue;
        }

        var currentTask = transaction.Workflow.GetCurrentTask();
        bool hasCertificates = transaction.GetIssuedCertificates().Count != 0;
        bool hasDocument = !transaction.Document.IsEmptyInstance;
        bool isDigitalizable = LRSWorkflowRules.IsDigitalizable(transaction.TransactionType, transaction.DocumentType);

        DateTime date = DocumentsData.GetLastSignTimeForAllTransactionDocuments(transaction);

        if (hasDocument && isDigitalizable) {
          var document = transaction.Document;

          if (document.Imaging.CanGenerateImagingControlID()) {
            document.Imaging.GenerateImagingControlID();
          }
        }

        if (transaction.Workflow.IsArchivable && !hasCertificates) {
          transaction.Workflow.SetNextStatus(LRSTransactionStatus.Archived, archive, String.Empty, date);

        } else if (!hasDocument && hasCertificates) {
          transaction.Workflow.SetNextStatus(LRSTransactionStatus.ToDeliver, currentTask.Responsible, String.Empty, date);
          transaction.Workflow.Take(String.Empty, deliveryDesk, date);

        } else if (hasDocument && !isDigitalizable) {
          transaction.Workflow.SetNextStatus(LRSTransactionStatus.ToDeliver, currentTask.Responsible, String.Empty, date);
          transaction.Workflow.Take(String.Empty, deliveryDesk, date);

        } else if (hasDocument && isDigitalizable && currentTask.Status == WorkflowTaskStatus.Pending) {
          transaction.Workflow.SetNextStatus(LRSTransactionStatus.ToDeliver, currentTask.Responsible, String.Empty, date);

        }

      }
    }


    static private void CleanTransaction(LRSTransaction transaction) {
      var currentTask = transaction.Workflow.GetCurrentTask();
      bool hasCertificates = transaction.GetIssuedCertificates().Count != 0;
      bool hasDocument = !transaction.Document.IsEmptyInstance;

      DateTime date = currentTask.CheckOutTime == ExecutionServer.DateMaxValue ? currentTask.CheckInTime : currentTask.CheckOutTime;

      if (hasDocument && LRSWorkflowRules.IsDigitalizable(transaction.TransactionType, transaction.DocumentType)) {
        var document = transaction.Document;

        if (document.Imaging.CanGenerateImagingControlID()) {
          document.Imaging.GenerateImagingControlID();
        }
      }

      if (currentTask.CurrentStatus == LRSTransactionStatus.ToDeliver ||
          currentTask.NextStatus == LRSTransactionStatus.ToDeliver ||
          currentTask.NextStatus == LRSTransactionStatus.Delivered) {
        transaction.Workflow.SetNextStatus(LRSTransactionStatus.Delivered, currentTask.Responsible,
                                          "Entregado por el sistema", date);

      } else if (currentTask.CurrentStatus == LRSTransactionStatus.ToReturn ||
                 currentTask.NextStatus == LRSTransactionStatus.ToReturn ||
                 currentTask.NextStatus == LRSTransactionStatus.Returned) {
        transaction.Workflow.SetNextStatus(LRSTransactionStatus.Returned, currentTask.Responsible,
                                          "Devuelto por el sistema", date);

      } else if (transaction.Workflow.IsArchivable && !hasCertificates) {
        transaction.Workflow.SetNextStatus(LRSTransactionStatus.Archived, currentTask.Responsible,
                                          "Archivado por el sistema", date);

      } else if (hasCertificates) {
        transaction.Workflow.SetNextStatus(LRSTransactionStatus.Delivered, currentTask.Responsible,
                                          "Entregado por el sistema", date.AddDays(1));

      } else if (!hasDocument && !hasCertificates &&
                 transaction.PresentationTime > DateTime.Parse("2015-01-01")) {
        transaction.Workflow.SetNextStatus(LRSTransactionStatus.Returned, currentTask.Responsible,
                                           "Devuelto por el sistema, sin documentos", date.AddDays(1));

      } else if (!hasDocument &&
                 transaction.PresentationTime < DateTime.Parse("2015-01-01")) {
        transaction.Workflow.SetNextStatus(LRSTransactionStatus.Delivered, currentTask.Responsible,
                                           "Entregado por el sistema, sin documentos", date.AddDays(1));


      } else {
        transaction.Workflow.SetNextStatus(LRSTransactionStatus.Delivered, currentTask.Responsible,
                                           "Entregado por el sistema", date.AddDays(1));

      }
    }


    static private void CleanUndelivered() {
      FixedList<LRSTransaction> transactions = GetUndeliveredTransactions();

      foreach (var transaction in transactions) {
        if ((!transaction.Document.IsEmptyInstance && !transaction.Document.IsClosed) ||
            transaction.GetIssuedCertificates().Exists((x) => !x.IsClosed)) {
          continue;
        }

        if ((transaction.Document.Security.UseESign && transaction.Document.Security.Unsigned()) ||
            transaction.GetIssuedCertificates().Exists((x) => x.UseESign && x.Unsigned())) {
          continue;
        }

        var currentTask = transaction.Workflow.GetCurrentTask();
        DateTime date = currentTask.CheckOutTime == ExecutionServer.DateMaxValue ? currentTask.CheckInTime : currentTask.CheckOutTime;

        if (currentTask.CurrentStatus == LRSTransactionStatus.ToDeliver) {
          transaction.Workflow.SetNextStatus(LRSTransactionStatus.Delivered, currentTask.Responsible,
                                            "Entregado por el sistema", date);

        } else if (currentTask.CurrentStatus == LRSTransactionStatus.ToReturn) {
          transaction.Workflow.SetNextStatus(LRSTransactionStatus.Returned, currentTask.Responsible,
                                            "Devuelto por el sistema", date);
        }

      } // foreach
    }


    static private void CleanWorkflowTransactions() {
      FixedList<LRSTransaction> transactions = GetTransactionsToBeCleaned();

      foreach (var transaction in transactions) {
        if ((!transaction.Document.IsEmptyInstance && !transaction.Document.IsClosed) ||
            transaction.GetIssuedCertificates().Exists((x) => !x.IsClosed)) {
          continue;
        }
        if (transaction.Document.Security.UseESign ||
            transaction.GetIssuedCertificates().Exists((x) => x.UseESign)) {
          continue;
        }

        CleanTransaction(transaction);
      } // foreach

    }


    static private void CleanWorkflowTransactionsWithOpenDocument() {
      FixedList<LRSTransaction> transactions = GetTransactionsToBeCleaned();

      foreach (var transaction in transactions) {
        var document = transaction.Document;

        if (document.IsEmptyInstance || document.IsClosed || document.Security.UseESign
            || transaction.PresentationTime > DateTime.Parse("2018-01-01")) {
          continue;
        }

        var currentTask = transaction.Workflow.GetCurrentTask();
        DateTime date = currentTask.CheckOutTime == ExecutionServer.DateMaxValue ? currentTask.CheckInTime : currentTask.CheckOutTime;

        transaction.Workflow.SetNextStatus(LRSTransactionStatus.Archived, currentTask.Responsible,
                                           "Archivado por el sistema, con documento abierto", date.AddDays(1));
      }

    }


    static private FixedList<LRSTransaction> GetSignedTransactionsInOnSignStatus() {
      var sql = "SELECT * FROM vwLRSSignedTransactionsInOnSignStatus " +
                "ORDER BY PresentationTime";

      var operation = DataOperation.Parse(sql);

      return DataReader.GetFixedList<LRSTransaction>(operation);
    }


    static private FixedList<LRSTransaction> GetTransactionsToBeCleaned() {
      var sql = "SELECT TOP 30000 * FROM vwLRSTransactionsToBeCleaned " +
                "ORDER BY PresentationTime";

      var operation = DataOperation.Parse(sql);

      return DataReader.GetFixedList<LRSTransaction>(operation);
    }


    static private FixedList<LRSTransaction> GetUndeliveredTransactions() {
      var sql = "SELECT * FROM vwLRSTransactionsToBeDelivered " +
                "ORDER BY PresentationTime";

      var operation = DataOperation.Parse(sql);

      return DataReader.GetFixedList<LRSTransaction>(operation);
    }


    static private void SendTransactionsToESignSystem() {
      var op = DataOperation.Parse("doLRSUpdateESignJobs");

      DataWriter.Execute(op);

      //var sql = "SELECT * FROM vwLRSTransactionsToBeSigned " +
      //          "ORDER BY PresentationTime";

      //var operation = DataOperation.Parse(sql);

      //FixedList<LRSTransaction> transactions = DataReader.GetFixedList<LRSTransaction>(operation);

      //var eSignService = new ESignService();

      //foreach (var transaction in transactions) {
      //  if ((!transaction.Document.IsEmptyInstance && !transaction.Document.IsClosed) ||
      //      transaction.GetIssuedCertificates().Exists(x => !x.IsClosed)) {
      //    continue;
      //  }

      //  SignRequestsDTO[] signRequests = BuildSignRequestsForTransaction(transaction);

      //  eSignService.RequestSign(signRequests);

      //}  // foreach
    }

    #endregion Private methods

  } // class TransactionCleaner

} // namespace Empiria.Land.Data
