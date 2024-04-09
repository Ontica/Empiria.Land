/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RealEstate                                     Pattern  : Empiria Object Type                 *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Represents a real estate or land property.                                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Data;

using Empiria.DataTypes;
using Empiria.Geography;

using Empiria.Land.Data;
using Empiria.Land.Providers;

namespace Empiria.Land.Registration {

  /// <summary>Represents a real estate or land property.</summary>
  public class RealEstate : Resource {

    #region Constructors and parsers

    private RealEstate() {
      // Required by Empiria Framework.
    }

    public RealEstate(RealEstateExtData data) {
      Assertion.Require(data, "data");

      this.RealEstateExtData = data;
    }

    static public new RealEstate Parse(int id) {
      return BaseObject.ParseId<RealEstate>(id);
    }

    static public new RealEstate TryParseWithUID(string propertyUID) {
      var resource = Resource.TryParseWithUID(propertyUID);

      if (resource == null) {
        return null;
      }

      if (resource is RealEstate) {
        return (RealEstate) resource;
      } else {
        return null;
      }
    }


    static public FixedList<Unit> LotSizeUnits() {
      var lotSizeUnitsList = GeneralList.Parse("Land.RealEstate.LotSize.Units");

      return lotSizeUnitsList.GetItems<Unit>();
    }


    static public FixedList<string> RealEstateKinds() {
      var kindsList = GeneralList.Parse("Land.RealEstate.Kinds");

      return kindsList.GetItems<string>();
    }


    static public FixedList<string> RealEstatePartitionKinds() {
      var kindsList = GeneralList.Parse("Land.RealEstate.Partition.Kinds");

      return kindsList.GetItems<string>();
    }


    static public new RealEstate Empty {
      get {
        return BaseObject.ParseEmpty<RealEstate>();
      }
    }

    #endregion Constructors and parsers

    #region Public properties

    public string CadastralKey {
      get {
        return this.RealEstateExtData.CadastralKey;
      }
    }


    [DataField("CadastreLinkingDate", Default = "Empiria.ExecutionServer.DateMinValue")]
    public DateTime CadastreLinkingDate {
      get;
      internal set;
    } = Empiria.ExecutionServer.DateMinValue;


    internal RealEstateExtData RealEstateExtData {
      get;
      private set;
    } = RealEstateExtData.Empty;



    public string MetesAndBounds {
      get {
        return this.RealEstateExtData.MetesAndBounds;
      }
    }


    [DataField("MunicipalityId")]
    public Municipality Municipality {
      get;
      set;
    }


    public Quantity LotSize {
      get;
      set;
    } = Quantity.Zero;


    public decimal BuildingArea {
      get {
        return this.RealEstateExtData.BuildingArea;
      }
    }


    public decimal UndividedPct {
      get {
        return this.RealEstateExtData.UndividedPct;
      }
    }


    public string Section {
      get {
        return this.RealEstateExtData.Section;
      }
    }


    public string Block {
      get {
        return this.RealEstateExtData.Block;
      }
    }


    public string Lot {
      get {
        return this.RealEstateExtData.Lot;
      }
    }


    public string Notes {
      get {
        return this.RealEstateExtData.Notes;
      }
    }


    internal protected override string Keywords {
      get {
        return EmpiriaString.BuildKeywords(base.Keywords, this.CadastralKey, this.PartitionNo,
                                           this.Municipality.FullName, this.RealEstateExtData.MetesAndBounds);
      }
    }


