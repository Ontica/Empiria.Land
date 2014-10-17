/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : JudicialOffice                                 Pattern  : Storage Item                        *
*  Version   : 2.0        Date: 23/Oct/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Represents a judicial office.                                                                 *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

using Empiria.Contacts;
using Empiria.Geography;

namespace Empiria.Land.Registration {

  /// <summary>Represents a judicial office.</summary>
  public class JudicialOffice : Organization {

    #region Constructors and parsers

    private JudicialOffice() {
      // Required by Empiria Framework.
    }

    static public new JudicialOffice Empty {
      get { return BaseObject.ParseEmpty<JudicialOffice>(); }
    }

    static public new JudicialOffice Unknown {
      get { return BaseObject.ParseUnknown<JudicialOffice>(); }
    }

    static public new JudicialOffice Parse(int id) {
      return BaseObject.ParseId<JudicialOffice>(id);
    }

    static private FixedList<JudicialOffice> GetJudicialOfficesInPlace(GeographicRegion place) {
      throw new NotImplementedException();

      //FixedList<JudicialOffice> list = place.GetContacts<JudicialOffice>("Region_JudicialOffices");

      //list.Sort((x, y) => x.Number.CompareTo(y.Number));

      //return list;
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

    public FixedList<Person> GetJudges() {
      FixedList<Person> list = base.GetLinks<Person>("JudicialOffice_Judges");

      list.Sort((x, y) => x.FamilyFullName.CompareTo(y.FamilyFullName));

      return list;
    }

    public FixedList<Person> GetJudges(TimePeriod period) {
      FixedList<Person> list = base.GetLinks<Person>("JudicialOffice_Judges", period);

      list.Sort((x, y) => x.FamilyFullName.CompareTo(y.FamilyFullName));

      return list;
    }

    #endregion Public methods

  } // class JudicialOffice

} // namespace Empiria.Land.Registration
