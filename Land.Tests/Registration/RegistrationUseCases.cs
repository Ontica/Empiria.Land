/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Registration Services                      Component : Test cases                              *
*  Assembly : Empiria.Land.Tests.dll                     Pattern   : Use cases tests                         *
*  Type     : RegistrationUseCasesTests                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases for recording act registration use cases.                                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Xunit;

using Empiria.Land.Registration.UseCases;
using Empiria.Land.Registration.Adapters;

namespace Empiria.Land.Tests.Registration {

  /// <summary>Test cases for recording act registration use cases.</summary>
  public class RegistrationUseCasesTests {

    #region Fields

    private readonly RegistrationUseCases _usecases;

    #endregion Fields

    #region Initialization

    public RegistrationUseCasesTests() => _usecases = RegistrationUseCases.UseCaseInteractor();

    ~RegistrationUseCasesTests() => _usecases.Dispose();

    #endregion Initialization

    #region Facts

    [Fact]
    public void Should_Create_Association_RecordingAct() {
      var command = new RegistrationCommand {
        Type = Land.Registration.RegistrationCommandType.CreateAssociation,
        Payload = new RegistrationCommandPayload {
          RecordingActTypeUID = TestingConstants.CREATE_ASSOCIATION_RECORDING_ACT_TYPE_UID
        }
      };
      InstrumentRecordingDto instrumentRecording =
               _usecases.CreateRecordingAct(TestingConstants.INSTRUMENT_RECORDING_UID, command);

      Assert.NotNull(instrumentRecording);
    }


    [Fact]
    public void Should_Create_NoProperty_RecordingAct() {
      var command = new RegistrationCommand {
        Type = Land.Registration.RegistrationCommandType.CreateNoProperty,
        Payload = new RegistrationCommandPayload {
          RecordingActTypeUID = TestingConstants.CREATE_NO_PROPERTY_RECORDING_ACT_TYPE_UID
        }
      };

      InstrumentRecordingDto instrumentRecording =
                 _usecases.CreateRecordingAct(TestingConstants.INSTRUMENT_RECORDING_UID, command);

      Assert.NotNull(instrumentRecording);
    }


    [Fact]
    public void Should_Create_RealEstate_RecordingAct() {
      var command = new RegistrationCommand {
        Type = Land.Registration.RegistrationCommandType.CreateRealEstate,
        Payload = new RegistrationCommandPayload {
          RecordingActTypeUID = TestingConstants.CREATE_REAL_ESTATE_RECORDING_ACT_TYPE_UID
        }
      };

      InstrumentRecordingDto instrumentRecording =
                _usecases.CreateRecordingAct(TestingConstants.INSTRUMENT_RECORDING_UID, command);

      Assert.NotNull(instrumentRecording);
    }


    [Fact]
    public void Should_Target_Existing_Association_RecordingAct() {
      var command = new RegistrationCommand {
        Type = Land.Registration.RegistrationCommandType.SelectAssociation,
        Payload = new RegistrationCommandPayload {
          RecordingActTypeUID = TestingConstants.TARGET_ASSOCIATION_RECORDING_ACT_TYPE_UID,
          RecordableSubjectUID = TestingConstants.ASSOCATION_UID
        }
      };

      InstrumentRecordingDto instrumentRecording =
               _usecases.CreateRecordingAct(TestingConstants.INSTRUMENT_RECORDING_UID, command);

      Assert.NotNull(instrumentRecording);
    }


    [Fact]
    public void Should_Target_Existing_NoProperty_RecordingAct() {
      var command = new RegistrationCommand {
        Type = Land.Registration.RegistrationCommandType.SelectNoProperty,
        Payload = new RegistrationCommandPayload {
          RecordingActTypeUID = TestingConstants.TARGET_NO_PROPERTY_RECORDING_ACT_TYPE_UID,
          RecordableSubjectUID = TestingConstants.NO_PROPERTY_UID
        }
      };

      InstrumentRecordingDto instrumentRecording =
               _usecases.CreateRecordingAct(TestingConstants.INSTRUMENT_RECORDING_UID, command);

      Assert.NotNull(instrumentRecording);
    }


    [Fact]
    public void Should_Target_Existing_RealEstate_RecordingAct() {
      var command = new RegistrationCommand {
        Type = Land.Registration.RegistrationCommandType.SelectRealEstate,
        Payload = new RegistrationCommandPayload {
          RecordingActTypeUID = TestingConstants.TARGET_REAL_ESTATE_RECORDING_ACT_TYPE_UID,
          RecordableSubjectUID = TestingConstants.REAL_ESTATE_UID
        }
      };

      InstrumentRecordingDto instrumentRecording =
               _usecases.CreateRecordingAct(TestingConstants.INSTRUMENT_RECORDING_UID, command);

      Assert.NotNull(instrumentRecording);
    }


    [Fact]
    public void Should_Create_RealEstate_On_Existing_Antecedent() {
      var command = new RegistrationCommand {
        Type = Land.Registration.RegistrationCommandType.SelectRealEstateAntecedent,
        Payload = new RegistrationCommandPayload {
          RecordingActTypeUID = TestingConstants.REAL_ESTATE_DOMAIN_ACT,
          RecordingBookUID = TestingConstants.RECORDING_BOOK_FOR_RECORDING_UID,
          BookEntryUID = TestingConstants.BOOK_ENTRY_FOR_RECORDING_UID
        }
      };

      InstrumentRecordingDto instrumentRecording =
               _usecases.CreateRecordingAct(TestingConstants.INSTRUMENT_RECORDING_UID, command);

      Assert.NotNull(instrumentRecording);
    }


    [Fact]
    public void Should_Create_RealEstate_On_Unregistered_Antecedent() {
      var command = new RegistrationCommand {
        Type = Land.Registration.RegistrationCommandType.SelectRealEstateAntecedent,
        Payload = new RegistrationCommandPayload {
          RecordingActTypeUID = TestingConstants.REAL_ESTATE_DOMAIN_ACT,
          RecordingBookUID = TestingConstants.RECORDING_BOOK_FOR_RECORDING_UID,
          BookEntryNo = "123"
        }
      };

      InstrumentRecordingDto instrumentRecording =
               _usecases.CreateRecordingAct(TestingConstants.INSTRUMENT_RECORDING_UID, command);

      Assert.NotNull(instrumentRecording);
    }

    #endregion Facts

  }  // class RegistrationUseCasesTests

}  // namespace Empiria.Land.Tests.Registration
