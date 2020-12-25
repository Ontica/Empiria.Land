/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Legal Instruments                          Component : Data Access Layer                       *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Services                           *
*  Type     : IssuersData                                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data read and write services for legal instrument issuers.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

using Empiria.Land.Instruments.Adapters;

namespace Empiria.Land.Instruments.Data {

  /// <summary>Data read and write services for legal instrument issuers.</summary>
  static internal class IssuersData {

    static internal FixedList<Issuer> GetList(IssuersSearchCommand searchCommand) {
      searchCommand.EnsureIsValid();

      string filter  = searchCommand.MapToFilterString();
      string orderBy = searchCommand.MapToSortString();

      string sql = $"SELECT TOP {searchCommand.PageSize} * " +
                    "FROM LRSIssuers " +
                   $"WHERE {filter} ORDER BY {orderBy}";

      var operation = DataOperation.Parse(sql);

      return DataReader.GetFixedList<Issuer>(operation);
    }


    static internal void WriteIssuer(Issuer o) {
      var op = DataOperation.Parse("writeLRSIssuer",
                  o.Id, o.UID, o.IssuerType.Id, o.Name,
                  o.EntityName, o.OfficialPosition, o.OfficeName, o.PlaceName,
                  o.ExtData.ToString(), o.Keywords,
                  o.RelatedContact.Id, o.RelatedEntity.Id, o.RelatedOffice.Id, o.RelatedPlace.Id,
                  o.FromDate, o.ToDate, (char) o.Status, o.PostedById, o.PostingTime, String.Empty);

      DataWriter.Execute(op);
    }

  }  // class IssuersData

}  // namespace Empiria.Land.Instruments.Data
