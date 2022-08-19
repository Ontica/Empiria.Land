/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Certificate Requests                       Component : Domain Layer                            *
*  Assembly : Empiria.Land.Transactions.dll              Pattern   : Builder                                 *
*  Type     : ApplicableCertificateTypesBuilder          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Builds lists of certificate types for a given context.                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.Land.Registration.Transactions;

using Empiria.Land.Certificates;

namespace Empiria.Land.Transactions.CertificateRequests {

  /// <summary>Builds lists of certificate types for a given context.</summary>
  internal class ApplicableCertificateTypesBuilder {

    private readonly FixedList<CertificateType> _baseList;

    internal ApplicableCertificateTypesBuilder(FixedList<CertificateType> baseList) {
      Assertion.Require(baseList, nameof(baseList));

      _baseList = baseList;
    }


    #region Methods

    internal FixedList<CertificateRequestTypeDto> BuildFor(LRSTransaction transaction) {
      Assertion.Require(transaction, nameof(transaction));

      var list = new List<CertificateRequestTypeDto>(_baseList.Count);

      foreach (CertificateType certificateType in _baseList) {
        if (IsApplicableTo(certificateType, transaction)) {
          CertificateRequestTypeDto dto = BuildCertificateTypeDto(certificateType);
          list.Add(dto);
        }
      }

      return list.ToFixedList();
    }

    #endregion Methods

    #region Helpers

    private CertificateRequestTypeDto BuildCertificateTypeDto(CertificateType certificateType) {
      return new CertificateRequestTypeDto {
        UID = certificateType.UID,
        Name = certificateType.DisplayName,
        IssuingCommands = BuildIssuingCommands(certificateType)
      };
    }


    private FixedList<CertificateRequestCommandTypeDto> BuildIssuingCommands(CertificateType certificateType) {
      var commands = new List<CertificateRequestCommandTypeDto>();

      commands.Add(new CertificateRequestCommandTypeDto {
        UID = CertificateRequestCommandType.OverRegisteredRealEstate.ToString(),
        Name = CertificateRequestCommandType.OverRegisteredRealEstate.Name(),
        Rules = CertificateRequestCommandType.OverRegisteredRealEstate.Rules(),
      });

      commands.Add(new CertificateRequestCommandTypeDto {
        UID = CertificateRequestCommandType.OverRealEstateAntecedent.ToString(),
        Name = CertificateRequestCommandType.OverRealEstateAntecedent.Name(),
        Rules = CertificateRequestCommandType.OverRealEstateAntecedent.Rules()
      });

      return commands.ToFixedList();
    }


    private bool IsApplicableTo(CertificateType certificateType,
                                LRSTransaction transaction) {
      return true;
    }

    #endregion Helpers

  }  // class ApplicableCertificateTypesBuilder

}  // namespace Empiria.Land.Transactions.CertificateRequests
