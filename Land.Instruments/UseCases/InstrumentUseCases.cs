/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Legal Instruments                          Component : Use cases Layer                         *
*  Assembly : Empiria.Land.UseCases.dll                  Pattern   : Use case interactor class               *
*  Type     : InstrumentUseCases                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases to interact with legal instruments.                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Services;

namespace Empiria.Land.Instruments.UseCases {

  /// <summary>Use cases to interact with legal instruments.</summary>
  public class InstrumentUseCases : UseCase {

    #region Constructors and parsers

    protected InstrumentUseCases() {
      // no-op
    }

    static public InstrumentUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<InstrumentUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public InstrumentDto GetInstrument(string instrumentUID) {
      Assertion.AssertObject(instrumentUID, "instrumentUID");

      var document = Instrument.Parse(instrumentUID);

      return InstrumentMapper.Map(document);
    }


    #endregion Use cases

  }  // class InstrumentUseCases

}  // namespace Empiria.Land.Instruments.UseCases

