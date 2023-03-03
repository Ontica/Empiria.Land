/* Empiria Land     ******************************************************************************************
*                                                                                                            *
*  Module   : ESign Services                             Component : Domain                                  *
*  Assembly : Empiria.Land.ESign.dll                     Pattern   : Empiria Object                          *
*  Type     : SignedDocumentEntry                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains data for an electronic-sign operation.                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;


namespace Empiria.Land.ESign.Domain {

  /// <summary>Contains data for an electronic-sign operation.</summary>
  public class SignedDocumentEntry {

    #region Constructors and parsers

    internal SignedDocumentEntry() {
      // Required by Empiria Framework.
    }

    #endregion Constructors and parsers

    #region Properties


    [DataField("TransactionId")]
    public int TransactionId {
      get; internal set;
    }


    [DataField("TransactionUID")]
    public string TransactionUID {
      get; internal set;
    }


    [DataField("DocumentType")]
    public string DocumentType {
      get; internal set;
    }


    [DataField("TransactionType")]
    public string TransactionType {
      get; internal set;
    }


    [DataField("InternalControlNo")]
    public string InternalControlNo {
      get; internal set;
    }


    [DataField("AssignedBy")]
    public string AssignedBy {
      get; internal set;
    }


    [DataField("Responsible")]
    public string Responsible {
      get; internal set;
    }


    [DataField("RequestedBy")]
    public string RequestedBy {
      get; internal set;
    }


    [DataField("TransactionStatus")]
    public string TransactionStatus {
      get; internal set;
    }


    //[DataField("RecorderOfficeId")]
    //public int RecorderOfficeId {
    //  get; internal set;
    //}


    [DataField("PresentationTime")]
    public DateTime PresentationTime {
      get; internal set;
    }


    #endregion Properties

  } // class SignedDocumentEntry

} // namespace Empiria.Land.ESign.Domain
