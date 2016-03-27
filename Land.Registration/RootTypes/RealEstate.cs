/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RealEstate                                     Pattern  : Empiria Object Type                 *
*  Version   : 2.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Represents a real estate or land property.                                                    *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;

using Empiria.DataTypes;
using Empiria.Geography;
using Empiria.Land.Registration.Data;

namespace Empiria.Land.Registration {

  /// <summary>Represents a real estate or land property.</summary>
  public class RealEstate : Resource {

    #region Constructors and parsers

    private RealEstate() {
      // Required by Empiria Framework.
    }

    internal RealEstate(string cadastralKey) {
      this.CadastralKey = cadastralKey;
    }

    static public new RealEstate Parse(int id) {
      return BaseObject.ParseId<RealEstate>(id);
    }

    static public new RealEstate TryParseWithUID(string propertyUID) {
      DataRow row = ResourceData.GetResourceWithUID(propertyUID);

      if (row != null) {
        return BaseObject.ParseDataRow<RealEstate>(row);
      } else {
        return null;
      }
    }

    static public RealEstate Empty {
      get { return BaseObject.ParseEmpty<RealEstate>(); }
    }

    #endregion Constructors and parsers

    #region Public properties

    [DataField("PropertyName")]
    public string Name {
      get;
      private set;
    }

