/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : ESign Services                             Component : Interface adapter                       *
*  Assembly : Empiria.Land.ESign.dll                     Pattern   : Query payload                           *
*  Type     : ESignQuery                                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Query payload used to generate ESign request.                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.ESign.Adapters {

  public class ESignQuery {

    public string UID {
      get; set;
    } = string.Empty;


    public string EventType {
      get; set;
    } = string.Empty;


    public FixedList<SignRequestDto> SignRequests {
      get; set;
    } = new FixedList<SignRequestDto>();


    public DateTime TimeStamp {
      get; set;
    }

  } // class ESignQuery

} // namespace Empiria.Land.ESign.Adapters
