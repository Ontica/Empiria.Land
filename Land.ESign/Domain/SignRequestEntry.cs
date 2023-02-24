/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : ESign Services                             Component : Domain Layer                            *
*  Assembly : Empiria.Land.ESign.dll                     Pattern   : Empiria Plain Object                    *
*  Type     : SignRequestEntry                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represent an entry for ESign Request.                                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.Land.ESign.Adapters;

namespace Empiria.Land.ESign.Domain {

  /// <summary>Represent an entry for ESign Request.</summary>
  public class SignRequestEntry {


    public string UID {
      get; internal set;
    }


    public string RequestedBy {
      get; internal set;
    }


    public DateTime RequestedTime {
      get; internal set;
    }


    public string SignStatus {
      get; internal set;
    }


    public string SignatureKind {
      get; internal set;
    }


    public string DigitalSignature {
      get; set;
    }


    public DocumentType Document {
      get; internal set;
    }


    public Filing Filing {
      get; internal set;
    }


  } // class SignRequestEntry

} // namespace Empiria.Land.ESign.Domain
