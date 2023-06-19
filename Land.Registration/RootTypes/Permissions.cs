/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transactions Management                    Component : Domain layer                            *
*  Assembly : Empiria.Land.Transactions.dll              Pattern   : Static methods                          *
*  Type     : Permissions                                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Temporarily permissions class to get user's recording office.                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Temporarily permissions class to get user's recording office.</summary>
  static public class Permissions {

    static public RecorderOffice GetUserRecorderOffice() {
      FixedList<string> currentUserPermissions = ExecutionServer.CurrentPrincipal.Permissions;

      if (currentUserPermissions.Contains("oficialia-zacatecas")) {
        return RecorderOffice.Parse(101);
      }
      if (currentUserPermissions.Contains("oficialia-fresnillo")) {
        return RecorderOffice.Parse(102);
      }

      throw Assertion.EnsureNoReachThisCode("User does not have permissions " +
                                            "to any registered recording service");

    }

  }  // class Permissions

}  // namespace Empiria.Land.Registration

