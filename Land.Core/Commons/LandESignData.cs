/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Commons                                      Component : Electronic sign                       *
*  Assembly : Empiria.Land.Core.dll                        Pattern   : Information Holder                    *
*  Type     : LandESignData                                License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Holds electronic sign data for Empiria Land documents.                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land {

  /// <summary>Holds electronic sign data for Empiria Land documents.</summary>
  public class LandESignData {

    public string DocumentID {
      get; set;
    }

    public string DocumentName {
      get; set;
    }

    public string Signature {
      get; set;
    }

    public DateTime SignedTime {
      get; set;
    }

    public string Digest {
      get; set;
    }

  }  // class LandESignData

}  // namespace Empiria.Land
