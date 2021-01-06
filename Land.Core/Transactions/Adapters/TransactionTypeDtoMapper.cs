/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Mapper class                            *
*  Type     : TransactionTypeDtoMapper                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Mapping methods from TransactionType instances to TransactionTypeDto models.                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Transactions.Adapters {

  /// <summary>Mapping methods from TransactionType instances to TransactionTypeDto models.</summary>
  static internal class TransactionTypeDtoMapper {

    static internal FixedList<TransactionTypeDto> Map(FixedList<LRSTransactionType> list) {
      return new FixedList<TransactionTypeDto>(list.Select((x) => Map(x)));
    }


    static internal TransactionTypeDto Map(LRSTransactionType transactionType) {
      var dto = new TransactionTypeDto {
        UID = transactionType.UID,
        Name = transactionType.Name
      };

      return dto;
    }

  }  // class TransactionTypeDtoMapper

}  // namespace Empiria.Land.Transactions.Adapters
