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

    static internal void AssertCanBeClosed(LRSTransaction transaction, TransactionStatus closeStatus) {
      var currentStatus = transaction.Workflow.CurrentStatus;

      Assertion.Require(currentStatus == TransactionStatus.ToDeliver || currentStatus == TransactionStatus.ToReturn,
                        $"El trámite {transaction.UID} no puede ser cerrado, " +
                        $"debido a que su estado actual es {currentStatus.GetStatusName()}.");

      if (closeStatus == TransactionStatus.ToDeliver) {
        AssertCanBeDelivered(transaction);
      } else if (closeStatus == TransactionStatus.ToReturn) {
        AssertCanBeReturned(transaction);
      } else {
        Assertion.RequireFail($"Invalid close status {closeStatus.GetStatusName()}.");
      }

    }


    static internal void AssertCanBeReceived(LRSTransaction transaction) {
      if (transaction.Workflow.CurrentStatus != TransactionStatus.Payment) {
        throw new LandRegistrationException(LandRegistrationException.Msg.CantReEntryTransaction,
                                            transaction.UID);
      }
      if (transaction.PaymentData.Payments.Count == 0 && !transaction.PaymentData.IsFeeWaiverApplicable
          && !IsEmptyServicesTransaction(transaction)) {
        throw new NotImplementedException("Este trámite todavía no tiene registrada una boleta de pago.");
      }
    }


    static internal void AssertIsReadyForReentry(LRSTransaction transaction) {
      var user = ExecutionServer.CurrentPrincipal;

      if (!user.IsInRole("Supervisor")) {
        throw new LandRegistrationException(LandRegistrationException.Msg.CantReEntryTransaction,
                                            transaction.UID);
      }

      if (!HasValidStatusForReentry(transaction)) {
        throw new LandRegistrationException(LandRegistrationException.Msg.CantReEntryTransaction,
                                            transaction.UID);
      }

      Assertion.Require(!transaction.LandRecord.IsRegisteredInRecordingBook,
            "No es posible reingresar este trámite debido a que fue inscrito en libros físicos, " +
            "y el sistema ya no permite hacer registros en libros.");

      AssertRecordingActsPrelation(transaction.LandRecord);
      AssertGraceDaysForReentry(transaction);
      AssertHasDigitalizedDocument(transaction);
    }


    static public void AssertValidStatusChange(LRSTransaction transaction, TransactionStatus nextStatus) {
      if (nextStatus == TransactionStatus.Received && transaction.PaymentData.Payments.Count == 0) {
        Assertion.RequireFail("Este trámite todavía no tiene registrada una boleta de pago. " +
                             $"Trámite: {transaction.UID} ({transaction.InternalControlNumber}");
      }

      // ToDo: Remove IsCertificateIssueCase condition when certificates have been implemented
      if (!IsRecordingDocumentCase(transaction) || IsCertificateIssueCase(transaction)) {
        return;
      }

      if (transaction.TransactionType.Id == 704) {
        return;
      }

      if (nextStatus == TransactionStatus.Revision || nextStatus == TransactionStatus.OnSign ||
          nextStatus == TransactionStatus.Archived || nextStatus == TransactionStatus.ToDeliver) {
        if (transaction.LandRecord.IsEmptyInstance || !transaction.LandRecord.IsClosed) {
          Assertion.RequireFail("Necesito se ingrese la información del documento a inscribir y " +
                                $"que éste se encuentre cerrado. " +
                                $"Trámite: {transaction.UID} ({transaction.InternalControlNumber}).");
        }
      }

      if (nextStatus == TransactionStatus.ToReturn && !transaction.LandRecord.IsEmptyInstance &&
          (transaction.LandRecord.IsClosed ||
           transaction.LandRecord.SecurityData.IsSigned ||
           transaction.LandRecord.RecordingActs.Count > 0)) {
        Assertion.RequireFail("No se puede devolver al interesado un trámite que tiene un documento " +
                              "cerrado, o que ya ha sido firmado, o cuyo sello registral tiene uno o " +
                              "más actos jurídicos registrados. " +
                             $"Trámite: {transaction.UID} ({transaction.InternalControlNumber}).");
      }
    }


    static internal bool CanBeDeleted(LRSTransaction transaction) {
      return (transaction.Workflow.CurrentStatus == TransactionStatus.Payment);
    }


    static internal bool CanEditLandRecord(LandRecord landRecord) {
      if (!(ExecutionServer.CurrentPrincipal.IsInRole("LandRegistrar"))) {
        return false;
      }
      if (landRecord.IsHistoricRecord) {
        return true;
      }
      var transaction = landRecord.Transaction;

      Assertion.Require(!transaction.IsEmptyInstance,
                        "Transaction can't be the empty instance, because the document is not historic.");

      if (transaction.Workflow.GetCurrentTask().Responsible.Id != ExecutionServer.CurrentUserId) {
        return false;
      }

      if (transaction.Workflow.CurrentStatus == TransactionStatus.Recording ||
          transaction.Workflow.CurrentStatus == TransactionStatus.Revision ||
          transaction.Workflow.CurrentStatus == TransactionStatus.Elaboration ||
          transaction.Workflow.CurrentStatus == TransactionStatus.Juridic) {
        return true;
      }

      return false;
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


    static public bool IsRecordingDocumentCase(LRSTransaction transaction) {
      if (EmpiriaMath.IsMemberOf(transaction.TransactionType.Id, new[] { 700, 704 , 705})) {
        return true;
      }

      return false;
    }


    #endregion Methods

    #region Helpers

    static private void AssertCanBeReturned(LRSTransaction transaction) {

      Assertion.Require(transaction.LandRecord.IsEmptyInstance ||
                        (!transaction.LandRecord.IsClosed && transaction.LandRecord.RecordingActs.Count == 0),
                        "No se puede devolver al interesado un trámite que tiene un documento " +
                        "cerrado, o que ya ha sido firmado, o cuyo sello registral tiene uno o " +
                        "más actos jurídicos registrados. " +
                        $"Trámite: {transaction.UID} ({transaction.InternalControlNumber}).");

      Assertion.Require(!transaction.HasCertificates,
                  "No se puede devolver al interesado un trámite que tiene uno o más certificados. " +
                  $"Trámite: {transaction.UID} ({transaction.InternalControlNumber}).");
    }



    static private void AssertCanBeDelivered(LRSTransaction transaction) {
      // ToDo: Remove these two ifs and check for one or more certificates when them have been implemented
      if (IsCertificateIssueCase(transaction)) {
        return;
      }
      if (!IsRecordingDocumentCase(transaction)) {
        return;
      }

      Assertion.Require(!transaction.LandRecord.IsEmptyInstance &&
                         transaction.LandRecord.IsClosed &&
                         transaction.LandRecord.SecurityData.IsSigned,
          "No se puede entregar al interesado un trámite que no tiene un documento cerrado y firmado. " +
          $"Trámite: {transaction.UID} ({transaction.InternalControlNumber}).");
    }


    static private void AssertGraceDaysForReentry(LRSTransaction transaction) {
      int graceDaysForReentry = ConfigurationData.Get<int>("GraceDaysForReentry", 365);

      DateTime lastDate = transaction.Workflow.GetCurrentTask().CheckOutTime;

      if (lastDate.AddDays(graceDaysForReentry) < DateTime.Today) {
        Assertion.RequireFail("Por motivos de seguridad y calidad en el registro de la información, " +
                             $"no es posible reingresar trámites después de {graceDaysForReentry} días contados " +
                              "a partir de su fecha de presentación original, de su fecha de registro, o bien, " +
                              "de la fecha del último reingreso.\n\n" +
                              "En su lugar se debe optar por registrar un nuevo trámite.");
      }
    }


    static private void AssertHasDigitalizedDocument(LRSTransaction transaction) {
      if (!IsDigitalizable(transaction)) {
        return;
      }

      if (transaction.LandRecord.IsEmptyInstance) {
        return;
      }

      if (transaction.LandRecord.ImagingControlID.Length != 0) {
        return;
      }

      int graceDaysForDigitalization = ConfigurationData.Get<int>("GraceDaysForDigitalization", int.MaxValue);

      DateTime lastDate = transaction.LandRecord.AuthorizationTime;

      if (lastDate.AddDays(graceDaysForDigitalization) <= DateTime.Now) {
        Assertion.RequireFail("No es posible reingresar este trámite debido a que el documento " +
                              "que se registró aún no ha sido digitalizado y ya " +
                              $"transcurrieron más de {graceDaysForDigitalization} días desde que éste se cerró.\n\n" +
                              "Favor de preguntar en la mesa de armado acerca de este documento.");
      }
    }


    static private void AssertRecordingActsPrelation(LandRecord landRecord) {
      if (landRecord.IsEmptyInstance) {
        return;
      }
      foreach (var recordingAct in landRecord.RecordingActs) {
        recordingAct.Validator.AssertIsLastInPrelationOrder();
      }
    }


    static private bool HasValidStatusForReentry(LRSTransaction transaction) {
      return (transaction.Workflow.CurrentStatus == TransactionStatus.Returned ||
              transaction.Workflow.CurrentStatus == TransactionStatus.Delivered ||
              transaction.Workflow.CurrentStatus == TransactionStatus.Archived);
    }


    static private bool IsRecorderOfficerCase(LRSTransaction transaction) {
      if (transaction.TransactionType.Id == 706 && transaction.RecorderOffice.Id == 147) {
        return true;
      }
      return false;
    }


    #endregion Helpers
  }  // class LRSWorkflowRules

}  // namespace Empiria.Land.Registration.Transactions
