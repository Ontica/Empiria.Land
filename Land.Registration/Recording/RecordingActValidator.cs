/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Registration                          Component : Domain Layer                            *
*  Assembly : Empiria.Land.Registration.dll              Pattern   : Service provider                        *
*  Type     : RecordingActValidator                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides validation services for recording acts.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Provides validation services for recording acts.</summary>
  internal class RecordingActValidator {

    #region Fields

    private readonly RecordingAct _recordingAct;

    #endregion Fields

    #region Constructors and parsers

    internal RecordingActValidator(RecordingAct recordingAct) {
      Assertion.Require(recordingAct, nameof(recordingAct));

      _recordingAct= recordingAct;
    }

    #endregion Constructors and parsers

    #region Methods

    public void AssertCanBeClosed() {
      var rule = _recordingAct.RecordingActType.RecordingRule;

      if (!_recordingAct.Resource.IsEmptyInstance) {
        _recordingAct.RecordingActType.AssertIsApplicableResource(_recordingAct.Resource);
      } else {
        Assertion.Require(rule.AppliesTo == RecordingRuleApplication.NoProperty,
                         "El acto jurídico " + _recordingAct.IndexedName +
                         " sólo puede aplicarse al folio real de un predio o asociación.");
      }

      if (!_recordingAct.BookEntry.IsEmptyInstance) {
        _recordingAct.BookEntry.AssertCanBeClosed();
      }

      _recordingAct.Resource.AssertIsStillAlive(_recordingAct.LandRecord.PresentationTime);

      this.AssertIsLastInPrelationOrder();

      this.AssertNoTrappedActs();

      this.AssertChainedRecordingAct();

      if (!_recordingAct.RecordingActType.RecordingRule.AllowUncompletedResource) {
        _recordingAct.Resource.AssertCanBeClosed();
      }

      // TODO: Validate recording act fields are completed

      this.AssertParties();
    }


    internal void AssertCanBeDeleted() {
      if (_recordingAct.BookEntry.Status == RecordableObjectStatus.Closed) {
        throw new LandRegistrationException(
                      LandRegistrationException.Msg.CantAlterRecordingActOnClosedRecording, _recordingAct.Id);
      }
      if (_recordingAct.Status == RecordableObjectStatus.Closed) {
        throw new LandRegistrationException(
                      LandRegistrationException.Msg.CantAlterClosedRecordingAct, _recordingAct.Id);
      }
    }


    internal void AssertCanBeOpened() {
      this.AssertIsLastInPrelationOrder();
      this.AssertDoesntHasEmittedCertificates();
    }


    internal void AssertChainedRecordingAct() {
      if (MeetsOperationalCondition()) {
        return;
      }

      var chainedRecordingActType = _recordingAct.RecordingActType.RecordingRule.ChainedRecordingActType;

      if (chainedRecordingActType.Equals(RecordingActType.Empty)) {
        return;
      }

      // Lookup the last chained act
      var lastChainedActInstance = _recordingAct.Resource.Tract.TryGetLastActiveChainedAct(chainedRecordingActType,
                                                                                           _recordingAct.LandRecord);
      // If exists an active chained act, then the assertion meets
      if (lastChainedActInstance != null) {
        return;
      }

      // Try to assert that the act is in the very first recorded document
      var tract = _recordingAct.Resource.Tract.GetClosedRecordingActsUntil(_recordingAct.LandRecord, true);

      // First check no real estates
      if (!(_recordingAct.Resource is RealEstate) &&
          (tract.Count == 0 || tract[0].LandRecord.Equals(_recordingAct.LandRecord))) {
        return;
      }

      // For real estates, this rule apply for new no-partitions
      if (_recordingAct.Resource is RealEstate && !((RealEstate) _recordingAct.Resource).IsPartition &&
          (tract.Count == 0 || tract[0].LandRecord.Equals(_recordingAct.LandRecord))) {
        return;
      }

      // When the chained act rule applies to a modification act, then lookup in this
      // recorded document for an act applied to a partition of this real estate
      // with the same ChainedRecordingActType, if it is found then the assertion meets.
      // Ejemplo: Tanto CV como Rectificación de medidas requieren aviso preventivo.
      // Si en el documento hay una CV sobre una fracción F de P, y también hay una
      // rectificación de medidas del predio P, entonces basta con que el aviso preventivo
      // exista para la fraccion F de P.
      if (_recordingAct.Resource is RealEstate && _recordingAct.RecordingActType.IsModificationActType) {
        foreach (RecordingAct recordingAct in _recordingAct.LandRecord.RecordingActs) {
          if (recordingAct.Equals(this)) {
            break;
          }
          if (recordingAct.Resource is RealEstate &&
              ((RealEstate) recordingAct.Resource).IsPartitionOf.Equals(_recordingAct.Resource) &&
              recordingAct.RecordingActType.RecordingRule.ChainedRecordingActType.Equals(chainedRecordingActType)) {
            recordingAct.Validator.AssertChainedRecordingAct();
            return;
          }
        }
      }

      Assertion.RequireFail("El acto jurídico " + _recordingAct.IndexedName +
                            " no pude ser inscrito debido a que el folio real no tiene registrado " +
                            "un acto de: '" + chainedRecordingActType.DisplayName + "'.\n\n" +
                            "Por lo anterior, esta operación no puede ser ejecutada.\n\n" +
                            "Favor de revisar la historia del predio involucrado. Es posible que el trámite donde " +
                            "viene el acto faltante aún no haya sido procesado o que el documento esté abierto.");
    }


    public void AssertIsLastInPrelationOrder() {

      // ToDo: Review this rule (seems like an operation issue)

      // Cancelation acts don't follow prelation rules
      // if (this.RecordingActType.IsCancelationActType) {
      //  return;
      // }

      var fullTract = _recordingAct.Resource.Tract.GetFullRecordingActs();

      fullTract = fullTract.FindAll((x) => !x.RecordingActType.RecordingRule.SkipPrelation);

      bool wrongPrelation = fullTract.Contains((x) => x.LandRecord.PresentationTime > _recordingAct.LandRecord.PresentationTime &&
                                                      x.LandRecord.IsClosed);

      //if (wrongPrelation) {
      //  Assertion.AssertFail("El acto jurídico " + this.IndexedName +
      //                       " hace referencia a un folio real que tiene registrado " +
      //                       "cuando menos otro acto jurídico con una prelación posterior " +
      //                       "a la de este documento.\n\n" +
      //                       "Por lo anterior, esta operación no puede ser ejecutada.\n\n" +
      //                       "Favor de revisar la historia del predio involucrado.");
      //}

    }


    internal bool IsAppliedOverNewPartition() {
      if (!(_recordingAct.Resource is RealEstate)) {
        return false;
      }

      RealEstate realEstate = (RealEstate) _recordingAct.Resource;

      return _recordingAct.ResourceRole.IsCreationalRole() &&
             !realEstate.IsPartitionOf.IsEmptyInstance &&
             realEstate.IsInTheRankOfTheFirstDomainAct(_recordingAct);
    }


    internal bool IsCompleted() {
      return (_recordingAct.Status == RecordableObjectStatus.Registered ||
              _recordingAct.Status == RecordableObjectStatus.Closed ||
              _recordingAct.Validator.HasCompleteInformation());
    }


    internal bool IsEditable() {
      return (_recordingAct.Status != RecordableObjectStatus.Registered &&
              _recordingAct.Status != RecordableObjectStatus.Closed &&
              _recordingAct.LandRecord.Status != RecordableObjectStatus.Closed);
    }


    public bool WasAliveOn(DateTime onDate) {
      if (this.WasCanceledOn(onDate)) {
        return false;
      }

      var autoCancelDays = _recordingAct.RecordingActType.RecordingRule.AutoCancel;
      if (autoCancelDays == 0) {
        return true;
      }
      return _recordingAct.LandRecord.PresentationTime.Date.AddDays(autoCancelDays) >= onDate.Date;
    }


    internal bool WasCanceledOn(DateTime onDate) {
      if (!_recordingAct.AmendedBy.IsEmptyInstance && _recordingAct.AmendedBy.RecordingActType.IsCancelationActType &&
           _recordingAct.AmendedBy.LandRecord.PresentationTime > onDate) {
        return true;
      }
      return false;
    }

    #endregion Methods

    #region Helpers

    private void AssertDoesntHasEmittedCertificates() {
      var certificates = _recordingAct.Resource.Tract.GetEmittedCerificates();

      bool wrongPrelation = certificates.Contains((x) => x.IsClosed && x.IssueTime > _recordingAct.LandRecord.AuthorizationTime &&
                                                         !x.Transaction.Equals(_recordingAct.LandRecord.Transaction));

      if (wrongPrelation) {
        Assertion.RequireFail("El acto jurídico " + _recordingAct.IndexedName +
                             " hace referencia a un folio real al cual se le " +
                             "emitió un certificado con fecha posterior " +
                             "a la fecha de autorización de este documento.\n\n" +
                             "Por lo anterior, esta operación no puede ser ejecutada.\n\n" +
                             "Favor de revisar la historia del predio involucrado.");
      }
    }


    private void AssertNoTrappedActs() {
      var tract = _recordingAct.Resource.Tract.GetRecordingActs();

      var trappedAct = tract.Find((x) => x.LandRecord.PresentationTime < _recordingAct.LandRecord.PresentationTime &&
                                  !x.LandRecord.IsClosed && !x.LandRecord.IsHistoricRecord);

      //if (trappedAct != null) {
      //  Assertion.AssertFail("Este documento no puede ser cerrado, ya que el acto jurídico\n" +
      //                       "{0} hace referencia al folio real '{1}' que tiene registrado " +
      //                       "movimientos en otro documento que está abierto y que tiene una prelación " +
      //                       "anterior al de este.\n\n" +
      //                       "Primero debe cerrarse dicho documento para evitar que sus actos " +
      //                       "queden atrapados en el orden de prelación y luego no pueda cerrarse.\n\n" +
      //                       "El documento en cuestión es el: {2}\n",
      //                       this.IndexedName, this.Resource.UID, trappedAct.Document.UID);
      //}
    }


    private void AssertParties() {
      var rule = _recordingAct.RecordingActType.RecordingRule;
      var parties = _recordingAct.Parties.List;
      var roles = _recordingAct.RecordingActType.GetPrimaryRoles();

      if (roles.Count == 0 || rule.AllowNoParties || roles.Count == 0) {
        return;
      }

      if (_recordingAct.IsChild) {
        return;
      }

      Assertion.Require(parties.Count != 0, "El acto jurídico " + _recordingAct.IndexedName +
                                           " requiere cuando menos una persona o propietario.");
      foreach (var role in roles) {
        var found = parties.Contains((x) => x.PartyRole.Equals(role));
        if (found) {
          return;
        }
      }
      Assertion.RequireFail("En el acto jurídico " + _recordingAct.IndexedName +
                           " no hay registradas personas o propietarios jugando alguno de" +
                           " los roles obligatorios para dicho tipo de acto.");
    }



    private bool HasCompleteInformation() {
      return false;
    }


    private bool MeetsOperationalCondition() {
      // Fixed rule, based on law
      if (_recordingAct.LandRecord.Instrument.IssueDate < DateTime.Parse("2014-01-01")) {
        return true;
      }

      // Temporarily rule, based on Tlaxcala Recording Office operation
      if (_recordingAct.LandRecord.PresentationTime < DateTime.Parse("2016-01-01")) {
        return true;
      }

      // Temporarily rule, based on Tlaxcala Recording Office operation
      if (_recordingAct.LandRecord.PresentationTime < DateTime.Parse("2016-09-26") &&
          DateTime.Today < DateTime.Parse("2016-10-02")) {
        return true;
      }
      return false;
    }


    #endregion Private methods

  } // class RecordingActValidator

} // namespace Empiria.Land.Registration
