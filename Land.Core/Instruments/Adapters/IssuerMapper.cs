/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Legal Instruments                          Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Mapper class                            *
*  Type     : IssuerMapper                               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map legal instruments issuers to IssuerDto objects.                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.Instruments.Adapters {

  static internal class IssuerMapper {

    static internal FixedList<IssuerDto> Map(FixedList<Issuer> list) {
      var mappedItems = list.Select((x) => Map(x));

      return new FixedList<IssuerDto>(mappedItems);
    }


    static internal IssuerDto Map(Issuer issuer) {
      var dto = new IssuerDto {
        UID = issuer.UID,
        Name = issuer.Name,
        OfficialPosition = issuer.OfficialPosition,
        Entity = issuer.EntityName,
        Place = issuer.PlaceName,
        Period = new Period {
          FromDate = issuer.FromDate,
          ToDate = issuer.ToDate,
        }
      };

      return dto;
    }

  }  // class IssuerMapper

}  // namespace Empiria.Land.Instruments.Adapters
