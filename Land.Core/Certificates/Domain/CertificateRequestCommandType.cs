/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Certificates                          Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Extended enumeration                    *
*  Type     : CertificateRequestCommandType              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Enumerates the commands used for request land certificates within a transaction context.       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Land.RecordableSubjects.Adapters;

using Empiria.Land.Certificates.Adapters;

namespace Empiria.Land.Certificates {

  /// <summary>Enumerates the commands used for request land certificates
  /// within a transaction context.</summary>
  public enum CertificateRequestCommandType {

    Undefined = 0,

    OverRegisteredRealEstate,

    OverRealEstateAntecedent,

    OverUnrecordedRealEstate,

    OverPersonName,

  }  // enum CreateTransactionCertificateCommandType


  static internal class CertificateRequestCommandTypeExtensions {

    static internal string Name(this CertificateRequestCommandType commandType) {
      switch (commandType) {

        case CertificateRequestCommandType.OverRegisteredRealEstate:
          return "Predio inscrito con folio real";

        case CertificateRequestCommandType.OverRealEstateAntecedent:
          return "Predio registrado en antecedente, sin folio real";

        case CertificateRequestCommandType.OverUnrecordedRealEstate:
          return "Predio no registrado";

        case CertificateRequestCommandType.OverPersonName:
          return "Persona sin predios registrados";

        case CertificateRequestCommandType.Undefined:
          return "La regla de registro no ha sido definida";

        default:
          throw Assertion.EnsureNoReachThisCode($"Unhandled registration command type '{commandType}'.");
      }
    }


    static internal CertificateRequestCommandTypeRulesDto Rules(this CertificateRequestCommandType commandType) {
      switch (commandType) {
        case CertificateRequestCommandType.Undefined:
          return new CertificateRequestCommandTypeRulesDto();

        case CertificateRequestCommandType.OverRegisteredRealEstate:
          return new CertificateRequestCommandTypeRulesDto {
            SubjectType = RecordableSubjectType.RealEstate,
            SelectSubject = true
          };

        case CertificateRequestCommandType.OverRealEstateAntecedent:
          return new CertificateRequestCommandTypeRulesDto {
            SubjectType = RecordableSubjectType.RealEstate,
            SelectBookEntry = true
          };

        case CertificateRequestCommandType.OverUnrecordedRealEstate:
          return new CertificateRequestCommandTypeRulesDto {
            SubjectType = RecordableSubjectType.None,
            GiveRealEstateDescription = true
          };

        case CertificateRequestCommandType.OverPersonName:
          return new CertificateRequestCommandTypeRulesDto {
            SubjectType = RecordableSubjectType.None,
            GivePersonName = true
          };


        default:
          throw Assertion.EnsureNoReachThisCode($"Unhandled registration command type '{commandType}'.");
      }
    }


  }  // class CertificateRequestCommandTypeExtensions

} // namespace Empiria.Land.Certificates
