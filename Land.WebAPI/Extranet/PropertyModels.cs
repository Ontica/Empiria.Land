/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Extranet Services                            Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : PropertyModels                               License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Response models for RealEstate entities.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Registration;

namespace Empiria.Land.WebApi.Extranet {

  /// <summary>Response models for RealEstate entities.</summary>
  static internal class PropertyModels {

    #region Response models

    static internal object ToResponse(this RealEstate realEstate) {
      return new {
        uid = realEstate.UID,
        name = realEstate.Name,
        realEstateType = new {
          uid = realEstate.RealEstateType.Id.ToString(),
          name = realEstate.RealEstateType.Name
        },
        cadastralKey = realEstate.CadastralKey,
        metesAndBounds = realEstate.MetesAndBounds,
        district = new {
          uid = realEstate.District.Id.ToString(),
          name = realEstate.District.Alias
        },
        municipality = new {
          uid = realEstate.Municipality.Id.ToString(),
          name = realEstate.Municipality.FullName
        },
        location = realEstate.LocationReference,
        lotSize = realEstate.LotSize,
        notes = realEstate.Notes,
        isPartition = realEstate.IsPartition,
        partitionOf = new {
          uid = realEstate.IsPartitionOf.UID,
          name = realEstate.IsPartitionOf.Name,
        }
      };
    }

    #endregion Response models

  }  // class PropertyModels

}  // // namespace Empiria.Land.WebApi.Extranet
