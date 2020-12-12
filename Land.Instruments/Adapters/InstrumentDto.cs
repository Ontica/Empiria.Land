/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Legal Instruments                          Component : Interface adapters                      *
*  Assembly : Empiria.Land.Instruments.dll               Pattern   : Data Transfer Object                    *
*  Type     : InstrumentDto                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO with data representing a legal instrument.                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Instruments.UseCases {

  /// <summary>Output DTO with data representing a legal instrument.</summary>
  public class InstrumentDto {

    public string UID {
      get; internal set;
    }

    public string Type {
      get; internal set;
    }

    public string Subtype {
      get; internal set;
    }

    public string Summary {
      get; internal set;
    }

    public string TypeName {
      get; internal set;
    }

    public string CategoryUID {
      get; internal set;
    }

    public string CategoryName {
      get; internal set;
    }

    public DateTime IssueDate {
      get; internal set;
    }

    public IssuerDto Issuer {
      get; internal set;
    }

    public int SheetsCount {
      get; internal set;
    }

    public string DocumentNo {
      get; internal set;
    }

    public string CaseNo {
      get; internal set;
    }

    public object Media {
      get; internal set;
    }

  }

}
