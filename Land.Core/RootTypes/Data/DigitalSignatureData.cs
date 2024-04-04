/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Recording services                           Component : Data services                         *
*  Assembly : Empiria.Land.Registration.dll                Pattern   : Data services provider                *
*  Type     : DigitalSignatureData                         License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Provides database read and write methods for digital signature data.                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Contacts;
using Empiria.Data;

using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Data {

  /// <summary>Provides database read and write methods for recording documents.</summary>
  static internal class DigitalSignatureData {

    #region Methods

    static internal bool IsSigned(IIdentifiable document) {
      var sql = $"SELECT * " +
                $"FROM vwLRSDocumentSign " +
                $"WHERE DocumentNo = '{document.UID}' " +
                $"AND SignStatus = 'S' AND DigitalSign <> ''";

      return DataReader.Count(DataOperation.Parse(sql)) == 1;
    }


    static internal string GetDigitalSignature(IIdentifiable document) {
      var sql = $"SELECT DigitalSign " +
                $"FROM vwLRSDocumentSign " +
                $"WHERE DocumentNo = '{document.UID}' " +
                $"AND SignStatus = 'S' AND DigitalSign <> ''";

      return DataReader.GetScalar<string>(DataOperation.Parse(sql),
                                          "NO TIENE FIRMA ELECTRÓNICA.");
    }


    static internal Person GetDigitalSignatureSignedBy(IIdentifiable document) {
      var sql = $"SELECT RequestedToId " +
                $"FROM vwLRSDocumentSign " +
                $"WHERE DocumentNo = '{document.UID}' " +
                $"AND SignStatus = 'S' AND DigitalSign <> ''";

      var signedById = DataReader.GetScalar<int>(DataOperation.Parse(sql), -1);

      return Person.Parse(signedById);
    }


    static internal DateTime GetLastSignTimeForAllTransactionDocuments(LRSTransaction transaction) {
      var sql = $"SELECT MAX(SignTime) " +
                $"FROM vwLRSDocumentSign " +
                $"WHERE TransactionNo = '{transaction.UID}' " +
                $"AND SignStatus = 'S' AND DigitalSign <> ''";

      return DataReader.GetScalar<DateTime>(DataOperation.Parse(sql));
    }

    #endregion Methods

  } // class DigitalSignatureData

} // namespace Empiria.Land.Data
