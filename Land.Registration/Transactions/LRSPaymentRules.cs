/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Transactions                      Assembly : Empiria.Land.Registration           *
*  Type      : LRSPaymentRules                                Pattern  : Static class                        *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Payment rules set for the Land Registration System.                                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Registration.Transactions {

  /// <summary>Payment rules set for the Land Registration System.</summary>
  static public class LRSPaymentRules {

    #region Methods

    internal static bool IsFeeWaiverApplicable(LRSTransaction transaction) {
      if (transaction.TransactionType.Id == 705 ||
          transaction.TransactionType.Id == 704 ||
          transaction.TransactionType.Id == 707) {
        return true;
      }

      return (transaction.TransactionType.Id == 700 && transaction.DocumentType.Id == 722);
    }

    #endregion Methods

  }  // class LRSPaymentRules

}  // namespace Empiria.Land.Registration.Transactions
