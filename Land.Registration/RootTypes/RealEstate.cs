/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RealEstate                                     Pattern  : Empiria Object Type                 *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Represents a real estate or land property.                                                    *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;
using System.Linq;

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

    public bool IsFirstDomainAct(RecordingAct recordingAct) {
      var list = this.GetRecordingActsTract();

      var firstDomainAct = list.Find((x) => x.RecordingActType.IsDomainActType);

      if (firstDomainAct != null) {
        return firstDomainAct.Equals(recordingAct);
      } else {
        return false;
      }
    }

    protected override string GenerateResourceUID() {
      return TransactionData.GeneratePropertyUID();
    }

    public RealEstate[] GetPartitions() {
      return ResourceData.GetRealEstatePartitions(this);
    }

    public bool IsInTheRankOfTheFirstDomainAct(RecordingAct recordingAct) {
      FixedList<RecordingAct> recordingActs = this.GetRecordingActsTractUntil(recordingAct, true);

      int recordingActIndex = recordingActs.IndexOf(recordingAct);

      if (recordingActIndex == -1) {
        return false;
      }

      for (int i = 0; i < recordingActIndex; i++) {
        if (recordingActs[i].RecordingActType.IsDomainActType ||
            recordingActs[i].RecordingActType.IsStructureActType) {
          return false;
        }
      }
      return true;
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

    internal RealEstate[] Subdivide(RealEstatePartition partitionInfo) {
      Assertion.Assert(!this.IsNew, "New properties can't be subdivided.");

      string[] partitionNumbers = partitionInfo.GetPartitions();
      RealEstate[] partitions = new RealEstate[partitionNumbers.Length];
      for (int i = 0; i < partitionNumbers.Length; i++) {
        partitions[i] = this.CreatePartition(partitionNumbers[i]);
      }
      return partitions;
    }

    #endregion Public methods

    #region Private methods

    private RealEstate CreatePartition(string partititionNo) {
      var lot = new RealEstate();
      lot.IsPartitionOf = this;
      lot.PartitionNo = partititionNo;    //partitionInfo.PartitionNo.ToString("00");
      lot.Save();

      return lot;
    }

    #endregion Private methods

  } // class RealEstate

} // namespace Empiria.Land.Registration
