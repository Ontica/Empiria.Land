﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Registration                          Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Enumeration                             *
*  Type     : RegistrationCommandType                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Enumerates the available registration commands.                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System.Collections.Generic;
using Empiria.Land.RecordableSubjects.Adapters;
using Empiria.Land.Registration.Adapters;

namespace Empiria.Land.Registration {

  public enum RegistrationCommandType {

    Undefined = 0,

    CreateAssociation,
    SelectAssociation,
    SelectAssociationAntecedent,
    SelectAssociationAct,

    CreateNoProperty,
    SelectNoProperty,
    SelectNoPropertyAntecedent,
    SelectNoPropertyAct,

    CreateRealEstate,
    SelectRealEstate,
    SelectRealEstateAntecedent,
    SelectRealEstateAct,

    CreateRealEstatePartition,
    CreateRealEstatePartitionForAntecedent,


    ///  Tract index edition commands

    AssociationTractIndex,
    AmendAssociationTractIndexAct,

    NoPropertyTractIndex,
    AmendNoPropertyTractIndexAct,

    RealEstateTractIndex,
    AmendRealEstateTractIndexAct,
    RealEstateTractIndexPartition,

  }  // enum RegistrationCommandType


  static internal class RegistrationCommandTypeExtensions {


    static internal FixedList<RegistrationCommandType> RegistrationCommandTypes(this RecordingActType recordingActType) {
      var rule = recordingActType.RecordingRule;

      var list = new List<RegistrationCommandType>();

      if (!rule.IsActive) {
        list.Add(RegistrationCommandType.Undefined);
        return list.ToFixedList();
      }

      switch (rule.AppliesTo) {
        case RecordingRuleApplication.Association:
          LoadAssociationCommandTypes(list, rule);
          break;

        case RecordingRuleApplication.RealEstate:
        case RecordingRuleApplication.Structure:
          LoadRealEstateCommandTypes(list, rule);
          break;

        case RecordingRuleApplication.NoProperty:
          LoadNoPropertyCommandTypes(list, rule);
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
          if (rule.AllowPartitions) {
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


    static internal string Name(this RegistrationCommandType commandType) {
      switch (commandType) {

        case RegistrationCommandType.CreateAssociation:
          return "Sociedad civil que NUNCA ha estado inscrita en el RPP (primera vez)";

        case RegistrationCommandType.SelectAssociation:
          return "Sociedad inscrita con folio electrónico";

        case RegistrationCommandType.SelectAssociationAntecedent:
          return "Sociedad registrada en antecedente, sin folio electrónico";

        case RegistrationCommandType.SelectAssociationAct:
          return "Sobre un acto jurídico de una sociedad";

        case RegistrationCommandType.CreateNoProperty:
          return "Documento que NUNCA ha esto inscrito en el RPP (primera vez)";

        case RegistrationCommandType.SelectNoProperty:
          return "Documento inscrito con folio electrónico";

        case RegistrationCommandType.SelectNoPropertyAntecedent:
          return "Documento registrado en antecedente, sin folio electrónico";

        case RegistrationCommandType.SelectNoPropertyAct:
          return "Sobre un acto jurídico de documentos y otros";

        case RegistrationCommandType.CreateRealEstate:
          return "Predio que NUNCA ha sido inscrito en el RPP (primera vez)";

        case RegistrationCommandType.SelectRealEstate:
          return "Predio inscrito con folio real";

        case RegistrationCommandType.CreateRealEstatePartition:
          return "Fracción de predio inscrito con folio real";

        case RegistrationCommandType.CreateRealEstatePartitionForAntecedent:
          return "Fracción de predio registrado en antecedente";

        case RegistrationCommandType.SelectRealEstateAct:
          return "Sobre un acto jurídico de predio con folio real";

        case RegistrationCommandType.SelectRealEstateAntecedent:
          return "Predio registrado en antecedente, sin folio real";

        case RegistrationCommandType.AssociationTractIndex:
          return "Sobre esta sociedad";

        case RegistrationCommandType.AmendAssociationTractIndexAct:
          return "Sobre un acto jurídico de esta sociedad";

        case RegistrationCommandType.NoPropertyTractIndex:
          return "Sobre este documento";

        case RegistrationCommandType.AmendNoPropertyTractIndexAct:
          return "Sobre un acto jurídico de este documento";

        case RegistrationCommandType.RealEstateTractIndex:
          return "Sobre este predio";

        case RegistrationCommandType.RealEstateTractIndexPartition:
          return "Sobre una fracción de este predio";

        case RegistrationCommandType.AmendRealEstateTractIndexAct:
          return "Sobre un acto jurídico de este predio";

        case RegistrationCommandType.Undefined:
          return "La regla de registro no ha sido definida";

        default:
          throw Assertion.EnsureNoReachThisCode($"Unhandled registration command type '{commandType}'.");
      }
    }


    static internal RegistrationCommandRuleDto Rules(this RegistrationCommandType commandType) {
      switch (commandType) {
        case RegistrationCommandType.Undefined:
          return new RegistrationCommandRuleDto();


        case RegistrationCommandType.CreateAssociation:
          return new RegistrationCommandRuleDto {
            SubjectType = RecordableSubjectType.Association
          };

        case RegistrationCommandType.SelectAssociation:
          return new RegistrationCommandRuleDto {
            SubjectType = RecordableSubjectType.Association,
            SelectSubject = true
          };

        case RegistrationCommandType.SelectAssociationAntecedent:
          return new RegistrationCommandRuleDto {
            SubjectType = RecordableSubjectType.Association,
            SelectBookEntry = true
          };

        case RegistrationCommandType.SelectAssociationAct:
          return new RegistrationCommandRuleDto {
            SubjectType = RecordableSubjectType.Association,
            SelectSubject = true,
            SelectTargetAct = true
          };

        case RegistrationCommandType.CreateNoProperty:
          return new RegistrationCommandRuleDto {
            SubjectType = RecordableSubjectType.NoProperty
          };

        case RegistrationCommandType.SelectNoProperty:
          return new RegistrationCommandRuleDto {
            SubjectType = RecordableSubjectType.NoProperty,
            SelectSubject = true
          };

        case RegistrationCommandType.SelectNoPropertyAntecedent:
          return new RegistrationCommandRuleDto {
            SubjectType = RecordableSubjectType.NoProperty,
            SelectBookEntry = true
          };


        case RegistrationCommandType.SelectNoPropertyAct:
          return new RegistrationCommandRuleDto {
            SubjectType = RecordableSubjectType.NoProperty,
            SelectSubject = true,
            SelectTargetAct = true
          };

        case RegistrationCommandType.CreateRealEstate:
          return new RegistrationCommandRuleDto {
            SubjectType = RecordableSubjectType.RealEstate
         };

        case RegistrationCommandType.SelectRealEstate:
          return new RegistrationCommandRuleDto {
            SubjectType = RecordableSubjectType.RealEstate,
            SelectSubject = true
          };

        case RegistrationCommandType.SelectRealEstateAntecedent:
          return new RegistrationCommandRuleDto {
            SubjectType = RecordableSubjectType.RealEstate,
            SelectBookEntry = true
          };

        case RegistrationCommandType.CreateRealEstatePartition:
          return new RegistrationCommandRuleDto {
            SubjectType = RecordableSubjectType.RealEstate,
            SelectSubject = true,
            NewPartition = true
          };


        case RegistrationCommandType.CreateRealEstatePartitionForAntecedent:
          return new RegistrationCommandRuleDto {
            SubjectType = RecordableSubjectType.RealEstate,
            SelectBookEntry = true,
            NewPartition = true
          };

        case RegistrationCommandType.SelectRealEstateAct:
          return new RegistrationCommandRuleDto {
            SubjectType = RecordableSubjectType.RealEstate,
            SelectSubject = true,
            SelectTargetAct = true
          };

        case RegistrationCommandType.AssociationTractIndex:
          return new RegistrationCommandRuleDto {
            SubjectType = RecordableSubjectType.Association,
            SelectBookEntry = true
          };

        case RegistrationCommandType.AmendAssociationTractIndexAct:
          return new RegistrationCommandRuleDto {
            SubjectType = RecordableSubjectType.Association,
            SelectBookEntry = true,
            SelectTargetAct = true
          };

        case RegistrationCommandType.NoPropertyTractIndex:
          return new RegistrationCommandRuleDto {
            SubjectType = RecordableSubjectType.NoProperty,
            SelectBookEntry = true
          };


        case RegistrationCommandType.AmendNoPropertyTractIndexAct:
          return new RegistrationCommandRuleDto {
            SubjectType = RecordableSubjectType.NoProperty,
            SelectBookEntry = true,
            SelectTargetAct = true
          };

        case RegistrationCommandType.RealEstateTractIndex:
          return new RegistrationCommandRuleDto {
            SubjectType = RecordableSubjectType.RealEstate,
            SelectBookEntry = true
          };

        case RegistrationCommandType.RealEstateTractIndexPartition:
          return new RegistrationCommandRuleDto {
            SubjectType = RecordableSubjectType.RealEstate,
            SelectBookEntry = true,
            NewPartition = true
          };


        case RegistrationCommandType.AmendRealEstateTractIndexAct:
          return new RegistrationCommandRuleDto {
            SubjectType = RecordableSubjectType.RealEstate,
            SelectBookEntry = true,
            SelectTargetAct = true
          };

        default:
          throw Assertion.EnsureNoReachThisCode($"Unhandled registration command type '{commandType}'.");
      }
    }


    #region Helper Methods

    static private void LoadAssociationCommandTypes(List<RegistrationCommandType> list, RecordingRule rule) {
      if (SelectUnregistered(rule)) {
        list.Add(RegistrationCommandType.CreateAssociation);
      }
      if (SelectRegistered(rule)) {
        list.Add(RegistrationCommandType.SelectAssociation);
      }
      if (SelectAntecedent(rule)) {
        list.Add(RegistrationCommandType.SelectAssociationAntecedent);
      }
    }


    static private void LoadNoPropertyCommandTypes(List<RegistrationCommandType> list, RecordingRule rule) {
      if (SelectUnregistered(rule)) {
        list.Add(RegistrationCommandType.CreateNoProperty);
      }
      if (SelectRegistered(rule)) {
        list.Add(RegistrationCommandType.SelectNoProperty);
      }
      if (SelectAntecedent(rule)) {
        list.Add(RegistrationCommandType.SelectNoPropertyAntecedent);
      }
    }


    static private void LoadRealEstateCommandTypes(List<RegistrationCommandType> list, RecordingRule rule) {
      if (SelectUnregistered(rule)) {
        list.Add(RegistrationCommandType.CreateRealEstate);
      }
      if (SelectRegistered(rule)) {
        list.Add(RegistrationCommandType.SelectRealEstate);
      }
      if (SelectAntecedent(rule)) {
        list.Add(RegistrationCommandType.SelectRealEstateAntecedent);
      }
      if (rule.AllowPartitions) {
        list.Add(RegistrationCommandType.CreateRealEstatePartition);
      }
      if (rule.AllowPartitions && SelectAntecedent(rule)) {
        list.Add(RegistrationCommandType.CreateRealEstatePartitionForAntecedent);
      }
    }


    static private bool SelectAntecedent(RecordingRule rule) {
      return rule.ResourceRecordingStatus == ResourceRecordingStatus.Antecedent;
    }


    static private bool SelectRegistered(RecordingRule rule) {
      return rule.ResourceRecordingStatus == ResourceRecordingStatus.Registered ||
             rule.ResourceRecordingStatus == ResourceRecordingStatus.Both;
    }


    static private bool SelectUnregistered(RecordingRule rule) {
      return rule.ResourceRecordingStatus == ResourceRecordingStatus.Unregistered ||
             rule.ResourceRecordingStatus == ResourceRecordingStatus.Both;
    }

    #endregion Helper Methods

  }  // class RegistrationCommandTypeExtensions

} // namespace Empiria.Land.Registration
