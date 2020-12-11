/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Electronic Sign Connector                  Component : Test cases                              *
*  Assembly : Empiria.Land.Tests.dll                     Pattern   : Test class                              *
*  Type     : ESignConnectorTests                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Integration tests for the electronic sign services connector.                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Threading.Tasks;
using Xunit;

using Empiria.Land.Integration;

using Empiria.Land.Tests;

namespace Empiria.Land.Providers.Tests {

  /// <summary>Integration tests for the electronic sign services connector.</summary>
  public class ESignConnectorTests {

    private readonly string SIGNED_DOCUMENT_UID = ConfigurationData.Get<string>("Testing.SignedDocumentUID");

    [Fact]
    public async Task Should_Get_Sign_Request_ByDocumentNumber() {
      CommonMethods.Authenticate();

      var connector = new ESignConnector();

      var signRequest = await connector.GetRequestByDocumentNumber(SIGNED_DOCUMENT_UID);

      Assert.Equal("Signed", signRequest.SignStatus);
    }

  }

}
