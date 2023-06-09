/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                 System   : Land Registration System              *
*  Namespace : Empiria.Land.Data                            Assembly : Empiria.Land.Registration             *
*  Type      : ResourceTractData                            Pattern  : Data Services                         *
*  Version   : 3.0                                          License  : Please read license.txt file          *
*                                                                                                            *
*  Summary   : Provides database read methods for resource recording tracts.                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Data;

using Empiria.Data;

using Empiria.Land.Registration;

namespace Empiria.Land.Data {

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

      return DataReader.GetFixedList<RecordingAct>(operation);
    }


    /// <summary>Gets those recording acts where the given resource appears as
    ///  the main resource or as the related resource.</summary>
    internal static FixedList<RecordingAct> GetResourceFullTractIndex(Resource resource) {
      if (resource.IsEmptyInstance) {
        return new FixedList<RecordingAct>();
      }

      var operation = DataOperation.Parse("qryLRSResourceFullTractIndex", resource.Id);

      return DataReader.GetFixedList<RecordingAct>(operation);
    }


    static public FixedList<RecordingAct> GetResourceRecordingActListUntil(Resource resource, RecordingAct breakAct,
                                                                           bool includeBreakAct) {
      if (resource.IsEmptyInstance) {
        return new FixedList<RecordingAct>();
      }

      var operation = DataOperation.Parse("qryLRSResourceRecordingActs", resource.Id);

      FixedList<RecordingAct> recordingActs = DataReader.GetFixedList<RecordingAct>(operation);

      List<RecordingAct> list = new List<RecordingAct>();

      foreach (var act in recordingActs) {
        if (act.Equals(breakAct)) {
          if (includeBreakAct) {
            list.Add(act);
          }
          break;
        } else {
          list.Add(act);
        }
      }

      return list.ToFixedList();
    }

    #endregion Public methods

  } // class ResourceTractData

} // namespace Empiria.Land.Data
