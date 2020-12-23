/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Test cases                              *
*  Assembly : Empiria.Land.Tests.dll                     Pattern   : Use cases tests class                   *
*  Type     : TransactionInstrumentUseCasesTests         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases for transaction instrument use cases.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using Xunit;

using Empiria.Land.Instruments.Adapters;
using Empiria.Land.Transactions.UseCases;

namespace Empiria.Land.Tests.Transactions {

  /// <summary>Test cases for transaction related use cases.</summary>
  public class TransactionInstrumentUseCasesTests {

    #region Fields

    private readonly string _TRANSACTION_UID = TestingConstants.TRANSACTION_UID;

    private readonly TransactionInstrumentUseCases _usecases;

    #endregion Fields

    #region Initialization

    public TransactionInstrumentUseCasesTests() => _usecases = TransactionInstrumentUseCases.UseCaseInteractor();

    ~TransactionInstrumentUseCasesTests() => _usecases.Dispose();

    #endregion Initialization

    #region Facts

    [Fact]
    public void Should_Get_A_Transaction_Instrument() {
      InstrumentDto instrument = _usecases.GetTransactionInstrument(_TRANSACTION_UID);

      Assert.NotNull(instrument.UID);
      Assert.NotNull(instrument.Type);
      Assert.NotNull(instrument.TypeName);
      Assert.NotNull(instrument.Issuer);
      Assert.NotNull(instrument.Issuer.Name);
    }


    [Fact]
    public void Should_Update_A_Transaction_Instrument() {
      var newSummary = EmpiriaString.BuildRandomString(3200);
      var newBinderNo = EmpiriaString.BuildRandomString(48);
      var newFolioNo = EmpiriaString.BuildRandomString(16);

      var fields = new InstrumentFields {
        Summary = newSummary,
        BinderNo = newBinderNo,
        Folio = newFolioNo
      };

      InstrumentDto changed = _usecases.UpdateTransactionInstrument(_TRANSACTION_UID, fields);

      Assert.Equal(newSummary, changed.Summary);
      Assert.Equal(newBinderNo, changed.BinderNo);
      Assert.Equal(newFolioNo, changed.Folio);
    }

    #endregion Facts

  }  // class TransactionInstrumentUseCasesTests

}  // namespace Empiria.Land.Tests.Transactions
