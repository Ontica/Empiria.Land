/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingSection                               Pattern  : Storage Item                        *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Describes a recording section under which all the books are legaly classified.                *
*                                                                                                            *
********************************* Copyright (c) 2009-2017. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

using Empiria.Land.Registration.Data;

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

    static public RecordingSection Empty {
      get { return BaseObject.ParseEmpty<RecordingSection>(); }
    }

    static public FixedList<RecordingSection> GetList() {
      var list = GeneralObject.ParseList<RecordingSection>();

      list.Sort((x, y) => x.NamedKey.CompareTo(y.NamedKey));

      return list;
    }

    #endregion Constructors and parsers

    #region Properties

    public bool UsePerpetualNumbering {
      get {
        if (ExecutionServer.LicenseName == "Tlaxcala") {
          return (this.Id == 1050);
        } else if (ExecutionServer.LicenseName == "Zacatecas") {
          return (this.Id == 1070);
        } else {
          return false;
        }
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
