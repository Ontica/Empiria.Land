﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Recording services                      Component : Recording documents                   *
*  Assembly : Empiria.Land.Registration.dll                Pattern   : Service provider                      *
*  Type     : LandRecordValidator                          License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Provides methods to check the integrity of recording documents and their processes.            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Provides methods to check the integrity of recording documents and their processes.</summary>
  public class LandRecordValidator {

    private readonly LandRecord _landRecord;

    #region Constructors and parsers

    public LandRecordValidator(LandRecord landRecord) {
      Assertion.Require(landRecord, nameof(landRecord));
      // Assertion.Require(!landRecord.IsEmptyInstance, "LandRecord can't be the empty instance.");

      _landRecord = landRecord;
    }

    #endregion Constructors and parsers

    #region Methods


    public void AssertCanBeClosed() {
      if (!_landRecord.Security.IsReadyToClose()) {
        Assertion.RequireFail("El usuario no tiene permisos para cerrar la inscripción o ésta no tiene un estado válido.");
      }

      //this.AssertGraceDaysForEdition();

      Assertion.Require(_landRecord.RecordingActs.Count > 0, "La inscripción no tiene actos jurídicos.");

      foreach (var recordingAct in _landRecord.RecordingActs) {
        recordingAct.Validator.AssertCanBeClosed();
      }
    }


    public void AssertCanBeOpened() {
      if (!_landRecord.Security.IsReadyToOpen()) {
        Assertion.RequireFail("El usuario no tiene permisos para abrir esta inscripción.");
      }

      //this.AssertGraceDaysForEdition();

      foreach (var recordingAct in _landRecord.RecordingActs) {
        recordingAct.Validator.AssertCanBeOpened();
      }
    }


    public void AssertCanBeElectronicallySigned() {
      Assertion.Require(LandRecordSecurityData.ESIGN_ENABLED,
          "No es posible firmar electrónicamente esta inscripción debido a que el servicio " +
          "de firma electrónica no está habilitado." + LandRecordDescriptionMessage());

      Assertion.Require(!_landRecord.IsHistoricRecord,
          "No se puede firmar una inscripción histórica." + LandRecordDescriptionMessage());

      Assertion.Require(_landRecord.IsClosed,
          "No se puede firmar una inscripción que no ha sido cerrada." + LandRecordDescriptionMessage());

      Assertion.Require(_landRecord.SecurityData.IsUnsigned,
          "No se puede firmar una inscripción que ya está firmada." + LandRecordDescriptionMessage());

      Assertion.Require(_landRecord.SecurityData.UsesESign,
          "No se puede firmar una inscripción que no está marcada para firma electrónica." + LandRecordDescriptionMessage());

      Assertion.Require(_landRecord.SecurityData.SignedBy.Equals(ExecutionServer.CurrentContact),
          "La inscripción está asignada para ser firmada " +
          $"por una persona distinta ({_landRecord.SecurityData.SignedBy.FullName}) " +
          $"a la que está intentando firmarla." + LandRecordDescriptionMessage());
    }


    public void AssertCanGenerateImagingControlID() {
      Assertion.Require(_landRecord.IsClosed, "Document is not closed.");

      Assertion.Require(_landRecord.ImagingControlID.Length == 0,
                        "Land record has already assigned an imaging control number.");

      Assertion.Require(_landRecord.RecordingActs.Count > 0, "Land record should have recording acts.");
      Assertion.Require(_landRecord.RecordingActs.CountAll((x) => !x.BookEntry.IsEmptyInstance) == 0,
                        "Land record can't have any recording acts that are related to physical book entries.");
    }


    public void AssertCanManualSign() {
      Assertion.Require(_landRecord.IsClosed,
              "No se puede firmar una inscripción que no está cerrada." + LandRecordDescriptionMessage());

      Assertion.Require(_landRecord.SecurityData.IsUnsigned,
              "No se puede volver a firmar una inscripción que ya está firmada." + LandRecordDescriptionMessage());

      Assertion.Require(!_landRecord.IsHistoricRecord,
              "No se puede firmar una inscripción histórica." + LandRecordDescriptionMessage());

      Assertion.Require(!LandRecordSecurityData.ESIGN_ENABLED,
              "No es posible efectuar firmar manualmente esta inscripción, " +
              "debido a que el servicio de firma electrónica está habilitado." + LandRecordDescriptionMessage());
    }


    public void AssertCanPrepareForElectronicSign() {
      Assertion.Require(LandRecordSecurityData.ESIGN_ENABLED,
                "No es posible preparar esta inscripción para firma electrónica, " +
                "debido a que el servicio de firma electrónica no está habilitado." + LandRecordDescriptionMessage());

      Assertion.Require(_landRecord.IsClosed,
                "No se puede enviar a firma una inscripción que no está cerrada." + LandRecordDescriptionMessage());

      Assertion.Require(_landRecord.SecurityData.IsUnsigned,
                "No se puede enviar a firma una inscripción que ya está firmada." + LandRecordDescriptionMessage());

      Assertion.Require(!_landRecord.IsHistoricRecord,
                "No se puede enviar a firma una inscripción histórica." + LandRecordDescriptionMessage());

    }


    public void AssertCanRemoveSign() {
      Assertion.Require(_landRecord.IsClosed,
          "No se puede desfirmar una inscripción que no está cerrada." + LandRecordDescriptionMessage());
    }


    public void AssertCanRevokeSign() {
      Assertion.Require(LandRecordSecurityData.ESIGN_ENABLED,
                        "No se puede revocar la firma de esta inscripción debido a que el servicio " +
                        "de firma electrónica no está habilitado." + LandRecordDescriptionMessage());

      Assertion.Require(_landRecord.SecurityData.UsesESign,
                       "Sólo se puede revocar la firma de una inscripción " +
                       "marcada para firma electrónica." + LandRecordDescriptionMessage());

      Assertion.Require(_landRecord.IsClosed,
                        "No se puede revocar la firma de una inscripción " +
                        "que no está cerrada." + LandRecordDescriptionMessage());

      Assertion.Require(_landRecord.SecurityData.IsSigned,
                 "No se puede revocar la firma de una inscripción " +
                 "que no ha sido firmada." + LandRecordDescriptionMessage());


      Assertion.Require(_landRecord.SecurityData.SignedBy.Equals(ExecutionServer.CurrentContact),
                 "Únicamente la persona que firmó la inscripción puede " +
                 "revocar la firma de la misma." + LandRecordDescriptionMessage());
    }


    public void AssertGraceDaysForEdition() {
      var transaction = _landRecord.Transaction;

      if (transaction.IsEmptyInstance) {
        return;
      }

      const int graceDaysForEdition = 45;

      DateTime lastDate = transaction.PresentationTime;

      if (transaction.LastReentryTime != ExecutionServer.DateMaxValue) {
        lastDate = transaction.LastReentryTime;
      }

      if (lastDate.AddDays(graceDaysForEdition) < DateTime.Today) {
        Assertion.RequireFail("Por motivos de seguridad y calidad en el registro de la información, " +
                             "no es posible modificar inscripciones de documentos en trámites de más de 45 días.\n\n" +
                             "En su lugar se puede optar por registrar un nuevo trámite, " +
                             "o quizás se pueda hacer un reingreso si no han transcurrido los " +
                             "90 días de gracia.");
      }
    }

    #endregion Methods

    #region Helpers

    private string LandRecordDescriptionMessage() {
      return $" Trámite {_landRecord.Transaction.UID}. Inscripción: {_landRecord.UID}.";
    }

    #endregion Helpers

  } // class LandRecordValidator

} // namespace Empiria.Land.Registration
