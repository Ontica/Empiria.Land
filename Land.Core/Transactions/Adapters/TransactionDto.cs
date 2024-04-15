/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transactions Management                    Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Transfer Object                    *
*  Type     : TransactionDto                             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO that holds full data related to a transaction.                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.DataTypes;

using Empiria.Land.Transactions.Payments.Adapters;

namespace Empiria.Land.Transactions.Adapters {

  /// <summary>Output DTO that holds full data related to a transaction.</summary>
  public class TransactionDto {

    public string UID {
      get; internal set;
    }

    public NamedEntityDto Type {
      get; internal set;
    }

    public NamedEntityDto Subtype {
      get; internal set;
    }

    public string TransactionID {
      get; internal set;
    }

    public RequestedByDto RequestedBy {
      get; internal set;
    }

    public DateTime PresentationTime {
      get; internal set;
    }

    public DateTime RegistrationTime {
      get; internal set;
    }

    public string InternalControlNo {
      get; internal set;
    }

    public NamedEntityDto Agency {
      get; internal set;
    }

    public NamedEntityDto FilingOffice {
      get; internal set;
    }

    public string InstrumentDescriptor {
      get; internal set;
    }

    public RequestedServiceDto[] RequestedServices {
      get; internal set;
    }

    public MediaData ControlVoucher {
      get; internal set;
    }

    public PaymentOrderDto PaymentOrder {
      get; internal set;
    }

    public PaymentDto Payment {
      get; internal set;
    }

    public BillingDto Billing {
      get; internal set;
    }

    public MediaData SubmissionReceipt {
      get; internal set;
    }

    public string StatusName {
      get; internal set;
    }

    public NamedEntityDto AssignedTo {
      get; internal set;
    }

    public string NextStatusName {
      get; internal set;
    }

    public NamedEntityDto NextAssignedTo {
      get; internal set;
    }

    public TransactionControlDataDto Actions {
      get; internal set;
    }

  }  // class TransactionDto



  /// <summary>Output DTO that holds minimal transaction data to be used as list items.</summary>
  public class TransactionDescriptor {

    public string UID {
      get; internal set;
    }

    public string Type {
      get; internal set;
    }

    public string Subtype {
      get; internal set;
    }

    public string TransactionID {
      get;
      internal set;
    }

    public string RequestedBy {
      get; internal set;
    }

    public DateTime PresentationTime {
      get; internal set;
    }

    public DateTime RegistrationTime {
      get; internal set;
    }

    public string InternalControlNo {
      get; internal set;
    }

    public string Status {
      get; internal set;
    }

    public string StatusName {
      get; internal set;
    }

    public string NextStatus {
      get; internal set;
    }

    public string NextStatusName {
      get; internal set;
    }

    public string AssignedToUID {
      get; internal set;
    }

    public string AssignedToName {
      get; internal set;
    }

    public string NextAssignedToName {
      get; internal set;
    }

  }  // class TransactionDescriptor



  /// <summary>Output DTO that holds transaction control data flags.</summary>
  public class TransactionControlDataDto {

    public TransationCanControlData Can {
      get; private set;
    } = new TransationCanControlData();


    public TransationShowControlData Show {
      get; private set;
    } = new TransationShowControlData();

  }  // class TransactionControlDataDto



  /// <summary>Output DTO that holds the 'Can' part of control data flags.</summary>
  public class TransationCanControlData {

    public bool Edit {
      get; internal set;
    }

    public bool Delete {
      get; internal set;
    }

    public bool Submit {
      get; internal set;
    }

    public bool EditServices {
      get; internal set;
    }

    public bool GeneratePaymentOrder {
      get; internal set;
    }

    public bool CancelPaymentOrder {
      get; internal set;
    }

    public bool EditPayment {
      get; internal set;
    }

    public bool CancelPayment {
      get; internal set;
    }

    public bool PrintControlVoucher {
      get; internal set;
    }

    public bool PrintSubmissionReceipt {
      get; internal set;
    }

    public bool UploadDocuments {
      get; internal set;
    }

  }  // class TransationCanControlData



  /// <summary>Output DTO that holds the 'Show' part of control data flags.</summary>
  public class TransationShowControlData {

    public bool ServiceEditor {
      get; internal set;
    }

    public bool PaymentReceiptEditor {
      get; internal set;
    }

    public bool PreprocessingTab {
      get; internal set;
    }

    public bool InstrumentRecordingTab {
      get; internal set;
    }

    public bool CertificatesEmissionTab {
      get; internal set;
    }

  }  // class TransationShowControlData

}  // namespace Empiria.Land.Transactions.Adapters
