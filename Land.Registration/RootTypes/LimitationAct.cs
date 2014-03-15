/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land                                   Assembly : Empiria.Land                        *
*  Type      : LimitationAct                                  Pattern  : Empiria Object Type                 *
*  Version   : 1.5        Date: 28/Mar/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Represents a property limitation or property assessment or mortgage act.                      *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System.Data;

namespace Empiria.Land.Registration {

  /// <summary>Represents a property limitation or property assessment or mortgage act.</summary>
  public class LimitationAct : RecordingAct {

    #region Fields

    private const string thisTypeName = "ObjectType.RecordingAct.LimitationAct";

    #endregion Fields

    #region Constructors and parsers

    private LimitationAct()
      : base(thisTypeName) {
      // For create instances use Create static method instead
    }

    protected LimitationAct(string typeName)
      : base(typeName) {
      // Required by Empiria Framework. Do not delete. Protected in not sealed classes, private otherwise
    }

    static public new LimitationAct Parse(int id) {
      return BaseObject.Parse<LimitationAct>(thisTypeName, id);
    }

    #endregion Constructors and parsers

    #region Public methods

    protected override void ImplementsLoadObjectData(DataRow row) {
      base.ImplementsLoadObjectData(row);
    }

    protected override void ImplementsSave() {
      base.ImplementsSave();
    }

    #endregion Public methods

  } // class LimitationAct

} // namespace Empiria.Land.Registration
