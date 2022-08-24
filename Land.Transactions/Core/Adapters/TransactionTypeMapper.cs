/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transactions Management                    Component : Interface adapters                      *
*  Assembly : Empiria.Land.Transactions.dll              Pattern   : Mapper class                            *
*  Type     : TransactionTypeMapper                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Mapping methods from TransactionType instances to TransactionTypeDto models.                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Transactions.Adapters {

  /// <summary>Mapping methods from TransactionType instances to TransactionTypeDto models.</summary>
  static internal class TransactionTypeMapper {

    static internal FixedList<TransactionTypeDto> Map(FixedList<LRSTransactionType> list) {
      return new FixedList<TransactionTypeDto>(list.Select((x) => Map(x)));
    }


    static internal TransactionTypeDto Map(LRSTransactionType transactionType) {
      var subtypes = transactionType.GetDocumentTypes();

      var dto = new TransactionTypeDto {
        UID = transactionType.UID,
        Name = transactionType.Name,
        Subtypes = subtypes.MapToNamedEntityArray(),
      };

      return dto;
    }

  }  // class TransactionTypeMapper

}  // namespace Empiria.Land.Transactions.Adapters
