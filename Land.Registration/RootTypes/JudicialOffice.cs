/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : JudicialOffice                                 Pattern  : Storage Item                        *
*  Version   : 2.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Represents a judicial office.                                                                 *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

using Empiria.Contacts;
using Empiria.DataTypes;
using Empiria.Geography;
using Empiria.Ontology;

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

    static public FixedList<JudicialOffice> GetList(GeographicRegion place) {
      var association = TypeAssociationInfo.Parse("JudicialOffice->Region");

      return association.GetInverseLinks<JudicialOffice>(place, (x, y) => x.Number.CompareTo(y.Number));
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

    public FixedList<Person> GetJudges(TimeFrame period) {
      FixedList<Person> list = base.GetLinks<Person>("JudicialOffice_Judges", period);

      list.Sort((x, y) => x.FamilyFullName.CompareTo(y.FamilyFullName));

      return list;
    }

    #endregion Public methods

  } // class JudicialOffice

} // namespace Empiria.Land.Registration
