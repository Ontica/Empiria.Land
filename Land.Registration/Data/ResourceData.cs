/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                 System   : Land Registration System              *
*  Namespace : Empiria.Land.Data                            Assembly : Empiria.Land.Registration             *
*  Type      : ResourceData                                 Pattern  : Data Services                         *
*  Version   : 3.0                                          License  : Please read license.txt file          *
*                                                                                                            *
*  Summary   : Provides database methods for recordable resources: real estate and associations.             *
*                                                                                                            *
********************************* Copyright (c) 2009-2017. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;

using Empiria.Data;

using Empiria.Land.Registration;

namespace Empiria.Land.Data {

  /// <summary>Provides database methods for recordable resources: real estate and associations.</summary>
  static public class ResourceData {

    #region Internal methods

    static internal bool ExistsResourceUID(string uniqueID) {
      DataOperation operation = DataOperation.Parse("getLRSPropertyWithUID", uniqueID);

      return (DataReader.Count(operation) != 0);
    }

    static internal FixedList<Resource> GetPhysicalRecordingResources(Recording recording) {
      var operation = DataOperation.Parse("qryLRSPhysicalRecordingResources", recording.Id);

      return DataReader.GetList<Resource>(operation,
                                          (x) => BaseObject.ParseList<Resource>(x)).ToFixedList();
    }

    internal static RealEstate[] GetRealEstatePartitions(RealEstate property) {
      if (property.IsNew || property.IsEmptyInstance) {
        return new RealEstate[0];
      }
      DataOperation operation = DataOperation.Parse("qryLRSRealEstatePartitions", property.Id);

      return DataReader.GetList<RealEstate>(operation,
                                          (x) => BaseObject.ParseList<RealEstate>(x)).ToArray();
    }

    static internal DataRow GetResourceWithUID(string uniqueID) {
      DataOperation operation = DataOperation.Parse("getLRSPropertyWithUID", uniqueID);

      return DataReader.GetDataRow(operation);
    }

    static internal void WriteAssociation(Association o) {
      var operation = DataOperation.Parse("writeLRSProperty", o.Id, o.GetEmpiriaType().Id, o.UID,
                                          String.Empty, o.AssociationExtData.ToString(), o.Keywords,
                                          -1, String.Empty, -1,
                                          o.PostingTime, o.PostedBy.Id,
                                          (char) o.Status, o.Integrity.GetUpdatedHashCode());

      DataWriter.Execute(operation);
    }

    static internal void WriteNoPropertyResource(NoPropertyResource o) {
      var operation = DataOperation.Parse("writeLRSProperty", o.Id, o.GetEmpiriaType().Id, o.UID,
                                          String.Empty, String.Empty, o.Keywords,
                                          -1, String.Empty, -1,
                                          o.PostingTime, o.PostedBy.Id,
                                          (char) o.Status, o.Integrity.GetUpdatedHashCode());
      DataWriter.Execute(operation);
    }

    static internal void WriteRealEstate(RealEstate o) {
      var operation = DataOperation.Parse("writeLRSProperty", o.Id, o.GetEmpiriaType().Id, o.UID,
                                          o.CadastralKey, o.RealEstateExtData.ToString(), o.Keywords,
                                          o.IsPartitionOf.Id, o.PartitionNo, o.MergedInto.Id,
                                          o.PostingTime, o.PostedBy.Id,
                                          (char) o.Status, o.Integrity.GetUpdatedHashCode());

      DataWriter.Execute(operation);
    }

    #endregion Internal methods

  } // class ResourceData

} // namespace Empiria.Land.Data
