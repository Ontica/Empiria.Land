﻿/* Empiria® Land 2013 ****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria® Land                                  System   : Land Registration System            *
*  Namespace : Empiria.Government.LandRegistration            Assembly : Empiria.Government.LandRegistration *
*  Type      : JudicialOffice                                 Pattern  : Storage Item                        *
*  Date      : 25/Jun/2013                                    Version  : 5.1     License: CC BY-NC-SA 3.0    *
*                                                                                                            *
*  Summary   : Represents a judicial office.                                                                 *
*                                                                                                            *
**************************************************** Copyright © La Vía Óntica SC + Ontica LLC. 1994-2013. **/
using System;
using System.Data;

using Empiria.Contacts;
using Empiria.Geography;


namespace Empiria.Government.LandRegistration {

  /// <summary>Represents a judicial office.</summary>
  public class JudicialOffice : Organization {

    #region Fields

    private const string thisTypeName = "ObjectType.Contact.Organization.JudicialOffice";

    private string number = String.Empty;

    #endregion Fields

    #region Constructors and parsers

    public JudicialOffice()
      : base(thisTypeName) {

    }

    protected JudicialOffice(string typeName)
      : base(typeName) {
      // Required by Empiria Framework. Do not delete. Protected in not sealed classes, private otherwise
    }

    static public new JudicialOffice Empty {
      get { return BaseObject.ParseEmpty<JudicialOffice>(thisTypeName); }
    }

    static public JudicialOffice Unknown {
      get { return BaseObject.ParseUnknown<JudicialOffice>(thisTypeName); }
    }

    static public new JudicialOffice Parse(int id) {
      return BaseObject.Parse<JudicialOffice>(thisTypeName, id);
    }

    static public ObjectList<JudicialOffice> GetList(GeographicRegionItem place) {
      ObjectList<JudicialOffice> list = place.GetContacts<JudicialOffice>("Region_JudicialOffices");

      list.Sort((x, y) => x.Number.CompareTo(y.Number));

      return list;
    }

    #endregion Constructors and parsers

    #region Public properties

    public string Number {
      get { return number; }
    }

    #endregion Public properties

    #region Public methods

    public ObjectList<Person> GetJudges() {
      ObjectList<Person> list = base.GetLinks<Person>("JudicialOffice_Judges");

      list.Sort((x, y) => x.FamilyFullName.CompareTo(y.FamilyFullName));

      return list;
    }

    public ObjectList<Person> GetJudges(TimePeriod period) {
      ObjectList<Person> list = base.GetLinks<Person>("JudicialOffice_Judges", period);

      list.Sort((x, y) => x.FamilyFullName.CompareTo(y.FamilyFullName));

      return list;
    }

    protected override void ImplementsLoadObjectData(DataRow row) {
      base.ImplementsLoadObjectData(row);
      this.number = (string) row["NickName"];
    }

    protected override void ImplementsSave() {
      base.ImplementsSave();
    }

    #endregion Public methods

  } // class JudicialOffice

} // namespace Empiria.Government.LandRegistration