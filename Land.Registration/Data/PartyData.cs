/* Empiria Land **********************************************************************************************
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
using System.Data;

using Empiria.Data;
using Empiria.Ontology;

using Empiria.Land.Registration;
using Empiria.Land.Registration.Adapters;

namespace Empiria.Land.Data {

  /// <summary>Provides database read and write methods for parties: people and organizations.</summary>
  static public class PartyData {

    #region Public methods

    static internal FixedList<RecordingActParty> GetRecordingPartyList(RecordingAct recordingAct) {
      string sql = "SELECT * FROM LRSRecordingActParties " +
                   "WHERE RecordingActId = " + recordingAct.Id.ToString() + " " +
                   "AND RecActPartyStatus <> 'X'";

      return DataReader.GetList<RecordingActParty>(DataOperation.Parse(sql),
                                                   (x) => BaseObject.ParseList<RecordingActParty>(x)).ToFixedList();
    }

    static public FixedList<RecordingActParty> GetRecordingPartyList(RecordingDocument document, Party party) {
      string sql = "SELECT LRSRecordingActParties.* " +
              "FROM LRSRecordingActParties INNER JOIN LRSRecordingActs " +
              "ON LRSRecordingActParties.RecordingActId = LRSRecordingActs.RecordingActId " +
              "WHERE (LRSRecordingActs.DocumentId = " + document.Id.ToString() + ") " +
              "AND (LRSRecordingActParties.RecActPartyStatus <> 'X') " +
              "AND (LRSRecordingActParties.PartyId = " + party.Id.ToString() +
              " OR LRSRecordingActParties.PartyOfId = " + party.Id.ToString() + ")";

      var operation = DataOperation.Parse(sql);
      return DataReader.GetList<RecordingActParty>(operation, (x) =>
                                                   BaseObject.ParseList<RecordingActParty>(x)).ToFixedList();
    }

    static public FixedList<RecordingActParty> GetInvolvedDomainParties(RecordingAct recordingAct) {
      var op = DataOperation.Parse("qryLRSResourceActiveOwnershipRecordingActParties",
                                   recordingAct.Resource.Id);

      return DataReader.GetList(op, (x) => BaseObject.ParseList<RecordingActParty>(x)).ToFixedList();
    }


    static internal DataTable GetParties(SearchPartiesCommand command) {
      string filter = SearchExpression.ParseAndLikeWithNoiseWords("PartyKeywords", command.Keywords);
      if (filter.Length != 0) {
        filter += " AND ";
      }
      filter += "(PartyStatus <> 'X')";

      return GeneralDataOperations.GetEntities("LRSParties", filter, "PartyFullName");
    }


    static public FixedList<RecordingActParty> GetSecondaryPartiesList(RecordingAct recordingAct) {
      string sql = "SELECT * FROM LRSRecordingActParties " +
                   "WHERE RecordingActId = " + recordingAct.Id.ToString() + " " +
                   "AND PartyOfId <> -1 AND RecActPartyStatus <> 'X'";

      return DataReader.GetList<RecordingActParty>(DataOperation.Parse(sql),
                                                  (x) => BaseObject.ParseList<RecordingActParty>(x)).ToFixedList();
    }

    static public RecordingActParty GetSecondaryParty(RecordingAct recordingAct, Party party) {
      FixedList<RecordingActParty> secondaries = GetSecondaryPartiesList(recordingAct);

      return secondaries.Find((x) => x.Party.Equals(party));
    }

    static private string GetPartyTypeInfoFilter(ObjectTypeInfo partyType) {
      if (partyType == null) {
        return "(PartyTypeId <> 0)";
      }
      if (partyType.IsAbstract) {
        return "(PartyTypeId IN (" + partyType.GetSubclassesFilter() + "))";
      } else {
        return "(PartyTypeId = " + partyType.Id.ToString() + ")";
      }
    }

    static public DataTable GetParties(ObjectTypeInfo partyType, string keywords) {
      string filter = SearchExpression.ParseAndLikeWithNoiseWords("PartyKeywords", keywords);
      if (filter.Length != 0) {
        filter += " AND ";
      }
      filter += GetPartyTypeInfoFilter(partyType);
      if (filter.Length != 0) {
        filter += " AND ";
      }
      filter += "(PartyStatus <> 'X')";

      return GeneralDataOperations.GetEntities("LRSParties", filter, "PartyFullName");
    }

    static public DataTable GetPartiesOnRecording(ObjectTypeInfo partyType,
                                                  PhysicalRecording recording, string keywords) {
      var operation = DataOperation.Parse("qryLRSPartiesOnRecording", recording.Id);

      string filter = SearchExpression.ParseAndLikeWithNoiseWords("PartyKeywords", keywords);
      if (filter.Length != 0) {
        filter += " AND ";
      }
      filter += GetPartyTypeInfoFilter(partyType);

      DataView view = DataReader.GetDataView(operation, filter, "PartyFullName");

      return view.ToTable();
    }


    internal static FixedList<RecordingActParty> GetRecordingActs(Party party) {
      string sql = "SELECT * FROM LRSRecordingActParties " +
                   "WHERE (PartyId = {0} OR PartyOfId = {0}) AND " +
                   "RecActPartyStatus <> 'X'";

      sql = String.Format(sql, party.Id);

      var list = DataReader.GetList(DataOperation.Parse(sql), (x) => BaseObject.ParseList<RecordingActParty>(x));

      list.Sort((x, y) => (((IResourceTractItem) x.RecordingAct).TractPrelationStamp).CompareTo(
                                                      ((IResourceTractItem) y.RecordingAct).TractPrelationStamp));
      return list.ToFixedList();
    }

    static public DataTable GetInvolvedParties(ObjectTypeInfo partyType, RecordingAct recordingAct,
                                               string keywords) {
      return new DataTable();
    }


    static public DataTable GetParties(PartyFilterType partyFilterType, ObjectTypeInfo partyType,
                                       RecordingAct recordingAct, string keywords) {
      switch (partyFilterType) {
        case PartyFilterType.ByKeywords:
          return GetParties(partyType, keywords);
        case PartyFilterType.OnInscription:
          return GetPartiesOnRecording(partyType, recordingAct.PhysicalRecording, keywords);
        case PartyFilterType.Involved:
          return GetInvolvedParties(partyType, recordingAct, keywords);
        default:
          throw new Empiria.Reflection.ReflectionException(Empiria.Reflection.ReflectionException.Msg.ConditionalOptionNotDefined,
                                                           partyFilterType.ToString());
      }
    }


    static internal void WriteParty(Party o) {
      var dataOperation = DataOperation.Parse("writeLRSParty", o.Id, o.UID, o.GetEmpiriaType().Id,
                                              o.FullName,o.RFC,
                                              (o is HumanParty) ? ((HumanParty)o).CURP : String.Empty,
                                              o.Notes, string.Empty, o.Keywords, (char) o.Status, String.Empty);
      DataWriter.Execute(dataOperation);
    }


    #endregion Internal methods

  } // class PartyData

} // namespace Empiria.Land.Data
