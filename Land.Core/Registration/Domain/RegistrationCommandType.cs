/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Registration                          Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Enumeration                             *
*  Type     : RegistrationCommandType                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Enumerates the available registration commands.                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.Registration {

  public enum RegistrationCommandType {

    Undefined = 0,

    CreateAssociation,
    SelectAssociation,
    SelectAssociationAct,

    CreateNoProperty,
    SelectNoProperty,
    SelectNoPropertyAct,

    CreateRealEstate,
    SelectRealEstate,
    SelectRealEstateAct,
    SelectRealEstatePartition,

  }  // enum RegistrationCommandType


  static internal class RegistrationCommandTypeExtensions {

    static internal string Name(this RegistrationCommandType commandType) {
      switch (commandType) {
        case RegistrationCommandType.Undefined:
          return "La regla de registro no ha sido definida";

        case RegistrationCommandType.CreateAssociation:
          return "Asociación a inscibirse por primera vez";

        case RegistrationCommandType.SelectAssociation:
          return "Asociación inscrita con folio electrónico";

        case RegistrationCommandType.SelectAssociationAct:
          return "Sobre un acto jurídico de una asociación";

        case RegistrationCommandType.CreateNoProperty:
          return "Otros a inscribirse por primera vez";

        case RegistrationCommandType.SelectNoProperty:
          return "Otros inscritos con folio electrónico";

        case RegistrationCommandType.SelectNoPropertyAct:
          return "Sobre un acto jurídico de documentos y otros";

        case RegistrationCommandType.CreateRealEstate:
          return "Predio a inscribirse por primera vez";

        case RegistrationCommandType.SelectRealEstate:
          return "Predio inscrito con folio real";

        case RegistrationCommandType.SelectRealEstatePartition:
          return "Fracción de predio inscrito con folio real";

        case RegistrationCommandType.SelectRealEstateAct:
          return "Sobre un acto jurídico de predio con folio real";


        default:
          throw Assertion.AssertNoReachThisCode($"Unhandled registration command type '{commandType}'.");
      }
    }

  }  // class RegistrationCommandTypeExtensions

} // namespace Empiria.Land.Registration
