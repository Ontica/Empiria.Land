﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Legal Instruments                          Component : Test cases                              *
*  Assembly : Empiria.Land.Tests.dll                     Pattern   : Test class                              *
*  Type     : InstrumentUseCasesTests                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases for legal instruments.                                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using Xunit;

using Empiria.Land.Instruments.Adapters;

namespace Empiria.Land.Instruments.UseCases.Tests {

  /// <summary>Test cases for legal instruments.</summary>
  public class InstrumentUseCasesTests {

    #region Fields

    private readonly string _INSTRUMENT_UID;

    private readonly InstrumentUseCases _usecases;

    #endregion Fields

    #region Initialization

    public InstrumentUseCasesTests() {
      _INSTRUMENT_UID = "RP73RX-94ZF28-HN34E7";

      _usecases = InstrumentUseCases.UseCaseInteractor();
    }


    ~InstrumentUseCasesTests() {
      _usecases.Dispose();
    }

    #endregion Initialization

    #region Facts

    [Fact]
    public void Should_Read_An_Instrument() {
      InstrumentDto instrument = _usecases.GetInstrument(_INSTRUMENT_UID);

      Assert.Equal(_INSTRUMENT_UID, instrument.UID);
    }


    #endregion Facts

  }  // class InstrumentUseCasesTests

}  // namespace Empiria.Land.Instruments.UseCases.Tests