﻿/* Empiria® Land 2013 ****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria® Land                                  System   : Land Registration System            *
*  Namespace : Empiria.Government.LandRegistration            Assembly : Empiria.Government.LandRegistration *
*  Type      : NotaryOffice                                   Pattern  : Storage Item                        *
*  Date      : 23/Oct/2013                                    Version  : 5.2     License: CC BY-NC-SA 3.0    *
*                                                                                                            *
*  Summary   : Represents a notary office.                                                                   *
*                                                                                                            *
**************************************************** Copyright © La Vía Óntica SC + Ontica LLC. 1999-2013. **/
using System;
using System.Data;

using Empiria.Contacts;
using Empiria.Geography;


namespace Empiria.Government.LandRegistration {

  /// <summary>Represents a notary office.</summary>
  public class NotaryOffice : Organization {

    #region Fields

    private const string thisTypeName = "ObjectType.Contact.Organization.NotaryOffice";

    private string number = String.Empty;

    #endregion Fields

    #region Constructors and parsers

    public NotaryOffice()
      : base(thisTypeName) {
    }

    protected NotaryOffice(string typeName)
      : base(typeName) {
      // Required by Empiria Framework. Do not delete. Protected in not sealed classes, private otherwise
    }

    static public new NotaryOffice Empty {
      get { return BaseObject.ParseEmpty<NotaryOffice>(thisTypeName); }
    }

    static public NotaryOffice Unknown {
      get { return BaseObject.ParseUnknown<NotaryOffice>(thisTypeName); }
    }

    static public new NotaryOffice Parse(int id) {
      return BaseObject.Parse<NotaryOffice>(thisTypeName, id);
    }

    static public ObjectList<NotaryOffice> GetList(GeographicRegionItem place) {
      ObjectList<NotaryOffice> list = place.GetContacts<NotaryOffice>("Region_NotaryOffices");

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

    public ObjectList<Person> GetNotaries() {
      ObjectList<Person> list = base.GetLinks<Person>("NotaryOffice_Notaries");

      list.Sort((x, y) => x.FamilyFullName.CompareTo(y.FamilyFullName));

      return list;
    }

    public ObjectList<Person> GetNotaries(TimePeriod period) {
      ObjectList<Person> list = base.GetLinks<Person>("NotaryOffice_Notaries", period);

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

  } // class NotaryOffice

} // namespace Empiria.Government.LandRegistration