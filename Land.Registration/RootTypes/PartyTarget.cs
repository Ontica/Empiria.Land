/* Empiria Land 2015 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : PartyTarget                                    Pattern  : Association Class                   *
*  Version   : 2.0        Date: 04/Jan/2015                   License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Application of a recording act with a party (person or organization).                         *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;

using Empiria.Contacts;
using Empiria.DataTypes;
using Empiria.Security;

using Empiria.Land.Registration.Data;

namespace Empiria.Land.Registration {

  /// <summary>Application of a recording act with a party (person or organization).</summary>
  public class PartyTarget : RecordingActTarget {

    #region Constructors and parsers

    protected PartyTarget() {
      // Required by Empiria Framework.
    }

    internal PartyTarget(RecordingAct recordingAct, Party party,
                         Resource resource) : base(recordingAct) {
      Assertion.AssertObject(party, "party");
      Assertion.AssertObject(resource, "resource");

      this.Party = party;
      //this.Resource = resource;
    }

    static public new ResourceTarget Parse(int id) {
      return BaseObject.ParseId<ResourceTarget>(id);
    }

    #endregion Constructors and parsers

    #region Public properties

    [DataField("TargetPartyId", Default = "Empiria.Land.Registration.HumanParty.Empty")]
    public Party Party {
      get;
      private set;
    }

    public ResourceRole ResourceRole {
      get {
        return ResourceRole.Informative;
      }
    }

    #endregion Public properties

    #region Public methods

    protected override void OnSave() {
      RecordingActsData.WritePartyTarget(this);
    }

    #endregion Public methods

  } // class PartyTarget

} // namespace Empiria.Land.Registration
