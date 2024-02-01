/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecorderOffice                                 Pattern  : Storage Item                        *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : A recorder of deeds office.                                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Contacts;
using Empiria.Geography;

using Empiria.Land.Data;

namespace Empiria.Land.Registration {

  /// <summary>A recorder of deeds office.</summary>
  public class RecorderOffice : Organization {

    #region Constructors and parsers

    private RecorderOffice() {
      // Required by Empiria Framework.
    }

    static public new RecorderOffice Empty {
      get { return RecorderOffice.ParseEmpty<RecorderOffice>(); }
    }

    static public new RecorderOffice Parse(int id) {
      return BaseObject.ParseId<RecorderOffice>(id);
    }

    static public new RecorderOffice Parse(string uid) {
      return BaseObject.ParseKey<RecorderOffice>(uid);
    }

    static public FixedList<RecorderOffice> GetList() {
      return BaseObject.GetList<RecorderOffice>("ContactStatus = 'A'")
                       .ToFixedList();
    }

    #endregion Constructors and parsers

    #region Public methods

    public FixedList<Municipality> GetMunicipalities() {
      return base.ExtendedData.GetList<Municipality>("municipalities", false)
                              .ToFixedList();
    }


    public FixedList<RecordingSection> GetRecordingSections() {
      return RecordingSection.GetList(this);
    }


    public Person GetSigner() {
      return ExtendedData.Get("recorderOfficerSigner", Person.Empty);
    }


    public string GetPlace() {
      return ExtendedData.Get("recorderOfficePlace", "Lugar no determinado");
    }

    public FixedList<RecordingBook> GetRecordingBooks(RecordingSection sectionType) {
      return RecordingBooksData.GetRecordingBooksInSection(this, sectionType);
    }


    #endregion Public methods

  } // class RecorderOffice

} // namespace Empiria.Land.Registration
