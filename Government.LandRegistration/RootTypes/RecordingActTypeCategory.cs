/* Empiria® Land 2014 ****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria® Land                                  System   : Land Registration System            *
*  Namespace : Empiria.Land                                   Assembly : Empiria.Land                        *
*  Type      : RecordingActTypeCategory                       Pattern  : Storage Item                        *
*  Date      : 28/Mar/2014                                    Version  : 5.5     License: CC BY-NC-SA 4.0    *
*                                                                                                            *
*  Summary   : Describes a recording type category or classificator.                                         *
*                                                                                                            *
**************************************************** Copyright © La Vía Óntica SC + Ontica LLC. 1999-2014. **/
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

    public ObjectList<RecordingActType> GetItems() {
      ObjectList<RecordingActType> list = base.GetTypeLinks<RecordingActType>("RecordingActTypeCategory_Items");

      list.Sort((x, y) => x.DisplayName.CompareTo(y.DisplayName));

      return list;
    }

    #endregion Public methods

  } // class RecordingActTypeCategory

} // namespace Empiria.Land.Registration
