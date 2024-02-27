/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Registration Services                      Component : Test cases                              *
*  Assembly : Empiria.Land.Tests.dll                     Pattern   : Use cases tests                         *
*  Type     : LandRecordUseCasesTests                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases for land records use cases.                                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Xunit;

using Empiria.Land.Registration.UseCases;
using Empiria.Land.Registration.Adapters;
using Empiria.Land.Registration;

namespace Empiria.Land.Tests.Registration {

  /// <summary>Test cases for recording act registration use cases.</summary>
  public class LandRecordUseCasesTests {

    #region Fields

    private readonly LandRecordUseCases _usecases;

    #endregion Fields

    #region Initialization

    public LandRecordUseCasesTests() => _usecases = LandRecordUseCases.UseCaseInteractor();

    ~LandRecordUseCasesTests() => _usecases.Dispose();

    #endregion Initialization

    #region Facts

    [Fact]
    public void Should_Get_An_Historic_Land_Record() {
      LandRecordDto landRecord = _usecases.GetLandRecord(TestingConstants.HISTORIC_LAND_RECORD_UID);

      Assert.NotNull(landRecord);
    }

    [Fact]
    public void Should_Get_Land_Record_PrimaryParties() {
      LandRecord landRecord = LandRecord.ParseGuid(TestingConstants.LAND_RECORD_UID);

      foreach (var act in landRecord.RecordingActs) {
        foreach (var party in act.Parties.PrimaryParties) {
          Assert.True(party.OwnershipPart.Unit.IsIndivisible);
        }
      }
    }

    #endregion Facts

  }  // class LandRecordUseCasesTests

}  // namespace Empiria.Land.Tests.Registration
