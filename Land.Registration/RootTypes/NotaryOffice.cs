/* Empiria Land 2015 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : NotaryOffice                                   Pattern  : Storage Item                        *
*  Version   : 2.0        Date: 04/Jan/2015                   License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Represents a notary office.                                                                   *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;

using Empiria.Contacts;
using Empiria.Geography;
using Empiria.Ontology;

namespace Empiria.Land.Registration {

  /// <summary>Represents a notary office.</summary>
  public class NotaryOffice : Organization {

    #region Constructors and parsers

    private NotaryOffice() {
      // Required by Empiria Framework.
    }

    static public new NotaryOffice Empty {
      get { return BaseObject.ParseEmpty<NotaryOffice>(); }
    }

    static public new NotaryOffice Unknown {
      get { return BaseObject.ParseUnknown<NotaryOffice>(); }
    }

    static public new NotaryOffice Parse(int id) {
      return BaseObject.ParseId<NotaryOffice>(id);
    }

    static public FixedList<NotaryOffice> GetList(GeographicRegion place) {
      var association = TypeAssociationInfo.Parse("NotaryOffice->Region");

      return association.GetInverseLinks<NotaryOffice>(place, (x, y) => x.Number.CompareTo(y.Number));
    }

    #endregion Constructors and parsers

    #region Public properties

    [DataField("NickName")]
    public string Number {
      get;
      private set;
    }

    public GeographicRegion Region {
      get {
        return base.GetLink<GeographicRegion>("NotaryOffice->Region");
      }
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
