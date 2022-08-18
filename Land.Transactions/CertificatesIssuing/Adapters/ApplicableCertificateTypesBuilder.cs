/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Certificates Issuing                       Component : Domain  Layer                           *
*  Assembly : Empiria.Land.Certificates.dll              Pattern   : Builder                                 *
*  Type     : ApplicableCertificateTypesBuilder          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Builds lists of certificate types for a given context.                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Certificates {

  /// <summary>Builds lists of certificate types for a given context.</summary>
  internal class ApplicableCertificateTypesBuilder {

    private readonly FixedList<CertificateType> _baseList;

    internal ApplicableCertificateTypesBuilder(FixedList<CertificateType> baseList) {
      Assertion.Require(baseList, nameof(baseList));

      _baseList = baseList;
    }


    #region Methods

    internal FixedList<CertificateTypeDto> BuildFor(LRSTransaction transaction) {
      Assertion.Require(transaction, nameof(transaction));

      var list = new List<CertificateTypeDto>(_baseList.Count);

      foreach (CertificateType certificateType in _baseList) {
        if (IsApplicableTo(certificateType, transaction)) {
          CertificateTypeDto dto = BuildCertificateTypeDto(certificateType);
          list.Add(dto);
        }
      }

      return list.ToFixedList();
    }

    #endregion Methods

    #region Helpers

    private CertificateTypeDto BuildCertificateTypeDto(CertificateType certificateType) {
      return new CertificateTypeDto {
        UID = certificateType.UID,
        Name = certificateType.DisplayName,
        IssuingCommands = BuildIssuingCommands(certificateType)
      };
    }


    private FixedList<CertificateIssuingCommandDto> BuildIssuingCommands(CertificateType certificateType) {
      var commands = new List<CertificateIssuingCommandDto>();

      commands.Add(new CertificateIssuingCommandDto {
        UID = CreateCertificateCommandType.OverRegisteredRealEstate.ToString(),
        Name = CreateCertificateCommandType.OverRegisteredRealEstate.Name(),
        Rules = CreateCertificateCommandType.OverRegisteredRealEstate.Rules(),
      });

      commands.Add(new CertificateIssuingCommandDto {
        UID = CreateCertificateCommandType.OverRealEstateAntecedent.ToString(),
        Name = CreateCertificateCommandType.OverRealEstateAntecedent.Name(),
        Rules = CreateCertificateCommandType.OverRealEstateAntecedent.Rules()
      });

      return commands.ToFixedList();
    }


    private bool IsApplicableTo(CertificateType certificateType,
                                LRSTransaction transaction) {
      return true;
    }

    #endregion Helpers

  }  // class ApplicableCertificateTypesBuilder

}  // namespace Empiria.Land.Certificates
