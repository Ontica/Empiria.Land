/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                 System   : Land Registration System              *
*  Namespace : Empiria.Land.Registration.Data               Assembly : Empiria.Land.Registration             *
*  Type      : PropertyData                                 Pattern  : Data Services                         *
*  Version   : 3.0                                          License  : Please read license.txt file          *
*                                                                                                            *
*  Summary   : Provides database read and write methods for recording books.                                 *
*                                                                                                            *
********************************* Copyright (c) 2009-2017. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;

using Empiria.Data;

namespace Empiria.Land.Registration.Data {

  /// <summary>Provides database read and write methods for recording books.</summary>
  static public class AnalyticsData {

    #region Public methods

    //static public DataView PerformanceByAnalyst(RecorderOffice recorderOffice, DateTime fromDate, DateTime toDate) {
    //  DataOperation dataOperation = DataOperation.Parse("rptLRSPerformanceByAnalyst", recorderOffice.Id, fromDate, toDate);

    //  dataOperation.ExecutionTimeout = 30;

    //  return DataReader.GetDataView(dataOperation);
    //}

    //static public DataView RecorderOfficesStats() {
    //  DataOperation dataOperation = DataOperation.Parse("SELECT * FROM vwLRSRecordingOfficeStats");

    //  //dataOperation.ExecutionTimeout = 30;

    //  return DataReader.GetDataView(dataOperation);
    //}

    //static public DataView RecordingActTypeIncidence(RecorderOffice recorderOffice, DateTime fromDate, DateTime toDate) {
    //  DataOperation dataOperation = DataOperation.Parse("rptLRSRecordingActsIncidence", recorderOffice.Id, fromDate, toDate);
    //  //dataOperation.ExecutionTimeout = 90;

    //  return DataReader.GetDataView(dataOperation);
    //}

    #endregion Public methods

  } // class AnalyticsData

} // namespace Empiria.Land.Registration.Data
