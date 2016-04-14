/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Transactions                      Assembly : Empiria.Land.Registration           *
*  Type      : LRSPaymentRules                                Pattern  : Static class                        *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Payment rules set for the Land Registration System.                                           *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.Registration.Transactions {

  /// <summary>Payment rules set for the Land Registration System.</summary>
  static public class LRSPaymentRules {

    #region Methods

    internal static bool IsFeeWaiverApplicable(LRSTransaction transaction) {
      return (transaction.TransactionType.Id == 704 ||
             (transaction.TransactionType.Id == 700 && transaction.DocumentType.Id == 722));
    }

    #endregion Methods

  }  // class LRSPaymentRules

}  // namespace Empiria.Land.Registration.Transactions
