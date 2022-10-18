/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Registration                          Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Enumeration                             *
*  Type     : RegistrationCommandType                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Enumerates the available registration commands.                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

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


  }  // class RegistrationCommandTypeExtensions

} // namespace Empiria.Land.Registration
