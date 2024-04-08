/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Messaging services                    Component : Subscription services                   *
*  Assembly : Empiria.Land.Registration.dll              Pattern   : Data Services                           *
*  Type     : MessagingData                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Read and write methods for land system messaging components.                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

namespace Empiria.Land.Messaging {

  /// <summary>Read and write methods for land system messaging components.</summary>
  static internal class MessagingData {

    #region Internal methods

    static internal void WriteSubscription(Subscription o) {
      var op = DataOperation.Parse("writeLRSPosting", o.Id, o.UID,
                                    o.SubscriptionType.ToString(), o.SubscribedObjectUID,
                                    o.SendTo.ToString(), String.Empty,
                                    o.PostingTime, -1, (char) o.Status);

      DataWriter.Execute(op);
    }


    #endregion Internal methods

  } // class MessagingData

} // namespace Empiria.Land.Data
