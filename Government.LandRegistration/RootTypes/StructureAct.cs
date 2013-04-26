/* Empiria® Land 2013 ****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria® Land                                  System   : Land Registration System            *
*  Namespace : Empiria.Government.LandRegistration            Assembly : Empiria.Government.LandRegistration *
*  Type      : StructureAct                                   Pattern  : Empiria Object Type                 *
*  Date      : 25/Jun/2013                                    Version  : 5.1     License: CC BY-NC-SA 3.0    *
*                                                                                                            *
*  Summary   : Represents multiple property join or property split onto multiple properties recoding act.    *
*                                                                                                            *
**************************************************** Copyright © La Vía Óntica SC + Ontica LLC. 1994-2013. **/
using System.Data;



namespace Empiria.Government.LandRegistration {

  /// <summary>Represents multiple property join or property split onto multiple 
  /// properties recoding act.</summary>
  public class StructureAct : RecordingAct {

    #region Fields

    private const string thisTypeName = "ObjectType.RecordingAct.StructureAct";

    #endregion Fields

    #region Constructors and parsers

    private StructureAct()
      : base(thisTypeName) {
      // For create instances use Create static method instead
    }

    protected StructureAct(string typeName)
      : base(typeName) {
      // Required by Empiria Framework. Do not delete. Protected in not sealed classes, private otherwise
    }

    static public new StructureAct Parse(int id) {
      return BaseObject.Parse<StructureAct>(thisTypeName, id);
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

  } // class StructureAct

} // namespace Empiria.Government.LandRegistration