﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Enumeration                             *
*  Type     : TransactionStatus                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Enumerates the different workflow stages for a transaction.                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.Transactions.Adapters {

  /// <summary>Enumerates the possible statuses of a transaction with respect of the office workflow.</summary>
  public enum TransactionStatus {

    Undefined,

    Payment = 'Y',

    Received = 'R',

    Reentry = 'N',

    Control = 'K',

    Recording = 'G',

    Elaboration = 'E',

    Revision = 'V',

    Juridic = 'J',

    Process = 'P',

    OnSign = 'S',

    Digitalization = 'A',

    ToDeliver = 'D',

    Delivered = 'C',

    ToReturn = 'L',

    Returned = 'Q',

    Deleted = 'X',

    Archived = 'H',

    All = '@',

  }  // enum TransactionStatus

} // namespace Empiria.Land.Transactions.Adapters
