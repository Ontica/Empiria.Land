/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Extranet Services                            Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Response methods                      *
*  Type     : InstrumentModels                             License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Response models for legal instruments.                                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections;
using System.Collections.Generic;

using Empiria.Land.Instruments;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.WebApi.Extranet {

  /// <summary>Response models for legal instruments.</summary>
  static internal class InstrumentModels {

    #region Response models

    static internal ICollection ToResponse(this IList<LegalInstrument> list) {
      ArrayList array = new ArrayList(list.Count);

      foreach (var item in list) {
        array.Add(item.ToResponse());
      }
      return array;
    }


    static internal object ToResponse(this LegalInstrument instrument) {
      return new {
        uid = instrument.UID,
        name = instrument.GetEmpiriaType().DisplayName + " " + instrument.Number,
        type = instrument.GetEmpiriaType().Name,
        typeName = instrument.GetEmpiriaType().DisplayName,
        requestedBy = instrument.RequestedBy,
        number = instrument.Number,
        summary = instrument.Summary,
        issueDate = instrument.IssueDate,
        postingTime = instrument.PostingTime,
        status = instrument.Status,

        projectedOperation = instrument.Summary,
        property = instrument.Property.IsEmptyInstance ? new { } : instrument.Property.ToResponse(),

        isSigned = instrument.ElectronicSign != String.Empty,
        isRequested = instrument.Status == InstrumentStatus.Requested,
        esign = instrument.ToESignResponse(),

        transaction = ToTransactionResponse(instrument)
      };
    }


    static private object ToESignResponse(this LegalInstrument instrument) {
      if (instrument.ElectronicSign == String.Empty) {
        return new { };
      }

      return new {
        hash = instrument.UID.ToString().Substring(0, 8).ToUpperInvariant(),
        seal = instrument.GetElectronicSeal(),
        sign = instrument.ElectronicSign
      };
     }


    static private object ToTransactionResponse(this LegalInstrument instrument) {
      if (instrument.TransactionUID.Length == 0) {
        return new {
          id = 0,
          uid = "",
          status = "",
          sendTo = "",
          rfc = "",
          receiptNo = "",
          presentationDate = "",
        };
      }

      var transaction = LRSTransaction.TryParse(instrument.TransactionUID, true);

      return new {
        id = transaction.Id,
        uid = transaction.UID,
        status = transaction.Workflow.CurrentStatus,
        sendTo = transaction.ExtensionData.SendTo.Address,
        rfc = transaction.ExtensionData.RFC,
        receiptNo = transaction.PaymentOrderData.PaymentReference,
        presentationDate = transaction.PresentationTime

      };
    }

    #endregion Response models

  }  // class InstrumentModels

}  // namespace Empiria.Land.WebApi.Extranet
