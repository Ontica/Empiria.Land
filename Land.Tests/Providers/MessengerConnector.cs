/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Empiria Land Providers                     Component : Test cases                              *
*  Assembly : Empiria.Land.Tests.dll                     Pattern   : Test class                              *
*  Type     : MessengerConnectorTests                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Integration tests for the messenger connector service.                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using Xunit;

using System;

using Empiria.Land.Messaging;

namespace Empiria.Land.Tests.Providers {

  /// <summary>Integration tests for the messenger connector service.</summary>
  public class MessengerConnectorTests {

    [Fact]
    public void Should_Start_LandMessenger() {
      Exception e = null;

      try {
        LandMessenger.Start();

      } catch (Exception exception) {
        e = exception;
      }

      Assert.Null(e);
    }

  }  // class MessengerConnectorTests

}  // namespace Empiria.Land.Tests.Providers
