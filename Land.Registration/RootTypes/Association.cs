/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : Association                                    Pattern  : Empiria Object Type                 *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Represents a social association or organization.                                              *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

using Empiria.Land.Registration.Data;

namespace Empiria.Land.Registration {

  /// <summary>Represents a social association or organization.</summary>
  public class Association : Resource {

    #region Constructors and parsers

    private Association() {
      // Required by Empiria Framework
    }

    internal Association(string name) {
      this.Name = name;
    }

    static public new Association Parse(int id) {
      return BaseObject.ParseId<Association>(id);
    }

    #endregion Constructors and parsers

    #region Public properties

    [DataField("PropertyName")]
    public string Name {
      get;
      private set;
    }

    internal protected override string Keywords {
      get {
        return EmpiriaString.BuildKeywords(base.Keywords, this.Name);
      }
    }

    #endregion Public properties

    #region Public methods

    protected override string GenerateResourceUID() {
      return TransactionData.GenerateAssociationUID();
    }

    public RecordingAct GetIncorporationAct() {
      FixedList<RecordingAct> tract = this.GetRecordingActsTractUntil(RecordingAct.Empty, false);

      if (tract.Count == 0) {         // Antecedent no registered
        return RecordingAct.Empty;
      }
      RecordingAct antecedent = tract.FindLast((x) => EmpiriaMath.IsMemberOf(x.Id, new int[] { 2750, 2709, 2710, 2711 }));    // Incorporation act
      if (antecedent != null) {
        return antecedent;
      } else if (tract[0].RecordingActType.Equals(RecordingActType.Empty)) {
        return tract[0];        // Incorporation act
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
