/* Empiria® Land 2013 ****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria® Land                                  System   : Land Registration System            *
*  Namespace : Empiria.Government.LandRegistration            Assembly : Empiria.Government.LandRegistration *
*  Type      : LRSTransactionType                             Pattern  : Storage Item                        *
*  Date      : 25/Jun/2013                                    Version  : 5.1     License: CC BY-NC-SA 3.0    *
*                                                                                                            *
*  Summary   : Describes a recorder office transaction type.                                                 *
*                                                                                                            *
**************************************************** Copyright © La Vía Óntica SC + Ontica LLC. 1994-2013. **/

namespace Empiria.Government.LandRegistration.Transactions {

  /// <summary>Describes a recorder office transaction type.</summary>
  public class LRSTransactionType : GeneralObject {

    #region Fields

    private const string thisTypeName = "ObjectType.GeneralObject.LRSTransactionType";

    #endregion Fields

    #region Constructors and parsers

    public LRSTransactionType()
      : base(thisTypeName) {

    }

    protected LRSTransactionType(string typeName)
      : base(typeName) {
      // Required by Empiria Framework. Do not delete. Protected in not sealed classes, private otherwise
    }

    static public LRSTransactionType Empty {
      get { return BaseObject.ParseEmpty<LRSTransactionType>(thisTypeName); }
    }

    static public LRSTransactionType Unknown {
      get { return BaseObject.ParseUnknown<LRSTransactionType>(thisTypeName); }
    }

    static public LRSTransactionType Parse(int id) {
      return BaseObject.Parse<LRSTransactionType>(thisTypeName, id);
    }

    static public ObjectList<LRSTransactionType> GetList() {
      ObjectList<LRSTransactionType> list = GeneralObject.ParseList<LRSTransactionType>(thisTypeName);

      list.Sort((x, y) => x.Name.CompareTo(y.Name));

      return list;
    }

    public ObjectList<LRSDocumentType> GetDocumentTypes() {
      ObjectList<LRSDocumentType> list = this.GetLinks<LRSDocumentType>("TransactionType_DocumentType");

      list.Sort((x, y) => x.Name.CompareTo(y.Name));

      return list;
    }

    #endregion Constructors and parsers

  } // class LRSTransactionType

} // namespace Empiria.Government.LandRegistration.Transactions