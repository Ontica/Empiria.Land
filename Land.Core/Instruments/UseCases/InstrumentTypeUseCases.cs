/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Legal Instruments                          Component : Use cases Layer                         *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Use case interactor class               *
*  Type     : InstrumentTypeUseCases                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to retrieve configuration data of legal instrument types.                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.Land.Instruments.Adapters;

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

    public FixedList<string> GetInstrumentKinds(InstrumentTypeEnum instrumentTypeValue) {
      Assertion.Assert(instrumentTypeValue != InstrumentTypeEnum.All,
                       "instrumentTypeValue can't have the value 'All'.");

      if (instrumentTypeValue == InstrumentTypeEnum.Empty) {
        return new FixedList<string>();
      }

      var instrumentType = InstrumentType.Parse(instrumentTypeValue);

      return new FixedList<string>(instrumentType.InstrumentKinds);
    }

    #endregion Use cases

  }  // class InstrumentTypeUseCases

}  // namespace Empiria.Land.Instruments.UseCases