    [DataField("PropertyKind")]
    private string _propertyKind = RealEstateKind.Empty;
    public RealEstateKind PropertyKind {
      get {
        return _propertyKind;
      }
      set {
        _propertyKind = value;
      }
    }

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
        return EmpiriaString.BuildKeywords(base.Keywords, this.CadastralKey, this.Name, this.PropertyKind.Value);
      }
    }

    [DataField("CadastralKey")]
    public string CadastralKey {
      get;
      private set;
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
    private LazyInstance<RealEstate> _isPartitionOf = LazyInstance<RealEstate>.Empty;
    public RealEstate IsPartitionOf {
      get { return _isPartitionOf.Value; }
      private set {
        _isPartitionOf = LazyInstance<RealEstate>.Parse(value);
      }
    }

    [DataField("PartitionNo")]
    public string PartitionNo {
      get;
      private set;
    }

    [DataField("MergedIntoId")]
    private LazyInstance<RealEstate> _mergedInto = LazyInstance<RealEstate>.Empty;
    public RealEstate MergedInto {
      get { return _mergedInto.Value; }
      private set {
        _mergedInto = LazyInstance<RealEstate>.Parse(value);
      }
    }

    #endregion Public properties

    #region Public methods

    protected override string GenerateResourceUID() {
      return TransactionData.GeneratePropertyUID();
    }

    public RecordingAct GetDomainAntecedent() {
      return this.GetDomainAntecedent(RecordingAct.Empty);
    }

    public RecordingAct GetDomainAntecedent(RecordingAct baseRecordingAct) {
      FixedList<RecordingAct> tract = this.GetRecordingActsTractUntil(baseRecordingAct, false);

      if (tract.Count == 0) {         // Antecedent no registered. OOJJOO Should throw an error?
        return RecordingAct.Empty;
      }
      // Get last domain act or undeterminated act
      RecordingAct antecedent = tract.FindLast((x) => x is DomainAct || x.RecordingActType.Id == 2200);
      if (antecedent != null) {
        return antecedent;
      } else if (tract[0].RecordingActType.Equals(RecordingActType.Empty)) {  // No registered act
        return tract[0];
      } else if (tract[0].RecordingActType.Id == 2371) {  // Denuncia de erección OOJJOO
        return tract[0];
      } else {
        return RecordingAct.Empty;
      }
    }

    public RealEstate[] GetPartitions() {
      return ResourceData.GetRealEstatePartitions(this);
    }

    /// <summary>Returns the temporary domain act if it exists, or RecordingAct.Empty otherwise.</summary>
    public RecordingAct GetProvisionalDomainAct() {
      //if (this.GetDomainAntecedent().Equals(RecordingAct.Empty)) {
      //  return RecordingAct.Empty;
      //}

      FixedList<RecordingAct> tract = this.GetRecordingActsTractUntil(RecordingAct.Empty, false);

      if (tract.Count == 0) {         // Antecedent no registered
        return RecordingAct.Empty;
      }
      // Avisos preventivos primero o segundo, o denuncia de erección
      var provisional = tract.FindLast((x) => x.RecordingActType.Id == 2284 ||
                                              x.RecordingActType.Id == 2257 ||
                                              x.RecordingActType.Id == 2371);
      if (provisional != null) {
        return provisional;
     // } else if (tract.Count == 2 && (tract[0].RecordingActType.Id == 2284 &&
     //                                 tract[1].RecordingActType.Id == 2257)) {
     //   return tract[0];
      } else {
        return RecordingAct.Empty;
      }
    }

    public bool IsProvisional() {
      return (this.GetDomainAntecedent() == RecordingAct.Empty &&
              this.GetProvisionalDomainAct() != RecordingAct.Empty);
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
      ResourceData.WriteRealEstate(this);
    }

    internal RealEstate Subdivide(RealEstatePartition partitionInfo) {
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
          return this.CreatePartition(partitionInfo);

          //var partitions = this.CreateAllPartitions(partitionInfo);
          //this.MergedInto = partitions[partitions.Length - 1];
          //this.Save();
          //return partitions[partitionInfo.PartitionNo - 1];
        default:
          throw Assertion.AssertNoReachThisCode();
      }
    }

    #endregion Public methods

    #region Private methods

    private RealEstate CreatePartition(RealEstatePartition partitionInfo) {
      //Property[] currentPartitions = this.GetPartitions();
      //Assertion.Assert(currentPartitions.Length < lotNumber,
      //                 "Current property partitions are greater or equal than the requested fraction number.");
      //Assertion.Assert(this.MergedInto.IsEmptyInstance,
      //                 "Current property already has been merged into one or more properties.");

      string prefix = GetPartitionSubtypeName(partitionInfo);

      var lot = new RealEstate(partitionInfo.CadastralKey);
      lot.IsPartitionOf = this;
      if (partitionInfo.PartitionNo != 0) {
        lot.PartitionNo = prefix + " " + partitionInfo.PartitionNo.ToString("00");
      } else {
        lot.PartitionNo = "sin número";
      }
      lot.LotSize = partitionInfo.Size.Amount;
      lot.LotSizeUnit = partitionInfo.Size.Unit;
      lot.ExtensionData.Add("Partition", partitionInfo.ToJson());
      lot.Save();

      return lot;
    }

    private string GetPartitionSubtypeName(RealEstatePartition partitionInfo) {
      if (partitionInfo.PartitionType != PropertyPartitionType.Full) {
        return String.Empty;
      }
      switch (partitionInfo.PartitionSubtype) {
        case PropertyPartitionSubtype.Apartment:
          return "Departamento";
        case PropertyPartitionSubtype.House:
          return "Casa";
        case PropertyPartitionSubtype.Lot:
          return "Lote";
        default:
          throw Assertion.AssertNoReachThisCode();
      }
    }

    private RealEstate[] CreateAllPartitions(RealEstatePartition partitionInfo) {
      RealEstate[] currentPartitions = this.GetPartitions();
      Assertion.Assert(currentPartitions.Length == 0, "Real estate already has partitions or lots.");

      RealEstate[] partitions = new RealEstate[partitionInfo.TotalPartitions];
      for (int i = 0; i < partitionInfo.TotalPartitions; i++) {
        RealEstate lot = new RealEstate(partitionInfo.CadastralKey);
        lot.IsPartitionOf = this;
        lot.PartitionNo = (i + 1).ToString("00");
        lot.Save();
        partitions[i] = lot;
      }
      return partitions;
    }

    #endregion Private methods

  } // class RealEstate

} // namespace Empiria.Land.Registration
