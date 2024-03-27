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

      Assertion.Require(landRecord.SecurityData.IsUnsigned ||
                        landRecord.SecurityData.SignType != SignType.Electronic,
                        "Esta inscripción fue firmada electrónicamente. " +
                        "Para poder abrirla, se necesita solicitar que se revoque la firma electrónica.");

      landRecord.Security.RemoveManualSign();

      landRecord.Open();

      return LandRecordMapper.Map(landRecord);
    }


    //ToDo: Remove this method after installation
    public void RefreshLandRecordsHashes() {
      var records = BaseObject.GetList<LandRecord>("LandRecordDIF = ''");

      foreach (var record in records) {
        record.Security.RefreshDIFHash();
      }
    }

    #endregion Use cases

  }  // class LandRecordUseCases

}  // namespace Empiria.Land.Registration.UseCases
