/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Media Files Management                Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Service provider                        *
*  Type     : LandMediaWriteServices                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides file write services for Empiria Land entities.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Storage;

using Empiria.Land.Transactions;

using Empiria.Land.Media.Adapters;

namespace Empiria.Land.Media {

  /// <summary>Provides file write services for Empiria Land entities.</summary>
  static internal class LandMediaWriteServices {


    static internal void RemoveTransactionFile(LRSTransaction transaction,
                                               LandMediaPosting posting) {
      Assertion.Require(transaction, nameof(transaction));
      Assertion.Require(posting, nameof(posting));

      RemovePosting(transaction, posting);

      var file = (StorageFile) posting.StorageItem;

      if (!LandMediaReadServices.HasFileReferences(file)) {

        StorageContainer container = file.Container;

        container.Remove(file);
      }
    }


    static internal LandMediaPosting StoreTransactionFile(LRSTransaction transaction,
                                                          InputFile inputFile) {
      LandMediaContent mediaContent = LandMediaFileFields.ConvertMediaContent(inputFile.AppContentType);

      Assertion.Require(mediaContent == LandMediaContent.InstrumentMainFile ||
                        mediaContent == LandMediaContent.InstrumentAuxiliaryFile,
                        $"Invalid mediaContent {mediaContent} for transaction files.");

      Assertion.Require(transaction, nameof(transaction));
      Assertion.Require(inputFile, nameof(inputFile));

      StorageContainer container = DetermineContainerFor(transaction);

      string relativePath = DetermineRelativePath(transaction);

      string fileExtension = "pdf";

      var fileName = $"{DateTime.Now.ToString("yyyy.MM.dd-HH.mm.ss.fff")}-{transaction.UID}.{fileExtension}";

      StorageFile storageFile = container.Store(relativePath, fileName, inputFile);

      return CreatePosting(mediaContent, storageFile, transaction);
    }


    static private StorageContainer DetermineContainerFor(LRSTransaction transaction) {
      string year = transaction.PresentationTime.ToString("yyyy");
      string month = transaction.PresentationTime.ToString("MM");

      // ToDo: look up containers by tags: "year == 2022" AND "month == 3"
      return StorageContainer.Parse(1000 + transaction.PresentationTime.Month);
    }


    static private string DetermineRelativePath(LRSTransaction transaction) {
      string year = transaction.PresentationTime.ToString("yyyy");
      string month = transaction.PresentationTime.ToString("MM");
      string officeName = transaction.RecorderOffice.ShortName.Replace(" ", string.Empty);

      return $"{year}-{month}-{officeName}";
    }


    static private LandMediaPosting CreatePosting(LandMediaContent contentType,
                                                  StorageFile storageFile,
                                                  LRSTransaction transaction) {
      var posting = new LandMediaPosting(contentType, storageFile);

      posting.LinkToTransaction(transaction);

      posting.Save();

      return posting;
    }


    static private void RemovePosting(LRSTransaction transaction,
                                      LandMediaPosting posting) {
      posting.Delete();

      posting.Save();
    }

  }  // class LandMediaWriteServices

}  // namespace Empiria.Land.Media
