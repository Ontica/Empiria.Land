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
using System.IO;
using System.Threading.Tasks;

using Empiria.Services;
using Empiria.Storage;

using Empiria.Land.Instruments;
using Empiria.Land.Instruments.Adapters;
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

        LandMediaFile landFile = LandMediaWriteServices.StoreTransactionFile(transaction, pdfFile);

        return LandMediaFileMapper.Map(landFile);
      });

      return task;
    }


    public Task RemoveTransactionMediaFile(string transactionUID, string mediaFileUID) {
      Assertion.Require(transactionUID, nameof(transactionUID));
      Assertion.Require(mediaFileUID, nameof(mediaFileUID));

      var task = new Task(() => {

        var transaction = LRSTransaction.Parse(transactionUID);

        var landFile = LandMediaFile.Parse(mediaFileUID);

        LandMediaWriteServices.RemoveTransactionFile(transaction, landFile);

      });

      return task;
    }


    public async Task<InstrumentDto> AppendInstrumentMediaFile(string instrumentUID,
                                                               LandMediaFileFields fields,
                                                               Stream fileStream) {
      ValidateArguments(instrumentUID, fields, fileStream);

      var instrument = Instrument.Parse(instrumentUID);

      var mediaFileSet = instrument.GetMediaFileSet();

      mediaFileSet.AssertCanBeAdded(fields);

      await mediaFileSet.Add(fields, fileStream);

      return InstrumentMapper.Map(instrument);
    }


    public InstrumentDto RemoveInstrumentMediaFile(string instrumentUID, string mediaFileUID) {
      Assertion.Require(instrumentUID, "instrumentUID");
      Assertion.Require(mediaFileUID, "mediaFileUID");

      var instrument = Instrument.Parse(instrumentUID);

      var mediaFileSet = instrument.GetMediaFileSet();

      mediaFileSet.AssertCanBeRemoved(mediaFileUID);

      mediaFileSet.Remove(mediaFileUID);

      return InstrumentMapper.Map(instrument);
    }


    public async Task<InstrumentDto> ReplaceInstrumentMediaFile(string instrumentUID,
                                                                string toReplaceMediaFileUID,
                                                                LandMediaFileFields fields,
                                                                Stream fileStream) {
      Assertion.Require(toReplaceMediaFileUID, "toReplaceMediaFileUID");
      ValidateArguments(instrumentUID, fields, fileStream);


      var instrument = Instrument.Parse(instrumentUID);

      var mediaFileSet = instrument.GetMediaFileSet();

      mediaFileSet.AssertCanBeReplaced(toReplaceMediaFileUID);

      await mediaFileSet.Replace(toReplaceMediaFileUID, fields, fileStream);

      return InstrumentMapper.Map(instrument);
    }


    #endregion Use cases

    #region Helper methods

    private void ValidateArguments(string instrumentUID,
                                   LandMediaFileFields fields,
                                   Stream fileStream) {
      Assertion.Require(instrumentUID, "instrumentUID");
      Assertion.Require(fileStream, "fileStream");
      Assertion.Require(fields, "fields");

      Assertion.Require(fields.MediaContent == LandMediaContent.InstrumentMainFile ||
                        fields.MediaContent == LandMediaContent.InstrumentAuxiliaryFile,
                       $"Unrecognized mediaContent value for a legal instrument file.");
    }

    #endregion Helper methods

  }  // class LandMediaFilesUseCases

}  // namespace Empiria.Land.Media.UseCases
