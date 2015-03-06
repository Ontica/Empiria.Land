/* Empiria Land 2015 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                 System   : Land Registration System              *
*  Namespace : Empiria.Land.Registration.Data               Assembly : Empiria.Land.Registration             *
*  Type      : PropertyData                                 Pattern  : Data Services                         *
*  Version   : 2.0        Date: 04/Jan/2015                 License  : Please read license.txt file          *
*                                                                                                            *
*  Summary   : Provides database read and write methods for recording books.                                 *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;

using Empiria.Data;
using Empiria.Ontology;

namespace Empiria.Land.Registration.Data {

  /// <summary>Provides database read and write methods for recording books.</summary>
  static public class PropertyData {

    #region Internal methods

    internal static Property[] GetPropertyPartitions(Property property) {
      if (property.IsNew || property.IsEmptyInstance) {
        return new Property[0];
      }
      DataOperation operation = DataOperation.Parse("qryLRSPropertyPartitions", property.Id);

      return DataReader.GetList<Property>(operation,
                                          (x) => BaseObject.ParseList<Property>(x)).ToArray();
    }

    static internal DataRow GetPropertyWithUID(string uniqueID) {
      DataOperation operation = DataOperation.Parse("getLRSPropertyWithUID", uniqueID);

      return DataReader.GetDataRow(operation);
    }

    static internal bool ExistsPropertyUID(string uniqueID) {
      DataOperation operation = DataOperation.Parse("getLRSPropertyWithUID", uniqueID);

      return (DataReader.Count(operation) != 0);
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

    static public DataTable GetPartiesOnRecordingBook(ObjectTypeInfo partyType,
                                                      RecordingBook recordingBook, string keywords) {
      var operation = DataOperation.Parse("qryLRSPartiesOnRecordingBook", recordingBook.Id);

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
        case PartyFilterType.OnRecordingBook:
          return GetPartiesOnRecordingBook(partyType, recordingAct.PhysicalRecording.RecordingBook, keywords);
        case PartyFilterType.Involved:
          return GetInvolvedParties(partyType, recordingAct, keywords);
        default:
          throw new Empiria.Reflection.ReflectionException(Empiria.Reflection.ReflectionException.Msg.ConditionalOptionNotDefined,
                                                           partyFilterType.ToString());
      }
    }

    static public DataTable GetSecondaryParties(Party party, RecordingAct exceptRecordingAct) {
      var operation = DataOperation.Parse("qryLRSSecondaryParties", exceptRecordingAct.Id, party.Id);

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
      return DataReader.GetList<RecordingActParty>(operation, (x) =>
                                                   BaseObject.ParseList<RecordingActParty>(x)).ToFixedList();
    }

    static internal FixedList<Resource> GetRecordingResources(Recording recording) {
      var operation = DataOperation.Parse("qryLRSRecordingProperties", recording.Id);

      return DataReader.GetList<Resource>(operation,
                                          (x) => BaseObject.ParseList<Resource>(x)).ToFixedList();
    }

    static public FixedList<TractIndexItem> GetRecordingPropertiesAnnotationsList(Recording recording) {
      throw new NotImplementedException();

      //var operation = DataOperation.Parse("qryLRSRecordingPropertiesAnnotations", recording.Id);

      //return DataReader.GetList<TractIndexItem>(operation, (x) =>
      //                                             BaseObject.ParseList<TractIndexItem>(x)).ToFixedList();
    }

    static internal int WriteHumanParty(HumanParty o) {
      var dataOperation = DataOperation.Parse("writeLRSParty", o.Id, o.GetEmpiriaType().Id, o.FullName,
                                               o.FirstName, o.FirstFamilyName, o.SecondFamilyName,
                                               o.MaritalFamilyName, o.ShortName, o.Nicknames, o.Tags,
                                               (char) o.Gender, o.RegistryDate, o.RegistryLocation.Id,
                                               o.CURPNumber, o.TaxIDNumber, o.IFENumber, String.Empty,
                                               String.Empty, String.Empty, String.Empty, -1, -1,
                                               ExecutionServer.DateMinValue, -1, ExecutionServer.DateMinValue,
                                               String.Empty, o.Notes, o.Keywords, o.PostedBy.Id, o.PostingTime,
                                               o.ReplacedById, (char) o.Status, String.Empty);

      return DataWriter.Execute(dataOperation);
    }

    static internal int WriteOrganizationParty(OrganizationParty o) {
      var dataOperation = DataOperation.Parse("writeLRSParty", o.Id, o.GetEmpiriaType().Id, o.FullName,
                                               String.Empty, String.Empty, String.Empty,
                                               String.Empty, o.ShortName, o.Nicknames, o.Tags,
                                               Contacts.Gender.NotApply, o.RegistryDate,
                                               o.RegistryLocation.Id, String.Empty, o.TaxIDNumber,
                                               String.Empty, o.AssocDocBookNumber, o.AssocDocNumber,
                                               o.AssocDocStartSheet, o.AssocDocEndSheet, o.AssocDocNotaryOffice.Id,
                                               o.AssocDocIssuedBy.Id, o.AssocDocIssueDate, o.AssocDocRecordingOffice.Id,
                                               o.AssocDocRecordingDate, o.AssocDocRecordingNumber, o.Notes, o.Keywords,
                                               o.PostedBy.Id, o.PostingTime, o.ReplacedById, (char) o.Status, String.Empty);

      return DataWriter.Execute(dataOperation);
    }

    static internal int WriteAssociation(Association o) {
      var operation = DataOperation.Parse("writeLRSProperty", o.Id, o.GetEmpiriaType().Id, o.UID,
                                          String.Empty, o.Name, o.PropertyKind.Value, o.Notes, 
                                          o.AntecedentNotes, o.AsText, String.Empty, 
                                          o.ExtensionData.ToString(), o.Keywords, 0, -1, -1, 
                                          String.Empty, -1, o.PostingTime, o.PostedBy.Id,
                                          (char) o.Status, o.Integrity.GetUpdatedHashCode());
      return DataWriter.Execute(operation);
    }

    static internal int WriteProperty(Property o) {
      var operation = DataOperation.Parse("writeLRSProperty", o.Id, o.GetEmpiriaType().Id, o.UID,
                                          o.CadastralKey, o.Name, o.PropertyKind.Value, o.Notes, 
                                          o.AntecedentNotes, o.AsText, o.Location.ToSearchVector(), 
                                          o.ExtensionData.ToString(), o.Keywords, o.LotSize, 
                                          o.LotSizeUnit.Id, o.IsPartitionOf.Id, o.PartitionNo, 
                                          o.MergedInto.Id, o.PostingTime, o.PostedBy.Id,
                                          (char) o.Status, o.Integrity.GetUpdatedHashCode());
      return DataWriter.Execute(operation);
    }

    static internal int WriteRecordingActParty(RecordingActParty o) {
      var dataOperation = DataOperation.Parse("writeLRSRecordingActParty", o.Id, o.RecordingAct.Id,
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
      Assertion.Assert(o.Id > 0, "Id needs to be positive.");
      Assertion.Assert(o.Resource.Id > 0, "property id needs to be positive.");
      Assertion.Assert(o.RecordingAct.Id > 0, "recotrding act id needs to be positive.");

      var dataOperation = DataOperation.Parse("writeLRSTractIndex", o.Id,
                                              o.Resource.Id, o.RecordingAct.Id, o.ExtensionData.ToJson(),
                                              (char) o.Status, o.Integrity.GetUpdatedHashCode());
      return DataWriter.Execute(dataOperation);
    }

    #endregion Internal methods

  } // class PropertyData

} // namespace Empiria.Land.Registration.Data
