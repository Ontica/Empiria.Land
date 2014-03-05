/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Transactions                      Assembly : Empiria.Land                        *
*  Type      : DocumentType                                   Pattern  : Storage Item                        *
*  Version   : 5.5        Date: 28/Mar/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Describes a recorder office document type.                                                    *
*                                                                                                            *
********************************* Copyright (c) 1999-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.Registration.Transactions {

  /// <summary>Describes a recorder office transaction type.</summary>
  public class LRSDocumentType : GeneralObject {

    #region Fields

    private const string thisTypeName = "ObjectType.GeneralObject.LRSDocumentType";

    #endregion Fields

    #region Constructors and parsers

    public LRSDocumentType()
      : base(thisTypeName) {

    }

    protected LRSDocumentType(string typeName)
      : base(typeName) {
      // Required by Empiria Framework. Do not delete. Protected in not sealed classes, private otherwise
    }

    static public LRSDocumentType Empty {
      get { return BaseObject.ParseEmpty<LRSDocumentType>(thisTypeName); }
    }

    static public LRSDocumentType Unknown {
      get { return BaseObject.ParseUnknown<LRSDocumentType>(thisTypeName); }
    }

    static public LRSDocumentType Parse(int id) {
      return BaseObject.Parse<LRSDocumentType>(thisTypeName, id);
    }

    static public ObjectList<LRSDocumentType> GetList() {
      ObjectList<LRSDocumentType> list = GeneralObject.ParseList<LRSDocumentType>(thisTypeName);

      list.Sort((x, y) => x.Name.CompareTo(y.Name));

      return list;
    }


    #endregion Constructors and parsers

    public string LArt {
      get { return base.NamedKey; }
    }

  } // class DocumentType

} // namespace Empiria.Land.Registration.Transactions