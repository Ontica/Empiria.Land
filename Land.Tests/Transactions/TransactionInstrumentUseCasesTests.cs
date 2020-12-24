/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Test cases                              *
*  Assembly : Empiria.Land.Tests.dll                     Pattern   : Use cases tests class                   *
*  Type     : TransactionInstrumentUseCasesTests         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases for transaction instrument use cases.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Xunit;

using Empiria.Land.Instruments;
using Empiria.Land.Instruments.Adapters;
using Empiria.Land.Transactions.UseCases;

namespace Empiria.Land.Tests.Transactions {

  /// <summary>Test cases for transaction related use cases.</summary>
  public class TransactionInstrumentUseCasesTests {

    #region Fields

    private readonly string TRANSACTION_UID = TestingConstants.TRANSACTION_UID;

    private readonly TransactionInstrumentUseCases _usecases;

    #endregion Fields

    #region Initialization

    public TransactionInstrumentUseCasesTests() => _usecases = TransactionInstrumentUseCases.UseCaseInteractor();

    ~TransactionInstrumentUseCasesTests() => _usecases.Dispose();

    #endregion Initialization

    #region Facts

    [Fact]
    public void Should_Get_A_Transaction_Instrument() {
      InstrumentDto instrument = _usecases.GetTransactionInstrument(TRANSACTION_UID);

      Assert.NotNull(instrument.UID);
      Assert.NotNull(instrument.TypeName);
      Assert.NotNull(instrument.Issuer);
      Assert.NotNull(instrument.Issuer.Name);
    }


    [Fact]
    public void Should_Create_A_Transaction_Instrument() {
      var fields = new InstrumentFields {
        Type = InstrumentTypeEnum.EscrituraPublica,
        Summary = EmpiriaString.BuildRandomString(1760),
        BinderNo = EmpiriaString.BuildRandomString(64),
        Folio = EmpiriaString.BuildRandomString(48),
        IssueDate = DateTime.Parse("2018-10-05")
      };

      InstrumentDto created = _usecases.CreateTransactionInstrument(TRANSACTION_UID, fields);

      Assert.Equal(fields.Type, created.Type);
      Assert.Equal(fields.Summary, created.Summary);
      Assert.Equal(fields.BinderNo, created.BinderNo);
      Assert.Equal(fields.Folio, created.Folio);
      Assert.Equal(fields.IssueDate, created.IssueDate);
    }


    [Fact]
    public void Should_Update_A_Transaction_Instrument() {
      var fields = new InstrumentFields {
        Summary = EmpiriaString.BuildRandomString(3200),
        BinderNo = EmpiriaString.BuildRandomString(48),
        Folio = EmpiriaString.BuildRandomString(16)
      };

      InstrumentDto changed = _usecases.UpdateTransactionInstrument(TRANSACTION_UID, fields);

      Assert.Equal(fields.Summary, changed.Summary);
      Assert.Equal(fields.BinderNo, changed.BinderNo);
      Assert.Equal(fields.Folio, changed.Folio);
    }

    #endregion Facts

  }  // class TransactionInstrumentUseCasesTests

}  // namespace Empiria.Land.Tests.Transactions
