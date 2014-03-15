/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land                                   Assembly : Empiria.Land                        *
*  Type      : CancelationAct                                 Pattern  : Empiria Object Type                 *
*  Version   : 1.5        Date: 28/Mar/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Recording act that serves for cancel other recording act.                                     *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
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
