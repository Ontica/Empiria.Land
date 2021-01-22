/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Payment Services                           Component : Integration Layer                       *
*  Assembly : Empiria.Land.Integration.dll               Pattern   : Data Transfer Object                    *
*  Type     : PaymentOrderRequestConceptDto              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : DTO with data related to each concept or service that is part of a payment order.              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.Integration.PaymentServices {

  /// <summary>DTO with data related to each concept or service that is part of a payment order.</summary>
  public class PaymentOrderRequestConceptDto {

    public string ConceptUID {
      get; set;
    } = string.Empty;


    public decimal Quantity {
      get; set;
    } = 1m;


    public decimal TaxableBase {
      get; set;
    } = decimal.Zero;


    public decimal Total {
      get; set;
    } = decimal.Zero;


  }  // class PaymentOrderRequestConceptDto

}  // namespace Empiria.Land.Integration.PaymentServices
