/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Legal Instruments                          Component : Use cases Layer                         *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Use case interactor class               *
*  Type     : InstrumentTypeUseCases                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to retrieve configuration data of legal instrument types.                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Services;

namespace Empiria.Land.Instruments.UseCases {

  /// <summary>Use cases used to retrieve configuration data of legal instrument types.</summary>
  public class InstrumentTypeUseCases : UseCase {

    #region Constructors and parsers

    protected InstrumentTypeUseCases() {
      // no-op
    }

    static public InstrumentTypeUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<InstrumentTypeUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public FixedList<string> GetInstrumentKinds(InstrumentTypeEnum instrumentTypeName) {
      Assertion.Assert(instrumentTypeName != InstrumentTypeEnum.All,
                       "instrumentTypeName can't have the value 'All'.");

      var instrumentType = InstrumentType.Parse(instrumentTypeName);

      return new FixedList<string>(instrumentType.InstrumentKinds);
    }

    #endregion Use cases

  }  // class InstrumentTypeUseCases

}  // namespace Empiria.Land.Instruments.UseCases
