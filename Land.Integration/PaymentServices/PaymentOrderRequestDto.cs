/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Payment Services                           Component : Integration Layer                       *
*  Assembly : Empiria.Land.Integration.dll               Pattern   : Data Transfer Object                    *
*  Type     : PaymentOrderRequestDto                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : DTO used to request the issuing of a payment order.                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.Integration.PaymentServices {

  /// <summary>DTO used to request the issuing of a payment order.</summary>
  public class PaymentOrderRequestDto {

    public string BaseTransactionUID {
      get; set;
    } = string.Empty;


    public string RequestedBy {
      get; set;
    } = string.Empty;


    public string Address {
      get; set;
    } = string.Empty;


    public string RFC {
      get; set;
    } = string.Empty;


    public PaymentOrderRequestConceptDto[] Concepts {
      get; set;
    } = new PaymentOrderRequestConceptDto[0];


  }  // class PaymentOrderRequestDto

}  // namespace Empiria.Land.Integration.PaymentServices
