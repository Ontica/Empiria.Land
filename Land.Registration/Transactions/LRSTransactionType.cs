/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Transactions                      Assembly : Empiria.Land.Registration           *
*  Type      : LRSTransactionType                             Pattern  : Storage Item                        *
*  Version   : 2.0        Date: 23/Oct/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Describes a recorder office transaction type.                                                 *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.Registration.Transactions {

  /// <summary>Describes a recorder office transaction type.</summary>
  public class LRSTransactionType : GeneralObject {

    #region Fields

    private const string thisTypeName = "ObjectType.GeneralObject.LRSTransactionType";

    #endregion Fields

    #region Constructors and parsers

    public LRSTransactionType() : base(thisTypeName) {

    }

    protected LRSTransactionType(string typeName) : base(typeName) {
      // Required by Empiria Framework. Do not delete. Protected in not sealed classes, private otherwise
    }

    static public LRSTransactionType Empty {
      get { return BaseObject.ParseEmpty<LRSTransactionType>(); }
    }

    static public LRSTransactionType Unknown {
      get { return BaseObject.ParseUnknown<LRSTransactionType>(); }
    }

    static public LRSTransactionType Parse(int id) {
      return BaseObject.ParseId<LRSTransactionType>(id);
    }

    static public FixedList<LRSTransactionType> GetList() {
      FixedList<LRSTransactionType> list = GeneralObject.ParseList<LRSTransactionType>();

      list.Sort((x, y) => x.Name.CompareTo(y.Name));

      return list;
    }

    public FixedList<LRSDocumentType> GetDocumentTypes() {
      FixedList<LRSDocumentType> list = this.GetLinks<LRSDocumentType>("TransactionType_DocumentType");

      list.Sort((x, y) => x.Name.CompareTo(y.Name));

      return list;
    }

    #endregion Constructors and parsers

  } // class LRSTransactionType

} // namespace Empiria.Land.Registration.Transactions
