/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Mapper class                            *
*  Type     : TransactionMapper                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains methods to map from LRSTransaction objects to TransactionDTOs.                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.DataTypes;

using Empiria.Land.Instruments.Adapters;

using Empiria.Land.Media;
using Empiria.Land.Media.Adapters;

using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Transactions.Adapters {

  /// <summary>Contains methods to map from LRSTransaction objects to TransactionDTOs.</summary>
  static internal class TransactionMapper {

    static internal FixedList<TransactionDto> Map(FixedList<LRSTransaction> list) {
      return new FixedList<TransactionDto>(list.Select((x) => Map(x)));
    }

    static internal TransactionDto Map(LRSTransaction transaction) {
      var currentTask = transaction.Workflow.GetCurrentTask();

      var dto = new TransactionDto();

      dto.UID = transaction.UID;
      dto.TransactionID = transaction.UID;
      dto.Type = transaction.TransactionType.MapToNamedEntity();
      dto.Subtype = transaction.DocumentType.MapToNamedEntity();
      dto.RequestedBy = GetRequestedByDto(transaction);
      dto.PresentationTime = transaction.PresentationTime;
      dto.InternalControlNo = transaction.InternalControlNoFormatted;
      dto.Agency = transaction.Agency.MapToNamedEntity();
      dto.FilingOffice = transaction.RecorderOffice.MapToNamedEntity();
      dto.InstrumentDescriptor = transaction.DocumentDescriptor;
      dto.RequestedServices = GetRequestedServicesDtoArray(transaction);
      dto.ControlVoucher = GetControlVoucherDto(transaction);
      dto.PaymentOrder = GetPaymentOrderDto(transaction);
      dto.Payment = GetPaymentDto(transaction);
      dto.SubmissionReceipt = GetSubmissionReceiptDto(transaction);
      dto.StatusName = currentTask.CurrentStatusName;
      dto.AssignedTo = currentTask.Responsible.MapToNamedEntity();
      dto.NextStatusName = currentTask.NextStatusName;
      dto.NextAssignedTo = currentTask.NextContact.MapToNamedEntity();
      dto.Actions = GetControlDataDto(transaction);

      return dto;
    }


    static internal TransactionPreprocessingDto Map(TransactionPreprocessingData data) {
      var mediaDto = LandMediaFileMapper.Map(data.TransactionMediaPostings);

      var dto = new TransactionPreprocessingDto();

      dto.Actions.Can.EditInstrument = data.CanEditInstrument;
      dto.Actions.Can.UploadInstrumentFiles = data.CanUploadInstrumentFiles;
      dto.Actions.Can.SetAntecedent = data.CanSetAntecedent;
      dto.Actions.Can.CreateAntecedent = data.CanCreateAntecedent;
      dto.Actions.Can.EditAntecedentRecordingActs = data.CanEditAntecedentRecordingActs;

      dto.Actions.Show.Instrument = data.ShowInstrument;
      dto.Actions.Show.InstrumentFiles = data.ShowInstrumentFiles;
      dto.Actions.Show.Antecedent = data.ShowAntecedent;
      dto.Actions.Show.AntecedentRecordingActs = data.ShowAntecedentRecordingActs;

      dto.Media = mediaDto;
      dto.Instrument = InstrumentMapper.Map(data.Instrument);
      dto.Antecedent = data.Antecedent;
      dto.AntecedentRecordingActs = data.AntecedentRecordingActs;

      return dto;
    }


    static internal FixedList<TransactionDescriptor> MapToDescriptor(FixedList<LRSTransaction> list) {
      var mappedItems = list.Select((x) => MapToDescriptor(x));

      return new FixedList<TransactionDescriptor>(mappedItems);
    }


    static internal TransactionDescriptor MapToDescriptor(LRSTransaction transaction) {
      var dto = new TransactionDescriptor();

      var currentTask = transaction.Workflow.GetCurrentTask();

      dto.UID = transaction.UID;
      dto.TransactionID = transaction.UID;
      dto.Type = transaction.TransactionType.Name;
      dto.Subtype = transaction.DocumentType.Name;
      dto.RequestedBy = transaction.RequestedBy;
      dto.PresentationTime = transaction.PresentationTime;
      dto.InternalControlNo = transaction.InternalControlNoFormatted;
      dto.Status = currentTask.CurrentStatus.ToString();
      dto.StatusName = currentTask.CurrentStatusName;
      dto.AssignedToUID = currentTask.Responsible.UID;
      dto.AssignedToName = currentTask.Responsible.Alias;

      dto.NextStatus = currentTask.NextStatus.ToString();
      dto.NextStatusName = currentTask.NextStatusName;
      dto.NextAssignedToName = currentTask.NextContact.Alias;

      return dto;
    }

    static internal LRSTransactionStatus MapStatus(TransactionStatus currentStatus) {
      return ((LRSTransactionStatus) (char) currentStatus);
    }

    static internal TransactionStatus MapStatus(LRSTransactionStatus currentStatus) {
      return ((TransactionStatus) (char) currentStatus);
    }

    #region Private methods

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

    static private PaymentOrderDto GetPaymentOrderDto(LRSTransaction transaction) {
      if (!transaction.HasPaymentOrder) {
        return null;
      }

      var po = transaction.PaymentOrder;

      return new PaymentOrderDto {
        UID = po.UID,
        DueDate = po.DueDate,
        IssueTime = po.IssueTime,
        Total = po.Total,
        Status = po.Status,
        Media = po.Media
      };
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


    private static MediaData GetSubmissionReceiptDto(LRSTransaction transaction) {
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

    #endregion Private methods

  }  // class TransactionMapper

}  // namespace Empiria.Land.Transactions.Adapters
