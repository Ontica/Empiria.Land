/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : StructureTractItem                             Pattern  : Association Class                   *
*  Version   : 2.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Application of a recording act that impacts the structure or shape of a real estate.          *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

using Empiria.Land.Registration.Data;

namespace Empiria.Land.Registration {

  /// <summary>Application of a recording act that impacts the structure or shape of a real estate.</summary>
  public class StructureTractItem : TractItem {

    #region Constructors and parsers

    protected StructureTractItem() {
      // Required by Empiria Framework.
    }

    internal StructureTractItem(RecordingAct recordingAct, RealEstate realEstate,
                                ResourceRole realEstateRole,
                                RealEstate relatedRealState = null,
                                string partitionName = "",
                                decimal recordingActPercentage = decimal.One) :
                                  base(recordingAct, realEstate, realEstateRole, recordingActPercentage) {
      Assertion.Assert(realEstateRole != ResourceRole.Edited &&
                       realEstateRole != ResourceRole.Informative,
                       "realEstateRole must be a structure role (distinct to 'Edited' or 'Informative').");

      Assertion.AssertObject(partitionName, "realEstatePartitionName");

      if (relatedRealState == null) {
        relatedRealState = RealEstate.Empty;
      }
      this.RelatedRealEstate = relatedRealState;
      this.PartitionName = partitionName;

    }

    #endregion Constructors and parsers

    #region Public properties

    [DataField("RelatedResourceId")]
    public RealEstate RelatedRealEstate {
      get;
      private set;
    }

    [DataField("ResourcePartitionName")]
    public string PartitionName {
      get;
      private set;
    }

    #endregion Public properties

    #region Public methods

    internal override void Delete() {
      base.Delete();
      this.RelatedRealEstate.TryDelete();
    }

    protected override void OnSave() {
      if (this.Resource.IsNew) {
        this.Resource.Save();
      }
      RecordingActsData.WriteStructureTractItem(this);
    }

    #endregion Public methods

  } // class StructureTractItem

} // namespace Empiria.Land.Registration
