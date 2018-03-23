﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  System   : Empiria Land                                 Module  : Tests                                   *
*  Assembly : Empiria.Land.Tests.dll                       Pattern : Static class                            *
*  Type     : CommonMethods                                License : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Auxiliary common methods used by unit tests.                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Security;

namespace Empiria.Land.Tests {

  static internal class CommonMethods {
    
    #region Auxiliary methods

    static internal void Authenticate() {
      string sessionToken = ConfigurationData.GetString("Testing.SessionToken");

      System.Threading.Thread.CurrentPrincipal = AuthenticationService.Authenticate(sessionToken);
    }

    #endregion Auxiliary methods

  }  // CommonMethods

}  // namespace Empiria.Land.Tests
