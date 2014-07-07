/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                 System   : Land Registration System              *
*  Namespace : Empiria.Land.Registration.Data               Assembly : Empiria.Land.Registration             *
*  Type      : PropertyData                                 Pattern  : Data Services Static Class            *
*  Version   : 2.0        Date: 23/Oct/2014                 License  : GNU AGPLv3  (See license.txt)         *
*                                                                                                            *
*    Summary   : Provides database read and write methods for recording books.                               *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;

using Empiria.Data;
using Empiria.Ontology;

namespace Empiria.Land.Registration.Data {

  /// <summary>Provides database read and write methods for recording books.</summary>
  static public class PropertyData {

    #region Internal methods

    static internal DataRow GetPropertyWithUniqueCode(string uniqueCode) {
      DataOperation operation = DataOperation.Parse("getLRSPropertyWithTractKey", uniqueCode);

      return DataReader.GetDataRow(operation);
    }

    static internal bool ExistsPropertyUniqueCode(string uniqueCode) {
      DataOperation operation = DataOperation.Parse("getLRSPropertyWithTractKey", uniqueCode);

      return (DataReader.Count(operation) != 0);
    }

    static public RecordingActParty GetLastRecordingActParty(Party party, DateTime searchStartDate) {
      DataOperation operation = DataOperation.Parse("qryLRSRecordingActPartyWithParty", party.Id);
      DataRow row = DataReader.GetDataRow(operation);
      if (row != null) {
        return RecordingActParty.Parse(row);
      } else {
        return null;
      }
    }

    static private string GetPartyTypeInfoFilter(ObjectTypeInfo partyType) {
      if (partyType == null) {
        return "(PartyTypeId <> 0)";
      }
      if (partyType.IsAbstract) {
        string temp = String.Empty;
        ObjectTypeInfo[] subTypes = partyType.GetSubclasses();
        for (int i = 0; i < subTypes.Length; i++) {
          if (temp.Length != 0) {
            temp += ",";
          }
          temp += subTypes[i].Id.ToString();
        }
        return "(PartyTypeId IN (" + temp + "))";
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
      DataOperation operation = DataOperation.Parse("qryLRSPartiesOnRecording", recording.Id);

      string filter = SearchExpression.ParseAndLikeWithNoiseWords("PartyKeywords", keywords);
      if (filter.Length != 0) {
        filter += " AND ";
      }
      filter += GetPartyTypeInfoFilter(partyType);

      DataView view = DataReader.GetDataView(operation, filter, "PartyFullName");

      return view.ToTable();
    }

    static public DataTable GetPartiesOnRecordingBook(ObjectTypeInfo partyType,
                                                      RecordingBook recordingBook, string keywords) {
      DataOperation operation = DataOperation.Parse("qryLRSPartiesOnRecordingBook", recordingBook.Id);

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
          return GetPartiesOnRecording(partyType, recordingAct.Recording, keywords);
        case PartyFilterType.OnRecordingBook:
          return GetPartiesOnRecordingBook(partyType, recordingAct.Recording.RecordingBook, keywords);
        case PartyFilterType.Involved:
          return GetInvolvedParties(partyType, recordingAct, keywords);
        default:
          throw new Empiria.Reflection.ReflectionException(Empiria.Reflection.ReflectionException.Msg.ConditionalOptionNotDefined,
                                                           partyFilterType.ToString());
      }
    }

    static public DataTable GetSecondaryParties(Party party, RecordingAct exceptRecordingAct) {
      DataOperation operation = DataOperation.Parse("qryLRSSecondaryParties", exceptRecordingAct.Id, party.Id);

      return DataReader.GetDataTable(operation);
    }

    static internal FixedList<RecordingActParty> GetRecordingPartiesList(Recording recording, Party party) {
      string sql = "SELECT LRSRecordingActParties.* " +
            "FROM LRSRecordingActParties INNER JOIN LRSRecordingActs " +
            "ON LRSRecordingActParties.RecordingActId = LRSRecordingActs.RecordingActId " +
            "WHERE (LRSRecordingActs.RecordingId = " + recording.Id.ToString() + ") " +
            "AND (LRSRecordingActParties.LinkStatus <> 'X') " +
            "AND (LRSRecordingActParties.PartyId = " + party.Id.ToString() +
            " OR LRSRecordingActParties.SecondaryPartyId = " + party.Id.ToString() + ")";

      var operation = DataOperation.Parse(sql);

      return DataReader.GetFixedList<RecordingActParty>(operation, (x) => RecordingActParty.Parse(x));
    }

    static public FixedList<TractIndexItem> GetRecordingPropertiesAnnotationsList(Recording recording) {
      DataOperation operation = DataOperation.Parse("qryLRSRecordingPropertiesAnnotations", recording.Id);

      DataView view = DataReader.GetDataView(operation);

      return new FixedList<TractIndexItem>((x) => TractIndexItem.Parse(x), view);
    }

