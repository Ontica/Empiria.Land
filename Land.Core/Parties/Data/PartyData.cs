﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                 System   : Land Registration System              *
*  Namespace : Empiria.Land.Data                            Assembly : Empiria.Land.Registration             *
*  Type      : PartyData                                    Pattern  : Data Services                         *
*  Version   : 3.0                                          License  : Please read license.txt file          *
*                                                                                                            *
*  Summary   : Provides database read and write methods for parties: people and organizations.               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

using Empiria.Land.Registration;
using Empiria.Land.Registration.Adapters;

namespace Empiria.Land.Data {

  /// <summary>Provides database read and write methods for parties: people and organizations.</summary>
  static public class PartyData {

    #region Public methods

    static internal FixedList<RecordingActParty> GetRecordingPartyList(RecordingAct recordingAct) {
      string sql = "SELECT * FROM LRSRecordingActParties " +
                   "WHERE RecordingActId = " + recordingAct.Id.ToString() + " " +
                   "AND RecActPartyStatus <> 'X' " +
                   "ORDER BY RecordingActPartyId";

      return DataReader.GetFixedList<RecordingActParty>(DataOperation.Parse(sql));
    }


    static internal FixedList<Party> GetParties(SearchPartiesCommand command) {
      string filter = SearchExpression.ParseAndLikeWithNoiseWords("PartyKeywords", command.Keywords);

      if (filter.Length != 0) {
        filter += " AND ";
      }
      filter += "(PartyStatus <> 'X')";

      string sql = "SELECT * FROM LRSParties " +
                  $"WHERE {filter} " +
                  "ORDER BY PartyFullName";

      return DataReader.GetFixedList<Party>(DataOperation.Parse(sql));
    }


    static public FixedList<RecordingActParty> GetSecondaryPartiesList(RecordingAct recordingAct) {
      string sql = "SELECT * FROM LRSRecordingActParties " +
                   "WHERE RecordingActId = " + recordingAct.Id.ToString() + " " +
                   "AND PartyOfId <> -1 AND RecActPartyStatus <> 'X' " +
                   "ORDER BY RecordingActPartyId";

      return DataReader.GetFixedList<RecordingActParty>(DataOperation.Parse(sql));
    }


    static internal void WriteParty(Party o) {
      var dataOperation = DataOperation.Parse("writeLRSParty", o.Id, o.UID, o.GetEmpiriaType().Id,
                                              o.FullName, o.RFC,
                                              (o is HumanParty) ? ((HumanParty)o).CURP : String.Empty,
                                              o.Notes, string.Empty, o.Keywords, (char) o.Status, String.Empty);
      DataWriter.Execute(dataOperation);
    }


    #endregion Internal methods

  } // class PartyData

} // namespace Empiria.Land.Data
