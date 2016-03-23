/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RealEstatePartition                            Pattern  : Standard Class                      *
*  Version   : 2.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Contains data about a property partition or subdivision.                                      *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

using Empiria.DataTypes;
using Empiria.Json;

namespace Empiria.Land.Registration {

  public enum PropertyPartitionType {
    None = 'N',
    Partial = 'P',
    Last = 'L',
    Full = 'F'
  }

  public enum PropertyPartitionSubtype {
    None = 'N',
    Whole = 'W',
    Lot = 'L',
    Apartment = 'A',
    House = 'H',
    Partial = 'P',
    PartialUnknown = 'Q',
    Last = 'S',
    LastUnknown = 'T',
  }

  /// <summary>Contains data about a real estate partition or subdivision.</summary>
  public class RealEstatePartition {

    #region Constructors and parsers

    public RealEstatePartition(string cadastralKey,
                               PropertyPartitionType partitionType = PropertyPartitionType.None,
                               PropertyPartitionSubtype partitionSubtype = PropertyPartitionSubtype.None,
                               int partitionNo = 0, int totalPartitions = 0,
                               decimal partitionSize = 0m, int partitionSizeUnitId = -1,
                               decimal availableSize = 0m, int availableSizeUnitId = -1) {
      this.CadastralKey = cadastralKey;
      this.PartitionType = partitionType;
      this.PartitionSubtype = partitionSubtype;
      this.PartitionNo = partitionNo;
      this.TotalPartitions = totalPartitions;
      this.Size = Quantity.Parse(Unit.Parse(partitionSizeUnitId), partitionSize);
      this.AvailableSize = Quantity.Parse(Unit.Parse(availableSizeUnitId), availableSize);
    }

    #endregion Constructors and parsers

    #region Properties

    public PropertyPartitionType PartitionType {
      get;
      private set;
    }

    public PropertyPartitionSubtype PartitionSubtype {
      get;
      private set;
    }

    public string CadastralKey {
      get;
      private set;
    }

    public int PartitionNo {
      get;
      private set;
    }

    public int TotalPartitions {
      get;
      private set;
    }

    public Quantity Size {
      get;
      private set;
    }

    public Quantity AvailableSize {
      get;
      private set;
    }

    #endregion Properties

    #region Public methods

    public void AssertValid() {

    }

    public JsonObject ToJson() {
      var json = new JsonObject();

      json.Add(new JsonItem("type", this.PartitionType.ToString()));
      json.Add(new JsonItem("subType", this.PartitionSubtype.ToString()));
      json.Add(new JsonItem("partitionNo", this.PartitionNo.ToString()));
      json.Add(new JsonItem("availableSize", this.AvailableSize.Amount));
      json.Add(new JsonItem("availableSizeUnit", this.AvailableSize.Unit.Id));
      if (this.TotalPartitions != 0) {
        json.Add(new JsonItem("totalPartitions", this.TotalPartitions));
      }
      return json;
    }

    #endregion Public methods

  }  // class RealEstatePartition

}  // namespace Empiria.Land.Registration
