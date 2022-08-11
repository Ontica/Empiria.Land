/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Registration Services                        Component : Test cases                            *
*  Assembly : Empiria.Land.Tests.dll                       Pattern   : Use cases tests class                 *
*  Type     : TransactionInstrumentRecordingUseCasesTests  License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Test cases for transaction instrument registration use cases.                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Xunit;

using Empiria.Land.Instruments.Adapters;

using Empiria.Land.Registration.UseCases;
using Empiria.Land.Registration.Adapters;

namespace Empiria.Land.Tests.Registration {

  /// <summary>Test cases for transaction instrument registration use cases.</summary>
  public class TransactionInstrumentRecordingUseCasesTests {

    #region Fields

    private readonly string TRANSACTION_UID = TestingConstants.TRANSACTION_UID;

    private readonly TransactionInstrumentRecordingUseCases _usecases;

    #endregion Fields

    #region Initialization

    public TransactionInstrumentRecordingUseCasesTests() => _usecases = TransactionInstrumentRecordingUseCases.UseCaseInteractor();

    ~TransactionInstrumentRecordingUseCasesTests() => _usecases.Dispose();

    #endregion Initialization

    #region Facts

    [Fact]
    public void Should_Change_A_TransactionInstrumentRecordingType() {
      InstrumentRecordingDto instrumentRecording = _usecases.GetTransactionInstrumentRecording(TRANSACTION_UID);

      InstrumentDto instrument = instrumentRecording.Instrument;

      var fields = new InstrumentFields {
        Type = instrument.Type == InstrumentTypeEnum.DocumentoJuzgado ?
                                  InstrumentTypeEnum.EscrituraPublica : InstrumentTypeEnum.DocumentoJuzgado,
        BinderNo = EmpiriaString.BuildRandomString(48)
      };

      InstrumentDto changed = _usecases.UpdateTransactionInstrumentRecording(TRANSACTION_UID, fields)
                                       .Instrument;

      Assert.Equal(fields.Type, changed.Type);
      Assert.Equal(fields.BinderNo, changed.BinderNo);

      Assert.Equal(instrument.IssueDate, changed.IssueDate);
      Assert.Equal(instrument.Issuer.UID, changed.Issuer.UID);
    }


    [Fact]
    public void Should_Get_A_Transaction_InstrumentRecording() {
      InstrumentRecordingDto instrumentRecording = _usecases.GetTransactionInstrumentRecording(TRANSACTION_UID);

      Assert.NotNull(instrumentRecording.UID);
      Assert.NotNull(instrumentRecording.Instrument.UID);
      Assert.NotNull(instrumentRecording.Instrument.TypeName);
      Assert.NotNull(instrumentRecording.Instrument.Issuer);
      Assert.NotNull(instrumentRecording.Instrument.Issuer.Name);
    }


    [Fact]
    public void Should_Create_A_Transaction_InstrumentRecording() {
      var fields = new InstrumentFields {
        Type = InstrumentTypeEnum.EscrituraPublica,
        Summary = EmpiriaString.BuildRandomString(1760),
        BinderNo = EmpiriaString.BuildRandomString(64),
        Folio = EmpiriaString.BuildRandomString(48),
        IssueDate = DateTime.Parse("2018-10-05")
      };

      InstrumentDto created = _usecases.CreateTransactionInstrumentRecording(TRANSACTION_UID, fields)
                                       .Instrument;

      Assert.Equal(fields.Type, created.Type);
      Assert.Equal(fields.Summary, created.Summary);
      Assert.Equal(fields.BinderNo, created.BinderNo);
      Assert.Equal(fields.Folio, created.Folio);
      Assert.Equal(fields.IssueDate, created.IssueDate);
    }


    [Fact]
    public void Should_Update_A_Transaction_InstrumentRecording() {
      var fields = new InstrumentFields {
        Summary = EmpiriaString.BuildRandomString(3200),
        BinderNo = EmpiriaString.BuildRandomString(48),
        Folio = EmpiriaString.BuildRandomString(16)
      };

      InstrumentDto changed = _usecases.UpdateTransactionInstrumentRecording(TRANSACTION_UID, fields)
                                       .Instrument;

      Assert.Equal(fields.Summary, changed.Summary);
      Assert.Equal(fields.BinderNo, changed.BinderNo);
      Assert.Equal(fields.Folio, changed.Folio);
    }

    #endregion Facts

  }  // class TransactionInstrumentRecordingUseCasesTests

}  // namespace Empiria.Land.Tests.Registration
