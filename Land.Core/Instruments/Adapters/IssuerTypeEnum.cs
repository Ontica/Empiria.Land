﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Legal Instruments                          Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Type Instances Enumeration              *
*  Type     : IssuerTypeEnum                             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Enumerates the instrument issuers types.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.Instruments.Adapters {

  /// <summary>Enumerates the instrument issuers types.</summary>
  public enum IssuerTypeEnum {

    Notary,

    Judge,

    PropertyTitleIssuer,

    AdministrativeAuthority,

    ThirdParty,

    All

  }  // enum IssuerTypeEnum

}  // namespace Empiria.Land.Instruments.Adapters
