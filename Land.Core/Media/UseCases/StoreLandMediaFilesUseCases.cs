/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Media Files Management                Component : Use cases Layer                         *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Use case interactor class               *
*  Type     : StoreLandMediaFilesUseCases                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases to store media files related to Empiria Land entities.                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Threading.Tasks;

using Empiria.Services;
using Empiria.Storage;

using Empiria.Land.Media.Adapters;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Media.UseCases {

  /// <summary>Use cases to store media files related to Empiria Land entities.</summary>
  public class StoreLandMediaFilesUseCases : UseCase {

    #region Constructors and parsers

    protected StoreLandMediaFilesUseCases() {
      // no-op
    }

    static public StoreLandMediaFilesUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<StoreLandMediaFilesUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public Task<LandMediaFileDto> AppendTransactionMediaFile(string transactionUID,
                                                             InputFile pdfFile) {
      Assertion.Require(transactionUID, nameof(transactionUID));
      Assertion.Require(pdfFile, nameof(pdfFile));

      var task = new Task<LandMediaFileDto>(() => {

        var transaction = LRSTransaction.Parse(transactionUID);

        LandMediaPosting landFile = LandMediaWriteServices.StoreTransactionFile(transaction, pdfFile);

        return LandMediaFileMapper.Map(landFile);
      });

      task.Start();

      return task;
    }


    public Task RemoveTransactionMediaFile(string transactionUID, string mediaFileUID) {
      Assertion.Require(transactionUID, nameof(transactionUID));
      Assertion.Require(mediaFileUID, nameof(mediaFileUID));

      var task = new Task(() => {

        var transaction = LRSTransaction.Parse(transactionUID);

        var landFile = LandMediaPosting.Parse(mediaFileUID);

        LandMediaWriteServices.RemoveTransactionFile(transaction, landFile);

      });

      return task;
    }

    #endregion Use cases

  }  // class LandMediaFilesUseCases

}  // namespace Empiria.Land.Media.UseCases
