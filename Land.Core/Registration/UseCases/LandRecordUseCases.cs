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

      var landRecord = RecordingDocument.TryParse(landRecordID);

      return (landRecord != null);
    }


    public LandRecordDto GetLandRecord(string landRecordUID) {
      Assertion.Require(landRecordUID, nameof(landRecordUID));

      RecordingDocument landRecord = RecordingDocument.ParseGuid(landRecordUID);

      return LandRecordMapper.Map(landRecord);
    }


    public LandRecordDto CloseLandRecord(string landRecordUID) {
      Assertion.Require(landRecordUID, nameof(landRecordUID));

      RecordingDocument landRecord = RecordingDocument.ParseGuid(landRecordUID);

      landRecord.Close();

      return LandRecordMapper.Map(landRecord);
    }


    public LandRecordDto OpenLandRecord(string landRecordUID) {
      Assertion.Require(landRecordUID, nameof(landRecordUID));

      RecordingDocument landRecord = RecordingDocument.ParseGuid(landRecordUID);

      landRecord.Open();

      return LandRecordMapper.Map(landRecord);
    }

    #endregion Use cases

  }  // class LandRecordUseCases

}  // namespace Empiria.Land.Registration.UseCases
