/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Legal Instruments                          Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Transfer Object                    *
*  Type     : InstrumentDto                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO with data representing a legal instrument.                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Instruments.Adapters {

  /// <summary>Output DTO with data representing a legal instrument.</summary>
  public class InstrumentDto {

    public string UID {
      get; internal set;
    }

    public InstrumentTypeEnum Type {
      get; internal set;
    }

    public string TypeName {
      get; internal set;
    }

    public string Kind {
      get; internal set;
    }

    public DateTime IssueDate {
      get; internal set;
    }

    public IssuerDto Issuer {
      get; internal set;
    }

    public string Summary {
      get; internal set;
    }

    public string AsText {
      get; internal set;
    }

    public string ControlID {
      get; internal set;
    }

    public string InstrumentNo {
      get; internal set;
    }

    public string BinderNo {
      get; internal set;
    }

    public string Folio {
      get; internal set;
    }

    public string EndFolio {
      get; internal set;
    }

    public int SheetsCount {
      get; internal set;
    }

    public object[] Media {
      get; internal set;
    }

    public string Status {
      get; internal set;
    }


  }  // class InstrumentDto

}  // namespace Empiria.Land.Instruments.Adapters
