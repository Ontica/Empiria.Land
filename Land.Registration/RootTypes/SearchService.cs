/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                 System   : Land Registration System              *
*  Namespace : Empiria.Land.Registration                    Assembly : Empiria.Land.Registration             *
*  Type      : SearchService                                Pattern  : Search Services                       *
*  Version   : 3.0                                          License  : Please read license.txt file          *
*                                                                                                            *
*  Summary   : Provides search services over documents, recording books, parties and resources.              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

using Empiria.Land.Certification;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Registration {

  /// <summary>Provides search services over documents, recording books, parties and resources.</summary>
  static public class SearchService {

    #region Public methods

    static public FixedList<FormerCertificate> Certificates(string keywords, string sort = "") {
      string filter = FilterExpression("CertificateKeywords", keywords);
      sort = SortExpression(sort, "CertificateUID");

      string sql = EntitySqlString("LRSCertificates", "CertificateStatus", filter, sort, 50);

      return DataReader.GetList(DataOperation.Parse(sql),
                                (x) => BaseObject.ParseList<FormerCertificate>(x)).ToFixedList();
    }


    static public FixedList<RecordingDocument> Documents(string keywords, string sort = "") {
      string filter = FilterExpression("DocumentKeywords", keywords);
      sort = SortExpression(sort, "DocumentUID");

      string sql = EntitySqlString("LRSDocuments", "DocumentStatus", filter, sort, 50);

      return DataReader.GetList(DataOperation.Parse(sql),
                                (x) => BaseObject.ParseList<RecordingDocument>(x)).ToFixedList();
    }


    static public FixedList<RecordingDocument> ImagingControlIDs(string imagingControlID, string sort = "") {
      string filter = FilterExpression("ImagingControlID", imagingControlID, true);
      sort = SortExpression(sort, "ImagingControlID DESC");

      string sql = EntitySqlString("LRSDocuments", "DocumentStatus", filter, sort, 250);

      return DataReader.GetList(DataOperation.Parse(sql),
                                (x) => BaseObject.ParseList<RecordingDocument>(x)).ToFixedList();
    }


    static public FixedList<LegacyParty> LegacyParties(string keywords) {
      string filter = String.Empty;

      if (keywords.Length != 0) {
        filter = SearchExpression.ParseAndLike("Keywords", EmpiriaString.BuildKeywords(keywords));
      }

      string sql = EntitySqlString("OldLegacyParties", String.Empty, filter, "FullName", 250);

      return DataReader.GetFixedList<LegacyParty>(DataOperation.Parse(sql));
    }


    static public FixedList<RecordingActParty> Parties(string keywords, string sort = "") {
      string filter = FilterExpression("FullSearchKeywords", keywords);
      sort = SortExpression(sort, "PartyFullName, PartyOfFullName");

      string sql = EntitySqlString("vwLRSRecordingActParties", "RecActPartyStatus",
                                    filter, sort, 250);

      return DataReader.GetList(DataOperation.Parse(sql),
                                (x) => BaseObject.ParseList<RecordingActParty>(x)).ToFixedList();
    }


    static public FixedList<RecordingActParty> PrimaryParties(string keywords, string sort = "") {
      string keywordsFilter = FilterExpression("PartyKeywords", keywords);
      const string domainActFilter = "[RecordingActTypeName] LIKE 'ObjectType.RecordingAct.DomainAct.%'";

      string filter = GeneralDataOperations.BuildSqlAndFilter(domainActFilter, keywordsFilter);

      sort = SortExpression(sort, "PartyFullName");

      string sql = EntitySqlString("vwLRSRecordingActParties", "", filter, sort, 250);

      return DataReader.GetList(DataOperation.Parse(sql),
                                (x) => BaseObject.ParseList<RecordingActParty>(x)).ToFixedList();
    }


    static public FixedList<PhysicalRecording> PhysicalRecordings(string keywords, string sort = "") {
      string filter = FilterExpression("RecordingKeywords", keywords);
      sort = SortExpression(sort, "RecordingAsText");
      string sql = EntitySqlString("LRSPhysicalRecordings", "RecordingStatus", filter, sort, 50);

      return DataReader.GetList(DataOperation.Parse(sql),
                                (x) => BaseObject.ParseList<PhysicalRecording>(x)).ToFixedList();
    }


    static public FixedList<RecordingBook> RecordingBooks(string keywords, string sort = "") {
      string filter = FilterExpression("BookKeywords", keywords);
      sort = SortExpression(sort, "BookAsText");
      string sql = EntitySqlString("LRSPhysicalBooks", "BookStatus", filter, sort, 50);

      return DataReader.GetList(DataOperation.Parse(sql),
                                (x) => BaseObject.ParseList<RecordingBook>(x)).ToFixedList();
    }


    static public FixedList<Resource> Resources(string keywords, string sort = "") {
      string filter = FilterExpression("PropertyKeywords", keywords);
      sort = SortExpression(sort, "PropertyUID");

      string sql = EntitySqlString("LRSProperties", "PropertyStatus", filter, sort, 50);

      return DataReader.GetList(DataOperation.Parse(sql),
                                (x) => BaseObject.ParseList<Resource>(x)).ToFixedList();
    }


    static public FixedList<LRSTransaction> Transactions(string keywords, string sort = "") {
      string filter = FilterExpression("TransactionKeywords", keywords);
      sort = SortExpression(sort, "TransactionUID");

      string sql = EntitySqlString("LRSTransactions", "TransactionStatus", filter, sort, 50);

      return DataReader.GetList(DataOperation.Parse(sql),
                                (x) => BaseObject.ParseList<LRSTransaction>(x)).ToFixedList();
    }

    #endregion Public methods

    #region Private methods


    static private string EntitySqlString(string entityTableName, string entityStatusFieldName,
                                          string filter, string sort, int howManyRows) {
      string sql = String.Empty;

      if (howManyRows > 0) {
        sql = "SELECT TOP " + howManyRows + " * FROM " + entityTableName;
      } else {
        sql = "SELECT * FROM " + entityTableName;
      }

      if (entityStatusFieldName.Length != 0) {
        filter = GeneralDataOperations.BuildSqlAndFilter(filter, entityStatusFieldName + " <> 'X'");
      }

      return sql + GeneralDataOperations.GetFilterSortSqlString(filter, sort);
    }


    static private string FilterExpression(string fieldName, string keywords, bool searchAsIs = false) {
      keywords = keywords ?? String.Empty;

      string filter = String.Empty;

      if (searchAsIs) {
        filter = SearchExpression.ParseLike(fieldName, keywords);
      } else {
        keywords = EmpiriaString.RemovePunctuation(keywords);
        if (!keywords.Contains(" ")) {
          filter = SearchExpression.ParseLike(fieldName, keywords);
        } else {
          filter = SearchExpression.ParseAndLike(fieldName, keywords);
        }
      }

      if (filter.Length != 0) {
        return filter;
      } else {
        return SearchExpression.NoRecordsFilter;
      }
    }


    static private string SortExpression(string sort, string defaultSort = "") {
      sort = sort ?? String.Empty;

      if (sort.Length == 0 && defaultSort.Length != 0) {
        return defaultSort;
      } else {
        return sort;
      }
    }

    #endregion Private methods

  } // class SearchService

} // namespace Empiria.Land.Registration
