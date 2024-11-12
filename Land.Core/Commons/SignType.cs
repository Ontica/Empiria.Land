/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Commons                                      Component : Electronic sign                       *
*  Assembly : Empiria.Land.Core.dll                        Pattern   : Enumeration                           *
*  Type     : SignType                                     License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Enumerates the different types of signs for land documents or certificates.                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land {

  /// <summary>Enumerates the different types of signs for land documents or certificates.</summary>
  public enum SignType {

    Undeterminated = 'U',

    Manual = 'M',

    Electronic = 'E',

    Historic = 'H',

  }  // enum SignType

}  // namespace Empiria.Land
