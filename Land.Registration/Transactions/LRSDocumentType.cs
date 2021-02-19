/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Transactions                      Assembly : Empiria.Land.Registration           *
*  Type      : DocumentType                                   Pattern  : Storage Item                        *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Describes a recorder office document type.                                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Contacts;

namespace Empiria.Land.Registration.Transactions {

  /// <summary>Describes a recorder office document type.</summary>
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

    static public LRSDocumentType Parse(string uid) {
      return BaseObject.ParseKey<LRSDocumentType>(uid);
    }

    static public FixedList<LRSDocumentType> GetList() {
      return GeneralObject.GetList<LRSDocumentType>();
    }

    static internal LRSDocumentType ParseFromInstrumentKind(string kind) {
      var documentType = GeneralObject.TryParse<LRSDocumentType>($"ObjectName = '{kind}'");

      return documentType ?? LRSDocumentType.Empty;
    }


    #endregion Constructors and parsers

    #region Properties

    FixedList<RecordingActType> _defaultRecordingActs = null;
    public FixedList<RecordingActType> DefaultRecordingActs {
      get {
        if (_defaultRecordingActs == null) {
          var list = base.ExtendedDataField.GetList<RecordingActType>("DefaultRecordingActTypes", false);

          _defaultRecordingActs = list.ToFixedList();
        }
        return _defaultRecordingActs;
      }
    }

    public string LArt {
      get { return base.NamedKey; }
    }

    private FixedList<Contact> _issuedByEntities = null;
    public FixedList<Contact> IssuedByEntities {
      get {
        if (_issuedByEntities == null) {
          _issuedByEntities = base.GetLinks<Contact>("DocumentType->IssuedBy");
        }
        return _issuedByEntities;
      }
    }

    #endregion Properties

  } // class DocumentType

} // namespace Empiria.Land.Registration.Transactions
