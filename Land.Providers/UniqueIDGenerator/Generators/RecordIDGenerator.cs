/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Providers                             Component : UniqueID Generator Provider             *
*  Assembly : Empiria.Land.Providers.dll                 Pattern   : Service                                 *
*  Type     : RecordIDGenerator                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides a service to generate a unique Record ID.                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Registration.UseCases;

namespace Empiria.Land.Providers {

  /// <summary>Provides a service to generate a unique Record ID</summary>
  internal class RecordIDGenerator {

    #region Constructor and fields

    private readonly string _recordingOfficeTag;

    internal RecordIDGenerator(string recordingOfficeTag) {
      _recordingOfficeTag = recordingOfficeTag;
    }

    #endregion Constructor and fields

    #region Service

    internal string GenerateID(string recordIDPrefix) {
      while (true) {
        string generatedID = BuildRecordID(recordIDPrefix);

        if (!ExistsRecordID(generatedID)) {
          return generatedID;
        }
      }
    }

    #endregion Service

    #region Helpers

    private string BuildRecordID(string recordIDPrefix) {
      string temp = String.Empty;
      int hashCode = 0;
      bool useLetters = false;

      for (int i = 0; i < 7; i++) {
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

      temp = $"{recordIDPrefix}-{_recordingOfficeTag}-" +
             temp.Substring(0, 4) + "-" +
             temp.Substring(4, 6) + "-" +
             temp.Substring(10, 4);

      hashCode = (hashCode * Convert.ToInt32(_recordingOfficeTag[0])) % 49;
      hashCode = (hashCode * Convert.ToInt32(_recordingOfficeTag[1])) % 53;

      temp += "ABCDEFGHJKMLNPQRSTUVWXYZ".Substring(hashCode % 24, 1);
      temp += "9A8B7C6D5E4F3G2H1JKR".Substring(hashCode % 20, 1);

      return temp;
    }


    static private bool ExistsRecordID(string generatedID) {
      using (var usecase = InstrumentRecordingUseCases.UseCaseInteractor()) {

        return usecase.ExistsInstrumentRecordingID(generatedID);
      }
    }

    #endregion Helpers

  }  // class RecordIDGenerator

}  // namespace Empiria.Land.Providers
