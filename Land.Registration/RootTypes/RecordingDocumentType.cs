/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingDocumentType                          Pattern  : PowerType Item                      *
*  Version   : 1.5        Date: 25/Jun/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Power type that describes recording document types.                                           *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

using Empiria.Ontology;

namespace Empiria.Land.Registration {

  /// <summary>Power type that describes recording document types.</summary>
  public sealed class RecordingDocumentType : PowerType<RecordingDocument> {

    #region Fields

    private const string thisTypeName = "PowerType.GeographicItemType";

    #endregion Fields

    #region Constructors and parsers

    private RecordingDocumentType(int typeId)
      : base(thisTypeName, typeId) {
      // Empiria PowerType pattern classes always has this constructor. Don't delete
    }

    static public new RecordingDocumentType Parse(int typeId) {
      return PowerType<RecordingDocument>.Parse<RecordingDocumentType>(typeId);
    }

    static internal RecordingDocumentType Parse(ObjectTypeInfo typeInfo) {
      return PowerType<RecordingDocument>.Parse<RecordingDocumentType>(typeInfo);
    }

    static public new RecordingDocumentType Empty {
      get {
        return RecordingDocumentType.Parse(ObjectTypeInfo.Parse("ObjectType.RecordingDocument.Empty"));
      }
    }

    #endregion Constructors and parsers

    #region Public methods

    internal new RecordingDocument CreateInstance() {
      RecordingDocument instance = base.CreateInstance();
      
      instance.DocumentType = this;
      
      return instance;
    }

    #endregion Public methods

  } // class RecordingDocumentType

} // namespace Empiria.Land.Registration
