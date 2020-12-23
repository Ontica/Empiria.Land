/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Legal Instruments                          Component : Test cases                              *
*  Assembly : Empiria.Land.Tests.dll                     Pattern   : Test class                              *
*  Type     : InstrumentTypeUseCasesTests                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases that retrive configuration data for legal instrument types.                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using Xunit;

using Empiria.Land.Instruments;
using Empiria.Land.Instruments.UseCases;

namespace Empiria.Land.Tests.Instruments {

  /// <summary>Test cases that retrive configuration data for legal instrument types.</summary>
  public class InstrumentTypeUseCasesTests {

    #region Fields

    private readonly InstrumentTypeUseCases _usecases;

    #endregion Fields

    #region Initialization

    public InstrumentTypeUseCasesTests() {
      _usecases = InstrumentTypeUseCases.UseCaseInteractor();
    }


    ~InstrumentTypeUseCasesTests() {
      _usecases.Dispose();
    }

    #endregion Initialization

    #region Facts


    [Fact]
    public void Should_Read_Instrument_Kinds() {
      FixedList<string> list = _usecases.GetInstrumentKinds(InstrumentTypeEnum.OficioNotaria);

      Assert.NotEmpty(list);

      list = _usecases.GetInstrumentKinds(InstrumentTypeEnum.DocumentoJuzgado);

      Assert.NotEmpty(list);

      list = _usecases.GetInstrumentKinds(InstrumentTypeEnum.DocumentoTerceros);

      Assert.NotEmpty(list);

      list = _usecases.GetInstrumentKinds(InstrumentTypeEnum.EscrituraPublica);

      Assert.Empty(list);
    }

    #endregion Facts

  }  // class InstrumentTypeUseCasesTests

}  // namespace Empiria.Land.Tests.Instruments
