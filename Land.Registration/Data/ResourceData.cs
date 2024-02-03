/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                 System   : Land Registration System              *
*  Namespace : Empiria.Land.Data                            Assembly : Empiria.Land.Registration             *
*  Type      : ResourceData                                 Pattern  : Data Services                         *
*  Version   : 3.0                                          License  : Please read license.txt file          *
*                                                                                                            *
*  Summary   : Provides database methods for recordable resources: real estate and associations.             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

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

    static internal RealEstate[] GetRealEstatePartitions(RealEstate property) {
      if (property.IsNew || property.IsEmptyInstance) {
        return new RealEstate[0];
      }
      DataOperation operation = DataOperation.Parse("qryLRSRealEstatePartitions", property.Id);

      return DataReader.GetFixedList<RealEstate>(operation).ToArray();
    }


    static internal Resource TryGetResourceWithUID(string uniqueID) {
      DataOperation operation = DataOperation.Parse("getLRSPropertyWithUID", uniqueID);

      return DataReader.GetObject<Resource>(operation, null);
    }


    static internal FixedList<Resource> SearchResources(string filter, string orderBy, int pageSize) {
      string sql = $"SELECT TOP {pageSize} LRSProperties.* " +
                   $"FROM LRSProperties " +
                   $"WHERE {filter} ORDER BY {orderBy}";

      var operation = DataOperation.Parse(sql);

      return DataReader.GetFixedList<Resource>(operation);
    }


    static internal void WriteAssociation(Association o) {
      var operation = DataOperation.Parse("writeLRSProperty", o.Id, o.GetEmpiriaType().Id, o.GUID, o.UID,
                                          o.Name, o.Kind, o.Description, o.RecorderOffice.Id, -1, String.Empty,
                                          ExecutionServer.DateMinValue, 0, -1,String.Empty,
                                          o.Keywords, -1, String.Empty, -1,
                                          o.PostingTime, o.PostedBy.Id, (char) o.Status, o.Integrity.GetUpdatedHashCode());

      DataWriter.Execute(operation);
    }


    static internal void WriteNoPropertyResource(NoPropertyResource o) {
      var operation = DataOperation.Parse("writeLRSProperty", o.Id, o.GetEmpiriaType().Id, o.GUID, o.UID,
                                           o.Name, o.Kind, o.Description, o.RecorderOffice.Id, -1, String.Empty,
                                           ExecutionServer.DateMinValue, 0, -1, String.Empty,
                                           o.Keywords, -1, String.Empty, -1,
                                           o.PostingTime, o.PostedBy.Id, (char) o.Status, o.Integrity.GetUpdatedHashCode());
      DataWriter.Execute(operation);
    }


    static internal void WriteRealEstate(RealEstate o) {
      var operation = DataOperation.Parse("writeLRSProperty", o.Id, o.GetEmpiriaType().Id, o.GUID, o.UID,
                                          o.Name, o.Kind, o.Description, o.RecorderOffice.Id, o.Municipality.Id, o.CadastralKey,
                                          o.CadastreLinkingDate, o.LotSize.Amount, o.LotSize.Unit.Id, o.RealEstateExtData.ToString(),
                                          o.Keywords, o.IsPartitionOf.Id, o.PartitionNo, o.MergedInto.Id,
                                          o.PostingTime, o.PostedBy.Id, (char) o.Status, o.Integrity.GetUpdatedHashCode());

      DataWriter.Execute(operation);
    }

    #endregion Internal methods

  } // class ResourceData

} // namespace Empiria.Land.Data
