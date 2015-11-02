/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Certification Services              *
*  Namespace : Empiria.Land.Certification                     Assembly : Empiria.Land.Certification.dll      *
*  Type      : LandCertificateType                            Pattern  : Power type                          *
*  Version   : 2.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Power type that describes (non) ownership certificates types.                                 *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

using Empiria.Ontology;

namespace Empiria.Land.Certification {

  ///<summary>Power type that describes (non) ownership certificates types.</summary>
  [Powertype(typeof(Certificate))]
  public sealed class CertificateType : Powertype {

    #region Constructors and parsers

    private CertificateType() {
      // Empiria powertype types always have this constructor.
    }

    static public new CertificateType Parse(int typeId) {
      return ObjectTypeInfo.Parse<CertificateType>(typeId);
    }

    static internal new CertificateType Parse(string typeName) {
      return ObjectTypeInfo.Parse<CertificateType>(typeName);
    }

    static public CertificateType Empty {
      get {
        return CertificateType.Parse("ObjectType.LandCertificate");
      }
    }

    #endregion Constructors and parsers

    #region Methods

    public string GetHtmlTemplateFileName() {
      var json = Empiria.Json.JsonObject.Parse(base.ExtensionData);

      return json.Get<String>("HtmlTemplateFileName");
    }

    #endregion Methods

  } // class CertificateType

} // namespace Empiria.Land.Certification
