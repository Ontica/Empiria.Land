/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transactions Management                    Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Input Data Transfer Object              *
*  Type     : TransactionFields                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Input DTO used to create or update transactions.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.Transactions {

  /// <summary>Input DTO used to create or update transactions.</summary>
  public class TransactionFields {

    public string TypeUID {
      get; set;
    } = string.Empty;


    public string SubtypeUID {
      get; set;
    } = string.Empty;


    public string FilingOfficeUID {
      get; set;
    } = string.Empty;


    public string AgencyUID {
      get; set;
    } = string.Empty;


    public string RequestedBy {
      get; set;
    } = string.Empty;


    public string RequestedByEmail {
      get; set;
    } = string.Empty;


    public string BillTo {
      get; set;
    } = string.Empty;


    public string RFC {
      get; set;
    } = string.Empty;


    public string InstrumentDescriptor {
      get; set;
    } = string.Empty;


  }  // class TransactionFields

}  // namespace Empiria.Land.Transactions.Adapters
