/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingSection                               Pattern  : Storage Item                        *
*  Version   : 2.0        Date: 23/Oct/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Describes a recording section under which all the books are legaly classified.                *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

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

  } // class RecordingSection

} // namespace Empiria.Land.Registration
