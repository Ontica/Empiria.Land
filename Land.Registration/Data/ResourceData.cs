/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                 System   : Land Registration System              *
*  Namespace : Empiria.Land.Registration.Data               Assembly : Empiria.Land.Registration             *
*  Type      : ResourceData                                 Pattern  : Data Services                         *
*  Version   : 2.1                                          License  : Please read license.txt file          *
*                                                                                                            *
*  Summary   : Provides database methods for recordable resources: real estate and associations.             *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;

using Empiria.Data;

namespace Empiria.Land.Registration.Data {

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

    static internal int WriteAssociation(Association o) {
      var operation = DataOperation.Parse("writeLRSProperty", o.Id, o.GetEmpiriaType().Id, o.UID,
                                          String.Empty, o.Name, String.Empty, o.Notes,
                                          o.AntecedentNotes, o.AsText, String.Empty,
                                          o.ExtensionData.ToString(), o.Keywords, 0, -1, -1,
                                          String.Empty, -1, o.PostingTime, o.PostedBy.Id,
                                          (char) o.Status, o.Integrity.GetUpdatedHashCode());
      return DataWriter.Execute(operation);
    }

    static internal int WriteNoPropertyResource(NoPropertyResource o) {
      var operation = DataOperation.Parse("writeLRSProperty", o.Id, o.GetEmpiriaType().Id, o.UID,
                                          String.Empty, String.Empty, String.Empty, o.Notes,
                                          o.AntecedentNotes, o.AsText, String.Empty,
                                          o.ExtensionData.ToString(), o.Keywords, 0, -1, -1,
                                          String.Empty, -1, o.PostingTime, o.PostedBy.Id,
                                          (char) o.Status, o.Integrity.GetUpdatedHashCode());
      return DataWriter.Execute(operation);
    }

    static internal int WriteRealEstate(RealEstate o) {
      var operation = DataOperation.Parse("writeLRSProperty", o.Id, o.GetEmpiriaType().Id, o.UID,
                                          o.CadastralKey, o.Name, o.PropertyKind, o.Notes,
                                          o.AntecedentNotes, o.AsText, o.Location.ToSearchVector(),
                                          o.ExtensionData.ToString(), o.Keywords, o.LotSize,
                                          o.LotSizeUnit.Id, o.IsPartitionOf.Id, o.PartitionNo,
                                          o.MergedInto.Id, o.PostingTime, o.PostedBy.Id,
                                          (char) o.Status, o.Integrity.GetUpdatedHashCode());
      return DataWriter.Execute(operation);
    }

    #endregion Internal methods

  } // class ResourceData

} // namespace Empiria.Land.Registration.Data
