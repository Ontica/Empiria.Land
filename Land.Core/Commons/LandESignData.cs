/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Recording services                      Component : Recording documents                   *
*  Assembly : Empiria.Land.Registration.dll                Pattern   : Information Holder                    *
*  Type     : LandESignData                                License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Holds electronic sign data for Empiria Land documents.                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Registration {

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

}  // namespace Empiria.Land.Registration
