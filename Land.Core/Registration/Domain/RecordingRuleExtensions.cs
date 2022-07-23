/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Registration                          Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Extension methods library               *
*  Type     : RecordingRuleExtensions                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Extension methods library for registration rules.                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.Land.Instruments;

namespace Empiria.Land.Registration {

  /// <summary>Extension methods library for registration rules.</summary>
  static internal class RecordingRuleExtensions {

    #region Methods

    static internal FixedList<RecordingActTypeCategory> ApplicableRecordingActTypes(this Instrument instrument) {
      return RecordingActTypeCategory.GetList("RecordingActTypesCategories.List");
    }


    static internal FixedList<RecordingActTypeCategory> ApplicableRecordingActTypes(this PhysicalRecording physicalRecording) {
      return RecordingActTypeCategory.GetList("RecordingActTypesCategories.List");
    }


    static internal FixedList<RecordingActTypeCategory> ApplicableRecordingActTypes(this Resource recordableSubject) {
      return RecordingActTypeCategory.GetList("RecordingActTypesCategories.List");
    }


    static internal FixedList<RegistrationCommandType> RegistrationCommandTypes(this RecordingActType recordingActType) {
      var rule = recordingActType.RecordingRule;

      var list = new List<RegistrationCommandType>();

      if (!rule.IsActive) {
        list.Add(RegistrationCommandType.Undefined);
        return list.ToFixedList();
      }

      switch (rule.AppliesTo) {
        case RecordingRuleApplication.Association:
          if (UnregisteredOrBoth(rule)) {
            list.Add(RegistrationCommandType.CreateAssociation);
          }
          if (RegisteredOrBoth(rule)) {
            list.Add(RegistrationCommandType.SelectAssociation);
            list.Add(RegistrationCommandType.SelectAssociationAntecedent);
          }
          break;

        case RecordingRuleApplication.RealEstate:
        case RecordingRuleApplication.Structure:
          if (UnregisteredOrBoth(rule)) {
            list.Add(RegistrationCommandType.CreateRealEstate);
          }
          if (RegisteredOrBoth(rule)) {
            list.Add(RegistrationCommandType.SelectRealEstate);
            list.Add(RegistrationCommandType.SelectRealEstateAntecedent);
          }
          if (rule.AllowsPartitions) {
            list.Add(RegistrationCommandType.CreateRealEstatePartition);
            list.Add(RegistrationCommandType.CreateRealEstatePartitionForAntecedent);
          }
          break;

        case RecordingRuleApplication.NoProperty:
          if (UnregisteredOrBoth(rule)) {
            list.Add(RegistrationCommandType.CreateNoProperty);
          }
          if (RegisteredOrBoth(rule)) {
            list.Add(RegistrationCommandType.SelectNoProperty);
            list.Add(RegistrationCommandType.SelectNoPropertyAntecedent);
          }
          break;

        case RecordingRuleApplication.RealEstateAct:
          list.Add(RegistrationCommandType.SelectRealEstateAct);
          break;

        case RecordingRuleApplication.AssociationAct:
          list.Add(RegistrationCommandType.SelectAssociationAct);
          break;

        case RecordingRuleApplication.NoPropertyAct:
          list.Add(RegistrationCommandType.SelectNoPropertyAct);

          break;
      }

      if (list.Count == 0) {
        list.Add(RegistrationCommandType.Undefined);
      }

      return list.ToFixedList();
    }


    static internal FixedList<RegistrationCommandType> TractIndexRegistrationCommandTypes(this RecordingActType recordingActType) {
      var rule = recordingActType.RecordingRule;

      var list = new List<RegistrationCommandType>();

      if (!rule.IsActive) {
        list.Add(RegistrationCommandType.Undefined);
        return list.ToFixedList();
      }

      switch (rule.AppliesTo) {
        case RecordingRuleApplication.Association:
          list.Add(RegistrationCommandType.AssociationTractIndex);
          break;

        case RecordingRuleApplication.AssociationAct:
          list.Add(RegistrationCommandType.AmendAssociationTractIndexAct);
          break;


        case RecordingRuleApplication.NoProperty:
          list.Add(RegistrationCommandType.NoPropertyTractIndex);
          break;

        case RecordingRuleApplication.NoPropertyAct:
          list.Add(RegistrationCommandType.AmendNoPropertyTractIndexAct);
          break;

        case RecordingRuleApplication.RealEstate:
        case RecordingRuleApplication.Structure:
          list.Add(RegistrationCommandType.RealEstateTractIndex);
          if (rule.AllowsPartitions) {
            list.Add(RegistrationCommandType.RealEstateTractIndexPartition);
          }
          break;

        case RecordingRuleApplication.RealEstateAct:
          list.Add(RegistrationCommandType.AmendRealEstateTractIndexAct);
          break;

      }

      if (list.Count == 0) {
        list.Add(RegistrationCommandType.Undefined);
      }

      return list.ToFixedList();
    }


    #endregion Methods

    #region Helper Methods

    static private bool RegisteredOrBoth(RecordingRule rule) {
      return rule.ResourceRecordingStatus == ResourceRecordingStatus.Registered ||
             rule.ResourceRecordingStatus == ResourceRecordingStatus.Both;
    }


    static private bool UnregisteredOrBoth(RecordingRule rule) {
      return rule.ResourceRecordingStatus == ResourceRecordingStatus.Unregistered ||
             rule.ResourceRecordingStatus == ResourceRecordingStatus.Both;
    }

    #endregion Helper Methods

  }  // class RecordingRuleExtensions

}  // namespace Empiria.Land.Registration
