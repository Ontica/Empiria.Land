﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingSection                               Pattern  : Storage Item                        *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Describes a recording section under which all the books are legaly classified.                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.Land.Data;

namespace Empiria.Land.Registration {

  /// <summary>Describes a recording section type under which all the books are legaly classified.</summary>
  public class RecordingSection : GeneralObject {

    #region Constructors and parsers

    private RecordingSection() {
      // Required by Empiria Framework.
    }

    static public RecordingSection Parse(int id) {
      return BaseObject.ParseId<RecordingSection>(id);
    }


    static public RecordingSection Parse(string uid) {
      return BaseObject.ParseKey<RecordingSection>(uid);
    }


    static public RecordingSection Empty {
      get { return BaseObject.ParseEmpty<RecordingSection>(); }
    }


    static public FixedList<RecordingSection> GetList() {
      var list = GeneralObject.GetList<RecordingSection>();

      list.Sort((x, y) => x.Name.CompareTo(y.Name));

      return list;
    }

    static public FixedList<RecordingSection> GetListForRecording() {
      GeneralList listType = GeneralList.Parse("CreateNextPhysicalRecordingSections.List");

      return listType.GetItems<RecordingSection>();
    }





    static public FixedList<RecordingSection> GetListForOwnershipRecordings(RecorderOffice recorderOffice) {
      var list = GeneralObject.GetList<RecordingSection>();

      list = list.FindAll(x => x.ContainsOwnershipActs);

      list.Sort((x, y) => x.NamedKey.CompareTo(y.NamedKey));

      return list;
    }

    #endregion Constructors and parsers


    #region Properties


    public bool ContainsOwnershipActs {
      get {
        return base.ExtendedDataField.Get("ContainsOwnershipActs", false);
      }
    }


    public bool UsesPerpetualNumbering {
      get {
        return base.ExtendedDataField.Get("UsesPerpetualNumbering", false);
      }
    }


    #endregion Properties


    #region Methods

    public FixedList<RecorderOffice> GetRecorderOffices() {
      return RecordingBooksData.GetRecorderOffices(this);
    }

    public FixedList<RecordingBook> GetRecordingBooks(RecorderOffice recorderOffice) {
      return RecordingBooksData.GetRecordingBooksInSection(recorderOffice, this);
    }

    #endregion Methods


  } // class RecordingSection

} // namespace Empiria.Land.Registration
