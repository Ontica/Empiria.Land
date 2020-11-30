/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Documents Recording                        Component : Test cases                              *
*  Assembly : Empiria.Land.Tests.dll                     Pattern   : Test class                              *
*  Type     : RecordedDocumentsUseCasesTests             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases for land documents recording.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using Xunit;

namespace Empiria.Land.Recording.UseCases.Tests {

  /// <summary>Test cases for land documents recording.</summary>
  public class RecordedDocumentsUseCasesTests {

    #region Fields

    private readonly string _RECORDED_DOCUMENT_UID;

    private readonly RecordedDocumentsUseCases _usecases;

    #endregion Fields


    #region Initialization

    public RecordedDocumentsUseCasesTests() {
      _RECORDED_DOCUMENT_UID = "RP73RX-94ZF28-HN34E7";

      _usecases = RecordedDocumentsUseCases.GetUseCaseInteractor();
    }


    ~RecordedDocumentsUseCasesTests() {
      _usecases.Dispose();
    }

    #endregion Initialization

    #region Facts

    [Fact]
    public void Should_Read_RecordedDocument() {
      RecordedDocumentDto document = _usecases.GetRecordedDocument(_RECORDED_DOCUMENT_UID);

      Assert.Equal(_RECORDED_DOCUMENT_UID, document.UID);
    }

    #endregion Facts

  }  // RecordedDocumentsUseCasesTests

}  // namespace Empiria.Land.Recording.UseCases.Tests
