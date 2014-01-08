/* Empiria® Land 2014 ****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria® Land                                  System   : Land Registration System            *
*  Namespace : Empiria.Land                                   Assembly : Empiria.Land                        *
*  Type      : CancelationAct                                 Pattern  : Empiria Object Type                 *
*  Date      : 28/Mar/2014                                    Version  : 5.5     License: CC BY-NC-SA 4.0    *
*                                                                                                            *
*  Summary   : Recording act that serves for cancel other recording act.                                     *
*                                                                                                            *
**************************************************** Copyright © La Vía Óntica SC + Ontica LLC. 1999-2014. **/
using System.Data;

namespace Empiria.Land.Registration {

  /// <summary>Recording act that serves for cancel other recording act.</summary>
  public class CancelationAct : RecordingAct {

    #region Fields

    private const string thisTypeName = "ObjectType.RecordingAct.CancelationAct";

    #endregion Fields

    #region Constructors and parsers

    private CancelationAct()
      : base(thisTypeName) {
      // For create instances use Create static method instead
    }

    protected CancelationAct(string typeName)
      : base(typeName) {
      // Required by Empiria Framework. Do not delete. Protected in not sealed classes, private otherwise
    }

    static public new CancelationAct Parse(int id) {
      return BaseObject.Parse<CancelationAct>(thisTypeName, id);
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

  } // class CancelationAct

} // namespace Empiria.Land.Registration
