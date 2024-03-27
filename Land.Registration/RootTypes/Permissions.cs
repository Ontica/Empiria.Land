/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transactions Management                    Component : Domain layer                            *
*  Assembly : Empiria.Land.Transactions.dll              Pattern   : Static methods                          *
*  Type     : Permissions                                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Permissions class to get user's recording office.                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Permissions class to get user's recording office.</summary>
  static public class Permissions {

    static public bool HasPermission(RecorderOffice recorderOffice) {
      FixedList<string> userPermissions = ExecutionServer.CurrentPrincipal.Permissions;

      return userPermissions.Contains(recorderOffice.PermissionTag);
    }


    static public RecorderOffice GetUserDefaultRecorderOffice() {
      FixedList<string> userPermissions = ExecutionServer.CurrentPrincipal.Permissions;

      var recorderOffices = RecorderOffice.GetList();

      foreach (var office in recorderOffices) {
        if (userPermissions.Contains(office.PermissionTag)) {
          return office;
        }
      }

      throw Assertion.EnsureNoReachThisCode(
          "La cuenta de acceso no tiene registrada ninguna oficialía.");
    }

  }  // class Permissions

}  // namespace Empiria.Land.Registration
