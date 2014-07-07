/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingActTypeCategory                       Pattern  : Storage Item                        *
*  Version   : 2.0        Date: 23/Oct/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Describes a recording type category or classificator.                                         *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Describes a recording type category or classificator.</summary>
  public class RecordingActTypeCategory : GeneralObject {

    #region Fields

    private const string thisTypeName = "ObjectType.GeneralObject.RecordingActTypeCategory";

    #endregion Fields

    #region Constructors and parsers

    public RecordingActTypeCategory()
      : base(thisTypeName) {

    }

    private RecordingActTypeCategory(string typeName)
      : base(typeName) {
      // Required by Empiria Framework. Do not delete. Protected in not sealed classes, private otherwise
    }

    static public RecordingActTypeCategory Parse(int id) {
      return BaseObject.Parse<RecordingActTypeCategory>(thisTypeName, id);
    }

    static public RecordingActTypeCategory Empty {
      get { return BaseObject.ParseEmpty<RecordingActTypeCategory>(thisTypeName); }
    }

    #endregion Constructors and parsers

    #region Public properties

    public new string NamedKey {
      get { return base.NamedKey; }
    }

    #endregion Public properties

    #region Public methods

    public FixedList<RecordingActType> GetItems() {
      FixedList<RecordingActType> list = base.GetTypeLinks<RecordingActType>("RecordingActTypeCategory_Items");

      list.Sort((x, y) => x.DisplayName.CompareTo(y.DisplayName));

      return list;
    }

    #endregion Public methods

  } // class RecordingActTypeCategory

} // namespace Empiria.Land.Registration