    static internal int WriteHumanParty(HumanParty o) {
      Assertion.Require(o.Id != 0, "HumanParty.Id can't be zero");
      DataOperation dataOperation = DataOperation.Parse("writeLRSParty", o.Id, o.ObjectTypeInfo.Id, o.FullName,
                                                        o.FirstName, o.FirstFamilyName, o.SecondFamilyName,
                                                        o.MaritalFamilyName, o.ShortName, o.Nicknames, o.Tags,
                                                        (char) o.Gender, o.RegistryDate, o.RegistryLocation.Id,
                                                        o.CURPNumber, o.TaxIDNumber, o.IFENumber, String.Empty,
                                                        String.Empty, String.Empty, String.Empty, -1, -1,
                                                        ExecutionServer.DateMinValue, -1, ExecutionServer.DateMinValue,
                                                        String.Empty, o.Notes, o.Keywords, o.PostedBy.Id, o.PostingTime,
                                                        o.ReplacedById, (char) o.Status, o.IntegrityHashCode);

      return DataWriter.Execute(dataOperation);
    }

    static internal int WriteOrganizationParty(OrganizationParty o) {
      Assertion.Require(o.Id != 0, "OrganizationParty.Id can't be zero");
      DataOperation dataOperation = DataOperation.Parse("writeLRSParty", o.Id, o.ObjectTypeInfo.Id, o.FullName,
                                                        String.Empty, String.Empty, String.Empty,
                                                        String.Empty, o.ShortName, o.Nicknames, o.Tags,
                                                        Contacts.Gender.NotApply,
                                                        o.RegistryDate, o.RegistryLocation.Id, String.Empty, o.TaxIDNumber,
                                                        String.Empty, o.AssocDocBookNumber, o.AssocDocNumber,
                                                        o.AssocDocStartSheet, o.AssocDocEndSheet, o.AssocDocNotaryOffice.Id,
                                                        o.AssocDocIssuedBy.Id, o.AssocDocIssueDate, o.AssocDocRecordingOffice.Id,
                                                        o.AssocDocRecordingDate, o.AssocDocRecordingNumber, o.Notes, o.Keywords,
                                                        o.PostedBy.Id, o.PostingTime, o.ReplacedById, (char) o.Status, o.IntegrityHashCode);

      return DataWriter.Execute(dataOperation);
    }

    static internal int WriteProperty(Property o) {
      Assertion.Require(o.Id != 0, "Property.Id can not be zero");
      DataOperation dataOperation = DataOperation.Parse("writeLRSProperty", o.Id, o.ObjectTypeInfo.Id, o.UniqueCode, 
                                                        o.Name, o.PropertyKind.Id, o.RecordingNotes, o.AntecedentNotes,
                                                        o.Location.ToJson(), o.Location.ToString(), o.Location.ToSearchVector(),
                                                        o.CadastralData.ToJson(), o.Keywords, 
                                                        o.PartitionOf.Id, o.PartitionNo, o.MergedInto.Id, o.PostedBy.Id,
                                                        o.PostingTime, (char) o.Status, o.Integrity.GetUpdatedHashCode());
      return DataWriter.Execute(dataOperation);
    }

    static internal int WriteRecordingActParty(RecordingActParty o) {
      Assertion.Require(o.Id != 0, "RecordingActParty.Id can't be zero");
      DataOperation dataOperation = DataOperation.Parse("writeLRSRecordingActParty", o.Id, o.RecordingAct.Id,
                                                        o.Party.Id, o.PartyRole.Id, o.SecondaryParty.Id,
                                                        o.SecondaryPartyRole.Id, o.Notes, (char) o.OwnershipMode,
                                                        o.OwnershipPart.Amount, o.OwnershipPart.Unit.Id,
                                                        (char) o.UsufructMode, o.UsufructTerm,
                                                        o.PartyOccupation.Id, o.PartyMarriageStatus.Id,
                                                        o.PartyAddress, o.PartyAddressPlace.Id, o.PostedBy.Id,
                                                        o.PostingTime, (char) o.Status, o.IntegrityHashCode);
      return DataWriter.Execute(dataOperation);
    }

    static internal int WriteTractIndexItem(TractIndexItem o) {
      Assertion.Require(o.Id != 0, "TractIndexItem.Id can't be zero");
      DataOperation dataOperation = DataOperation.Parse("writeLRSPropertyTractIndex", o.Id,
                                                        o.Property.Id, o.RecordingAct.Id, o.ExtensionData.ToJson(),
                                                        o.PostedBy.Id, o.PostingTime, (char) o.Status, 
                                                        o.Integrity.GetUpdatedHashCode());
      return DataWriter.Execute(dataOperation);
    }

    #endregion Internal methods

  } // class PropertyData

} // namespace Empiria.Land.Registration.Registration.Data
