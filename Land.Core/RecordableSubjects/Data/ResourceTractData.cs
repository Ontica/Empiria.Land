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

using System.Collections.Generic;

using Empiria.Data;

using Empiria.Land.Registration;

namespace Empiria.Land.Data {

  /// <summary>Provides database read methods for resource recording tracts.</summary>
  static internal class ResourceTractData {

    #region Methods

    /// <summary>Gets those recording acts where the given resource appears as
    ///  the main resource (ResourceId field).</summary>
    static internal FixedList<RecordingAct> GetResourceRecordingActList(Resource resource) {
      if (resource.IsEmptyInstance) {
        return new FixedList<RecordingAct>();
      }

      var op = DataOperation.Parse("qryLRSResourceRecordingActs", resource.Id);

      return DataReader.GetFixedList<RecordingAct>(op);
    }


    /// <summary>Gets those recording acts where the given resource appears as
    ///  the main resource or as the related resource.</summary>
    static internal FixedList<RecordingAct> GetResourceFullTractIndex(Resource resource) {
      if (resource.IsEmptyInstance) {
        return new FixedList<RecordingAct>();
      }

      var op = DataOperation.Parse("qryLRSResourceFullTractIndex", resource.Id);

      return DataReader.GetFixedList<RecordingAct>(op);
    }


    static internal FixedList<RecordingAct> GetResourceRecordingActListUntil(Resource resource, RecordingAct breakAct,
                                                                           bool includeBreakAct) {
      if (resource.IsEmptyInstance) {
        return new FixedList<RecordingAct>();
      }

      var op = DataOperation.Parse("qryLRSResourceRecordingActs", resource.Id);

      FixedList<RecordingAct> recordingActs = DataReader.GetFixedList<RecordingAct>(op);

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

    #endregion Methods

  } // class ResourceTractData

} // namespace Empiria.Land.Data
