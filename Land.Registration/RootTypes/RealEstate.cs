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

    internal RealEstate(RealEstateExtData data) {
      Assertion.AssertObject(data, "data");

      this.RealEstateExtData = data;
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

    static public new RealEstate Empty {
      get { return BaseObject.ParseEmpty<RealEstate>(); }
    }

    #endregion Constructors and parsers

    #region Public properties

    public string CadastralKey {
      get {
        return this.RealEstateExtData.CadastralKey;
      }
    }

    internal RealEstateExtData RealEstateExtData {
      get;
      private set;
    } = RealEstateExtData.Empty;


    public string Name {
      get {
        return this.RealEstateExtData.Name;
      }
    }

    public RealEstateType RealEstateType {
      get {
        return this.RealEstateExtData.RealEstateType;
      }
    }

    public string MetesAndBounds {
      get {
        return this.RealEstateExtData.MetesAndBounds;
      }
    }

    public RecorderOffice District {
      get {
        return this.RealEstateExtData.District;
      }
    }

    public Municipality Municipality {
      get {
        return this.RealEstateExtData.Municipality;
      }
    }

    public string LocationReference {
      get {
        return this.RealEstateExtData.LocationReference;
      }
    }

    public Quantity LotSize {
      get {
        return this.RealEstateExtData.LotSize;
      }
    }

    public string Notes {
      get {
        return this.RealEstateExtData.Notes;
      }
    }

    internal protected override string Keywords {
      get {
        return EmpiriaString.BuildKeywords(base.Keywords, this.CadastralKey, this.Name, this.RealEstateType.Name);
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
      Assertion.Assert(!this.RealEstateType.IsEmptyInstance,
                       "Se requiere proporcionar la información del predio con folio real {0}.", this.UID);
      Assertion.Assert(!this.District.IsEmptyInstance,
                      "Predio " + this.UID +
                      ":\nSe requiere proporcionar el Distrito judicial al que pertenece el predio.");
      Assertion.Assert(!this.Municipality.IsEmptyInstance,
                      "Predio " + this.UID +
                      ":\nSe requiere proporcionar el municipio donde se ubica el predio.");
      Assertion.Assert(this.LotSize != Quantity.Zero,
                      "Predio " + this.UID  +
                      ":\nSe requiere proporcionar la superficie del predio.");
    }

    public string AsText() {
      if (this.RealEstateType.IsEmptyInstance) {
        return "Información disponible únicamente en documentos físicos.";
      }

      const string template = "predio denominado {0}, de tipo {1}";
      return String.Format(template, this.Name, this.RealEstateType.Name);
    }

    public bool HasHardLimitationActs {
      get {
        var tract = base.Tract.GetRecordingActs();

        var lastAct = tract.FindLast((x) => (x.WasAliveOn(DateTime.Now) &&
                                             x.RecordingActType.RecordingRule.IsHardLimitation &&
                                             x.Document.IsClosed
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

      Assertion.Assert(recordingActIndex != -1,
                       "Supplied recordingAct doesn't belong to the property tract.");

      for (int i = 0; i < recordingActIndex; i++) {
        if (recordingActs[i].RecordingActType.IsDomainActType ||
            recordingActs[i].RecordingActType.IsStructureActType) {
          return false;
        }
      }
      return true;
    }

    protected override string GenerateResourceUID() {
      return TransactionData.GeneratePropertyUID();
    }

    public FixedList<RecordingAct> GetHardLimitationActs() {
      var tract = base.Tract.GetRecordingActs();

      tract = tract.FindAll((x) => (x.WasAliveOn(DateTime.Now) &&
                                    x.RecordingActType.RecordingRule.IsHardLimitation &&
                                    x.Document.IsClosed
                            ));
      return tract;
    }

    public RecordingAct LastDomainAct {
      get {
        var tract = base.Tract.GetRecordingActs();

        var lastDomainAct = tract.FindLast((x) => x.WasAliveOn(DateTime.Now) &&
                                            x.RecordingActType.IsDomainActType &&
                                            x.Document.IsClosed);
        return lastDomainAct;
      }
    }

    public string CurrentOwners {
      get {
        var parties = this.LastDomainAct.GetParties();

        parties = parties.FindAll((x) => x.PartyOf.IsEmptyInstance);

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

    public RealEstate[] GetPartitions() {
      return ResourceData.GetRealEstatePartitions(this);
    }

    protected override void OnLoadObjectData(DataRow row) {
      this.RealEstateExtData = RealEstateExtData.Parse((string) row["PropertyExtData"]);
    }

    protected override void OnSave() {
      ResourceData.WriteRealEstate(this);
    }

    public void SetExtData(RealEstateExtData newData) {
      Assertion.AssertObject(newData, "newData");

      newData.AssertIsValid();

      this.RealEstateExtData = newData;
    }

    public void SetPartitionNo(string newPartitionNo) {
      Assertion.Assert(this.IsPartition,
                       "This real estate is not a partition of another property.");

      this.PartitionNo = EmpiriaString.TrimAll(newPartitionNo);

      Assertion.AssertObject(this.PartitionNo, "newPartitionNo");
    }

    internal RealEstate[] Subdivide(RealEstatePartitionDTO partitionInfo) {
      Assertion.Assert(!this.IsNew, "New properties can't be subdivided.");

      string[] partitionNames = partitionInfo.GetPartitionNames();
      RealEstate[] partitions = new RealEstate[partitionNames.Length];
      for (int i = 0; i < partitionNames.Length; i++) {
        partitions[i] = this.CreatePartition(partitionNames[i]);
      }
      return partitions;
    }

    #endregion Public methods

    #region Private methods

    private RealEstate CreatePartition(string partititionNo) {
      var lot = new RealEstate();
      lot.IsPartitionOf = this;
      lot.PartitionNo = partititionNo;
      lot.Save();

      return lot;
    }

    #endregion Private methods

  } // class RealEstate

} // namespace Empiria.Land.Registration
