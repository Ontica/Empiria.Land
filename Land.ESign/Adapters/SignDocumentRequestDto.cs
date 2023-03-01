/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : ESign Services                             Component : Interface adapter                       *
*  Assembly : Empiria.Land.ESign.dll                     Pattern   : Data Transfer Object                    *
*  Type     : SignDocumentRequestDto                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO used to return the entries of sign document.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.ESign.Adapters {

  /// <summary>Output DTO used to return the entries of sign document.</summary>
  public class SignDocumentRequestDto {


    public string UID {
      get; set;
    }


    public string RequestedBy {
      get; set;
    }


    public DateTime RequestedTime {
      get; set;
    }


    public string SignStatus {
      get; set;
    }


    public string SignatureKind {
      get; set;
    }


    public string DigitalSignature {
      get; set;
    }


    public DocumentType Document {
      get; set;
    }


    public DocumentFiling Filing {
      get; set;
    }


  } // class SignDocumentRequestDto


  public class DocumentType {


    public string UID {
      get; set;
    }


    public string Type {
      get; set;
    }


    public string DocumentNumber {
      get; set;
    }


    public string Description {
      get; set;
    }


    public string Uri {
      get; set;
    }


  } // class DocumentType


  public class DocumentFiling {


    public string FilingNo {
      get; set;
    }


    public DateTime FilingTime {
      get; set;
    }


    public string FiledBy {
      get; set;
    }


    public string PostedBy {
      get; set;
    }


  } // class DocumentFiling


}
