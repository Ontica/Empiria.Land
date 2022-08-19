/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : Association                                    Pattern  : Empiria Object Type                 *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Represents an association or organization.                                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Data;
using Empiria.Land.Providers;

namespace Empiria.Land.Registration {

  /// <summary>Represents an association or organization.</summary>
  public class Association : Resource {

    #region Constructors and parsers

    public Association() {
      // Required by Empiria Framework
    }


    static public new Association Parse(int id) {
      return BaseObject.ParseId<Association>(id);
    }

    static public FixedList<string> AssociationKinds() {
      var kindsList = GeneralList.Parse("Land.Associations.Kinds");

      return kindsList.GetItems<string>();
    }

    #endregion Constructors and parsers

    #region Public properties


    internal protected override string Keywords {
      get {
        return EmpiriaString.BuildKeywords(base.Keywords, this.Name, this.Description);
      }
    }

    #endregion Public properties

    #region Public methods

    public override void AssertCanBeClosed() {
      Assertion.Require(this.Name.Length != 0,
                       "Se requiere el nombre de la asociación o sociedad civil.");
    }

    protected override string GenerateResourceUID() {
      IUniqueIDGeneratorProvider provider = ExternalProviders.GetUniqueIDGeneratorProvider();

      return provider.GenerateAssociationID();
    }


    public override ResourceShapshotData GetSnapshotData() {
      return new AssociationShapshotData {
        Kind = this.Kind,
        Name = this.Name,
        Description = this.Description,
        Status = ((char) this.Status).ToString()
      };
    }


    public RecordingAct GetIncorporationAct() {
      FixedList<RecordingAct> tract = base.Tract.GetRecordingActs();

      if (tract.Count == 0) {
        return RecordingAct.Empty;
      }
      RecordingAct incorporationAct = tract.Find((x) => EmpiriaMath.IsMemberOf(x.RecordingActType.Id,
                                                 new int[] { 2750, 2709, 2710, 2711, 2712 }));    // Incorporation acts
      if (incorporationAct != null) {
        return incorporationAct;
      } else if (!tract[0].PhysicalRecording.IsEmptyInstance) {
        return tract[0];
      } else {
        return RecordingAct.Empty;
      }
    }

    protected override void OnSave() {
      ResourceData.WriteAssociation(this);
    }

    #endregion Public methods

  }  // class Association

} // namespace Empiria.Land.Registration
