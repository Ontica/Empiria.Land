/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transactions Management                    Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Transfer Object                    *
*  Type     : TransactionTypeDto                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO that describes transaction types.                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.Transactions.Adapters {

  /// <summary>Output DTO that describes transaction types.</summary>
  public class TransactionTypeDto {

    public string UID {
      get; internal set;
    }

    public string Name {
      get; internal set;
    }

    public NamedEntityDto[] Subtypes {
      get; internal set;
    }

  }  // class TransactionTypeDto

}  // namespace Empiria.Land.Transactions.Adapters
