﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                 System   : Land Registration System              *
*  Namespace : Empiria.Land.Registration.Data               Assembly : Empiria.Land.Registration             *
*  Type      : PartyData                                    Pattern  : Data Services                         *
*  Version   : 2.1                                          License  : Please read license.txt file          *
*                                                                                                            *
*  Summary   : Provides db read and write methods for parties: human and organizations.                      *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;
using System.Collections.Generic;

using Empiria.Data;
using Empiria.Ontology;

namespace Empiria.Land.Registration.Data {

  /// <summary>Provides db read and write methods for parties: human and organizations.</summary>
  static public class PartyData {

    #region Public methods

    static public FixedList<RecordingActParty> GetRecordingPartyList(RecordingAct recordingAct) {
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
      var list = new List<RecordingActParty>();
      foreach (var tractItem in recordingAct.TractIndex) {
        var op = DataOperation.Parse("qryLRSResourceActiveOwnershipRecordingActParties", tractItem.Resource.Id);

        var temp = DataReader.GetList<RecordingActParty>(op, (x) => BaseObject.ParseList<RecordingActParty>(x));

        if (list.Count == 0) {
          list.AddRange(temp);
        } else {
          list.AddRange(temp.FindAll((x) => !list.Contains(x)));
        }
      }
      return list.ToFixedList();
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


    static public RecordingActParty TryGetLastRecordingActParty(Party party, DateTime searchStartDate) {
      var operation = DataOperation.Parse("qryLRSRecordingActPartyWithParty", party.Id);
      DataRow row = DataReader.GetDataRow(operation);
      if (row != null) {
        return BaseObject.ParseDataRow<RecordingActParty>(row);
      } else {
        return null;
      }
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
                                                  Recording recording, string keywords) {
      var operation = DataOperation.Parse("qryLRSPartiesOnRecording", recording.Id);

      string filter = SearchExpression.ParseAndLikeWithNoiseWords("PartyKeywords", keywords);
      if (filter.Length != 0) {
        filter += " AND ";
      }
      filter += GetPartyTypeInfoFilter(partyType);

      DataView view = DataReader.GetDataView(operation, filter, "PartyFullName");

      return view.ToTable();
    }

    static public DataTable GetInvolvedParties(ObjectTypeInfo partyType, RecordingAct recordingAct,
                                               string keywords) {
      DataTable table = new DataTable();

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

    static internal FixedList<Resource> GetPhysicalRecordingResources(Recording recording) {
      var operation = DataOperation.Parse("qryLRSPhysicalRecordingProperties", recording.Id);

      return DataReader.GetList<Resource>(operation,
                                          (x) => BaseObject.ParseList<Resource>(x)).ToFixedList();
    }


    static internal int WriteParty(Party o) {
      var dataOperation = DataOperation.Parse("writeLRSParty", o.Id, o.GetEmpiriaType().Id,
                                              o.FullName, o.Notes, o.ExtendedData,
                                              o.Keywords, o.Status, String.Empty);
      return DataWriter.Execute(dataOperation);
    }

    #endregion Internal methods

  } // class PartyData

} // namespace Empiria.Land.Registration.Data
