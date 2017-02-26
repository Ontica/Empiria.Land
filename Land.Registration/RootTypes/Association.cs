/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : Association                                    Pattern  : Empiria Object Type                 *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Represents a social association or organization.                                              *
*                                                                                                            *
********************************* Copyright (c) 2009-2017. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;

using Empiria.Land.Registration.Data;

namespace Empiria.Land.Registration {

  /// <summary>Represents a social association or organization.</summary>
  public class Association : Resource {

    #region Constructors and parsers

    private Association() {
      // Required by Empiria Framework
    }

    internal Association(AssociationExtData data) {
      Assertion.AssertObject(data, "data");

      this.AssociationExtData = data;
    }

    static public new Association Parse(int id) {
      return BaseObject.ParseId<Association>(id);
    }

    #endregion Constructors and parsers

    #region Public properties

    internal AssociationExtData AssociationExtData {
      get;
      private set;
    } = AssociationExtData.Empty;


    public string Name {
      get {
        return this.AssociationExtData.Name;
      }
    }

    internal protected override string Keywords {
      get {
        return EmpiriaString.BuildKeywords(base.Keywords, this.Name);
      }
    }

    #endregion Public properties

    #region Public methods

    public override void AssertCanBeClosed() {
      Assertion.Assert(this.Name.Length != 0,
                       "Se requiere el nombre de la asociación o sociedad civil.");
    }

    protected override string GenerateResourceUID() {
      return TransactionData.GenerateAssociationUID();
    }

    public string GetAssociationTypeName() {
      RecordingAct incorporationAct = this.GetIncorporationAct();

      if (!incorporationAct.IsEmptyInstance &&
          incorporationAct.RecordingActType.RecordingRule.ResourceTypeName.Length != 0) {
        return incorporationAct.RecordingActType.RecordingRule.ResourceTypeName;
      }

      if (this.Name.EndsWith("S.C.") || this.Name.EndsWith("SC") || this.Name.Contains("sociedad civil")) {
        return "Sociedad Civil";
      } else if (this.Name.EndsWith("A.C.") || this.Name.EndsWith("AC") ||
                 this.Name.Contains("asociacion civil") || this.Name.Contains("asociación civil")) {
        return "Asociación Civil";
      } else if (this.Name.EndsWith("A.R.") || this.Name.EndsWith("AR") ||
                 this.Name.Contains("asociacion religiosa") || this.Name.Contains("asociación religiosa")) {
        return "Asociación Religiosa";
      } else if (this.Name.EndsWith("S.S.") || this.Name.EndsWith("S:S.") ||
                 this.Name.Contains("solidaridad social"))  {
        return "Sociedad de Solidaridad Social";
      } else {
        return "Sociedad";
      }
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
      }
      if (incorporationAct != null) {
          return incorporationAct;
        } else {
        return RecordingAct.Empty;
      }
    }

    protected override void OnLoadObjectData(DataRow row) {
      this.AssociationExtData = AssociationExtData.ParseJson((string) row["PropertyExtData"]);
    }

    protected override void OnSave() {
      ResourceData.WriteAssociation(this);
    }

    #endregion Public methods

  }  // class Association

} // namespace Empiria.Land.Registration
