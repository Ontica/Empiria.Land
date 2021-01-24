﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Mapper class                            *
*  Type     : TransactionDtoMapper                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains methods to map from LRSTransaction objects to TransactionDTOs.                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Instruments;
using Empiria.Land.Instruments.Adapters;

using Empiria.Land.RecordableEntities.Adapters;

using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Transactions.Adapters {

  /// <summary>Contains methods to map from LRSTransaction objects to TransactionDTOs.</summary>
  static internal class TransactionDtoMapper {

    static internal FixedList<TransactionDto> Map(FixedList<LRSTransaction> list) {
      return new FixedList<TransactionDto>(list.Select((x) => Map(x)));
    }

    static internal TransactionDto Map(LRSTransaction transaction) {
      var dto = new TransactionDto();

      dto.UID = transaction.UID;
      dto.TransactionID = transaction.UID;
      dto.Type = transaction.TransactionType.MapToNamedEntity();
      dto.Subtype = transaction.DocumentType.MapToNamedEntity();
      dto.RequestedBy = GetRequestedByDto(transaction);
      dto.Agency = transaction.Agency.MapToNamedEntity();
      dto.RecorderOffice = transaction.RecorderOffice.MapToNamedEntity();
      dto.Instrument = GetInstrumentDto(transaction);
      dto.InstrumentDescriptor = transaction.DocumentDescriptor;
      dto.RecordableTarget = GetRecordableTargetDto(transaction);
      dto.RequestedServices = GetRequestedServicesDtoArray(transaction);
      dto.PaymentOrder = GetPaymentOrderDto(transaction);
      dto.Payment = GetPaymentDto(transaction);
      dto.PresentationTime = transaction.PresentationTime;
      dto.Status = transaction.Workflow.CurrentStatus.ToString();
      dto.StatusName = transaction.Workflow.CurrentStatusName;
      dto.Actions = GetControlDataDto(transaction);

      return dto;
    }

    #region Private methods

    static private TransactionControlDataDto GetControlDataDto(LRSTransaction transaction) {
      TransactionControlData controlData = transaction.ControlData;
      TransactionControlDataDto dto = new TransactionControlDataDto();

      dto.Can.Edit = controlData.CanEdit;
      dto.Can.Delete = controlData.CanDelete;
      dto.Can.Submit = controlData.CanSubmit;
      dto.Can.EditServices = controlData.CanEditServices;
      dto.Can.GeneratePaymentOrder = controlData.CanGeneratePaymentOrder;
      dto.Can.CancelPaymentOrder = controlData.CanCancelPaymentOrder;
      dto.Can.EditPaymentReceipt = controlData.CanEditPaymentReceipt;
      dto.Can.UploadDocuments = controlData.CanUploadDocuments;
      dto.Can.EditInstrument = controlData.CanEditInstrument;
      dto.Can.EditRecordingActs = controlData.CanEditRecordingActs;
      dto.Can.EditCertificates = controlData.CanEditCertificates;

      dto.Show.ServiceEditor = controlData.ShowServiceEditor;
      dto.Show.PaymentReceiptEditor = controlData.ShowPaymentReceiptEditor;
      dto.Show.UploadDocumentsTab = controlData.ShowUploadDocumentsTab;
      dto.Show.InstrumentRecordingTab = controlData.ShowInstrumentRecordingTab;
      dto.Show.CertificatesEmissionTab = controlData.ShowCertificatesEmissionTab;

      return dto;
    }


    static private PaymentFields GetPaymentDto(LRSTransaction transaction) {
      if (!transaction.HasPayment) {
        return null;
      }
      var payment = transaction.Payments[0];

      return new PaymentFields {
        ReceiptNo = payment.ReceiptNo,
        Total = payment.ReceiptTotal,
        Status = transaction.PaymentOrder.Status
      };
    }

    static private PaymentOrder GetPaymentOrderDto(LRSTransaction transaction) {
      if (!transaction.HasPaymentOrder) {
        return null;
      }
      return transaction.PaymentOrder;
    }


    static private RequestedServiceDto[] GetRequestedServicesDtoArray(LRSTransaction transaction) {
      var servicesList = transaction.Items;

      RequestedServiceDto[] array = new RequestedServiceDto[servicesList.Count];

      for (int i = 0; i < servicesList.Count; i++) {
        array[i] = GetRequestedServiceDto(servicesList[i]);
      }

      return array;
    }


    static private RequestedServiceDto GetRequestedServiceDto(LRSTransactionItem service) {
      return new RequestedServiceDto {
        UID = service.UID,
        Type = service.TransactionItemType.Name,
        TypeName = service.TransactionItemType.DisplayName,
        TreasuryCode = service.TreasuryCode.FinancialConceptCode,
        LegalBasis = service.TreasuryCode.Name,
        Notes = service.Notes,
        Unit = service.Quantity.Unit.UID,
        UnitName = service.Quantity.Unit.Name,
        Quantity = service.Quantity.Amount,
        TaxableBase = service.OperationValue.Amount,
        Subtotal = service.Fee.Total
      };
    }


    static private RecordableEntityShortModel GetRecordableTargetDto(LRSTransaction transaction) {
      var resource = transaction.BaseResource;

      if (resource.IsEmptyInstance) {
        return null;
      }

      return new RecordableEntityShortModel {
        UID = resource.UID,
        Type = resource.GetEmpiriaType().Name,
        Subtype = "No determinado",
        RecordableID = resource.UID,
        MediaUri = String.Empty
      };
    }


    static private InstrumentDto GetInstrumentDto(LRSTransaction transaction) {
      if (transaction.InstrumentId == -1) {
        return null;
      }

      var instrument = Instrument.Parse(transaction.InstrumentId);

      return InstrumentMapper.Map(instrument);
    }


    static private RequestedByDto GetRequestedByDto(LRSTransaction transaction) {
      return new RequestedByDto {
        Name = transaction.RequestedBy,
        Email = transaction.ExtensionData.SendTo.Address
      };
    }

    #endregion Private methods

  }  // class TransactionDtoMapper

}  // namespace Empiria.Land.Transactions.Adapters
