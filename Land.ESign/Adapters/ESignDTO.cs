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


    public string UID {
      get; set;
    }


    public string EventType {
      get; set;
    }


    public DateTime TimeStamp {
      get; set;
    } = DateTime.Now;


    public SignDocumentRequestDto SignRequests {
      get; set;
    }


  } // class ESignDTO


}
