/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Certificates Issuing                       Component : Domain Layer                            *
*  Assembly : Empiria.Land.Certificates.dll              Pattern   : Extended enumeration                   *
*  Type     : CertificateCommandType                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Enumerates the available commands used for create land certificates.                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.RecordableSubjects.Adapters;

namespace Empiria.Land.Certificates {

  /// <summary>Enumerates the available commands used for create land certificates.</summary>
  public enum CreateCertificateCommandType {

    Undefined = 0,

    OverRegisteredRealEstate,

    OverRealEstateAntecedent

  }  // enum CreateCertificateCommandType


  static internal class CreateCertificateCommandTypeExtensions {

    static internal string Name(this CreateCertificateCommandType commandType) {
      switch (commandType) {

        case CreateCertificateCommandType.OverRegisteredRealEstate:
          return "Predio inscrito con folio real";

        case CreateCertificateCommandType.OverRealEstateAntecedent:
          return "Predio registrado en antecedente, sin folio real";

        case CreateCertificateCommandType.Undefined:
          return "La regla de registro no ha sido definida";

        default:
          throw Assertion.EnsureNoReachThisCode($"Unhandled registration command type '{commandType}'.");
      }
    }


    static internal CertificateIssuingCommandRuleDto Rules(this CreateCertificateCommandType commandType) {
      switch (commandType) {
        case CreateCertificateCommandType.Undefined:
          return new CertificateIssuingCommandRuleDto();

        case CreateCertificateCommandType.OverRegisteredRealEstate:
          return new CertificateIssuingCommandRuleDto {
            SubjectType = RecordableSubjectType.RealEstate,
            SelectSubject = true
          };

        case CreateCertificateCommandType.OverRealEstateAntecedent:
          return new CertificateIssuingCommandRuleDto {
            SubjectType = RecordableSubjectType.RealEstate,
            SelectBookEntry = true
          };

        default:
          throw Assertion.EnsureNoReachThisCode($"Unhandled registration command type '{commandType}'.");
      }
    }


  }  // class CreateCertificateCommandTypeExtensions

} // namespace Empiria.Land.Certificates
