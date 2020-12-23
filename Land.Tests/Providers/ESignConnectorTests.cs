/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Empiria Land Providers                     Component : Test cases                              *
*  Assembly : Empiria.Land.Tests.dll                     Pattern   : Test class                              *
*  Type     : ESignConnectorTests                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Integration tests for the electronic sign services connector.                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using Xunit;

using System.Threading.Tasks;

using Empiria.Land.Integration;

namespace Empiria.Land.Tests.Providers {

  /// <summary>Integration tests for the electronic sign services connector.</summary>
  public class ESignConnectorTests {

    private readonly string _SIGNED_DOCUMENT_UID = TestingConstants.SIGNED_DOCUMENT_UID;

    [Fact]
    public async Task Should_Get_Sign_Request_ByDocumentNumber() {
      CommonMethods.Authenticate();

      var connector = new ESignConnector();

      var signRequest = await connector.GetRequestByDocumentNumber(_SIGNED_DOCUMENT_UID);

      Assert.Equal("Signed", signRequest.SignStatus);
    }

  }  // class ESignConnectorTests

}  // namespace Empiria.Land.Tests.Providers
