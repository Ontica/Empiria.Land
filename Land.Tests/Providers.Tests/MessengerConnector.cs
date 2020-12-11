/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Messenger Connector                        Component : Test cases                              *
*  Assembly : Empiria.Land.Tests.dll                     Pattern   : Test class                              *
*  Type     : MessengerConnectorTests                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Integration tests for the messenger connector service.                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Xunit;

using Empiria.Land.Messaging;

namespace Empiria.Land.Providers.Tests {

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

  }

}
