/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Certificates Issuing                       Component : Use cases Layer                         *
*  Assembly : Empiria.Land.Certificates.dll              Pattern   : Use case interactor class               *
*  Type     : TransactionCertificatesUseCases            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases that links transactions with Land certificates.                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

namespace Empiria.Land.Certificates.UseCases {

  /// <summary>Use cases that links transactions with certificates.</summary>
  public class TransactionCertificatesUseCases : UseCase {

    #region Constructors and parsers

    static public TransactionCertificatesUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<TransactionCertificatesUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public FixedList<CertificateTypeDto> CertificateTypes(string transactionUID) {
      throw new NotImplementedException();
    }

    #endregion Use cases

  }  // class TransactionCertificatesUseCases

}  // namespace Empiria.Land.Certificates.UseCases
