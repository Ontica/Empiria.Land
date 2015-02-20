﻿/* Empiria Land 2015 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : Recording                                      Pattern  : Empiria Object Type                 *
*  Version   : 2.0        Date: 04/Jan/2015                   License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Represents a real estate property.                                                            *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;

using Empiria.Contacts;
using Empiria.DataTypes;
using Empiria.Geography;
using Empiria.Json;
using Empiria.Security;
using Empiria.Land.Registration.Data;

namespace Empiria.Land.Registration {

  /// <summary>Represents a real estate property.</summary>
  public class Property : Resource {

    #region Constructors and parsers

    internal Property() {
      // Required by Empiria Framework.
    }

    static public new Property Parse(int id) {
      return BaseObject.ParseId<Property>(id);
    }

    static public new Property TryParseWithUID(string propertyUID) {
      DataRow row = PropertyData.GetPropertyWithUID(propertyUID);

      if (row != null) {
        return BaseObject.ParseDataRow<Property>(row);
      } else {
        return null;
      }
    }

    static public Property Empty {
      get { return BaseObject.ParseEmpty<Property>(); }
    }

    #endregion Constructors and parsers

    #region Public properties

    public Address Location {
      get;
      private set;
    }

    public CadastralInfo CadastralData {
      get;
      private set;
    }

    internal protected override string Keywords {
      get {
        return EmpiriaString.BuildKeywords(base.Keywords, this.CadastralData.CadastralCode,
                                           this.PartitionNo, this.Location.Keywords);
      }
    }

    [DataField("LotSize")]
    public decimal LotSize {
      get;
      private set;
    }

    [DataField("LotSizeUnitId")]
    public Unit LotSizeUnit {
      get;
      private set;
    }

    [DataField("PartitionOfId")]
    private LazyInstance<Property> _isPartitionOf = LazyInstance<Property>.Empty;
    public Property IsPartitionOf {
      get { return _isPartitionOf.Value; }
      private set {
        _isPartitionOf = LazyInstance<Property>.Parse(value);
      }
    }

    [DataField("PartitionNo")]
    public string PartitionNo {
      get;
      private set;
    }

    [DataField("MergedIntoId")]
    private LazyInstance<Property> _mergedInto = LazyInstance<Property>.Empty;
    public Property MergedInto {
      get { return _mergedInto.Value; }
      private set {
        _mergedInto = LazyInstance<Property>.Parse(value);
      }
    }

    #endregion Public properties

    #region Public methods

    protected override string CreatePropertyKey() {
      return TransactionData.GeneratePropertyKey();
    }

    protected override void OnInitialize() {
      this.Location = new Address();
      this.CadastralData = new CadastralInfo();
    }

    protected override void OnLoadObjectData(DataRow row) {
      this.Location = new Address();
      this.CadastralData = new CadastralInfo();
      //this.Location = Address.FromJson((string) row["LocationExtData"]);
      //this.CadastralData = CadastralInfo.FromJson((string) row["CadastralExtData"]);
    }

    protected override void OnSave() {
      PropertyData.WriteProperty(this);
    }
    internal Property Subdivide(PropertyPartition partitionInfo) {
      Assertion.Assert(!this.IsNew, "New properties can't be subdivided.");

      switch (partitionInfo.PartitionType) {
        case PropertyPartitionType.Partial:
          return this.CreatePartition(partitionInfo);
        case PropertyPartitionType.Last:
          var partition = this.CreatePartition(partitionInfo);
          this.MergedInto = partition;
          this.Save();
          return partition;
        case PropertyPartitionType.Full:
          var partitions = this.CreateAllPartitions(partitionInfo);
          this.MergedInto = partitions[partitions.Length - 1];
          this.Save();
          return partitions[partitionInfo.PartitionNo - 1];
        default:
          throw Assertion.AssertNoReachThisCode();
      }
    }

    public Property[] GetPartitions() {
      return PropertyData.GetPropertyPartitions(this);
    }

    private Property CreatePartition(PropertyPartition partitionInfo) {
      //Property[] currentPartitions = this.GetPartitions();
      //Assertion.Assert(currentPartitions.Length < lotNumber,
      //                 "Current property partitions are greater or equal than the requested fraction number.");
      //Assertion.Assert(this.MergedInto.IsEmptyInstance,
      //                 "Current property already has been merged into one or more properties.");

      var lot = new Property();
      lot.IsPartitionOf = this;
      if (partitionInfo.PartitionNo != 0) {
        lot.PartitionNo = partitionInfo.PartitionNo.ToString("00");
      } else {
        lot.PartitionNo = "sin número";
      }
      lot.LotSize = partitionInfo.Size.Amount;
      lot.LotSizeUnit = partitionInfo.Size.Unit;
      lot.ExtensionData.Add("Partition", partitionInfo.ToJson());
      lot.Save();

      return lot;
    }

    private Property[] CreateAllPartitions(PropertyPartition partitionInfo) {
      Property[] currentPartitions = this.GetPartitions();
      Assertion.Assert(currentPartitions.Length == 0, "Property already has partitions or lots.");

      Property[] partitions = new Property[partitionInfo.TotalPartitions];
      for (int i = 0; i < partitionInfo.TotalPartitions; i++) {
        Property lot = new Property();
        lot.IsPartitionOf = this;
        lot.PartitionNo = (i + 1).ToString("00");
        lot.Save();
        partitions[i] = lot;
      }
      return partitions;
    }

    #endregion Public methods

  } // class Property

} // namespace Empiria.Land.Registration
