/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Registration                          Component : Use cases Layer                         *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Use case interactor class               *
*  Type     : LandRecordUseCases                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for transaction land record edition and retrieving.                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.Land.Registration.Adapters;
using Empiria.Contacts;
using Empiria.Security;

namespace Empiria.Land.Registration.UseCases {

  /// <summary>Use cases for transaction land record edition and retrieving.</summary>
  public class LandRecordUseCases : UseCase {

    #region Constructors and parsers

    protected LandRecordUseCases() {
      // no-op
    }

    static public LandRecordUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<LandRecordUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public bool ExistsLandRecordID(string landRecordID) {
      Assertion.Require(landRecordID, nameof(landRecordID));

      var landRecord = LandRecord.TryParse(landRecordID);

      return (landRecord != null);
    }


    public LandRecordDto GetLandRecord(string landRecordUID) {
      Assertion.Require(landRecordUID, nameof(landRecordUID));

      LandRecord landRecord = LandRecord.ParseGuid(landRecordUID);

      return LandRecordMapper.Map(landRecord);
    }


    public LandRecordDto CloseLandRecord(string landRecordUID) {
      Assertion.Require(landRecordUID, nameof(landRecordUID));

      LandRecord landRecord = LandRecord.ParseGuid(landRecordUID);

      landRecord.Close();

      if (!LandRecordSecurityData.ESIGN_ENABLED) {
        landRecord.Security.ManualSign();
      } else {
        landRecord.Security.SetElectronicSignerData(landRecord.RecorderOffice.Signer);
      }

      return LandRecordMapper.Map(landRecord);
    }


    public LandRecordDto OpenLandRecord(string landRecordUID) {
      Assertion.Require(landRecordUID, nameof(landRecordUID));

      LandRecord landRecord = LandRecord.ParseGuid(landRecordUID);

      landRecord.Security.AssertCanBeOpened();

      landRecord.Security.RemoveSign();

      landRecord.Open();

      return LandRecordMapper.Map(landRecord);
    }

    #endregion Use cases

    #region Temp use cases

    // ToDo: Remove this use case when all opened land records are manually closed
    public LandRecordDto ManualCloseLandRecord(string landRecordID, ManualCloseRecordFields fields) {
      Assertion.Require(landRecordID, nameof(landRecordID));
      Assertion.Require(fields, nameof(fields));

      LandRecord landRecord = LandRecord.TryParse(landRecordID);

      Assertion.Require(landRecord, $"Invalid landRecord {landRecordID}.");

      Assertion.Require(!landRecord.IsClosed,
                        $"LandRecord {landRecordID} was already closed.");

      Assertion.Require(!landRecord.IsHistoricRecord, $"Land record {landRecordID} is historic, can't be closed'.");
      Assertion.Require(landRecord.SecurityData.SignType != SignType.Electronic,
                        $"LandRecord {landRecordID} sign type is marked as electronic sign, can't be closed");

      Assertion.Require(landRecord.Transaction.Workflow.CurrentStatus == Transactions.TransactionStatus.Returned ||
                        landRecord.Transaction.Workflow.CurrentStatus == Transactions.TransactionStatus.Delivered ||
                        landRecord.Transaction.Workflow.CurrentStatus == Transactions.TransactionStatus.Archived,
                        $"Invalid LandRecord {landRecordID} current status {landRecord.Transaction.Workflow.CurrentStatus}");

      Assertion.Require(landRecord.RecordingActs.Count > 0,
                       $"LandRecord {landRecordID} without recording acts, can't be closed.");

      foreach (var recordingAct in landRecord.RecordingActs) {
        recordingAct.Validator.AssertCanBeManuallyClosed();
      }

      fields.DigitalSeal = GenerateDigitalSeal();

      landRecord.ManuallyClose(fields);

      landRecord.SecurityData.SetManualSignData(landRecord, fields);

      landRecord.Save();

      Data.LandRecordsData.SaveSecurityData(landRecord);

      return LandRecordMapper.Map(landRecord);


      string GenerateDigitalSeal() {
        var transaction = landRecord.Transaction;

        string s = "||" + transaction.UID + "|" + landRecord.UID;

        for (int i = 0; i < landRecord.RecordingActs.Count; i++) {
          s += "|" + landRecord.RecordingActs[i].Id.ToString();
        }
        s += "||";

        return Cryptographer.SignTextWithSystemCredentials(s);
      }

    }

    #endregion Temp use cases

  }  // class LandRecordUseCases

  // ToDo: Remove this class when all opened land records are manually closed
  public class ManualCloseRecordFields {

    public DateTime AuthorizationTime {
      get; set;
    }

    public int AuthorizedById {
      get; set;
    }

    public string SecurityHash {
      get; set;
    }

    public int SignedById {
      get; set;
    }

    public string DigitalSeal {
      get; set;
    }

    internal Person AuthorizedBy {
      get {
        return Person.Parse(AuthorizedById);
      }
    }

    public Person SignedBy {
      get {
        return Person.Parse(SignedById);
      }
    }

  }  // temp class ManualCloseRecordFields

}  // namespace Empiria.Land.Registration.UseCases
