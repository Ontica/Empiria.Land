/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Documents Recording                        Component : Use cases Layer                         *
*  Assembly : Empiria.Land.UseCases.dll                  Pattern   : Use case interactor class               *
*  Type     : RecordedDocumentsUseCases                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for documents recording.                                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Services;

using Empiria.Land.Registration;

namespace Empiria.Land.Recording.UseCases {

  /// <summary>Use cases for land documents recording.</summary>
  public class RecordedDocumentsUseCases : UseCase {

    #region Constructors and parsers

    protected RecordedDocumentsUseCases() {
      // no-op
    }

    static public RecordedDocumentsUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<RecordedDocumentsUseCases>();
    }

    #endregion Constructors and parsers

    #region Query Use cases

    public RecordedDocumentDto GetRecordedDocument(string documentUID) {
      Assertion.AssertObject(documentUID, "documentUID");

      var document = RecordingDocument.Parse(documentUID);

      return RecordedDocumentMapper.Map(document);
    }

    #endregion Query Use cases

  }  // class RecordedDocumentsUseCases

}  // namespace Empiria.Land.Recording.UseCases
