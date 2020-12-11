/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Legal Instruments                          Component : Interface adapters                      *
*  Assembly : Empiria.Land.UseCases.dll                  Pattern   : Data Transfer Object                    *
*  Type     : InstrumentDto                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds data related to a legal instrument.                                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.Instruments.UseCases {

  /// <summary>Holds data related to a legal instrument.</summary>
  public class InstrumentDto {

    public string UID {
      get; internal set;
    }

    public string Type {
      get; internal set;
    }

    public string Subtype {
      get;
      internal set;
    }

    public string Summary {
      get; internal set;
    }

  }

}