    public bool IsPartition {
      get {
        return !this.IsPartitionOf.IsEmptyInstance;
      }
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

    public override void AssertCanBeClosed() {
      if (this.IsSpecialCase) {
        return;
      }
      Assertion.Require(this.Kind,
          $"Se requiere proporcionar la información del predio con folio electrónico {this.UID}.");
      Assertion.Require(!this.RecorderOffice.IsEmptyInstance,
                      "Predio " + this.UID +
                      ":\nSe requiere proporcionar el Distrito judicial al que pertenece el predio.");
      Assertion.Require(!this.Municipality.IsEmptyInstance,
                      "Predio " + this.UID +
                      ":\nSe requiere proporcionar el municipio donde se ubica el predio.");
      Assertion.Require(this.LotSize != Quantity.Zero,
                      "Predio " + this.UID  +
                      ":\nSe requiere proporcionar la superficie del predio.");
    }


    public bool HasHardLimitationActs {
      get {
        var tract = base.Tract.GetRecordingActs();

        var lastAct = tract.FindLast((x) => (x.Validator.WasAliveOn(DateTime.Now) &&
                                             x.RecordingActType.RecordingRule.IsHardLimitation &&
                                             x.LandRecord.IsClosed
                                             ));
        return lastAct != null;
      }
    }


    public bool IsFirstDomainAct(RecordingAct recordingAct) {
      var list = base.Tract.GetRecordingActs();

      var firstDomainAct = list.Find((x) => x.RecordingActType.IsDomainActType);

      if (firstDomainAct != null) {
        return firstDomainAct.Equals(recordingAct);
      } else {
        return false;
      }
    }


    public bool IsInTheRankOfTheFirstDomainAct(RecordingAct recordingAct) {
      FixedList<RecordingAct> recordingActs = this.Tract.GetRecordingActsUntil(recordingAct, true);

      int recordingActIndex = recordingActs.IndexOf(recordingAct);

      Assertion.Require(recordingActIndex != -1,
                       $"Supplied recordingAct {recordingAct.Id} doesn't belong to the property tract '{this.UID}'.");

      for (int i = 0; i < recordingActIndex; i++) {
        if (recordingActs[i].RecordingActType.IsDomainActType ||
            recordingActs[i].RecordingActType.IsStructureActType) {
          return false;
        }
      }
      return true;
    }


    protected override string GenerateResourceUID() {
      IUniqueIDGeneratorProvider provider = ExternalProviders.GetUniqueIDGeneratorProvider();

      return provider.GenerateRealEstateID();
    }

    public override ResourceShapshotData GetSnapshotData() {
      return new RealEstateShapshotData {
        Kind = this.Kind,
        Name = this.Name,
        Description = this.Description,
        Notes = this.Notes,
        MunicipalityId = this.Municipality.Id,
        CadastralKey = this.CadastralKey,
        CadastreLinkingDate = this.CadastreLinkingDate,
        LotSize = this.LotSize.Amount,
        LotSizeUnitId = this.LotSize.Unit.Id,
        PartitionNo = this.PartitionNo,
        MetesAndBounds = this.MetesAndBounds,
        Status = ((char) this.Status).ToString()
      };
    }


    public FixedList<RecordingAct> GetHardLimitationActs() {
      var tract = base.Tract.GetRecordingActs();

      tract = tract.FindAll((x) => (x.Validator.WasAliveOn(DateTime.Now) &&
                                    x.RecordingActType.RecordingRule.IsHardLimitation &&
                                    x.LandRecord.IsClosed
                            ));
      return tract;
    }


    public RecordingAct LastDomainAct {
      get {
        var tract = base.Tract.GetRecordingActs();

        var lastDomainAct = tract.FindLast((x) => x.Validator.WasAliveOn(DateTime.Now) &&
                                            x.RecordingActType.IsDomainActType &&
                                            (x.LandRecord.IsClosed || !x.BookEntry.IsEmptyInstance));
        return lastDomainAct;
      }
    }


    public string CurrentOwners {
      get {
        var parties = this.CurrentOwnersList;

        string temp = String.Empty;

        foreach (var party in parties) {
          if (temp.Length != 0) {
            temp += ", ";
          }
          temp += party.Party.FullName;
        }
        return temp;
      }
    }


    public FixedList<RecordingActParty> CurrentOwnersList {
      get {
        var parties = this.LastDomainAct.Parties.PrimaryParties;

        if (parties != null) {
          return parties;
        } else {
          return new FixedList<RecordingActParty>();
        }
      }
    }


    public RealEstate[] GetPartitions() {
      return ResourceData.GetRealEstatePartitions(this);
    }


    protected override void OnLoadObjectData(DataRow row) {
      this.RealEstateExtData = RealEstateExtData.Parse((string) row["PropertyExtData"]);
      this.LotSize = Quantity.Parse(Unit.Parse((int) row["LotSizeUnitId"]),
                                               (decimal) row["LotSize"]);
    }


    protected override void OnSave() {
      ResourceData.WriteRealEstate(this);
    }


    public void SetExtData(RealEstateExtData newData) {
      Assertion.Require(newData, "newData");

      newData.AssertIsValid();

      this.RealEstateExtData = newData;
    }


    public void SetPartitionNo(string newPartitionNo) {
      Assertion.Require(this.IsPartition,
                       "This real estate is not a partition of another property.");

      this.PartitionNo = EmpiriaString.TrimAll(newPartitionNo);

      Assertion.Require(this.PartitionNo, "newPartitionNo");
    }


    internal RealEstate[] Subdivide(RealEstatePartitionDTO partitionInfo) {
      Assertion.Require(!this.IsNew, "New properties can't be subdivided.");

      RealEstate[] partitions = new RealEstate[1];

      partitions[0] = this.CreatePartition(partitionInfo);


      // Code for lotification creation
      // string[] partitionNames = partitionInfo.GetPartitionNames();
      // RealEstate[] partitions = new RealEstate[partitionNames.Length];
      // for (int i = 0; i < partitionNames.Length; i++) {
      //  partitions[i] = this.CreatePartition(partitionNames[i]);
      //}

      return partitions;
    }


    #endregion Public methods

    #region Private methods

    private RealEstate CreatePartition(RealEstatePartitionDTO partitionInfo) {
      var lot = new RealEstate();
      lot.IsPartitionOf = this;
      lot.RecorderOffice = this.RecorderOffice;
      lot.Municipality = this.Municipality;
      lot.Kind = partitionInfo.PartitionType;
      lot.PartitionNo = partitionInfo.PartitionNumber;

      lot.Save();

      return lot;
    }


    #endregion Private methods

  } // class RealEstate

} // namespace Empiria.Land.Registration
