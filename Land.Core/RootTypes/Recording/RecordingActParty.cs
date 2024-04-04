/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingActParty                              Pattern  : Association Class                   *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Represents a roled association between a recording act and a party.                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Data;

using Empiria.Contacts;
using Empiria.DataTypes;

using Empiria.Land.Data;
using Empiria.Land.Registration.Adapters;

namespace Empiria.Land.Registration {

  /// <summary>Represents a roled association between a recording act and a party.</summary>
  public class RecordingActParty : BaseObject {

    #region Constructors and parsers

    private RecordingActParty() {
      // Required by Empiria Framework.
    }

    internal RecordingActParty(RecordingAct recordingAct, Party party,
                               BasePartyRole role, Party partyOf) {
      this.RecordingAct = recordingAct;
      this.Party = party;
      this.PartyRole = role;
      this.PartyOf = partyOf;

      if (this.PartyRole is DomainActPartyRole) {
        this.IsOwnershipStillActive = true;
      }
    }


    static public RecordingActParty Parse(int id) => BaseObject.ParseId<RecordingActParty>(id);

    static public RecordingActParty Parse(string uid) => BaseObject.ParseKey<RecordingActParty>(uid);

    static public RecordingActParty Empty => BaseObject.ParseEmpty<RecordingActParty>();


    static internal RecordingActParty Create(RecordingAct recordingAct, Party party,
                                             DomainActPartyRole role) {
      Assertion.Require(recordingAct, "recordingAct");
      Assertion.Require(party, "party");
      Assertion.Require(role, "role");

      return new RecordingActParty(recordingAct, party, role, Party.Empty);
    }

    static internal RecordingActParty Create(RecordingAct recordingAct, Party party,
                                             SecondaryPartyRole role, Party partyOf) {
      Assertion.Require(recordingAct, "recordingAct");
      Assertion.Require(party, "party");
      Assertion.Require(role, "role");
      Assertion.Require(partyOf, "partyOf");

      return new RecordingActParty(recordingAct, party, role, partyOf);
    }

    #endregion Constructors and parsers

    #region Public properties


    [DataField("RecordingActId")]
    public RecordingAct RecordingAct {
      get;
      private set;
    }


    [DataField("PartyId")]
    public Party Party {
      get;
      private set;
    }


    [DataField("PartyRoleId")]
    public BasePartyRole PartyRole {
      get;
      private set;
    }


    public RecordingActPartyType RoleType {
      get {
        if (PartyOf.IsEmptyInstance) {
          return RecordingActPartyType.Primary;
        } else {
          return RecordingActPartyType.Secondary;
        }
      }
    }


    [DataField("PartyOfId")]
    public Party PartyOf {
      get;
      private set;
    }


    public Quantity OwnershipPart {
      get;
      set;
    } = Quantity.Zero;


    [DataField("IsOwnershipStillActive")]
    public bool IsOwnershipStillActive {
      get;
      private set;
    }


    [DataField("RecActPartyNotes")]
    public string Notes {
      get;
      set;
    }

    public string AsText {
      get;
    } = String.Empty;


    [DataField("RecActPartyExtData")]
    public string ExtendedData {
      get;
      private set;
    }


    [DataField("PostedById")]
    public Contact PostedBy {
      get;
      private set;
    }

    [DataField("RecActPartyStatus", Default = RecordableObjectStatus.Pending)]
    public RecordableObjectStatus Status {
      get;
      set;
    }


    public string IntegrityHashCode {
      get;
      private set;
    } = String.Empty;


    #endregion Public properties

    #region Public methods

    public void Delete() {
      this.Status = RecordableObjectStatus.Deleted;
      base.Save();
    }

    protected override void OnLoadObjectData(DataRow row) {
      this.OwnershipPart = Quantity.Parse(Unit.Parse((int) row["OwnershipPartUnitId"]),
                                                     (decimal) row["OwnershipPartAmount"]);
    }

    protected override void OnSave() {
      if (base.IsNew) {
        this.PostedBy = ExecutionServer.CurrentContact;
      }
      RecordingActsData.WriteRecordingActParty(this);
    }

    #endregion Public methods

  } // class RecordingActParty

} // namespace Empiria.Land.Registration
