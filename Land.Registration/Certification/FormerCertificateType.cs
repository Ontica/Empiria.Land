/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Certification Services              *
*  Namespace : Empiria.Land.Certification                     Assembly : Empiria.Land.Certification.dll      *
*  Type      : FormerCertificateType                          Pattern  : Power type                          *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Former Power type that describes certificates types.                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Ontology;

namespace Empiria.Land.Certification {

  ///<summary>Former Power type that describes certificates types.</summary>
  [Powertype(typeof(FormerCertificate))]
  public sealed class FormerCertificateType : Powertype {

    #region Constructors and parsers

    private FormerCertificateType() {
      // Empiria powertype types always have this constructor.
    }

    static public new FormerCertificateType Parse(int typeId) {
      return ObjectTypeInfo.Parse<FormerCertificateType>(typeId);
    }

    static internal new FormerCertificateType Parse(string typeName) {
      return ObjectTypeInfo.Parse<FormerCertificateType>(typeName);
    }

    static public FormerCertificateType Empty {
      get {
        return FormerCertificateType.Parse("ObjectType.LandCertificate");
      }
    }

    #endregion Constructors and parsers

    #region Methods

    public string GetHtmlTemplateFileName() {
      var json = base.ExtensionData;

      return json.Get<String>("HtmlTemplateFileName");
    }

    #endregion Methods

  } // class FormerCertificateType

} // namespace Empiria.Land.Certification
