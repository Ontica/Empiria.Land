/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingActParty                              Pattern  : Association Class                   *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Represents a roled association between a recording act and a party.                           *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;

using Empiria.Contacts;
using Empiria.DataTypes;

using Empiria.Land.Registration.Data;

namespace Empiria.Land.Registration {

  public enum OwnershipMode {
    None = 'N',
    Bare = 'B',
    Coowner = 'C',
    Owner = 'O',
    Undefined = 'U',
  }

  public enum UsufructMode {
    None = 'N',
    LifeTime = 'L',
    Time = 'T',
    Date = 'D',
    Payment = 'P',
    Condition = 'C',
    Undefined = 'U',
  }

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
    }

    static public RecordingActParty Parse(int id) {
      return BaseObject.ParseId<RecordingActParty>(id);
    }

    static public RecordingActParty Create(RecordingAct recordingAct, Party party,
                                           DomainActPartyRole role) {
      Assertion.AssertObject(recordingAct, "recordingAct");
      Assertion.AssertObject(party, "party");
      Assertion.AssertObject(role, "role");

      return new RecordingActParty(recordingAct, party, role, Party.Empty);
    }

    static public RecordingActParty Create(RecordingAct recordingAct, Party party,
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
      FixedList<RecordingActParty> secondaries = PartyData.GetSecondaryPartiesList(this.RecordingAct);
      for (int i = 0; i < secondaries.Count; i++) {
        secondaries[i].Status = RecordableObjectStatus.Deleted;
        secondaries[i].Save();
        if (secondaries[i].PartyOf.TryGetLastRecordingActParty(ExecutionServer.DateMinValue) == null) {
          secondaries[i].PartyOf.Delete();
        }
      }
      this.Status = RecordableObjectStatus.Deleted;
      base.Save();
      if (this.Party.TryGetLastRecordingActParty(ExecutionServer.DateMinValue) == null) {
        this.Party.Delete();
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

    public string ZUsufructTerm {
      get;
      set;
    } = String.Empty;

    public UsufructMode ZUsufructMode {
      get;
      set;
    } = UsufructMode.None;

    public OwnershipMode ZOwnershipMode {
      get;
      set;
    } = OwnershipMode.None;


  } // class RecordingActParty

} // namespace Empiria.Land.Registration
