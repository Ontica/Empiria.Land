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

    private readonly string _recordingOfficeTag;

    public UniqueIDGeneratorProvider(string recordingOfficeTag) {
      Assertion.Require(recordingOfficeTag, nameof(recordingOfficeTag));
      Assertion.Require(recordingOfficeTag.Length == 2, "recordingOfficeTag must be two chars long.");

      _recordingOfficeTag = recordingOfficeTag;
    }


    #endregion Constructor and fields

    #region Methods

    public string GenerateAssociationID() {
      var generator = new RecordableSubjectIDGenerator(_recordingOfficeTag);

      return generator.GenerateAssociationID("PM");
    }


    public string GenerateCertificateID() {
      var generator = new CertificateIDGenerator(_recordingOfficeTag);

      return generator.GenerateID("CE");
    }


    public string GenerateRecordID() {
      var generator = new RecordIDGenerator(_recordingOfficeTag);

      return generator.GenerateID("RP");
    }


    public string GenerateNoPropertyID() {
      var generator = new RecordableSubjectIDGenerator(_recordingOfficeTag);

      return generator.GenerateNoPropertyID("RD");
    }


    public string GenerateRealEstateID() {
      var generator = new RecordableSubjectIDGenerator(_recordingOfficeTag);

      return generator.GenerateRealEstateID("FR");
    }


    public string GenerateTransactionID() {
      var generator = new TransactionIDGenerator(_recordingOfficeTag);

      return generator.GenerateID("TR");
    }

    #endregion Methods

  }  // class UniqueIDGeneratorProvider

}  //namespace Empiria.Land.Providers
