/* Empiria® Land 2013 ****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria® Land                                  System   : Land Registration System            *
*  Namespace : Empiria.Government.LandRegistration            Assembly : Empiria.Government.LandRegistration *
*  Type      : Certificate                                    Pattern  : Empiria Object Type                 *
*  Date      : 25/Jun/2013                                    Version  : 5.1     License: CC BY-NC-SA 3.0    *
*                                                                                                            *
*  Summary   : Certificate emission acts.                                                                    *
*                                                                                                            *
**************************************************** Copyright © La Vía Óntica SC + Ontica LLC. 1994-2013. **/
using System.Data;

namespace Empiria.Government.LandRegistration {

  /// <summary>Certificate emission acts.</summary>
  public class Certificate : RecordingAct {

    #region Fields

    private const string thisTypeName = "ObjectType.RecordingAct.Certificate";

    #endregion Fields

    #region Constructors and parsers

    private Certificate()
      : base(thisTypeName) {
      // For create instances use Create static method instead    
    }

    protected Certificate(string typeName)
      : base(typeName) {
      // Required by Empiria Framework. Do not delete. Protected in not sealed classes, private otherwise
    }

    static public new Certificate Parse(int id) {
      return BaseObject.Parse<Certificate>(thisTypeName, id);
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

  } // class Certificate

} // namespace Empiria.Government.LandRegistration