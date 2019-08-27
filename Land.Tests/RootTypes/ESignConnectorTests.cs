/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  System   : Empiria Land                                 Module  : Tests                                   *
*  Assembly : Empiria.Land.Tests.dll                       Pattern : Test class                              *
*  Type     : FilingTests                                  License : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Tests for filing objects functionality.                                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Threading.Tasks;
using Xunit;

using Empiria.Land.Integration;

namespace Empiria.Land.Tests {

  public class ESignConnectorTests {

    [Fact]
    public async Task Should_Get_RequestByDocumentNumber() {
      CommonMethods.Authenticate();

      var connector = new ESignConnector();

      var signRequest = await connector.GetRequestByDocumentNumber("RP29YD-83AM47-NR87C8");

      Assert.Equal("Signed", signRequest.SignStatus);
    }

  }  // ESignConnectorTests

}  // namespace Empiria.Land.Tests
