/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : NotaryOffice                                   Pattern  : Storage Item                        *
*  Version   : 2.0        Date: 23/Oct/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Represents a notary office.                                                                   *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;

using Empiria.Contacts;
using Empiria.Geography;

namespace Empiria.Land.Registration {

  /// <summary>Represents a notary office.</summary>
  public class NotaryOffice : Organization {

    #region Fields

    private const string thisTypeName = "ObjectType.Contact.Organization.NotaryOffice";

    #endregion Fields

    #region Constructors and parsers

    public NotaryOffice() : base(thisTypeName) {
    }

    protected NotaryOffice(string typeName) : base(typeName) {
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

    static public FixedList<NotaryOffice> GetList(GeographicRegionItem place) {
      FixedList<NotaryOffice> list = place.GetContacts<NotaryOffice>("Region_NotaryOffices");

      list.Sort((x, y) => x.Number.CompareTo(y.Number));

      return list;
    }

    #endregion Constructors and parsers

    #region Public properties

    [DataField("NickName")]
    public string Number {
      get;
      private set;
    }

    #endregion Public properties

    #region Public methods

    public FixedList<Person> GetNotaries() {
      FixedList<Person> list = base.GetLinks<Person>("NotaryOffice_Notaries");

      list.Sort((x, y) => x.FamilyFullName.CompareTo(y.FamilyFullName));

      return list;
    }

    public FixedList<Person> GetNotaries(TimePeriod period) {
      FixedList<Person> list = base.GetLinks<Person>("NotaryOffice_Notaries", period);

      list.Sort((x, y) => x.FamilyFullName.CompareTo(y.FamilyFullName));

      return list;
    }

    #endregion Public methods

  } // class NotaryOffice

} // namespace Empiria.Land.Registration
