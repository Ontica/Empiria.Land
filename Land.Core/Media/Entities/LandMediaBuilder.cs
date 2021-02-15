/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Media Files Management                Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Service provider                        *
*  Type     : LandMediaBuilder                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Builds internal media files like payment orders or transaction submission receipts.            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Media.Adapters;

using Empiria.Land.Registration.Transactions;


namespace Empiria.Land.Media {

  /// <summary>Builds internal media files like payment orders or transaction submission receipts.</summary>
  internal class LandMediaBuilder {

    private readonly static string MEDIA_URL = ConfigurationData.GetString("PaymentOrder.Url");

    internal LandMediaBuilder(LandMediaContent mediaContent, LRSTransaction transaction) {
      this.MediaContent = mediaContent;
      this.Transaction = transaction;
    }

    internal LandMediaContent MediaContent {
      get;
    }

    internal LRSTransaction Transaction {
      get;
    }


    internal MediaDto GetMediaDto() {
      var dto = new MediaDto();

      switch (this.MediaContent) {
        case LandMediaContent.TransactionSubmissionReceipt:

          dto.Url = $"{MEDIA_URL}/transaction.receipt.aspx?uid={Transaction.UID}";
          dto.MediaType = $"text/html";

          return dto;
        default:
          throw Assertion.AssertNoReachThisCode($"GetMediaDto() method can't process files of " +
                                                $"media content {this.MediaContent}.");
      }
    }

  }  // class LandMediaBuilder

}  // namespace Empiria.Land.Media
