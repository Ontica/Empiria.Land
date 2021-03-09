/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Transfer Object                    *
*  Type     : TransactionControlDataDto                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO that holds transaction control data flags.                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Transactions.Adapters {

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
