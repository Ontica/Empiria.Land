/* Empiria Land 2015 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Transactions                      Assembly : Empiria.Land.Registration           *
*  Type      : LRSTransactionType                             Pattern  : Storage Item                        *
*  Version   : 2.0        Date: 04/Jan/2015                   License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Describes a recorder office transaction type.                                                 *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.Registration.Transactions {

  /// <summary>Describes a recorder office transaction type.</summary>
  public class LRSTransactionType : GeneralObject {

    #region Constructors and parsers

    private LRSTransactionType() {
      // Required by Empiria Framework.
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
      return GeneralObject.ParseList<LRSTransactionType>();
    }

    public FixedList<LRSDocumentType> GetDocumentTypes() {
      FixedList<LRSDocumentType> list = this.GetLinks<LRSDocumentType>("TransactionType_DocumentType");

      list.Sort((x, y) => x.Name.CompareTo(y.Name));

      return list;
    }

    #endregion Constructors and parsers

  } // class LRSTransactionType

} // namespace Empiria.Land.Registration.Transactions
