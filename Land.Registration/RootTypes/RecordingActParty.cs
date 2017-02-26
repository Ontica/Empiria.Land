/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingActParty                              Pattern  : Association Class                   *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Represents a roled association between a recording act and a party.                           *
*                                                                                                            *
********************************* Copyright (c) 2009-2017. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;

using Empiria.Contacts;
using Empiria.DataTypes;

using Empiria.Land.Registration.Data;

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

    static public RecordingActParty Parse(int id) {
      return BaseObject.ParseId<RecordingActParty>(id);
    }

    static internal RecordingActParty Create(RecordingAct recordingAct, Party party,
                                             DomainActPartyRole role) {
      Assertion.AssertObject(recordingAct, "recordingAct");
      Assertion.AssertObject(party, "party");
      Assertion.AssertObject(role, "role");

      return new RecordingActParty(recordingAct, party, role, Party.Empty);
    }

    static internal RecordingActParty Create(RecordingAct recordingAct, Party party,
                                             SecondaryPartyRole role, Party partyOf) {
      Assertion.AssertObject(recordingAct, "recordingAct");
      Assertion.AssertObject(party, "party");
      Assertion.AssertObject(role, "role");
      Assertion.AssertObject(partyOf, "partyOf");

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

    public string StatusName {
      get {
        switch (this.Status) {
          case RecordableObjectStatus.Obsolete:
            return "No vigente";
          case RecordableObjectStatus.NoLegible:
            return "No legible";
          case RecordableObjectStatus.Incomplete:
            return "Incompleto";
          case RecordableObjectStatus.Pending:
            return "Pendiente";
          case RecordableObjectStatus.Registered:
            return "Registrado";
          case RecordableObjectStatus.Closed:
            return "Cerrado";
          case RecordableObjectStatus.Deleted:
            return "Eliminado";
          default:
            return "No determinado";
        }
      } // get
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

    public string GetOwnershipPartAsText() {
      if (this.OwnershipPart.Unit.IsEmptyInstance) {
        return String.Empty;
      }
      if (this.OwnershipPart.Unit == Unit.UndividedUnit) {
        return "Proindiviso";
      } else if (this.OwnershipPart.Unit == Unit.FullUnit) {
        return "Totalidad";
      } else if (this.OwnershipPart.Unit == Unit.Percentage) {
        return (this.OwnershipPart.Amount / 100).ToString("P2");
      } else if (this.OwnershipPart.Unit == Unit.SquareMeters) {
        return (this.OwnershipPart.Amount).ToString("N2") + " m2";
      } else if (this.OwnershipPart.Unit.Id == 624) {
        return (this.OwnershipPart.Amount).ToString("N2") + " ha";
      } else {
        throw Assertion.AssertNoReachThisCode("Invalid ownership part unit.");
      }
    }

    protected override void OnLoadObjectData(DataRow row) {
      this.OwnershipPart = Quantity.Parse(Unit.Parse((int) row["OwnershipPartUnitId"]),
                                                     (decimal) row["OwnershipPartAmount"]);
    }

    protected override void OnSave() {
      if (base.IsNew) {
        this.PostedBy = Contact.Parse(ExecutionServer.CurrentUserId);
      }
      RecordingActsData.WriteRecordingActParty(this);
    }

    #endregion Public methods

  } // class RecordingActParty

} // namespace Empiria.Land.Registration
