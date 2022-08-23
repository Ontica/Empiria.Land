/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transactions Preprocessing                   Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : TransactionMediaFilesController              License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web API used to add, remove and replace transaction's media files.                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Threading.Tasks;
using System.Web.Http;

using Empiria.Storage;
using Empiria.WebApi;

using Empiria.Land.Media.UseCases;

using Empiria.Land.Transactions.Preprocessing.Services;

namespace Empiria.Land.Transactions.Preprocessing.WebApi {

  /// <summary>Web API used to add, remove and replace transaction's media files.</summary>
  public class TransactionMediaFilesController : WebApiController {

    #region Web Apis


    [HttpPost]
    [Route("v5/land/transactions/{transactionUID:length(19)}/media-files")]
    public async Task<SingleObjectModel> AppendTransactionMediaFile([FromUri] string transactionUID) {

      string mediaContent = base.GetFormDataFromHttpRequest("mediaContent");

      InputFile pdfFile = base.GetInputFileFromHttpRequest(mediaContent);

      using (var usecases = StoreLandMediaFilesUseCases.UseCaseInteractor()) {
        _ = await usecases.AppendTransactionMediaFile(transactionUID, pdfFile).ConfigureAwait(false);

        TransactionPreprocessingDto dto = GetTransactionPreprocessingDto(transactionUID);

        return new SingleObjectModel(this.Request, dto);
      }
    }


    [HttpDelete]
    [Route("v5/land/transactions/{transactionUID:length(19)}/media-files/{mediaFileUID:guid}")]
    public async Task<SingleObjectModel> RemoveTransactionMediaFile([FromUri] string transactionUID,
                                                                    [FromUri] string mediaFileUID) {

      using (var usecases = StoreLandMediaFilesUseCases.UseCaseInteractor()) {
        await usecases.RemoveTransactionMediaFile(transactionUID, mediaFileUID);

        TransactionPreprocessingDto dto = GetTransactionPreprocessingDto(transactionUID);

        return new SingleObjectModel(this.Request, dto);
      }
    }


    #endregion Web Apis

    #region Helper methods


    private TransactionPreprocessingDto GetTransactionPreprocessingDto(string transactionUID) {
      using (var usecases = TransactionPreprocessingServices.ServiceInteractor()) {
        return usecases.GetPreprocessingData(transactionUID);
      }
    }

    #endregion Helper methods

  }  // class TransactionMediaFilesController

}  // namespace Empiria.Land.Transactions.Preprocessing.WebApi
