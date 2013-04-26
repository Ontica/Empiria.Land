/* Empiria® Land 2013 ****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria® Land                                  System   : Land Registration System            *
*  Namespace : Empiria.Government.LandRegistration            Assembly : Empiria.Government.LandRegistration *
*  Type      : AnnotationAct                                  Pattern  : Empiria Object Type                 *
*  Date      : 25/Jun/2013                                    Version  : 5.1     License: CC BY-NC-SA 3.0    *
*                                                                                                            *
*  Summary   : Represents an annotation recording act.                                                       *
*                                                                                                            *
**************************************************** Copyright © La Vía Óntica SC + Ontica LLC. 1994-2013. **/
using System.Data;

namespace Empiria.Government.LandRegistration {

  /// <summary>Represents a domain traslative recording act.</summary>
  public class AnnotationAct : RecordingAct {

    #region Fields

    private const string thisTypeName = "ObjectType.RecordingAct.AnnotationAct";

    #endregion Fields

    #region Constructors and parsers

    private AnnotationAct()
      : base(thisTypeName) {
      // For create instances use Create static method instead
    }

    protected AnnotationAct(string typeName)
      : base(typeName) {
      // Required by Empiria Framework. Do not delete. Protected in not sealed classes, private otherwise
    }

    static public new AnnotationAct Parse(int id) {
      return BaseObject.Parse<AnnotationAct>(thisTypeName, id);
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

  } // class AnnotationAct

} // namespace Empiria.Government.LandRegistration