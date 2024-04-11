/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Certificates Issuing                       Component : Domain Layer                            *
*  Assembly : Empiria.Land.Certificates.dll              Pattern   : Powertype                               *
*  Type     : CertificateType                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Powertype that defines a Land certificate type.                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Ontology;

using Empiria.Land.Providers;

namespace Empiria.Land.Certificates {

  ///<summary>Power type that describes certificates types.</summary>
  [Powertype(typeof(Certificate))]
  public sealed class CertificateType : Powertype {

    #region Constructors and parsers

    private CertificateType() {
      // Empiria powertype types always have this constructor.
    }


    static internal new CertificateType Parse(int typeId) {
      return ObjectTypeInfo.Parse<CertificateType>(typeId);
    }


    static internal new CertificateType Parse(string typeName) {
      return ObjectTypeInfo.Parse<CertificateType>(typeName);
    }


    static public FixedList<CertificateType> GetList() {
      GeneralList list = GeneralList.Parse("CertificateTypes.Default.List");

      return list.GetItems<CertificateType>();
    }


    static internal CertificateType Empty => CertificateType.Parse("ObjectType.LandCertificate");


    #endregion Constructors and parsers

    #region Properties

    internal string HtmlTemplateFileName {
      get {
       return base.ExtensionData.Get<String>("HtmlTemplateFileName");
      }
    }

    #endregion Properties


    #region Methods

    internal string CreateCertificateID() {
      IUniqueIDGeneratorProvider provider = ExternalProviders.GetUniqueIDGeneratorProvider();

      return provider.GenerateCertificateID();
    }

    #endregion Methods

  } // class CertificateType

} // namespace Empiria.Land.Certificates
