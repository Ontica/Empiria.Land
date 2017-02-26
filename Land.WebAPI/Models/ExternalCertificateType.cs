/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Services                       *
*  Namespace : Empiria.Land.WebApi.Models                     Assembly : Empiria.Land.WebApi.dll             *
*  Type      : ExternalCertificateType                        Pattern  : Enumeration Type                    *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Contains the list of certificates types to be used from external systems.                     *
*                                                                                                            *
********************************* Copyright (c) 2009-2017. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.WebApi.Models {

  /// <summary>Contains the list of certificates types to be used from external systems.</summary>
  public enum ExternalCertificateType {

    /// <summary>Used to initialize variables of this type.</summary>
    Undefined = -1,

    /// <summary>Aka as Certificado de Propiedad.</summary>
    Property = 1,

    /// <summary>Aka as Certificado de Inscripción.</summary>
    Registration = 2,

    /// <summary>Aka as Certificado de Libertad de gravamen/gravamen.</summary>
    NoEncumbrance = 4,

  }  // enum ExternalCertificateType

}  // namespace Empiria.Land.WebApi.Models
