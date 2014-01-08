/* Empiria® Land 2014 ****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria® Land                                  System   : Land Registration System            *
*  Namespace : Empiria.Land                                   Assembly : Empiria.Land                        *
*  Type      : TransactionAct                                 Pattern  : Empiria Object Type                 *
*  Date      : 28/Mar/2014                                    Version  : 5.5     License: CC BY-NC-SA 4.0    *
*                                                                                                            *
*  Summary   : Transaction act that serves only for payment and control functions.                           *
*              Transaction are not recordable.                                                               *
*                                                                                                            *
**************************************************** Copyright © La Vía Óntica SC + Ontica LLC. 1999-2014. **/
using System.Data;

namespace Empiria.Land.Registration {

  /// <summary>Transaction acts serves only for payment and control functions.
  ///  Transaction acts are not recordable.</summary>
  public class TransactionAct : RecordingAct {

    #region Fields

    private const string thisTypeName = "ObjectType.RecordingAct.TransactionAct";

    #endregion Fields

    #region Constructors and parsers

    private TransactionAct()
      : base(thisTypeName) {
      // For create instances use Create static method instead    
    }

    protected TransactionAct(string typeName)
      : base(typeName) {
      // Required by Empiria Framework. Do not delete. Protected in not sealed classes, private otherwise
    }

    static public new TransactionAct Parse(int id) {
      return BaseObject.Parse<TransactionAct>(thisTypeName, id);
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

  } // class TransactionAct

} // namespace Empiria.Land.Registration
