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

    public FixedList<RecordingActType> GetItems() {
      throw new NotImplementedException();

      //FixedList<RecordingActType> list = base.GetTypeLinks<RecordingActType>("RecordingActTypeCategory_Items");

      //list.Sort((x, y) => x.DisplayName.CompareTo(y.DisplayName));

      //return list;
    }

    #endregion Public methods

  } // class RecordingActTypeCategory

} // namespace Empiria.Land.Registration
