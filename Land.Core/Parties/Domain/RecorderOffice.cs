﻿/* Empiria Land **********************************************************************************************
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
      return BaseObject.GetList<RecorderOffice>("ContactStatus = 'A'", "ContactId")
                       .ToFixedList();
    }

    #endregion Constructors and parsers

    #region Properties

    public string PermissionTag {
      get {
        return ExtendedData.Get("permissionTag", string.Empty);
      }
    }


    public string Place {
      get {
        return ExtendedData.Get("recorderOfficePlace", "Lugar no determinado");
      }
    }

    public Person Signer {
      get {
        return ExtendedData.Get("recorderOfficerSigner", Person.Empty);
      }
    }

    public int WorkflowModelId {
      get {
        return ExtendedData.Get("workflowModelId", -1);
      }
    }

    #endregion Properties

    #region Methods

    public FixedList<Person> GetAttendantSigners() {
      return ExtendedData.GetFixedList<Person>("attendantSigners", false);
    }


    public FixedList<Municipality> GetMunicipalities() {
      return base.ExtendedData.GetFixedList<Municipality>("municipalities", false);
    }


    public FixedList<RecordingSection> GetRecordingSections() {
      return RecordingSection.GetList(this);
    }


    public FixedList<RecordingBook> GetRecordingBooks(RecordingSection sectionType) {
      return RecordingBooksData.GetRecordingBooksInSection(this, sectionType);
    }


    public bool IsAttendantSigner(Person person) {
      return GetAttendantSigners().Contains(person);
    }

    #endregion Methods

  } // class RecorderOffice

} // namespace Empiria.Land.Registration
