﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transactions Management                    Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Mapper class                            *
*  Type     : TransactionMapper                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains methods to map from LRSTransaction objects to TransactionDTOs.                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Storage;

using Empiria.Land.Media;
using Empiria.Land.Media.Adapters;

using Empiria.Land.Transactions.Payments.Adapters;

namespace Empiria.Land.Transactions.Adapters {

  /// <summary>Contains methods to map from LRSTransaction objects to TransactionDTOs.</summary>
  static public class TransactionMapper {

    static internal FixedList<TransactionDto> Map(FixedList<LRSTransaction> list) {
      return list.Select((x) => Map(x))
                 .ToFixedList();
    }

    static internal TransactionDto Map(LRSTransaction transaction) {
      var currentTask = transaction.Workflow.GetCurrentTask();

      return new TransactionDto {
        UID = transaction.UID,
        TransactionID = transaction.UID,
        Type = transaction.TransactionType.MapToNamedEntity(),
        Subtype = transaction.DocumentType.MapToNamedEntity(),
        RequestedBy = GetRequestedByDto(transaction),
        PresentationTime = transaction.PresentationTime,
        RegistrationTime = transaction.LandRecord.AuthorizationTime,
        InternalControlNo = transaction.InternalControlNumberFormatted,
        Agency = transaction.Agency.MapToNamedEntity(),
        FilingOffice = transaction.RecorderOffice.MapToNamedEntity(),
        InstrumentDescriptor = transaction.DocumentDescriptor,
        RequestedServices = GetRequestedServicesDtoArray(transaction),
        ControlVoucher = GetControlVoucherDto(transaction),
        PaymentOrder = TransactionPaymentsMapper.GetPaymentOrderDto(transaction),
        Payment = TransactionPaymentsMapper.GetPaymentDto(transaction),
        Billing = TransactionPaymentsMapper.GetBillingDto(transaction),
        SubmissionReceipt = GetSubmissionReceiptDto(transaction),
        StatusName = currentTask.CurrentStatusName,
        AssignedTo = currentTask.Responsible.MapToNamedEntity(),
        NextStatusName = currentTask.NextStatusName,
        NextAssignedTo = currentTask.NextContact.MapToNamedEntity(),
        Actions = GetControlDataDto(transaction)
      };
    }


    static public FixedList<TransactionDescriptor> MapToDescriptor(FixedList<LRSTransaction> list) {
      return list.Select((x) => MapToDescriptor(x))
                 .ToFixedList();
    }


    static internal TransactionDescriptor MapToDescriptor(LRSTransaction transaction) {
      var currentTask = transaction.Workflow.GetCurrentTask();

      return new TransactionDescriptor {
        UID = transaction.UID,
        TransactionID = transaction.UID,
        Type = transaction.TransactionType.Name,
        Subtype = transaction.DocumentType.Name,
        RequestedBy = transaction.RequestedBy,
        PresentationTime = transaction.PresentationTime,
        RegistrationTime = transaction.LandRecord.AuthorizationTime,
        InternalControlNo = transaction.InternalControlNumberFormatted,
        Status = currentTask.CurrentStatus.ToString(),
        StatusName = currentTask.CurrentStatusName,
        AssignedToUID = currentTask.Responsible.UID,
        AssignedToName = currentTask.Responsible.ShortName,
        NextStatus = currentTask.NextStatus.ToString(),
        NextStatusName = currentTask.NextStatusName,
        NextAssignedToName = currentTask.NextContact.ShortName
      };
    }

    #region Helpers

    static private TransactionControlDataDto GetControlDataDto(LRSTransaction transaction) {
      TransactionControlData controlData = transaction.ControlData;

      TransactionControlDataDto dto = new TransactionControlDataDto();

      dto.Can.Edit = controlData.CanEdit;
      dto.Can.Delete = controlData.CanDelete;
      dto.Can.Submit = controlData.CanSubmit;

      dto.Can.PrintSubmissionReceipt = controlData.CanPrintSubmissionReceipt;
      dto.Can.PrintControlVoucher = controlData.CanPrintControlVoucher;

      dto.Can.EditServices = controlData.CanEditServices;
      dto.Can.GeneratePaymentOrder = controlData.CanGeneratePaymentOrder;
      dto.Can.CancelPaymentOrder = controlData.CanCancelPaymentOrder;
      dto.Can.EditPayment = controlData.CanEditPayment;
      dto.Can.CancelPayment = controlData.CanCancelPayment;
      dto.Can.UploadDocuments = controlData.CanUploadDocuments;

      dto.Show.ServiceEditor = controlData.ShowServiceEditor;
      dto.Show.PaymentReceiptEditor = controlData.ShowPaymentReceiptEditor;
      dto.Show.PreprocessingTab = controlData.ShowPreprocessingTab;
      dto.Show.InstrumentRecordingTab = controlData.ShowInstrumentRecordingTab;
      dto.Show.CertificatesEmissionTab = controlData.ShowCertificatesEmissionTab;

      return dto;
    }


    static private MediaData GetControlVoucherDto(LRSTransaction transaction) {
      if (!transaction.ControlData.CanPrintControlVoucher) {
        return null;
      }

      var mediaBuilder = new LandMediaBuilder();

      return mediaBuilder.GetMediaDto(LandMediaContent.TransactionControlVoucher, transaction.UID);
    }


    static private RequestedServiceDto[] GetRequestedServicesDtoArray(LRSTransaction transaction) {
      var servicesList = transaction.Services;

      RequestedServiceDto[] array = new RequestedServiceDto[servicesList.Count];

      for (int i = 0; i < servicesList.Count; i++) {
        array[i] = GetRequestedServiceDto(servicesList[i]);
      }

      return array;
    }


    static private RequestedServiceDto GetRequestedServiceDto(LRSTransactionService service) {
      return new RequestedServiceDto {
        UID = service.UID,
        Type = service.ServiceType.Name,
        TypeName = service.ServiceType.DisplayName,
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


    static private MediaData GetSubmissionReceiptDto(LRSTransaction transaction) {
      if (!transaction.ControlData.CanPrintSubmissionReceipt) {
        return null;
      }

      var mediaBuilder = new LandMediaBuilder();

      return mediaBuilder.GetMediaDto(LandMediaContent.TransactionSubmissionReceipt, transaction.UID);
    }


    static private RequestedByDto GetRequestedByDto(LRSTransaction transaction) {
      return new RequestedByDto {
        Name = transaction.RequestedBy,
        Email = transaction.ExtensionData.SendTo.Address
      };
    }

    #endregion Helpers

  }  // class TransactionMapper

}  // namespace Empiria.Land.Transactions.Adapters
