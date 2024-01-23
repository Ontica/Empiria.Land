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

    internal ApplicableRecordingActTypesBuilder(FixedList<RecordingActTypeCategory> baseList) {
      Assertion.Require(baseList, nameof(baseList));

      _baseList = baseList;
    }


    internal ApplicableRecordingActTypeList BuildFor(Instrument instrument) {
      var list = new ApplicableRecordingActTypeList();

      foreach (var category in _baseList) {
        foreach (var actType in category.RecordingActTypes) {
          var item = new ApplicableRecordingActType(actType,
                                                    actType.RegistrationCommandTypes(),
                                                    category);
          list.Add(item);
        }
      }

      return list;
    }



    internal ApplicableRecordingActTypeList BuildFor(Resource recordableSubject) {
      var list = new ApplicableRecordingActTypeList();

      foreach (var category in _baseList) {
        foreach (var actType in category.RecordingActTypes) {
          var item = new ApplicableRecordingActType(actType,
                                                    actType.TractIndexRegistrationCommandTypes(),
                                                    category);

          if (IsApplicableTo(recordableSubject, item)) {
            list.Add(item);
          }
        }
      }

      return list;
    }

    internal ApplicableRecordingActTypeList BuildFor(BookEntry bookEntry) {
      var list = new ApplicableRecordingActTypeList();

      foreach (var category in _baseList) {
        foreach (var actType in category.RecordingActTypes) {
          var item = new ApplicableRecordingActType(actType,
                                                    actType.RegistrationCommandTypes(),
                                                    category);
          list.Add(item);
        }
      }

      return list;
    }


    private bool IsApplicableTo(Resource recordableSubject,
                                ApplicableRecordingActType actType) {
      if (recordableSubject is RealEstate &&
          (actType.RecordingActType.AppliesTo == RecordingRuleApplication.RealEstate ||
           actType.RecordingActType.AppliesTo == RecordingRuleApplication.RealEstateAct)) {
        return true;
      }

      if (recordableSubject is NoPropertyResource &&
          (actType.RecordingActType.AppliesTo == RecordingRuleApplication.NoProperty ||
           actType.RecordingActType.AppliesTo == RecordingRuleApplication.NoPropertyAct)) {
        return true;
      }

      if (recordableSubject is Association &&
          (actType.RecordingActType.AppliesTo == RecordingRuleApplication.Association ||
           actType.RecordingActType.AppliesTo == RecordingRuleApplication.AssociationAct)) {
        return true;
      }

      return false;
    }

  }  // class ApplicableRecordingActTypesBuilder

}  // ApplicableRecordingActTypesBuilder
