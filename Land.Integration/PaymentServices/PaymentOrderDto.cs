/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Payment Services                           Component : Integration Layer                       *
*  Assembly : Empiria.Land.Integration.dll               Pattern   : Data Transfer Object                    *
*  Type     : PaymentOrderDto                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data structure with payment order data returned by an payment service provider.                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

namespace Empiria.Land.Integration.PaymentServices {

  /// <summary>Data structure with payment order data returned by an payment service provider.</summary>
  public class PaymentOrderDto : IPaymentOrder {

    private readonly Dictionary<string, object> _attributes = new Dictionary<string, object>();

    public string UID {
      get; set;
    } = string.Empty;


    public DateTime IssueTime {
      get; set;
    }


    public DateTime DueDate {
      get; set;
    }


    public decimal Total {
      get; set;
    }


    public bool IsEmpty {
      get {
        return string.IsNullOrEmpty(this.UID);
      }
    }


    public IDictionary<string, object> Attributes {
      get {
        return _attributes;
      }
    }


    public void AddAttribute(string key, object value) {
      _attributes.Add(key, value);
    }


    public bool RemoveAttribute(string key) {
      return _attributes.Remove(key);
    }


  }  // class PaymentOrderDto

} // namespace Empiria.Land.Integration.PaymentServices
