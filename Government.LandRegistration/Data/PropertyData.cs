/* Empiria® Land 2013 ****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria® Land                                System   : Land Registration System              *
*  Namespace : Empiria.Government.LandRegistration.Data     Assembly : Empiria.Government.LandRegistration   *
*  Type      : PropertyData                                 Pattern  : Data Services Static Class            *
*  Date      : 25/Jun/2013                                  Version  : 5.1     License: CC BY-NC-SA 3.0      *
*                                                                                                            *
*    Summary   : Provides database read and write methods for recording books.                               *
*                                                                                                            *
**************************************************** Copyright © La Vía Óntica SC + Ontica LLC. 1994-2013. **/
using System;
using System.Data;

using Empiria.Data;
using Empiria.Ontology;

namespace Empiria.Government.LandRegistration.Data {

  /// <summary>Provides database read and write methods for recording books.</summary>
  static public class PropertyData {

    #region Internal methods

    static internal DataRow GetPropertyWithTractKey(string propertyTractKey) {
      DataOperation operation = DataOperation.Parse("getLRSPropertyWithTractKey", propertyTractKey);

      return DataReader.GetDataRow(operation);
    }

    static internal bool ExistsPropertyTractKey(string propertyTractKey) {
      DataOperation operation = DataOperation.Parse("getLRSPropertyWithTractKey", propertyTractKey);

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

    static public ObjectList<PropertyEvent> GetPropertiesEventsList(RecordingAct recordingAct) {
      DataOperation operation = DataOperation.Parse("qryLRSRecordingActPropertiesEvents", recordingAct.Id);

      DataView view = DataReader.GetDataView(operation);

      return new ObjectList<PropertyEvent>((x) => PropertyEvent.Parse(x), view);
    }

    static internal ObjectList<RecordingActParty> GetInvolvedDomainParties(RecordingAct recordingAct) {
      string sql = String.Empty;
      if (!recordingAct.IsAnnotation) {
        sql = "SELECT * FROM LRSRecordingActParties " +
              "WHERE RecordingActId = " + recordingAct.Id.ToString() + " " +
              "AND PartyRoleId <> -1 AND LinkStatus <> 'X'";

      } else {
        string ids = String.Empty;
        ObjectList<PropertyEvent> events = recordingAct.PropertiesEvents;
        for (int i = 0; i < events.Count; i++) {
          ObjectList<RecordingAct> acts = events[i].Property.GetRecordingActsTract();

          for (int j = 0; j < acts.Count; j++) {
            if (ids.Length != 0) {
              ids += ",";
            }
            ids += acts[j].Id.ToString();
          }
        }
        sql = "SELECT DISTINCT * " +
              "FROM LRSRecordingActParties " +
              "WHERE (RecordingActId IN (" + ids + ") AND PartyRoleId <> -1) " +
              "AND (LinkStatus <> 'X')";
      }
      DataOperation operation = DataOperation.Parse(sql);

      return new ObjectList<RecordingActParty>((x) => RecordingActParty.Parse(x), DataReader.GetDataView(operation));
    }

    static internal ObjectList<RecordingActParty> GetRecordingActPartiesList(RecordingAct recordingAct) {
      string sql = String.Empty;

      sql = "SELECT * FROM LRSRecordingActParties " +
            "WHERE RecordingActId = " + recordingAct.Id.ToString() + " " +
            "AND LinkStatus <> 'X'";
      DataOperation operation = DataOperation.Parse(sql);

      return new ObjectList<RecordingActParty>((x) => RecordingActParty.Parse(x), DataReader.GetDataView(operation));
    }

    static internal ObjectList<RecordingActParty> GetRecordingPartiesList(Recording recording, Party party) {
      string sql = String.Empty;

      sql = "SELECT LRSRecordingActParties.* " +
            "FROM LRSRecordingActParties INNER JOIN LRSRecordingActs " +
            "ON LRSRecordingActParties.RecordingActId = LRSRecordingActs.RecordingActId " +
            "WHERE (LRSRecordingActs.RecordingId = " + recording.Id.ToString() + ") " +
            "AND (LRSRecordingActParties.LinkStatus <> 'X') " +
            "AND (LRSRecordingActParties.PartyId = " + party.Id.ToString() +
                " OR LRSRecordingActParties.SecondaryPartyId = " + party.Id.ToString() + ")";
      DataOperation operation = DataOperation.Parse(sql);

      return new ObjectList<RecordingActParty>((x) => RecordingActParty.Parse(x), DataReader.GetDataView(operation));
    }

    static internal ObjectList<RecordingActParty> GetDomainPartyList(RecordingAct recordingAct) {
      string sql = String.Empty;

      sql = "SELECT * FROM LRSRecordingActParties " +
            "WHERE RecordingActId = " + recordingAct.Id.ToString() + " " +
            "AND PartyRoleId <> -1 AND LinkStatus <> 'X'";

      DataOperation operation = DataOperation.Parse(sql);

      return new ObjectList<RecordingActParty>((x) => RecordingActParty.Parse(x), DataReader.GetDataView(operation));
    }

    static internal ObjectList<Property> GetRecordingActPropertiesList(RecordingAct recordingAct) {
      DataOperation operation = DataOperation.Parse("qryLRSRecordingActProperties", recordingAct.Id);

      return new ObjectList<Property>((x) => Property.Parse(x), DataReader.GetDataView(operation));
    }

    static internal ObjectList<RecordingActParty> GetSecondaryPartiesList(RecordingAct recordingAct) {
      string sql = "SELECT * FROM LRSRecordingActParties " +
                   "WHERE RecordingActId = " + recordingAct.Id.ToString() + " " +
                   "AND SecondaryPartyRoleId <> -1 AND LinkStatus <> 'X'";

      DataOperation operation = DataOperation.Parse(sql);

      return new ObjectList<RecordingActParty>((x) => RecordingActParty.Parse(x), DataReader.GetDataView(operation));
    }

    static public ObjectList<PropertyEvent> GetRecordingPropertiesAnnotationsList(Recording recording) {
      DataOperation operation = DataOperation.Parse("qryLRSRecordingPropertiesAnnotations", recording.Id);

      DataView view = DataReader.GetDataView(operation);

      return new ObjectList<PropertyEvent>((x) => PropertyEvent.Parse(x), view);
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
      Assertion.Require(o.Id != 0, "Property.Id can't be zero");
      DataOperation dataOperation = DataOperation.Parse("writeLRSProperty", o.Id, o.PropertyType.Id, o.TractKey,
                                                        o.CadastralOffice.Id, o.CadastralObjectId, o.CadastralKey,
                                                        o.LandUse.Id, o.CommonName, o.Antecedent,
                                                        o.Municipality.Id, o.Settlement.Id,
                                                        o.Street.Id, o.PostalCode.Id, o.ExternalNumber, o.InternalNumber,
                                                        o.BuildingTag, o.FloorTag, o.FractionTag, o.BatchTag,
                                                        o.BlockTag, o.SectionTag,
                                                        o.SuperSectionTag, o.FromStreet.Id, o.ToStreet.Id, o.Ubication,
                                                        o.FirstKnownOwner, o.Notes, o.Keywords, o.PostedBy.Id,
                                                        (char) o.Status, o.IntegrityHashCode);
      return DataWriter.Execute(dataOperation);
    }

    static internal int WritePropertyEvent(PropertyEvent o) {
      Assertion.Require(o.Id != 0, "PropertyEvent.Id can't be zero");
      DataOperation dataOperation = DataOperation.Parse("writeLRSPropertyEvent", o.Id, o.Property.Id, o.RecordingAct.Id,
                                                        o.MetesAndBounds, o.Notes, o.TotalArea.Amount, o.TotalArea.Unit.Id,
                                                        o.FloorArea.Amount, o.FloorArea.Unit.Id,
                                                        o.CommonArea.Amount, o.CommonArea.Unit.Id, (char) o.Status);
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

    #endregion Internal methods

  } // class PropertyData

} // namespace Empiria.Government.LandRegistration.Data