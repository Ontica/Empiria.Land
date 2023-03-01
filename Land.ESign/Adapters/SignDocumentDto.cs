/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : ESign Services                             Component : Interface adapter                       *
*  Assembly : Empiria.Land.ESign.dll                     Pattern   : Data Transfer Object                    *
*  Type     : SignDocumentDto                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO used to return the entries of ESign document.                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.ESign.Adapters {

  /// <summary>Output DTO used to return the entries of ESign document.</summary>
  public class SignDocumentDto {

    public int TransactionId {
      get; set;
    }


    public string TransactionUID {
      get; set;
    }


    public string DocumentType {
      get; set;
    }


    public string TransactionType {
      get; set;
    }


    public string InternalControlNo {
      get; set;
    }


    public string AssignedById {
      get; set;
    }


    public string AssignedBy {
      get; set;
    }


    public string RequestedBy {
      get; set;
    }


    public string TransactionStatus {
      get; set;
    }


    public int RecorderOfficeId {
      get; set;
    }


    public DateTime PresentationTime {
      get; set;
    }

  } // class SignDocumentDto

} // namespace Empiria.Land.ESign.Adapters
