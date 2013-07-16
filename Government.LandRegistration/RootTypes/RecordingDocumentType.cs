/* Empiria® Land 2013 ****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria® Land                                  System   : Land Registration System            *
*  Namespace : Empiria.Government.LandRegistration            Assembly : Empiria.Government.LandRegistration *
*  Type      : RecordingDocumentType                          Pattern  : PowerType Item                      *
*  Date      : 25/Jun/2013                                    Version  : 5.1     License: CC BY-NC-SA 3.0    *
*                                                                                                            *
*  Summary   : Power type that describes recording document types.                                           *
*                                                                                                            *
**************************************************** Copyright © La Vía Óntica SC + Ontica LLC. 1999-2013. **/

using Empiria.Ontology;

namespace Empiria.Government.LandRegistration {

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

    static public RecordingDocumentType Empty {
      get {
        return RecordingDocumentType.Parse(ObjectTypeInfo.Parse("ObjectType.RecordingDocument.Empty"));
      }
    }

    #endregion Constructors and parsers

  } // class RecordingDocumentType

} // namespace Empiria.Government.LandRegistration