/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transactions Management                    Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Input Data Tansfer Object               *
*  Type     : RequestedServiceFields                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Input DTO that serves to update transaction requested services.                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.Transactions.Adapters {

  /// <summary>Input DTO that serves to update transaction requested services.</summary>
  public class RequestedServiceFields {

    public string ServiceUID {
      get; set;
    }

    public string FeeConceptUID {
      get; set;
    }

    public string Notes {
      get; set;
    } = string.Empty;


    public decimal TaxableBase {
      get; set;
    } = decimal.Zero;


    public decimal Quantity {
      get; set;
    } = 1.0m;


    public string UnitUID {
      get; set;
    }


    public decimal Subtotal {
      get; set;
    } = decimal.Zero;



    public void AssertValid() {
      Assertion.Require(ServiceUID, nameof(ServiceUID));
      Assertion.Require(FeeConceptUID, nameof(FeeConceptUID));
      Assertion.Require(UnitUID, nameof(UnitUID));

      Assertion.Require(Quantity > 0, "Quantity must be a positive number.");
      Assertion.Require(Subtotal >= 0, "Subtotal must be a non-negative number.");

      this.Notes = this.Notes ?? string.Empty;
    }

  }  // class RequestedServiceFields

} // namespace Empiria.Land.Transactions
