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
                                               LandMediaFile landFile) {
      Assertion.Require(transaction, nameof(transaction));
      Assertion.Require(landFile, nameof(landFile));

      UnlinkFile(transaction, landFile);

      //if (!landFile.HasReferences) {
      //  StorageContainer container = landFile.GetContainer();

      //  container.Remove(landFile);
      //}
    }


    static internal LandMediaFile StoreTransactionFile(LRSTransaction transaction,
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

      return LinkFile(mediaContent, transaction, storageFile);
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


    static private LandMediaFile LinkFile(LandMediaContent contentType,
                                          LRSTransaction transaction,
                                          StorageFile storageFile) {
      var mediaFile = new LandMediaFile();

      mediaFile.Save();

      return mediaFile;
    }


    static private void UnlinkFile(LRSTransaction transaction,
                                   LandMediaFile landFile) {
      throw new NotImplementedException();
    }

  }  // class LandMediaWriteServices

}  // namespace Empiria.Land.Media
