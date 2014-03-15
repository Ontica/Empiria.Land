/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land                                   Assembly : Empiria.Land                        *
*  Type      : Party                                          Pattern  : Empiria Object Type                 *
*  Version   : 1.5        Date: 28/Mar/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Abstract class that represents a recording act party.                                         *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;

using Empiria.Contacts;
using Empiria.Geography;
using Empiria.Land.Registration.Data;
using Empiria.Ontology;

namespace Empiria.Land.Registration {

  public enum PartyStatus {
    Pending = 'P',
    Registered = 'R',
    Closed = 'C',
    Deleted = 'X'
  }

  public enum PartyFilterType {
    ByKeywords = 1,
    OnInscription = 2,
    OnRecordingBook = 3,
    Involved = 4,
  }

  /// <summary>Abstract class that represents a recording act party.</summary>
  public abstract class Party : BaseObject {

    #region Abstract members

    protected abstract string ImplementsRegistryID();

    #endregion Abstract members

    #region Fields

    private const string thisTypeName = "ObjectType.Party";

    private string fullName = String.Empty;
    private string shortName = String.Empty;
    private string nicknames = String.Empty;
    private string tags = String.Empty;
    private GeographicRegionItem registryLocation = GeographicRegionItem.Empty;
    private DateTime registryDate = ExecutionServer.DateMaxValue;
    private string taxIDNumber = String.Empty;
    private string notes = String.Empty;
    private string keywords = String.Empty;
    private Contact postedBy = Contact.Parse(ExecutionServer.CurrentUserId);
    private DateTime postingTime = DateTime.Now;
    private int replacedById = 0;
    private PartyStatus status = PartyStatus.Pending;
    private string integrityHashCode = String.Empty;

    #endregion Fields

    #region Constructors and parsers

    protected Party(string typeName)
      : base(typeName) {
      // Empiria Object Type pattern classes always has this constructor. Don't delete
    }

    static public Party Parse(int id) {
      return BaseObject.Parse<Party>(thisTypeName, id);
    }

    static internal Party Parse(DataRow dataRow) {
      return BaseObject.Parse<Party>(thisTypeName, dataRow);
    }

    static public ObjectList<Party> GetList(ObjectTypeInfo partyType, string keywords) {
      DataTable table = PropertyData.GetParties(partyType, keywords);

      return new ObjectList<Party>((x) => Party.Parse(x), table);
    }

    static public ObjectList<Party> GetList(PartyFilterType partyFilterType, ObjectTypeInfo partyType,
                                            RecordingAct recordingAct, string keywords) {
      DataTable table = PropertyData.GetParties(partyFilterType, partyType, recordingAct, keywords);

      return new ObjectList<Party>((x) => Party.Parse(x), table);
    }

    #endregion Constructors and parsers

    #region Public properties

    public string ExtendedName {
      get {
        string temp = this.FullName;

        if (this.RegistryID.Length != 0) {
          temp += " (" + this.RegistryID + ")";
        }
        if (this.RegistryLocation.IsEmptyInstance || this.RegistryLocation.Equals(GeographicRegionItem.Unknown)) {
          return temp;
        } else {
          return temp + " " + this.RegistryLocation.FullName;
        }
      }
    }

    public string FullName {
      get { return fullName; }
      set { fullName = EmpiriaString.TrimAll(value); }
    }

    public string IntegrityHashCode {
      get { return integrityHashCode; }
    }

    public string Keywords {
      get { return keywords; }
      protected set { keywords = value; }
    }

    public string Nicknames {
      get { return nicknames; }
      set { nicknames = EmpiriaString.TrimAll(value); }
    }

    public string Notes {
      get { return notes; }
      set { notes = EmpiriaString.TrimAll(value); }
    }

    public Contact PostedBy {
      get { return postedBy; }
    }

    public DateTime PostingTime {
      get { return postingTime; }
    }

    public DateTime RegistryDate {
      get { return registryDate; }
      set { registryDate = value; }
    }

    public string RegistryID {
      get { return ImplementsRegistryID(); }
    }

    public GeographicRegionItem RegistryLocation {
      get { return registryLocation; }
      set { registryLocation = value; }
    }

    public int ReplacedById {
      get { return replacedById; }
    }

    public string ShortName {
      get { return shortName; }
      set { shortName = EmpiriaString.TrimAll(value); }
    }

    public PartyStatus Status {
      get { return status; }
    }

    public string StatusName {
      get {
        switch (status) {
          case PartyStatus.Pending:
            return "Pendiente";
          case PartyStatus.Registered:
            return "Registrada";
          case PartyStatus.Closed:
            return "Cerrada";
          case PartyStatus.Deleted:
            return "Eliminada";
          default:
            return "No determinado";
        }
      }
    }

    public string Tags {
      get { return tags; }
      set { tags = EmpiriaString.TrimAll(value); }
    }

    public string TaxIDNumber {
      get { return taxIDNumber; }
      set { taxIDNumber = value; }
    }

    #endregion Public properties

    #region Public methods

    internal void Delete() {
      this.status = PartyStatus.Deleted;
      this.Save();
    }

    public RecordingActParty GetLastRecordingActParty(DateTime searchStartDate) {
      return PropertyData.GetLastRecordingActParty(this, searchStartDate);
    }

    protected override void ImplementsLoadObjectData(DataRow row) {
      this.fullName = (string) row["PartyFullName"];
      this.shortName = (string) row["PartyShortName"];
      this.nicknames = (string) row["Nicknames"];
      this.tags = (string) row["PartyTags"];
      this.registryDate = (DateTime) row["RegistryDate"];
      this.registryLocation = GeographicRegionItem.Parse((int) row["RegistryLocationId"]);
      this.taxIDNumber = (string) row["TaxIDNumber"];
      this.notes = (string) row["PartyNotes"];
      this.keywords = (string) row["PartyKeywords"];
      this.postedBy = Contact.Parse((int) row["PostedById"]);
      this.postingTime = (DateTime) row["PostingTime"];
      this.replacedById = (int) row["ReplacedById"];
      this.status = (PartyStatus) Convert.ToChar(row["PartyStatus"]);
      this.integrityHashCode = (string) row["PartyRIHC"];
    }

    protected override void ImplementsSave() {
      if (base.IsNew) {
        this.postedBy = Contact.Parse(ExecutionServer.CurrentUserId);
        this.postingTime = DateTime.Now;
      }
    }

    #endregion Public methods

  } // class Party

} // namespace Empiria.Land.Registration
