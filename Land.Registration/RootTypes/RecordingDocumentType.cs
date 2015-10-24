/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingDocumentType                          Pattern  : Power type                          *
*  Version   : 2.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Power type that describes recording document types.                                           *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

using Empiria.Ontology;

namespace Empiria.Land.Registration {

  /// <summary>Power type that describes recording document types.</summary>
  [Powertype(typeof(RecordingDocument))]
  public sealed class RecordingDocumentType : Powertype {

    #region Constructors and parsers

    private RecordingDocumentType() {
      // Empiria powertype types always have this constructor.
    }

    static public new RecordingDocumentType Parse(int typeId) {
      return ObjectTypeInfo.Parse<RecordingDocumentType>(typeId);
    }

    static internal new RecordingDocumentType Parse(string typeName) {
      return ObjectTypeInfo.Parse<RecordingDocumentType>(typeName);
    }

    static public RecordingDocumentType Empty {
      get {
        return RecordingDocumentType.Parse("ObjectType.RecordingDocument");
      }
    }

    #endregion Constructors and parsers

  } // class RecordingDocumentType

} // namespace Empiria.Land.Registration
