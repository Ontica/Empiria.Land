/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Input Data Holder                       *
*  Type     : RequestedServiceFields                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data structure that serves to update transaction requested services.                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.Transactions.Adapters {

  /// <summary>Data structure that serves to update transaction requested services.</summary>
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
      Assertion.AssertObject(ServiceUID, "ServiceUID");
      Assertion.AssertObject(FeeConceptUID, "FeeConceptUID");
      Assertion.AssertObject(UnitUID, "UnitUID");

      Assertion.Assert(Quantity > 0, "Quantity must be a positive number.");
      Assertion.Assert(Subtotal >= 0, "Subtotal must be a not negative number.");

      this.Notes = this.Notes ?? string.Empty;
    }

  }  // class RequestedServiceFields

} // namespace Empiria.Land.Transactions.Adapters
