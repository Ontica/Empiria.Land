/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Services                       *
*  Namespace : Empiria.Land.Services                          Assembly : Empiria.Land.Services.dll           *
*  Type      : ExternalTransactionType                        Pattern  : Enumeration Type                    *
*  Version   : 2.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : List of services provided to external systems.                                                *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.WebApi.Models {

  /// <summary>List of services provided to external systems.</summary>
  public enum ExternalTransactionType {

    /// <summary>Used to initialize variables of this type.</summary>
    Undefined = -1,

    /// <summary>/// Aka as Solicitud de folio real.</summary>
    GenerateRealPropertyUID = 1,

    /// <summary>/// Aka as Emisión de Certificados.</summary>
    CertificateIssuing = 2,

    /// <summary>/// Aka as Registro de Avisos preventivos.</summary>
    PendingSaleNoteRecording = 4,

  }  // enum ExternalTransactionType

}  // namespace Empiria.Land.Services
