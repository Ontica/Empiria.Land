/* Empiria® Land 2014 ****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria® Land                                  System   : Land Registration System            *
*  Namespace : Empiria.Land                                   Assembly : Empiria.Land                        *
*  Type      : RecordingCertificate                           Pattern  : Empiria Object Type                 *
*  Date      : 28/Mar/2014                                    Version  : 5.5     License: CC BY-NC-SA 4.0    *
*                                                                                                            *
*  Summary   : Certificate emission and information search acts.                                             *
*                                                                                                            *
**************************************************** Copyright © La Vía Óntica SC + Ontica LLC. 1999-2014. **/
using System.Data;

namespace Empiria.Land.Registration {

  /// <summary>Certificate emission and information search acts.</summary>
  public class RecordingCertificate : TransactionAct {

    #region Fields

    private const string thisTypeName = "ObjectType.RecordingAct.TransactionAct.Certificate";

    #endregion Fields

    #region Constructors and parsers

    private RecordingCertificate() : base(thisTypeName) {
      // For create instances use Create static method instead    
    }

    protected RecordingCertificate(string typeName) : base(typeName) {
      // Required by Empiria Framework. Do not delete. Protected in not sealed classes, private otherwise
    }

    static public new RecordingCertificate Parse(int id) {
      return BaseObject.Parse<RecordingCertificate>(thisTypeName, id);
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

  } // class RecordingCertificate

} // namespace Empiria.Land.Registration
