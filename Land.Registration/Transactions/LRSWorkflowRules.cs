/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Filing                                       Component : Filing Workflow                       *
*  Assembly : Empiria.Land.Registration.dll                Pattern   : Micro-workflow                        *
*  Type     : LRSWorkflowRules                             License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Micro-workflow rules set for the Land Registration System.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Contacts;

namespace Empiria.Land.Registration.Transactions {

  /// <summary>Micro-workflow rules set for the Land Registration System.</summary>
  static public class LRSWorkflowRules {

    #region Properties

    static public Contact InterestedContact {
      get {
        return Person.Parse(-6);
      }
    }

    #endregion Properties

    #region Methods

    static internal void AssertDigitalizedDocument(LRSTransaction transaction) {
      if (!IsDigitalizable(transaction)) {
        return;
      }

      if (transaction.LandRecord.IsEmptyInstance) {
        return;
      }

      if (transaction.LandRecord.ImagingControlID.Length != 0) {
        return;
      }

      if (ExecutionServer.CurrentUserId == 2) {
        return;
      }

      int graceDaysForImaging = 90;

      DateTime lastDate = transaction.LandRecord.AuthorizationTime;

      if (lastDate.AddDays(graceDaysForImaging) <= DateTime.Now) {
        Assertion.RequireFail("No es posible reingresar este trámite debido a que el documento " +
                              "que se registró aún no ha sido digitalizado y ya " +
                              $"transcurrieron más de {graceDaysForImaging} días desde que éste se cerró.\n\n" +
                              "Favor de preguntar en la mesa de armado acerca de este documento.");
      }
    }


    static internal void AssertGraceDaysForReentry(LRSTransaction transaction) {
      // int graceDaysForReentry = ConfigurationData.GetInteger("GraceDaysForReentry");

      if (ExecutionServer.CurrentUserId == 2) {
        return;
      }

      int graceDaysForReentry = 10;

      DateTime lastDate = transaction.PresentationTime;

      //if (transaction.LastReentryTime != ExecutionServer.DateMaxValue) {
      //  lastDate = transaction.LastReentryTime;
      //}
      if (!transaction.LandRecord.IsEmptyInstance) {
        lastDate = transaction.LandRecord.AuthorizationTime;
      }

      if (lastDate.AddDays(graceDaysForReentry) <= DateTime.Now) {
        Assertion.RequireFail("Por motivos de seguridad y calidad en el registro de la información, " +
                             $"no es posible reingresar trámites después de {graceDaysForReentry} días contados " +
                              "a partir de su fecha de presentación original, de su fecha de registro, o bien, " +
                              "de la fecha del último reingreso.\n\n" +
                              "En su lugar se debe optar por registrar un nuevo trámite.");
      }
    }


    static internal void AssertRecordingActsPrelation(LandRecord landRecord) {
      foreach (var recordingAct in landRecord.RecordingActs) {
        recordingAct.Validator.AssertIsLastInPrelationOrder();
      }
    }


    static internal bool CanBeDeleted(LRSTransaction transaction) {
      return (transaction.Workflow.CurrentStatus == TransactionStatus.Payment);
    }


    static internal void CanBeReceived(LRSTransaction transaction) {
      if (transaction.Workflow.CurrentStatus != TransactionStatus.Payment) {
        throw new LandRegistrationException(LandRegistrationException.Msg.CantReEntryTransaction,
                                            transaction.UID);
      }
      if (transaction.PaymentData.Payments.Count == 0 && !transaction.PaymentData.IsFeeWaiverApplicable
          && !IsEmptyServicesTransaction(transaction)) {
        throw new NotImplementedException("Este trámite todavía no tiene registrada una boleta de pago.");
      }
    }


    static internal TransactionStatus GetNextStatusAfterReceive(LRSTransaction transaction) {
      if (ExecutionServer.LicenseName == "Zacatecas") {
        return TransactionStatus.Control;
      }

      if (LRSWorkflowRules.IsRecorderOfficerCase(transaction)) {
        return TransactionStatus.Revision;

      } else if (LRSWorkflowRules.IsJuridicCase(transaction)) {
        return TransactionStatus.Juridic;

      } else if (LRSWorkflowRules.IsRecordingDocumentCase(transaction)) {
        return TransactionStatus.Recording;

      } else if (LRSWorkflowRules.IsCertificateIssueCase(transaction)) {
        return TransactionStatus.Elaboration;

      } else {
        return TransactionStatus.Control;
      }
    }


    static public bool IsArchivable(LRSTransaction transaction) {
      if (transaction.DocumentType.Id == 761) {
        return true;
      }
      return false;
    }


    static public bool IsCertificateIssueCase(LRSTransaction transaction) {
      return transaction.TransactionType.Id == 701 || transaction.TransactionType.Id == 704;
    }


    static public bool IsDigitalizable(LRSTransaction transaction) {
      return IsRecordingDocumentCase(transaction);
    }


    static public bool IsEmptyServicesTransaction(LRSTransaction transaction) {
      if (transaction.TransactionType.Id == 704 || transaction.TransactionType.Id == 705) {
        return true;
      }

      return false;
    }


