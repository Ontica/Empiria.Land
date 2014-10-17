/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Transactions                      Assembly : Empiria.Land.Registration           *
*  Type      : DocumentType                                   Pattern  : Storage Item                        *
*  Version   : 2.0        Date: 23/Oct/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Describes a recorder office document type.                                                    *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.Registration.Transactions {

  /// <summary>Describes a recorder office transaction type.</summary>
  public class LRSDocumentType : GeneralObject {

    #region Constructors and parsers

    private LRSDocumentType() {
      // Required by Empiria Framework.
    }

    static public LRSDocumentType Empty {
      get { return BaseObject.ParseEmpty<LRSDocumentType>(); }
    }

    static public LRSDocumentType Unknown {
      get { return BaseObject.ParseUnknown<LRSDocumentType>(); }
    }

    static public LRSDocumentType Parse(int id) {
      return BaseObject.ParseId<LRSDocumentType>(id);
    }

    static public FixedList<LRSDocumentType> GetList() {
      return GeneralObject.ParseList<LRSDocumentType>();
    }

    #endregion Constructors and parsers

    public string LArt {
      get { return base.NamedKey; }
    }

  } // class DocumentType

} // namespace Empiria.Land.Registration.Transactions
