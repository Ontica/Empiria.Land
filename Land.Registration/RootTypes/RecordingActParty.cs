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

    #region Constructors and parsers

    private RecordingActParty() {
      // Required by Empiria Framework.
    }

    internal RecordingActParty(RecordingAct recordingAct, Party party) {
      this.RecordingAct = recordingAct;
      this.Party = party;
    }

    static public RecordingActParty Parse(int id) {
      return BaseObject.ParseId<RecordingActParty>(id);
    }

    static public RecordingActParty Create(RecordingAct recordingAct, Party party) {
      return new RecordingActParty(recordingAct, party);
    }

    static public FixedList<RecordingActParty> GetList(RecordingAct recordingAct) {
      return RecordingActsData.GetRecordingActPartiesList(recordingAct);
    }

    static public FixedList<RecordingActParty> GetList(Recording recording, Party party) {
      return ResourceData.GetRecordingPartiesList(recording, party);
    }

    static public RecordingActParty GetDomainParty(RecordingAct recordingAct, Party party) {
      FixedList<RecordingActParty> owners = GetInvolvedDomainParties(recordingAct);

      return owners.Find((x) => x.Party.Equals(party));
    }

    static public FixedList<RecordingActParty> GetDomainPartyList(RecordingAct recordingAct) {
      return RecordingActsData.GetDomainPartyList(recordingAct);
    }

    static public FixedList<RecordingActParty> GetInvolvedDomainParties(RecordingAct recordingAct) {
      throw new NotImplementedException("OOJJOO");

      //return RecordingActsData.GetInvolvedDomainParties(recordingAct);
    }

    static public FixedList<RecordingActParty> GetSecondaryPartiesList(RecordingAct recordingAct) {
      return RecordingActsData.GetSecondaryPartiesList(recordingAct);
    }

    static public RecordingActParty GetSecondaryParty(RecordingAct recordingAct, Party party) {
      FixedList<RecordingActParty> secondaries = GetSecondaryPartiesList(recordingAct);

      return secondaries.Find((x) => x.Party.Equals(party));
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
    public DomainActPartyRole PartyRole {
      get;
      set;
    }

    [DataField("SecondaryPartyId")]
    public Party SecondaryParty {
      get;
      set;
    }

    [DataField("SecondaryPartyRoleId")]
    public PartiesRole SecondaryPartyRole {
      get;
      set;
    }

    [DataField("RecordingActPartyNotes")]
    public string Notes {
      get;
      set;
    }

    [DataField("OwnershipMode", Default = OwnershipMode.Undefined)]
    public OwnershipMode OwnershipMode {
      get;
      set;
    }

    public Quantity OwnershipPart {
      get;
      set;
    }

    [DataField("UsufructTerm")]
    public string UsufructTerm {
      get;
      set;
    }

    [DataField("UsufructMode", Default = UsufructMode.Undefined)]
    public UsufructMode UsufructMode {
      get;
      set;
    }

    [DataField("PostedById")]
    public Contact PostedBy {
      get;
      private set;
    }

    [DataField("PostingTime", Default = "DateTime.Now")]
    public DateTime PostingTime {
      get;
      private set;
    }

    [DataField("LinkStatus", Default = RecordableObjectStatus.Pending)]
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
    }

    public string DomainName {
      get {
        if (!this.RecordingAct.IsAnnotation) {
          return RecordingActParty.GetDomainName(this);
        } else {
          var domainParty = RecordingActParty.GetDomainParty(this.RecordingAct, this.Party);
          if (domainParty != null) {
            return RecordingActParty.GetDomainName(domainParty);
          } else {
            return "Ninguno";
          } // if
        } // else
      } // get
    }

    public string DomainPartName {
      get {
        if (!this.RecordingAct.IsAnnotation) {
          return RecordingActParty.GetDomainPartName(this);
        } else {
          var domainParty = RecordingActParty.GetDomainParty(this.RecordingAct, this.Party);
          if (domainParty != null) {
            return RecordingActParty.GetDomainPartName(domainParty);
          } else {
            return String.Empty;
          } // if
        } // else
      } // get
    }

    [DataField("PartyOccupationId")]
    public Occupation PartyOccupation {
      get;
      set;
    }

    [DataField("PartyMarriageStatusId")]
    public MarriageStatus PartyMarriageStatus {
      get;
      set;
    }

    [DataField("PartyAddress")]
    public string PartyAddress {
      get;
      set;
    }

    [DataField("PartyAddressPlaceId")]
    public GeographicRegion PartyAddressPlace {
      get;
      set;
    }

    #endregion Public properties

    #region Public methods

    public void Delete() {
      FixedList<RecordingActParty> secondaries = GetSecondaryPartiesList(this.RecordingAct);
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

    protected override void OnLoadObjectData(DataRow row) {
      this.OwnershipPart = Quantity.Parse(Unit.Parse((int) row["OwnershipPartUnitId"]),
                                                     (decimal) row["OwnershipPartAmount"]);
    }

    protected override void OnSave() {
      if (base.IsNew) {
        this.PostingTime = DateTime.Now;
        this.PostedBy = Contact.Parse(ExecutionServer.CurrentUserId);
      }
      RecordingActsData.WriteRecordingActParty(this);
    }

    #endregion Public methods

    #region Private methods

    static private string GetDomainName(RecordingActParty party) {
      switch (party.OwnershipMode) {
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
          return party.UsufructTerm;
        case Land.Registration.UsufructMode.Condition:
          return party.UsufructTerm;
        case Land.Registration.UsufructMode.Date:
          return "Hasta el " + Convert.ToDateTime(party.UsufructTerm).ToString("dd/MMM/yyyy");
      }
      return "Ninguno";
    }

    static private string GetDomainPartName(RecordingActParty party) {
      if (party.OwnershipMode == Land.Registration.OwnershipMode.Owner) {
        return String.Empty;
      }
      if (party.OwnershipPart.Unit == Unit.FullUnit ||
          party.OwnershipPart.Unit == Unit.UndividedUnit) {
        return party.OwnershipPart.Unit.Symbol;
      }
      if (party.OwnershipPart.Unit.IsEmptyInstance) {
        return String.Empty;
      }
      return party.OwnershipPart.ToString();
    }

    #endregion Private methods

  } // class RecordingActParty

} // namespace Empiria.Land.Registration