    static public bool IsFinished(LRSTransaction transaction) {
      return (transaction.Workflow.CurrentStatus == TransactionStatus.Delivered ||
              transaction.Workflow.CurrentStatus == TransactionStatus.Returned ||
              transaction.Workflow.CurrentStatus == TransactionStatus.Archived);
    }


    static public bool IsForElaborationOnly(LRSTransaction transaction) {
      return transaction.TransactionType.Id == 701 || transaction.DocumentType.Id == 734;
    }


    static private bool IsJuridicCase(LRSTransaction transaction) {
      if (transaction.TransactionType.Id == 706 && transaction.DocumentType.Id == 734) {
        return true;
      }
      return false;
    }


    static public bool IsReadyForDeliveryOrReturn(LRSTransaction transaction) {
      return (transaction.Workflow.CurrentStatus == TransactionStatus.ToDeliver ||
              transaction.Workflow.CurrentStatus == TransactionStatus.ToReturn);
    }


    static public bool IsReadyForElectronicDelivery(LRSTransaction transaction, string messageUID) {
      if (String.IsNullOrWhiteSpace(messageUID)) {
        return false;
      }
      if (!IsReadyForDeliveryOrReturn(transaction)) {
        return false;
      }
      if (LRSWorkflowRules.IsDigitalizable(transaction)) {
        return false;
      }
      return false;
    }

    static public bool IsReadyForGenerateImagingControlID(LRSTransaction transaction) {
      if (transaction.IsEmptyInstance || transaction.LandRecord.IsEmptyInstance) {
        return false;
      }
      if (!ExecutionServer.CurrentPrincipal.IsInRole("Digitizer")) {
        return false;
      }
      if (transaction.LandRecord.IsEmptyInstance) {
        return false;
      }
      if (transaction.LandRecord.ImagingControlID.Length != 0) {
        return false;
      }
      if (transaction.LandRecord.RecordingActs.Count == 0) {
        return false;
      }
      if (!transaction.LandRecord.IsClosed) {
        return false;
      }
      if (!IsDigitalizable(transaction)) {
        return false;
      }
      if (transaction.Workflow.CurrentStatus == TransactionStatus.Digitalization ||
          transaction.Workflow.CurrentStatus == TransactionStatus.ToDeliver ||
          transaction.Workflow.CurrentStatus == TransactionStatus.Delivered) {
        return true;
      }

      return false;
    }


    static public bool IsReadyForReentry(LRSTransaction transaction) {
      var user = ExecutionServer.CurrentPrincipal;

      return ((transaction.Workflow.CurrentStatus == TransactionStatus.Returned ||
              (transaction.Workflow.CurrentStatus == TransactionStatus.Delivered ||
               transaction.Workflow.CurrentStatus == TransactionStatus.Archived)) &&
               user.IsInRole("Supervisor"));
    }


    static private bool IsRecorderOfficerCase(LRSTransaction transaction) {
      if (transaction.TransactionType.Id == 706 && transaction.RecorderOffice.Id == 147) {
        return true;
      }
      return false;
    }


    static public bool IsRecordingDocumentCase(LRSTransaction transaction) {
      if (EmpiriaMath.IsMemberOf(transaction.TransactionType.Id, new[] { 700, 704 , 705})) {
        return true;
      }

      return false;
    }


    static internal bool UserCanEditLandRecord(LandRecord landRecord) {
      if (!(ExecutionServer.CurrentPrincipal.IsInRole("LandRegistrar"))) {
        return false;
      }
      if (landRecord.IsHistoricRecord) {
        return true;
      }
      var transaction = landRecord.GetTransaction();

      Assertion.Require(!transaction.IsEmptyInstance,
                        "Transaction can't be the empty instance, because the document is not historic.");

      if (!(transaction.Workflow.CurrentStatus == TransactionStatus.Recording ||
            transaction.Workflow.CurrentStatus == TransactionStatus.Elaboration ||
            transaction.Workflow.CurrentStatus == TransactionStatus.Juridic)) {
        return false;
      }

      if (transaction.Workflow.GetCurrentTask().Responsible.Id == ExecutionServer.CurrentUserId) {
        return true;
      } else {
        return false;
      }
    }


    static public string ValidateStatusChange(LRSTransaction transaction, TransactionStatus nextStatus) {
      if (nextStatus == TransactionStatus.Received && transaction.PaymentData.Payments.Count == 0) {
        return "Este trámite todavía no tiene registrada una boleta de pago.";
      }

      if (!IsRecordingDocumentCase(transaction)) {
        return String.Empty;
      }

      if (transaction.TransactionType.Id == 704) {
        return String.Empty;
      }

      if (nextStatus == TransactionStatus.Revision || nextStatus == TransactionStatus.OnSign ||
          nextStatus == TransactionStatus.Archived || nextStatus == TransactionStatus.ToDeliver) {
        if (transaction.LandRecord.IsEmptyInstance) {
          return "Necesito primero se ingrese la información del documento a inscribir.";
        }
      }

      return String.Empty;
    }

    #endregion Methods

  }  // class LRSWorkflowRules

}  // namespace Empiria.Land.Registration.Transactions
