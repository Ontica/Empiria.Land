/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Media Files Management                Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Transfer Object                    *
*  Type     : MediaDto                                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data structure with media data.                                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Media.Adapters {

  /// <summary>Data structure with media data.</summary>
  public class MediaDto {

    public string Url {
      get; internal set;
    } = string.Empty;


    public string MediaType {
      get; internal set;
    } = string.Empty;


  }  // public class MediaDto

}  // namespace Empiria.Land.Transactions.Adapters
