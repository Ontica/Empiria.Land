/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Commons                                      Component : Electronic sign                       *
*  Assembly : Empiria.Land.Core.dll                        Pattern   : Enumeration                           *
*  Type     : SignStatus                                   License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Enumerates the statuses of a signed land document or certificate.                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land {

  /// <summary>Enumerates the statuses of a signed land document or certificate.</summary>
  public enum SignStatus {

    Unsigned = 'U',

    Signed = 'S',

    Refused = 'F',

    Revoked = 'K',

    Undefined = 'X',

  }  // enum SignStatus

}  // namespace Empiria.Land
