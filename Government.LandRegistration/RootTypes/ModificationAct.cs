﻿/* Empiria® Land 2013 ****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria® Land                                  System   : Land Registration System            *
*  Namespace : Empiria.Government.LandRegistration            Assembly : Empiria.Government.LandRegistration *
*  Type      : ModificationAct                                Pattern  : Empiria Object Type                 *
*  Date      : 25/Jun/2013                                    Version  : 5.1     License: CC BY-NC-SA 3.0    *
*                                                                                                            *
*  Summary   : Recording act that serves for modify information relative to other recording act.             *
*                                                                                                            *
**************************************************** Copyright © La Vía Óntica SC + Ontica LLC. 1994-2013. **/
using System.Data;


namespace Empiria.Government.LandRegistration {

  /// <summary>Recording act that serves for modify information relative to other recording act.</summary>
  public class ModificationAct : RecordingAct {

    #region Fields

    private const string thisTypeName = "ObjectType.RecordingAct.ModificationAct";

    #endregion Fields

    #region Constructors and parsers

    private ModificationAct()
      : base(thisTypeName) {
      // For create instances use Create static method instead
    }

    protected ModificationAct(string typeName)
      : base(typeName) {
      // Required by Empiria Framework. Do not delete. Protected in not sealed classes, private otherwise
    }

    static public new ModificationAct Parse(int id) {
      return BaseObject.Parse<ModificationAct>(thisTypeName, id);
    }

    #endregion Constructors and parsers

    #region Public properties

    #endregion Public properties

    #region Public methods

    protected override void ImplementsLoadObjectData(DataRow row) {
      base.ImplementsLoadObjectData(row);
    }

    protected override void ImplementsSave() {
      base.ImplementsSave();
    }

    #endregion Public methods

  } // class ModificationAct

} // namespace Empiria.Government.LandRegistration