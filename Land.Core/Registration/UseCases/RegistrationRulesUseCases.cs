/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Registration                          Component : Use cases Layer                         *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Use case interactor class               *
*  Type     : RegistrationRulesUseCases                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for get Empiria Land registration rules.                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.Land.Instruments;
using Empiria.Land.Registration.Adapters;

namespace Empiria.Land.Registration.UseCases {

  /// <summary>Use cases for get Empiria Land registration rules.</summary>
  public partial class RegistrationRulesUseCases : UseCase {

    #region Constructors and parsers

    protected RegistrationRulesUseCases() {
      // no-op
    }

    static public RegistrationRulesUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<RegistrationRulesUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public FixedList<RecordingActTypeGroupDto> RecordingActTypesForInstrument(string instrumentUID) {
      Assertion.AssertObject(instrumentUID, "instrumentUID");

      var instrument = Instrument.Parse(instrumentUID);

      FixedList<RecordingActTypeCategory> recordingActTypesList = instrument.ApplicableRecordingActTypes();

      return RecordingActTypeMapper.Map(recordingActTypesList);
    }

    #endregion Use cases

  }  // class RegistrationRulesUseCases

}  // namespace Empiria.Land.Registration.UseCases
