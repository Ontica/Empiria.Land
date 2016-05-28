/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                 System   : Land Registration System              *
*  Namespace : Empiria.Land.Registration                    Assembly : Empiria.Land.Registration             *
*  Type      : SearchService                                Pattern  : Search Services                       *
*  Version   : 2.1                                          License  : Please read license.txt file          *
*                                                                                                            *
*  Summary   : Provides search services over documents, recording books, parties and resources.              *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

using Empiria.Data;
using Empiria.Land.Certification;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Registration {

  /// <summary>Provides search services over documents, recording books, parties and resources.</summary>
  static public class SearchService {

    #region Public methods

    static public FixedList<Certificate> Certificates(string keywords, string sort = "") {
      string filter = FilterExpression("CertificateKeywords", keywords);
      sort = SortExpression(sort, "CertificateUID");

      string sql = EntitySqlString("LRSCertificates", "CertificateStatus", filter, sort, 50);

      return DataReader.GetList(DataOperation.Parse(sql),
                                (x) => BaseObject.ParseList<Certificate>(x)).ToFixedList();
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
      sort = SortExpression(sort, "ImagingControlID");

      string sql = EntitySqlString("LRSDocuments", "DocumentStatus", filter, sort, 50);

      return DataReader.GetList(DataOperation.Parse(sql),
                                (x) => BaseObject.ParseList<RecordingDocument>(x)).ToFixedList();
    }


    static public FixedList<RecordingActParty> Parties(string keywords, string sort = "") {
      string filter = FilterExpression("FullSearchKeywords", keywords);
      sort = SortExpression(sort, "PartyFullName, PartyOfFullName");

      string sql = EntitySqlString("vwLRSRecordingActParties", "RecActPartyStatus", filter, sort, 50);

      return DataReader.GetList(DataOperation.Parse(sql),
                                (x) => BaseObject.ParseList<RecordingActParty>(x)).ToFixedList();
    }


    static public FixedList<Recording> PhysicalRecordings(string keywords, string sort = "") {
      string filter = FilterExpression("RecordingKeywords", keywords);
      sort = SortExpression(sort, "RecordingAsText");
      string sql = EntitySqlString("LRSPhysicalRecordings", "RecordingStatus", filter, sort, 50);

      return DataReader.GetList(DataOperation.Parse(sql),
                                (x) => BaseObject.ParseList<Recording>(x)).ToFixedList();
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
        filter = SearchExpression.ParseLike(fieldName, keywords, true);
      } else {
        keywords = EmpiriaString.RemovePunctuation(keywords);
        if (!keywords.Contains(" ")) {
          filter = SearchExpression.ParseLike(fieldName, keywords, true);
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
