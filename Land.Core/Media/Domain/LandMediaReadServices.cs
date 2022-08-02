/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Media Files Management                Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Service provider                        *
*  Type     : LandMediaReadServices                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides file read services for Empiria Land entities.                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Media {

  internal class LandMediaReadServices {

    static internal FixedList<LandMediaPosting> GetTransactionFiles(LRSTransaction transaction) {
      throw new NotImplementedException();
    }

  }  // class LandMediaReadServices

}  // namespace Empiria.Land.Media
