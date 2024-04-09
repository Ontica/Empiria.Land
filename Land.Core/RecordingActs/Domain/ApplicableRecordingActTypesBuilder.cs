/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Registration                          Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Builder                                 *
*  Type     : ApplicableRecordingActTypesBuilder         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides services to build applicable recording act types lists,                               *
*             based on a given recording context.                                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Instruments;

namespace Empiria.Land.Registration {

  /// <summary>Provides services to build applicable recording act types lists,
  /// based on a given recording context.</summary>
  internal class ApplicableRecordingActTypesBuilder {

    private readonly FixedList<RecordingActTypeCategory> _baseList;

    private ApplicableRecordingActTypesBuilder() {
      _baseList = RecordingActTypeCategory.GetList("RecordingActTypesCategories.List");
    }


    static internal ApplicableRecordingActTypeList ApplicableRecordingActTypes(Instrument instrument) {
      var builder = new ApplicableRecordingActTypesBuilder();

      return builder.BuildFor(instrument);
    }


    static internal ApplicableRecordingActTypeList ApplicableRecordingActTypes(BookEntry bookEntry) {
      var builder = new ApplicableRecordingActTypesBuilder();

      return builder.BuildFor(bookEntry);
    }


    static internal ApplicableRecordingActTypeList ApplicableRecordingActTypes(Resource recordableSubject) {
      var builder = new ApplicableRecordingActTypesBuilder();

      return builder.BuildFor(recordableSubject);
    }


    internal ApplicableRecordingActTypeList BuildFor(Instrument instrument) {
      var list = new ApplicableRecordingActTypeList();

      foreach (var category in _baseList) {
        foreach (var recordingActType in category.RecordingActTypes) {
          var item = new ApplicableRecordingActType(recordingActType,
                                                    recordingActType.RegistrationCommandTypes(),
                                                    category);
          list.Add(item);
        }
      }

      return list;
    }


    internal ApplicableRecordingActTypeList BuildFor(Resource recordableSubject) {
      var list = new ApplicableRecordingActTypeList();

      foreach (var category in _baseList) {
        foreach (var recordingActType in category.RecordingActTypes) {
          var item = new ApplicableRecordingActType(recordingActType,
                                                    recordingActType.TractIndexRegistrationCommandTypes(),
                                                    category);

          if (IsApplicableTo(item.RecordingActType, recordableSubject)) {
            list.Add(item);
          }
        }
      }

      return list;
    }


    internal ApplicableRecordingActTypeList BuildFor(BookEntry bookEntry) {
      var list = new ApplicableRecordingActTypeList();

      foreach (var category in _baseList) {
        foreach (var recordingActType in category.RecordingActTypes) {
          var item = new ApplicableRecordingActType(recordingActType,
                                                    recordingActType.RegistrationCommandTypes(),
                                                    category);
          list.Add(item);
        }
      }

      return list;
    }


    private bool IsApplicableTo(RecordingActType recordingActType,
                                Resource recordableSubject) {

      if (recordableSubject is RealEstate &&
          (recordingActType.AppliesTo == RecordingRuleApplication.RealEstate ||
           recordingActType.AppliesTo == RecordingRuleApplication.RealEstateAct)) {
        return true;
      }

      if (recordableSubject is NoPropertyResource &&
          (recordingActType.AppliesTo == RecordingRuleApplication.NoProperty ||
           recordingActType.AppliesTo == RecordingRuleApplication.NoPropertyAct)) {
        return true;
      }

      if (recordableSubject is Association &&
          (recordingActType.AppliesTo == RecordingRuleApplication.Association ||
           recordingActType.AppliesTo == RecordingRuleApplication.AssociationAct)) {
        return true;
      }

      return false;
    }

  }  // class ApplicableRecordingActTypesBuilder

}  // ApplicableRecordingActTypesBuilder
