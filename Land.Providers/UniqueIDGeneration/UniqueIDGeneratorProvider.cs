/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Providers                             Component : UniqueID Generator Provider             *
*  Assembly : Empiria.Land.Providers.dll                 Pattern   : Provider implementation                 *
*  Type     : UniqueIDGeneratorProvider                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides unique ID generation services for land-related documents and objects.                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Providers {

  /// <summary>Provides unique ID generation services for land-related documents and objects.</summary>
  public class UniqueIDGeneratorProvider : IUniqueIDGeneratorProvider {

    #region Constructor and fields

    private readonly string CUSTOMER_ID;

    public UniqueIDGeneratorProvider() {
      switch (ExecutionServer.LicenseName) {
        case "Tlaxcala":
          CUSTOMER_ID = "TL";
          break;

        case "Zacatecas":
          CUSTOMER_ID = "ZS";
          break;

        default:
          throw Assertion.EnsureNoReachThisCode(
            $"Unhandled license name {ExecutionServer.LicenseName}.");
      }
    }


    #endregion Constructor and fields

    #region Methods

    public string GenerateAssociationUID() {
      var generator = new RecordableSubjectIDGenerator(CUSTOMER_ID);

      return generator.GenerateAssociationID();
    }


    public string GenerateCertificateUID() {
      var generator = new CertificateIDGenerator(CUSTOMER_ID);

      return generator.GenerateID();
    }


    public string GenerateDocumentUID() {
      var generator = new RecordingDocumentIDGenerator(CUSTOMER_ID);

      return generator.GenerateID();
    }


    public string GenerateNoPropertyResourceUID() {
      var generator = new RecordableSubjectIDGenerator(CUSTOMER_ID);

      return generator.GenerateNoPropertyID();
    }


    public string GeneratePropertyUID() {
      var generator = new RecordableSubjectIDGenerator(CUSTOMER_ID);

      return generator.GenerateRealEstateID();
    }


    public string GenerateTransactionUID() {
      var generator = new TransactionIDGenerator(CUSTOMER_ID);

      return generator.GenerateID();
    }

    #endregion Methods

  }  // class UniqueIDGeneratorProvider

}  //namespace Empiria.Land.Providers
