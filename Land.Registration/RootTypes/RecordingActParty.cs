/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingActParty                              Pattern  : Association Class                   *
*  Version   : 1.5        Date: 25/Jun/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Represents a roled association between a recording act and a party.                           *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;

using Empiria.Contacts;
using Empiria.DataTypes;
using Empiria.Geography;

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

    #region Fields

    private const string thisTypeName = "ObjectType.RecordingActParty";


    private RecordingAct recordingAct = null;
    private Party party = null;
    private DomainActPartyRole partyRole = DomainActPartyRole.Empty;
    private Party secondaryParty = null;
    private PartiesRole secondaryPartyRole = PartiesRole.Empty;
    private string notes = String.Empty;
    private OwnershipMode ownershipMode = OwnershipMode.Undefined;
    private Quantity ownershipPart = Quantity.Parse(Unit.Empty, 0m);
    private string usufructTerm = String.Empty;
    private UsufructMode usufructMode = UsufructMode.Undefined;
    private Occupation partyOccupation = Occupation.Empty;
    private MarriageStatus partyMarriageStatus = MarriageStatus.Empty;
    private string partyAddress = String.Empty;
    private GeographicRegionItem partyAddressPlace = GeographicRegionItem.Empty;
    private Contact postedBy = Contact.Parse(ExecutionServer.CurrentUserId);
    private DateTime postingTime = DateTime.Now;
    private RecordableObjectStatus status = RecordableObjectStatus.Pending;
    private string integrityHashCode = String.Empty;

    #endregion Fields

    #region Constructors and parserssp

    private RecordingActParty()
      : base(thisTypeName) {

    }

    protected RecordingActParty(string typeName)
      : base(typeName) {
      // Required by Empiria Framework. Do not delete. Protected in not sealed classes, private otherwise
    }

    internal RecordingActParty(RecordingAct recordingAct, Party party)
      : base(thisTypeName) {
      this.recordingAct = recordingAct;
      this.party = party;
    }

    static public RecordingActParty Create(RecordingAct recordingAct, Party party) {
      return new RecordingActParty(recordingAct, party);
    }

    static public ObjectList<RecordingActParty> GetList(RecordingAct recordingAct) {
      return RecordingActsData.GetRecordingActPartiesList(recordingAct);
    }

    static public ObjectList<RecordingActParty> GetList(Recording recording, Party party) {
      return PropertyData.GetRecordingPartiesList(recording, party);
    }

    public static RecordingActParty GetDomainParty(RecordingAct recordingAct, Party party) {
      ObjectList<RecordingActParty> owners = GetInvolvedDomainParties(recordingAct);

      return owners.Find((x) => x.Party.Equals(party));
    }

    static public ObjectList<RecordingActParty> GetDomainPartyList(RecordingAct recordingAct) {
      return RecordingActsData.GetDomainPartyList(recordingAct);
    }

    static public ObjectList<RecordingActParty> GetInvolvedDomainParties(RecordingAct recordingAct) {
      return RecordingActsData.GetInvolvedDomainParties(recordingAct);
    }

    static public ObjectList<RecordingActParty> GetSecondaryPartiesList(RecordingAct recordingAct) {
      return RecordingActsData.GetSecondaryPartiesList(recordingAct);
    }

    public static RecordingActParty GetSecondaryParty(RecordingAct recordingAct, Party party) {
      ObjectList<RecordingActParty> secondaries = GetSecondaryPartiesList(recordingAct);

      return secondaries.Find((x) => x.Party.Equals(party));
    }

    static public RecordingActParty Parse(int id) {
      return BaseObject.Parse<RecordingActParty>(thisTypeName, id);
    }

    static internal RecordingActParty Parse(DataRow dataRow) {
      return BaseObject.Parse<RecordingActParty>(thisTypeName, dataRow);
    }

    #endregion Constructors and parsers

    #region Public properties

    public string DomainName {
      get {
        if (!this.RecordingAct.IsAnnotation) {
          return GetDomainName(this);
        } else {
          RecordingActParty domainParty = RecordingActParty.GetDomainParty(this.recordingAct, this.Party);
          if (domainParty != null) {
            return GetDomainName(domainParty);
          } else {
            return "Ninguno";
          } // if
        } // else
      } // get
    }

    static private string GetDomainName(RecordingActParty party) {
      switch (party.ownershipMode) {
        case Land.Registration.OwnershipMode.Bare:
          return "Nuda propiedad";
        case Land.Registration.OwnershipMode.Coowner:
          return "Copropietario";
        case Land.Registration.OwnershipMode.Owner:
          return "Propietario único";
      }
      switch (party.UsufructMode) {
        case Land.Registration.UsufructMode.LifeTime:
          return "Vitalicio";
        case Land.Registration.UsufructMode.Payment:
          return "Hasta recibir pago";
        case Land.Registration.UsufructMode.Time:
          return party.usufructTerm;
        case Land.Registration.UsufructMode.Condition:
          return party.usufructTerm;
        case Land.Registration.UsufructMode.Date:
          return "Hasta el " + Convert.ToDateTime(party.usufructTerm).ToString("dd/MMM/yyyy");
      }
      return "Ninguno";
    }

    public string DomainPartName {
      get {
        if (!this.RecordingAct.IsAnnotation) {
          return GetDomainPartName(this);
        } else {
          RecordingActParty domainParty = RecordingActParty.GetDomainParty(this.recordingAct, this.party);
          if (domainParty != null) {
            return GetDomainPartName(domainParty);
          } else {
            return String.Empty;
          } // if
        } // else
      } // get
    }

    static private string GetDomainPartName(RecordingActParty party) {
      if (party.OwnershipMode == Land.Registration.OwnershipMode.Owner) {
        return String.Empty;
      }
      if (party.OwnershipPart.Unit == Unit.FullUnit || party.OwnershipPart.Unit == Unit.UndividedUnit) {
        return party.OwnershipPart.Unit.Symbol;
      }
      if (party.OwnershipPart.Unit.IsEmptyInstance) {
        return String.Empty;
      }
      return party.ownershipPart.ToString();
    }

    public string IntegrityHashCode {
      get { return integrityHashCode; }
    }

    public string Notes {
      get { return notes; }
      set { notes = value; }
    }

    public OwnershipMode OwnershipMode {
      get { return ownershipMode; }
      set { ownershipMode = value; }
    }

    public Quantity OwnershipPart {
      get { return ownershipPart; }
      set { ownershipPart = value; }
    }

    public Party Party {
      get { return party; }
    }

    public string PartyAddress {
      get { return partyAddress; }
      set { partyAddress = value; }
    }

    public GeographicRegionItem PartyAddressPlace {
      get { return partyAddressPlace; }
      set { partyAddressPlace = value; }
    }

    public MarriageStatus PartyMarriageStatus {
      get { return partyMarriageStatus; }
      set { partyMarriageStatus = value; }
    }

    public Occupation PartyOccupation {
      get { return partyOccupation; }
      set { partyOccupation = value; }
    }

    public DomainActPartyRole PartyRole {
      get { return partyRole; }
      set { partyRole = value; }
    }

    public Contact PostedBy {
      get { return postedBy; }
    }

    public DateTime PostingTime {
      get { return postingTime; }
    }

    public RecordingAct RecordingAct {
      get { return recordingAct; }
    }

    public Party SecondaryParty {
      get { return secondaryParty; }
      set { secondaryParty = value; }
    }

    public PartiesRole SecondaryPartyRole {
      get { return secondaryPartyRole; }
      set { secondaryPartyRole = value; }
    }

    public RecordableObjectStatus Status {
      get { return status; }
      set { status = value; }
    }

    public string StatusName {
      get {
        switch (status) {
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

    public UsufructMode UsufructMode {
      get { return usufructMode; }
      set { usufructMode = value; }
    }

    public string UsufructTerm {
      get { return usufructTerm; }
      set { usufructTerm = value; }
    }

    #endregion Public properties

    #region Public methods

    public void Delete() {
      ObjectList<RecordingActParty> secondaries = GetSecondaryPartiesList(this.RecordingAct);
      for (int i = 0; i < secondaries.Count; i++) {
        secondaries[i].Status = RecordableObjectStatus.Deleted;
        secondaries[i].Save();
        if (secondaries[i].SecondaryParty.GetLastRecordingActParty(ExecutionServer.DateMinValue) == null) {
          secondaries[i].SecondaryParty.Delete();
        }
      }
      this.Status = RecordableObjectStatus.Deleted;
      base.Save();
      if (this.Party.GetLastRecordingActParty(ExecutionServer.DateMinValue) == null) {
        this.Party.Delete();
      }
    }

    protected override void ImplementsLoadObjectData(DataRow row) {
      this.recordingAct = RecordingAct.Parse((int) row["RecordingActId"]);
      this.party = Party.Parse((int) row["PartyId"]);
      this.partyRole = DomainActPartyRole.Parse((int) row["PartyRoleId"]);
      this.secondaryParty = Party.Parse((int) row["SecondaryPartyId"]);
      this.secondaryPartyRole = PartiesRole.Parse((int) row["SecondaryPartyRoleId"]);
      this.notes = (string) row["RecordingActPartyNotes"];
      this.ownershipMode = (OwnershipMode) Convert.ToChar(row["OwnershipMode"]);
      this.ownershipPart = Quantity.Parse(Unit.Parse((int) row["OwnershipPartUnitId"]), (decimal) row["OwnershipPartAmount"]);
      this.usufructMode = (UsufructMode) Convert.ToChar(row["UsufructMode"]);
      this.usufructTerm = (string) row["UsufructEndCondition"];
      this.partyOccupation = Occupation.Parse((int) row["PartyOccupationId"]);
      this.partyMarriageStatus = MarriageStatus.Parse((int) row["PartyMarriageStatusId"]);
      this.partyAddress = (string) row["PartyAddress"];
      this.partyAddressPlace = GeographicRegionItem.Parse((int) row["PartyAddressPlaceId"]);
      this.postedBy = Contact.Parse((int) row["PostedById"]);
      this.postingTime = (DateTime) row["PostingTime"];
      this.status = (RecordableObjectStatus) Convert.ToChar(row["LinkStatus"]);
      this.integrityHashCode = (string) row["LinkRIHC"];
    }

    protected override void ImplementsSave() {
      if (base.IsNew) {
        this.postingTime = DateTime.Now;
        this.postedBy = Contact.Parse(ExecutionServer.CurrentUserId);
      }
      PropertyData.WriteRecordingActParty(this);
    }

    #endregion Public methods

  } // class RecordingActParty

} // namespace Empiria.Land.Registration
