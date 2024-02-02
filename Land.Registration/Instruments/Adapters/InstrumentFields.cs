/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Legal Instruments                          Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Input Data Holder                       *
*  Type     : InstrumentFields                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data structure that serves as an adapter to update instruments data.                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Instruments.Adapters {

  /// <summary>Data structure used to update instruments data.</summary>
  public class InstrumentFields {

    public InstrumentTypeEnum? Type {
      get; set;
    }

    public string Kind {
      get; set;
    } = string.Empty;


    public string IssuerUID {
      get; set;
    }


    public DateTime? IssueDate {
      get; set;
    }


    public string Summary {
      get; set;
    } = string.Empty;


    public string InstrumentNo {
      get; set;
    } = string.Empty;


    public string BinderNo {
      get; set;
    } = string.Empty;


    public string Folio {
      get; set;
    } = string.Empty;


    public string EndFolio {
      get; set;
    } = string.Empty;


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
