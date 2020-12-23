/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Enumeration                             *
*  Type     : TransactionStatus                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Enumerates the different workflow stages for a transaction.                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.Transactions {

  /// <summary>Enumerates the possible statuses of a transaction with respect of the office workflow.</summary>
  public enum TransactionStatus {

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

} // namespace Empiria.Land.Transactions
