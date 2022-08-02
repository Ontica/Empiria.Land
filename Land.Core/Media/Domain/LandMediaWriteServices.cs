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

using Empiria.Land.Media.Adapters;
using Empiria.Land.Registration.Transactions;


namespace Empiria.Land.Media {

  /// <summary>Provides file write services for Empiria Land entities.</summary>
  static internal class LandMediaWriteServices {


    static internal void RemoveTransactionFile(LRSTransaction transaction,
                                               LandMediaPosting landFile) {
      Assertion.Require(transaction, nameof(transaction));
      Assertion.Require(landFile, nameof(landFile));

      UnlinkFile(transaction, landFile);

      //if (!landFile.HasReferences) {
      //  StorageContainer container = landFile.GetContainer();

      //  container.Remove(landFile);
      //}
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

      var fileName = $"{DateTime.Now.ToString("yyyy.MM.dd-HH.mm.ss")}-{transaction.UID}.pdf";

      StorageFile storageFile = container.Store(relativePath, fileName, inputFile);

      return LinkFile(mediaContent, storageFile, transaction);
    }


    private static string DetermineRelativePath(LRSTransaction transaction) {
      string year = transaction.PresentationTime.ToString("yyyy");
      string month = transaction.PresentationTime.ToString("MM");
      string officeName = transaction.RecorderOffice.Alias.Replace(" ", string.Empty);

      return $"{year}-{month}-{officeName}";
    }

    static private StorageContainer DetermineContainerFor(LRSTransaction transaction) {
      string year = transaction.PresentationTime.ToString("yyyy");
      string month = transaction.PresentationTime.ToString("MM");

      // ToDo: look up containers by tags: "year == 2022" AND "month == 3"
      return StorageContainer.Parse(1000 + transaction.PresentationTime.Month);
    }


    static private LandMediaPosting LinkFile(LandMediaContent contentType,
                                             StorageFile storageFile,
                                             LRSTransaction transaction) {
      var posting = new LandMediaPosting(contentType, storageFile);

      posting.LinkToTransaction(transaction);

      posting.Save();

      return posting;
    }


    static private void UnlinkFile(LRSTransaction transaction,
                                   LandMediaPosting landFile) {
      throw new NotImplementedException();
    }

  }  // class LandMediaWriteServices

}  // namespace Empiria.Land.Media
