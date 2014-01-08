/* Empiria® Land 2014 ****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria® Land                                  System   : Land Registration System            *
*  Namespace : Empiria.Land                                   Assembly : Empiria.Land                        *
*  Type      : InformationAct                                 Pattern  : Empiria Object Type                 *
*  Date      : 28/Mar/2014                                    Version  : 5.5     License: CC BY-NC-SA 4.0    *
*                                                                                                            *
*  Summary   : Represents an information recording act that are not limitations and can be applied to        *
*              properties, persons or neither.                                                               *
*                                                                                                            *
**************************************************** Copyright © La Vía Óntica SC + Ontica LLC. 1999-2014. **/
using System.Data;

namespace Empiria.Land.Registration {

  /// <summary>Represents an information recording act that are not limitations and can be applied to 
  /// properties, persons or neither.</summary>
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

} // namespace Empiria.Land.Registration
