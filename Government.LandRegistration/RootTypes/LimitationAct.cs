/* Empiria® Land 2013 ****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria® Land                                  System   : Land Registration System            *
*  Namespace : Empiria.Government.LandRegistration            Assembly : Empiria.Government.LandRegistration *
*  Type      : LimitationAct                                  Pattern  : Empiria Object Type                 *
*  Date      : 23/Oct/2013                                    Version  : 5.2     License: CC BY-NC-SA 3.0    *
*                                                                                                            *
*  Summary   : Represents a property limitation or property assessment or mortgage act.                       *
*                                                                                                            *
**************************************************** Copyright © La Vía Óntica SC + Ontica LLC. 1999-2013. **/
using System.Data;

namespace Empiria.Government.LandRegistration {

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

} // namespace Empiria.Government.LandRegistration