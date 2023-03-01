/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Electronic Sign Services                   Component : Test cases                              *
*  Assembly : Empiria.Land.Tests.dll                     Pattern   : Use cases tests class                   *
*  Type     : ESignUseCasesTests                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases for ESign.                                                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using Empiria.Land.ESign.Adapters;
using Empiria.Land.ESign.UseCases;
using Xunit;

namespace Empiria.Land.Tests.ESign {

  /// <summary>Test cases for ESign.</summary>
  public class ESignUseCasesTests {


    [Fact]
    public void ShouldGetSignedDocuments() {

      using (var useCase = ESignEngineUseCases.UseCaseInteractor()) {

        int recorderOfficeId = 101;
        FixedList<SignDocumentDto> documents = useCase.GetSignedDocuments(recorderOfficeId);
        Assert.NotNull(documents);
      }

    }

  }
}
