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
  static internal class ResourceData {

    #region Methods

    static internal bool ExistsResourceUID(string uniqueID) {
      return TryGetResourceWithUID(uniqueID) != null;
    }


    static internal FixedList<RealEstate> GetRealEstatePartitions(RealEstate property) {
      if (property.IsNew || property.IsEmptyInstance) {
        return new FixedList<RealEstate>();
      }

      var sql = "SELECT * FROM LRSProperties " +
               $"WHERE PartitionOfId = {property.Id} AND PropertyStatus <> 'X' " +
               $"ORDER BY PartitionNo";

      var op = DataOperation.Parse(sql);

      return DataReader.GetFixedList<RealEstate>(op);
    }


    static internal Resource TryGetResourceWithUID(string uniqueID) {

      var sql = "SELECT * FROM LRSProperties " +
               $"WHERE PropertyUID = '{uniqueID}'";

      var op = DataOperation.Parse(sql);

      return DataReader.GetObject<Resource>(op, null);
    }


    static internal FixedList<Resource> SearchResources(string filter, string orderBy, int pageSize) {

      string sql = $"SELECT TOP {pageSize} LRSProperties.* " +
                   $"FROM LRSProperties " +
                   $"WHERE {filter} ORDER BY {orderBy}";

      var op = DataOperation.Parse(sql);

      return DataReader.GetFixedList<Resource>(op);
    }


    static internal void WriteAssociation(Association o) {
      var op = DataOperation.Parse("writeLRSProperty", o.Id, o.GetEmpiriaType().Id, o.GUID, o.UID,
                                   o.Name, o.Kind, o.Description, o.RecorderOffice.Id, -1, String.Empty,
                                   ExecutionServer.DateMinValue, 0, -1,String.Empty,
                                   o.Keywords, -1, String.Empty, -1,
                                   o.PostingTime, o.PostedBy.Id, (char) o.Status, o.Integrity.GetUpdatedHashCode());

      DataWriter.Execute(op);
    }


    static internal void WriteNoPropertyResource(NoPropertyResource o) {
      var op = DataOperation.Parse("writeLRSProperty", o.Id, o.GetEmpiriaType().Id, o.GUID, o.UID,
                                   o.Name, o.Kind, o.Description, o.RecorderOffice.Id, -1, String.Empty,
                                   ExecutionServer.DateMinValue, 0, -1, String.Empty,
                                   o.Keywords, -1, String.Empty, -1,
                                   o.PostingTime, o.PostedBy.Id, (char) o.Status, o.Integrity.GetUpdatedHashCode());
      DataWriter.Execute(op);
    }


    static internal void WriteRealEstate(RealEstate o) {
      var op = DataOperation.Parse("writeLRSProperty", o.Id, o.GetEmpiriaType().Id, o.GUID, o.UID,
                                   o.Name, o.Kind, o.Description, o.RecorderOffice.Id, o.Municipality.Id, o.CadastralKey,
                                   o.CadastreLinkingDate, o.LotSize.Amount, o.LotSize.Unit.Id, o.RealEstateExtData.ToString(),
                                   o.Keywords, o.IsPartitionOf.Id, o.PartitionNo, o.MergedInto.Id,
                                   o.PostingTime, o.PostedBy.Id, (char) o.Status, o.Integrity.GetUpdatedHashCode());

      DataWriter.Execute(op);
    }

    #endregion Methods

  } // class ResourceData

} // namespace Empiria.Land.Data
