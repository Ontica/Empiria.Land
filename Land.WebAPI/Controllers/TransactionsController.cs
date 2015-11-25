﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*	 Solution  : Empiria Land                                     System   : Land Web API                      *
*  Namespace : Empiria.Land.WebApi                              Assembly : Empiria.Land.WebApi.dll           *
*  Type      : TransactionsController                           Pattern  : Web API                           *
*  Version   : 1.0                                              License  : Please read license.txt file      *
*                                                                                                            *
*  Summary   : Contains services to integrate Empiria Land with third-party transaction systems.             *
*                                                                                                            *
********************************* Copyright (c) 2014-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Web.Http;

using Empiria.Data;
using Empiria.WebApi;
using Empiria.WebApi.Models;

using Empiria.Land.Certification;
using Empiria.Land.Registration.Transactions;
using Empiria.Land.WebApi.Models;

namespace Empiria.Land.WebApi {

  /// <summary>Contains services to integrate Empiria Land with third-party transaction systems.</summary>
  //[WebApiAuthorizationFilter(WebApiClaimType.ClientApp_Controller, "ThirdPartyTransactionController")]
  public class TransactionsController : WebApiController {

    #region Public APIs

    [HttpGet, AllowAnonymous]
    [Route("v1/transactions/{transactionUID}")]
    public SingleObjectModel GetTransaction(string transactionUID) {
      try {
        base.RequireResource(transactionUID, "transactionUID");

        string sql = "SELECT * FROM vwLRSTransactionForWS WHERE TransactionKey = '" + transactionUID + "'";

        var data = DataReader.GetDataTable(DataOperation.Parse(sql));

        if (data != null) {
          return new SingleObjectModel(this.Request, data, "Empiria.Land.Transaction");
        } else {
          throw new ResourceNotFoundException("Transaction.UID",
                                              "Transaction with identifier '{0}' was not found.",
                                              transactionUID);
        }
      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }

    /// <summary>Request a new transaction to issue a land certificate.</summary>
    [HttpPost]
    [Route("v1/transactions/request-certificate")]
    public SingleObjectModel RequestCertificate([FromBody] CertificateRequest certificateRequest) {
      try {
        base.RequireBody(certificateRequest);

        var transaction = certificateRequest.CreateTransaction();

        return new SingleObjectModel(this.Request, this.GetTransactionModel(transaction),
                                     "Empiria.Land.CertificateIssuingTransaction");
      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }

    ///// <summary>Request a new transaction to issue an unregistered property UID (aka folio real).</summary>
    //[HttpPost]
    //[Route("v1/citys/request-property-uid")]
    //public SingleObjectModel RequestRealPropertyUID([FromBody] RealPropertyUIDRequestDTO requestData) {
    //  try {
    //    base.RequireBody(requestData);

    //    var transaction = RealPropertyUIDRequestDTO.CreateTransaction(requestData);

    //    return this.BuildTransactionResponse(transaction);
    //  } catch (Exception e) {
    //    throw base.CreateHttpException(e);
    //  }
    //}

    #endregion Public APIs

    #region Private methods

    private object GetTransactionModel(LRSTransaction o) {
      var externalTransaction = o.ExtensionData.GetObject<CertificateRequest>("ExternalTransaction");

      return new {
        uid = o.UID,
        externalTransactionNo = externalTransaction.TransactionNo,
        requestedBy = o.RequestedBy,
        presentationTime = o.PresentationTime,
        realPropertyUID = externalTransaction.RealPropertyUID,
        estimatedDueTime = o.EstimatedDueTime,
        status = o.StatusName,
      };
    }

    private SingleObjectModel BuildCertificateAsTextResponse(Certificate certificate) {
      return new SingleObjectModel(this.Request, this.GetCertificateAsTextModel(certificate),
                                   "Empiria.Land.CertificateAsText");
    }

    private object GetCertificateModel(Certificate o) {
      return new {
        uid = o.UID,
        type = new {
          uid = o.CertificateType.Name,
          displayName = o.CertificateType.DisplayName,
        },
        transaction = new {
          uid = o.Transaction.UID,
          requestedBy = o.Transaction.RequestedBy,
          presentationTime = o.Transaction.PresentationTime,
          paymentReceipt = o.Transaction.Payments.ReceiptNumbers,
        },
        status = new {
          uid = o.Status.ToString(),
          code = (char) o.Status,
        },
        recorderOffice = new {
          id = o.RecorderOffice.Id,
          name = o.RecorderOffice.Alias,
        },
        property = new {
          uid = o.Property.UID,
          commonName = o.ExtensionData.PropertyCommonName,
          location = o.ExtensionData.PropertyLocation,
          metesAndBounds = o.ExtensionData.PropertyMetesAndBounds,
        },
        fromOwnerName = o.ExtensionData.FromOwnerName,
        toOwnerName = o.OwnerName,
        operation = o.ExtensionData.Operation,
        operationDate = o.ExtensionData.OperationDate,
        seekForName = o.ExtensionData.SeekForName,
        startingYear = o.ExtensionData.StartingYear,
        marginalNotes = o.ExtensionData.MarginalNotes,
        useMarginalNotesAsFullBody = o.ExtensionData.UseMarginalNotesAsFullBody,
        //userNotes = o.UserNotes,
      };
    }

    private object GetCertificateAsTextModel(Certificate o) {
      return new {
        uid = o.UID,
        type = new {
          uid = o.CertificateType.Name,
          displayName = o.CertificateType.DisplayName,
        },
        status = new {
          uid = o.Status.ToString(),
          code = (char) o.Status,
        },
        text = o.AsText,
      };
    }

    #endregion Private methods

  }  // class CITySIntegrationController

}  // namespace Empiria.Land.WebApi
