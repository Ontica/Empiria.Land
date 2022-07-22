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

      StorageContainer container = DetermineContainerFor(mediaContent, transaction);

      StorageFile storageFile = container.Add(inputFile);

      return LinkFile(mediaContent, transaction, storageFile);
    }


    static private StorageContainer DetermineContainerFor(LandMediaContent mediaContent,
                                                          LRSTransaction transaction) {
      string year = transaction.PresentationTime.ToString("yyyy");
      string month = transaction.PresentationTime.ToString("MM");
      string officeName = transaction.RecorderOffice.Alias;

      string path = $"{year}\\{year}-{month}\\{year}-{month}-{officeName}";

      // return StorageContainer.Parse(mediaContent.ToString(), path);
      return StorageContainer.Parse(1001);
    }


    static private LandMediaFile LinkFile(LandMediaContent contentType,
                                          LRSTransaction transaction,
                                          StorageFile storageFile) {
      throw new NotImplementedException();
    }


    static private void UnlinkFile(LRSTransaction transaction,
                                   LandMediaFile landFile) {
      throw new NotImplementedException();
    }

  }  // class LandMediaWriteServices

}  // namespace Empiria.Land.Media
