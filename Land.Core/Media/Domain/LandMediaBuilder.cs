﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Media Files Management                Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Service provider                        *
*  Type     : LandMediaBuilder                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Builds internal media files like payment orders or transaction submission receipts.            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.DataTypes;
using Empiria.Land.Media.Adapters;

using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Media {

  /// <summary>Builds internal media files like payment orders or transaction submission receipts.</summary>
  internal class LandMediaBuilder {

    private readonly static string MEDIA_URL = ConfigurationData.GetString("LandMediaBuilder.DefaultUrl");

    internal LandMediaBuilder(LandMediaContent mediaContent) {
      this.MediaContent = mediaContent;
    }

    internal LandMediaContent MediaContent {
      get;
    }

    internal LRSTransaction Transaction {
      get;
    }


    internal MediaData GetMediaDto(params string[] parameters) {
      switch (this.MediaContent) {

        case LandMediaContent.TransactionControlVoucher:
          return new MediaData("text/html",
                                $"{MEDIA_URL}/receipts/control.voucher.aspx?uid={parameters[0]}");

        case LandMediaContent.TransactionPaymentOrder:
          return new MediaData("text/html",
                                $"{MEDIA_URL}/receipts/payment.order.aspx?uid={parameters[0]}");

        case LandMediaContent.TransactionSubmissionReceipt:
          return new MediaData("text/html",
                                $"{MEDIA_URL}/receipts/transaction.receipt.aspx?uid={parameters[0]}");

        case LandMediaContent.BookEntryRegistrationStamp:
          return new MediaData("text/html",
                              $"{MEDIA_URL}/recording-stamps/physical-recording.stamp.aspx?" +
                              $"id={parameters[0]}&transactionId={parameters[1]}");

        default:
          throw Assertion.AssertNoReachThisCode($"GetMediaDto() method can't process files of " +
                                                $"media content {this.MediaContent}.");
      }
    }

  }  // class LandMediaBuilder

}  // namespace Empiria.Land.Media