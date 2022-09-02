/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Serch services                             Component : Data Access Layer                       *
*  Assembly : Empiria.Land.SearchServices.dll            Pattern   : Data Services                           *
*  Type     : SearchServicesData                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data read services for search services.                                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

using Empiria.Land.Registration;

namespace Empiria.Land.SearchServices.Data {

  /// <summary>Data read services for search services.</summary>
  static internal class SearchServicesData {

    static internal FixedList<RecordingActParty> GetRecordingActParties(string filter, string sort, int pageSize) {
      string sql = $"SELECT TOP {pageSize} LRSRecordingActParties.* " +
                    "FROM LRSRecordingActParties INNER JOIN LRSParties " +
                    "ON LRSRecordingActParties.PartyId = LRSParties.PartyId " +
                   $"WHERE {filter} " +
                    "AND (LRSRecordingActParties.RecActPartyStatus <> 'X') " +
                   $"ORDER BY {sort}";

      var operation = DataOperation.Parse(sql);

      return DataReader.GetFixedList<RecordingActParty>(operation);
    }

  }  // class SearchServicesData

}  // namespace Empiria.Land.SearchServices.Data
