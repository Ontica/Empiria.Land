/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transactions Management                    Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Information Holder                      *
*  Type     : DocumentType                               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes a Recorder Office document type.                                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Registration;

namespace Empiria.Land.Transactions {

  /// <summary>Describes a Recorder Office document type.</summary>
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

    #endregion Properties

  } // class LRSDocumentType

} // namespace Empiria.Land.Transactions
