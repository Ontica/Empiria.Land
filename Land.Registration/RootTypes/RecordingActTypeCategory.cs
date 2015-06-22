/* Empiria Land 2015 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingActTypeCategory                       Pattern  : Storage Item                        *
*  Version   : 2.0        Date: 25/Jun/2015                   License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Describes a recording type category or classificator.                                         *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Describes a recording type category or classificator.</summary>
  public class RecordingActTypeCategory : GeneralObject {

    #region Constructors and parsers

    private RecordingActTypeCategory() {
      // Required by Empiria Framework.
    }

    static public RecordingActTypeCategory Parse(int id) {
      return BaseObject.ParseId<RecordingActTypeCategory>(id);
    }

    static public RecordingActTypeCategory Empty {
      get { return BaseObject.ParseEmpty<RecordingActTypeCategory>(); }
    }

    static public FixedList<RecordingActTypeCategory> GetList(string listName) {
      GeneralList listType = GeneralList.Parse(listName);

      return listType.GetItems<RecordingActTypeCategory>();
    }

    #endregion Constructors and parsers

    #region Public properties

    public string UniqueKey {
      get { return base.NamedKey; }
    }

    #endregion Public properties

    #region Public methods

    //[DataField(ExtensionDataFieldName + ".RecordingActTypes")]
    // OOJJOO: Improve

    FixedList<RecordingActType> _recordingActTypesList = null;
    public FixedList<RecordingActType> RecordingActTypes {
      get {
        if (_recordingActTypesList == null) {
          var list = base.ExtendedDataField.GetList<RecordingActType>("RecordingActTypes");
          list.Sort((x, y) => x.DisplayName.CompareTo(y.DisplayName));
          _recordingActTypesList = list.ToFixedList();
        }
        return _recordingActTypesList;
      }
    }

    #endregion Public methods

  } // class RecordingActTypeCategory

} // namespace Empiria.Land.Registration
