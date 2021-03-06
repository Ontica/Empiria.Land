﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Certification Services              *
*  Namespace : Empiria.Land.Certification                     Assembly : Empiria.Land.Certification.dll      *
*  Type      : LandCertificateType                            Pattern  : Power type                          *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Power type that describes certificates types.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Ontology;

namespace Empiria.Land.Certification {

  ///<summary>Power type that describes certificates types.</summary>
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
      var json = base.ExtensionData;

      return json.Get<String>("HtmlTemplateFileName");
    }

    #endregion Methods

  } // class CertificateType

} // namespace Empiria.Land.Certification
