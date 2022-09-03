/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Search services                            Component : Interface adapters                      *
*  Assembly : Empiria.Land.SearchServices.dll            Pattern   : Data Transfer Object                    *
*  Type     : RecordDto                                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes a land system electronic record.                                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.SearchServices {

  /// <summary>Describes a land system electronic record.</summary>
  public class RecordDto {

    internal RecordDto() {
      // no-op
    }

    public string UID {
      get; internal set;
    }


    public string RecordID {
      get; internal set;
    }


    public string RecorderOffice {
      get; internal set;
    }


    public DateTime RecordingTime {
      get; internal set;
    }


    public string RecordedBy {
      get; internal set;
    }


    public string AuthorizedBy {
      get; internal set;
    }


    public string Instrument {
      get; internal set;
    }


    public string BookEntry {
      get; internal set;
    }


    public DateTime PresentationTime {
      get; internal set;
    }


    public RecordTransactionDto Transaction {
      get; internal set;
    }

  }  // class RecordDto



  /// <summary>Inner DTO with properties that describes a transaction for a RecordDto object.</summary>
  public class RecordTransactionDto {

    internal RecordTransactionDto() {
      // no-op
    }

    public string UID {
      get; internal set;
    }


    public string TransactionID {
      get; internal set;
    }


    public string InternalControlNo {
      get; internal set;
    }

  }  // class RecordTransactionDto

}  // namespace Empiria.Land.SearchServices
