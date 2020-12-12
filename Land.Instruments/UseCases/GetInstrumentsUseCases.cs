/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Legal Instruments                          Component : Use cases Layer                         *
*  Assembly : Empiria.Land.UseCases.dll                  Pattern   : Use case interactor class               *
*  Type     : GetInstrumentsUseCases                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for legal instruments retrieving.                                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using Empiria.Services;

namespace Empiria.Land.Instruments.UseCases {

  /// <summary>Use cases for legal instruments retrieving.</summary>
  public class GetInstrumentsUseCases : UseCase {

    #region Constructors and parsers

    protected GetInstrumentsUseCases() {
      // no-op
    }

    static public GetInstrumentsUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<GetInstrumentsUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public InstrumentDto GetInstrument(string instrumentUID) {
      Assertion.AssertObject(instrumentUID, "instrumentUID");

      var document = Instrument.Parse(instrumentUID);

      return InstrumentMapper.Map(document);
    }

    #endregion Use cases

  }

}
