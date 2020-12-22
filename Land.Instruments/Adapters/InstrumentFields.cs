/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Legal Instruments                          Component : Domain Layer                            *
*  Assembly : Empiria.Land.Instruments.dll               Pattern   : Input Data Holder                       *
*  Type     : InstrumentFields                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data structure that serves as an adapter to update instruments data.                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Instruments.Adapters {

  /// <summary>Data structure used to update instruments data.</summary>
  public class InstrumentFields {

    public InstrumentTypeEnum Type {
      get; set;
    }

    public string Kind {
      get; set;
    }

    public string IssuerUID {
      get; set;
    }

    public DateTime? IssueDate {
      get; set;
    }

    public string Summary {
      get; set;
    }

    public string InstrumentNo {
      get; set;
    }

    public string BinderNo {
      get; set;
    }

    public string Folio {
      get; set;
    }

    public string EndFolio {
      get; set;
    }

    public int? SheetsCount {
      get; set;
    }


    internal Issuer Issuer {
      get {
        if (this.IssuerUID == null) {
          return null;
        }
        return Issuer.Parse(this.IssuerUID);
      }
    }

  }  // class InstrumentFields

}  // namespace Empiria.Land.Instruments.Adapters
