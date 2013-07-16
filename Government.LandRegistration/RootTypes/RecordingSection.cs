/* Empiria® Land 2013 ****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria® Land                                  System   : Land Registration System            *
*  Namespace : Empiria.Government.LandRegistration            Assembly : Empiria.Government.LandRegistration *
*  Type      : RecordingSection                               Pattern  : Storage Item                        *
*  Date      : 25/Jun/2013                                    Version  : 5.1     License: CC BY-NC-SA 3.0    *
*                                                                                                            *
*  Summary   : Describes a recording section under which all the books are legaly classified.                *
*                                                                                                            *
**************************************************** Copyright © La Vía Óntica SC + Ontica LLC. 1999-2013. **/
using System;

namespace Empiria.Government.LandRegistration {

  /// <summary>Describes a recording section type under which all the books are legaly classified.</summary>
  public class RecordingSection : GeneralObject {

    #region Fields

    private const string thisTypeName = "ObjectType.GeneralObject.RecordingSection";

    #endregion Fields

    #region Constructors and parsers

    public RecordingSection() : base(thisTypeName) {

    }

    private RecordingSection(string typeName) : base(typeName) {
      // Required by Empiria Framework. Do not delete. Protected in not sealed classes, private otherwise
    }

    static public RecordingSection Parse(int id) {
      return BaseObject.Parse<RecordingSection>(thisTypeName, id);
    }

    static public RecordingSection Empty {
      get { return BaseObject.ParseEmpty<RecordingSection>(thisTypeName); }
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

} // namespace Empiria.Government.LandRegistration