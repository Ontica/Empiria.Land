/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : ESign Services                             Component : Interface adapter                       *
*  Assembly : Empiria.Land.ESign.dll                     Pattern   : Data Transfer Object                    *
*  Type     : ESignDTO                                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : DTO with land ESign data.                                                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.ESign.Adapters {

  /// <summary>DTO with land ESign data.</summary>
  public class ESignDTO {


    public FixedList<SignRequestDto> SignRequests {
      get; set;
    } = new FixedList<SignRequestDto>();


  } // class ESignDTO


  public class SignRequestDto {


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


    public Filing Filing {
      get; set;
    }


  } // class SignRequest


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


  public class Filing {


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


  } // class Filing


}
