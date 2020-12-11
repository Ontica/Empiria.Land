/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Legal Instruments                          Component : Test cases                              *
*  Assembly : Empiria.Land.Tests.dll                     Pattern   : Test class                              *
*  Type     : GetInstrumentsUseCasesTests                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases for legal instruments retrieving.                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using Xunit;

namespace Empiria.Land.Instruments.UseCases.Tests {

  /// <summary>Test cases for legal instruments retrieving.</summary>
  public class GetInstrumentsUseCasesTests {

    #region Fields

    private readonly string _INSTRUMENT_UID;

    private readonly GetInstrumentsUseCases _usecases;

    #endregion Fields

    #region Initialization

    public GetInstrumentsUseCasesTests() {
      _INSTRUMENT_UID = "RP73RX-94ZF28-HN34E7";

      _usecases = GetInstrumentsUseCases.UseCaseInteractor();
    }


    ~GetInstrumentsUseCasesTests() {
      _usecases.Dispose();
    }

    #endregion Initialization

    #region Facts

    [Fact]
    public void Should_Read_Instrument() {
      InstrumentDto instrument = _usecases.GetInstrument(_INSTRUMENT_UID);

      Assert.Equal(_INSTRUMENT_UID, instrument.UID);
    }

    #endregion Facts

  }

}
