/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                 System   : Land Registration System              *
*  Namespace : Empiria.Land.Registration.Data               Assembly : Empiria.Land.Registration             *
*  Type      : ResourceTractData                            Pattern  : Data Services                         *
*  Version   : 3.0                                          License  : Please read license.txt file          *
*                                                                                                            *
*  Summary   : Provides database read methods for resource recording tracts.                                 *
*                                                                                                            *
********************************* Copyright (c) 2009-2017. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Collections.Generic;
using System.Data;

using Empiria.Data;

namespace Empiria.Land.Registration.Data {

  /// <summary>Provides database read methods for resource recording tracts.</summary>
  static public class ResourceTractData {

    #region Public methods

    /// <summary>Gets those recording acts where the given resource appears as
    ///  the main resource (ResourceId field).</summary>
    static public FixedList<RecordingAct> GetResourceRecordingActList(Resource resource) {
      if (resource.IsEmptyInstance) {
        return new FixedList<RecordingAct>();
      }

      var operation = DataOperation.Parse("qryLRSResourceRecordingActs", resource.Id);

      return DataReader.GetList<RecordingAct>(operation,
                                              (x) => BaseObject.ParseList<RecordingAct>(x)).ToFixedList();
    }


    /// <summary>Gets those recording acts where the given resource appears as
    ///  the main resource or as the related resource.</summary>
    internal static FixedList<RecordingAct> GetResourceFullTractIndex(Resource resource) {
      if (resource.IsEmptyInstance) {
        return new FixedList<RecordingAct>();
      }

      var operation = DataOperation.Parse("qryLRSResourceFullTractIndex", resource.Id);

      return DataReader.GetList<RecordingAct>(operation,
                                              (x) => BaseObject.ParseList<RecordingAct>(x)).ToFixedList();
    }


    static public FixedList<RecordingAct> GetResourceRecordingActListUntil(Resource resource, RecordingAct breakAct,
                                                                           bool includeBreakAct) {
      if (resource.IsEmptyInstance) {
        return new FixedList<RecordingAct>();
      }

      var operation = DataOperation.Parse("qryLRSResourceRecordingActs", resource.Id);
      DataTable table = DataReader.GetDataTable(operation);
      List<RecordingAct> list = new List<RecordingAct>();
      foreach (DataRow row in table.Rows) {
        var recordingAct = BaseObject.ParseDataRow<RecordingAct>(row);
        if (recordingAct.Equals(breakAct)) {
          if (includeBreakAct) {
            list.Add(BaseObject.ParseDataRow<RecordingAct>(row));
          }
          break;
        } else {
          list.Add(BaseObject.ParseDataRow<RecordingAct>(row));
        }
      }
      return list.ToFixedList();
    }

    #endregion Public methods

  } // class ResourceTractData

} // namespace Empiria.Land.Registration.Data
