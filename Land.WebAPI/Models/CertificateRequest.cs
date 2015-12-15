/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Services                       *
*  Namespace : Empiria.Land.Services                          Assembly : Empiria.Land.Services.dll           *
*  Type      : CertificateRequest                             Pattern  : External Interfacer                 *
*  Version   : 2.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Certificate request from an external transaction system.                                      *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.WebApi.Models {

  /// <summary>Certificate request from an external transaction system.</summary>
  public class CertificateRequest : ExternalTransactionBase {

    /// <summary>The certificate type to be issued.</summary>
    public ExternalCertificateType CertificateType {
      get;
      set;
    } = ExternalCertificateType.Undefined;

    /// <summary>The real property unique ID.</summary>
    public string RealPropertyUID {
      get;
      set;
    } = String.Empty;

    /// <summary>Creates a Land Transaction using the data of this certificate request.</summary>
    /// <returns>The Land Transaction created instance.</returns>
    internal LRSTransaction CreateTransaction() {
      var transaction = new LRSTransaction(LRSTransactionType.Parse(702)) {
        RequestedBy = this.RequestedBy,
      };

      transaction.Save();
      transaction.AddPayment(this.PaymentReceiptNo, this.PaymentAmount);
      transaction.Receive("Citys Certificate Request Test");
      return transaction;
    }

  }  // class CertificateRequest

}  // namespace Empiria.Land.Services
