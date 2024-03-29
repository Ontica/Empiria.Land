﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Providers                             Component : UniqueID Generator Provider             *
*  Assembly : Empiria.Land.Providers.dll                 Pattern   : Service                                 *
*  Type     : TransactionIDGenerator                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides a service to generate a unique Transaction ID.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Transactions.UseCases;

namespace Empiria.Land.Providers {

  internal class TransactionIDGenerator {

    #region Constructor and fields

    private readonly string _recordingOfficeTag;

    internal TransactionIDGenerator(string recordingOfficeTag) {
      _recordingOfficeTag = recordingOfficeTag;
    }

    #endregion Constructor and fields

    #region Service

    internal string GenerateID(string transactionsIDPrefix) {
      while (true) {
        string generatedID = BuildTransactionID(transactionsIDPrefix);

        if (!ExistsTransactionID(generatedID)) {
          return generatedID;
        }
      }
    }

    #endregion Service

    #region Helpers

    private string BuildTransactionID(string transactionsIDPrefix) {
      string temp = String.Empty;
      int hashCode = 0;
      bool useLetters = false;

      for (int i = 0; i < 5; i++) {
        if (useLetters) {
          temp += EmpiriaMath.GetRandomCharacter(temp);
          temp += EmpiriaMath.GetRandomCharacter(temp);
        } else {
          temp += EmpiriaMath.GetRandomDigit(temp);
          temp += EmpiriaMath.GetRandomDigit(temp);
        }
        hashCode += ((Convert.ToInt32(temp[temp.Length - 2]) +
                      Convert.ToInt32(temp[temp.Length - 1])) % ((int) Math.Pow(i + 1, 2)));
        useLetters = !useLetters;
      }

      temp = $"{transactionsIDPrefix}-{_recordingOfficeTag}-" +
             temp.Substring(0, 5) + "-" +
             temp.Substring(5, 5);

      hashCode = (hashCode * Convert.ToInt32(_recordingOfficeTag[0])) % 49;
      hashCode = (hashCode * Convert.ToInt32(_recordingOfficeTag[1])) % 53;

      return temp + "-" + (hashCode % 10).ToString();
    }


    static private bool ExistsTransactionID(string generatedID) {
      using (var usecase = TransactionUseCases.UseCaseInteractor()) {

        return usecase.ExistsTransactionID(generatedID);
      }
    }

    #endregion Helpers

  }  // class TransactionIDGenerator

}  // namespace Empiria.Land.Providers
