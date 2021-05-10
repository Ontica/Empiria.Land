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

using Empiria.DataTypes;
using Empiria.Land.Media.Adapters;
using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Media {

  /// <summary>Builds internal media files like payment orders or transaction submission receipts.</summary>
  internal class LandMediaBuilder {

    private readonly static string MEDIA_URL = ConfigurationData.GetString("LandMediaBuilder.DefaultUrl");

    internal LandMediaBuilder() {
      // no-op
    }


    internal MediaData GetMediaDto(LandMediaContent mediaContent, params string[] parameters) {
      switch (mediaContent) {

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

        case LandMediaContent.RegistrationStamp:
          return new MediaData("text/html",
                              $"{MEDIA_URL}/recording-stamps/registration.stamp.aspx?" +
                              $"uid={parameters[0]}");

        default:
          throw Assertion.AssertNoReachThisCode($"GetMediaDto() method can't process files of " +
                                                $"media content {mediaContent}.");
      }
    }

    internal FixedList<LandMediaFile> GetLandMediaFiles(LandMediaContent mediaContent, BaseObject instance) {
      var repository = new LandMediaFilesRepository();

      switch (mediaContent) {
        case LandMediaContent.BookEntryMediaFiles:
          return repository.GetFiles(mediaContent, instance);

        default:
          throw Assertion.AssertNoReachThisCode();
      }
    }

  }  // class LandMediaBuilder

}  // namespace Empiria.Land.Media
