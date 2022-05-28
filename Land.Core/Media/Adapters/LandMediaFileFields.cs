/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Media Files Management                Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Input Data Holder                       *
*  Type     : LandMediaFileFields                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data structure used to create an Empiria Land system's media file or update its data.          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Storage;

namespace Empiria.Land.Media.Adapters {

  /// <summary>Data structure used to create an Empiria Land system's media file or update its data.</summary>
  public class LandMediaFileFields : MediaFileFields {


    static public LandMediaContent ConvertMediaContent(string value) {
      LandMediaContent result;

      if (Enum.TryParse(value, out result)) {
        return result;
      } else {
        throw Assertion.EnsureNoReachThisCode($"Unrecognized MediaContent value '{value}'.");
      }
    }


    public new LandMediaContent MediaContent {
      get {
        return ConvertMediaContent(base.MediaContent);
      }
      set {
        base.MediaContent = value.ToString();
      }
    }

  }  // class LandMediaFileFields

} // namespace Empiria.Land.Media.Adapters
