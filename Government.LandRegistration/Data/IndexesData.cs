﻿/* Empiria® Land 2013 ****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria® Land                                System   : Land Registration System              *
*  Namespace : Empiria.Government.LandRegistration.Data     Assembly : Empiria.Government.LandRegistration   *
*  Type      : PartyIndexData                               Pattern  : Data Services Static Class            *
*  Date      : 25/Jun/2013                                  Version  : 5.1     License: CC BY-NC-SA 3.0      *
*                                                                                                            *
*    Summary   : Provides database read and write methods for recording books.                               *
*                                                                                                            *
**************************************************** Copyright © La Vía Óntica SC + Ontica LLC. 1999-2013. **/
using System;
using System.Collections.Generic;
using System.Data;
using Empiria.Data;

namespace Empiria.Government.LandRegistration.Data {

  public class PartyIndexData {

    internal PartyIndexData(DataRowView row) {
      this.Id = (int) row["PartyIndexId"];
      this.PartyFullName = (string) row["PartyFullName"];
      this.PropertyTag = (string) row["PropertyTag"];
      this.RecordingBook = (string) row["RecordingBook"];
      this.RecordingDocument = (string) row["RecordingDocument"];
      this.PropertyAddress = (string) row["PropertyAddress"];
    }

    public int Id { get; set; }
    public string PartyFullName { get; set; }
    public string PropertyTag { get; set; }
    public string RecordingBook { get; set; }
    public string RecordingDocument { get; set; }
    public string PropertyAddress { get; set; }
  }

  /// <summary>Provides database read and write methods for recording books.</summary>
  static public class IndexesData {

    #region Public methods


    static public List<PartyIndexData> FindParty(string partyKeywords, string sort) {
      string filter = SearchExpression.ParseAndLikeWithNoiseWords("PartyKeywords", partyKeywords);

      DataOperation dataOperation = DataOperation.Parse("SELECT * FROM vwLRSPartiesIndex WHERE " + filter);

      DataView view = DataReader.GetDataView(dataOperation, String.Empty, sort);
      List<PartyIndexData> list = new List<PartyIndexData>(view.Count);
      for (int i = 0; i < view.Count; i++) {
        list.Add(new PartyIndexData(view[i]));
      }
      return list;
    }

    static public DataView FindByParty(RecorderOffice recorderOffice,
                                        DateTime fromDate, DateTime toDate, string partyKeywords) {
      string filter = SearchExpression.ParseAndLikeWithNoiseWords("PartyKeywords", partyKeywords);
      if (!recorderOffice.IsEmptyInstance) {
        if (filter.Length != 0) {
          filter += " AND ";
        }
        filter += "(RecorderOfficeId = " + recorderOffice.Id.ToString() + ")";
      }

      string sort = "FirstFamilyName, SecondFamilyName, FirstName, PartyId, PropertyId";

      DataOperation dataOperation = DataOperation.Parse("SELECT * FROM vwLRSIndexesMaster WHERE " + filter);

      return DataReader.GetDataView(dataOperation, String.Empty, sort);
    }

    static public DataView FindByProperty(RecorderOffice recorderOffice,
                                           DateTime fromDate, DateTime toDate, string propertyKeywords) {
      string filter = SearchExpression.ParseAndLikeWithNoiseWords("PropertyKeywords", propertyKeywords);
      if (!recorderOffice.IsEmptyInstance) {
        if (filter.Length != 0) {
          filter += " AND ";
        }
        filter += "(RecorderOfficeId = " + recorderOffice.Id.ToString() + ")";
      }

      string sort = "Municipality, Settlement, Street, ExternalNumber, InternalNumber, PropertyId";
      DataOperation dataOperation = DataOperation.Parse("SELECT * FROM vwLRSIndexesMaster WHERE " + filter);

      return DataReader.GetDataView(dataOperation, String.Empty, sort);
    }


    #endregion Public methods

  } // class IndexesData

} // namespace Empiria.Government.LandRegistration.Data