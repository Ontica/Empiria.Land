/* Empiria® Land 2013 ****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria® Land                                  System   : Land Registration System            *
*  Namespace : Empiria.Government.LandRegistration            Assembly : Empiria.Government.LandRegistration *
*  Type      : InformationAct                                 Pattern  : Empiria Object Type                 *
*  Date      : 25/Jun/2013                                    Version  : 5.1     License: CC BY-NC-SA 3.0    *
*                                                                                                            *
*  Summary   : Represents an information recording act that does not have properties.                        *
*                                                                                                            *
**************************************************** Copyright © La Vía Óntica SC + Ontica LLC. 1994-2013. **/
using System.Data;


namespace Empiria.Government.LandRegistration {

  /// <summary> Represents an information recording act that does not have properties.</summary>
  public class InformationAct : RecordingAct {

    #region Fields

    private const string thisTypeName = "ObjectType.RecordingAct.InformationAct";

    #endregion Fields

    #region Constructors and parsers

    private InformationAct()
      : base(thisTypeName) {
      // For create instances use Create static method instead
    }

    protected InformationAct(string typeName)
      : base(typeName) {
      // Required by Empiria Framework. Do not delete. Protected in not sealed classes, private otherwise
    }

    static public new InformationAct Parse(int id) {
      return BaseObject.Parse<InformationAct>(thisTypeName, id);
    }

    static public InformationAct Empty {
      get { return BaseObject.ParseEmpty<InformationAct>(thisTypeName); }
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

  } // class InformationAct

} // namespace Empiria.Government.LandRegistration