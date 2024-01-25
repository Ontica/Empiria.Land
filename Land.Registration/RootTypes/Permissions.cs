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
      if (currentUserPermissions.Contains("oficialia-jerez")) {
        return RecorderOffice.Parse(103);
      }
      if (currentUserPermissions.Contains("oficialia-rio-grande")) {
        return RecorderOffice.Parse(104);
      }
      if (currentUserPermissions.Contains("oficialia-sombrerete")) {
        return RecorderOffice.Parse(105);
      }
      if (currentUserPermissions.Contains("oficialia-tlaltenango")) {
        return RecorderOffice.Parse(106);
      }
      if (currentUserPermissions.Contains("oficialia-calera")) {
        return RecorderOffice.Parse(107);
      }
      if (currentUserPermissions.Contains("oficialia-jalpa")) {
        return RecorderOffice.Parse(109);
      }
      if (currentUserPermissions.Contains("oficialia-juchipila")) {
        return RecorderOffice.Parse(110);
      }
      if (currentUserPermissions.Contains("oficialia-loreto")) {
        return RecorderOffice.Parse(111);
      }
      if (currentUserPermissions.Contains("oficialia-nochistlan")) {
        return RecorderOffice.Parse(113);
      }
      if (currentUserPermissions.Contains("oficialia-ojocaliente")) {
        return RecorderOffice.Parse(114);
      }
      throw Assertion.EnsureNoReachThisCode("User does not have permissions " +
                                            "to any registered recording service.");

    }

  }  // class Permissions

}  // namespace Empiria.Land.Registration
