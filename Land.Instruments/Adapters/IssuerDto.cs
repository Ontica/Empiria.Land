/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Legal Instruments                          Component : Interface adapters                      *
*  Assembly : Empiria.Land.Instruments.dll               Pattern   : Data Transfer Object                    *
*  Type     : IssuerDto                                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO with data representing a legal instrument issuer like a notary or judge.            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Instruments.Adapters {

  /// <summary>Output DTO with data representing a legal instrument issuer like a notary or judge.</summary>
  public class IssuerDto {

    public string UID {
      get; internal set;
    } = String.Empty;


    public string Name {
      get; internal set;
    } = String.Empty;


    public string OfficialPosition {
      get; internal set;
    } = String.Empty;


    public string Entity {
      get; internal set;
    } = String.Empty;


    public string Place {
      get; internal set;
    } = String.Empty;


    public Period Period {
      get; internal set;
    }

  }  // class IssuerDto



  public class Period {

    public DateTime FromDate {
      get; internal set;
    }

    public DateTime ToDate {
      get; internal set;
    }

  }  // class Period


}  // namespace Empiria.Land.Instruments.Adapters
