/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Recordable Subjects                        Component : Use cases Layer                         *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Use case interactor class               *
*  Type     : TractIndexUseCases                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for retrieve recordable subject's tract indexes.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.Land.Registration;

using Empiria.Land.RecordableSubjects.Adapters;

namespace Empiria.Land.RecordableSubjects.UseCases {

  /// <summary>Use cases for retrieve recordable subject's tract indexes.</summary>
  public partial class TractIndexUseCases : UseCase {

    #region Constructors and parsers

    protected TractIndexUseCases() {
      // no-op
    }

    static public TractIndexUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<TractIndexUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public TractIndexDto AmendableRecordingActs(string recordableSubjectUID,
                                                string instrumentRecordingUID,
                                                string amendmentRecordingActTypeUID) {
      Assertion.Require(recordableSubjectUID, "recordableSubjectUID");
      Assertion.Require(instrumentRecordingUID, "instrumentRecordingUID");
      Assertion.Require(amendmentRecordingActTypeUID, "amendmentRecordingActTypeUID");

      var amendmentRecordingActType = RecordingActType.Parse(amendmentRecordingActTypeUID);

      var recordableSubject = Resource.ParseGuid(recordableSubjectUID);

      FixedList<RecordingActType> appliesTo = amendmentRecordingActType.GetAppliesToRecordingActTypesList();

      EmpiriaLog.Debug(appliesTo.Count.ToString());

      FixedList<RecordingAct> list = recordableSubject.Tract.GetRecordingActs();

      var amendableActs = list.FindAll((x) => appliesTo.Contains(x.RecordingActType));

      return TractIndexMapper.Map(recordableSubject, amendableActs);
    }

    #endregion Use cases

  }  // class TractIndexUseCases

}  // namespace Empiria.Land.RecordableSubjects.UseCases
