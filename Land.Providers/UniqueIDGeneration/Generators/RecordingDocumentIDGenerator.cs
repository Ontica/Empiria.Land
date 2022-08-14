/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Providers                             Component : UniqueID Generator Provider             *
*  Assembly : Empiria.Land.Providers.dll                 Pattern   : Service                                 *
*  Type     : RecordingDocumentIDGenerator               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides a service to generate a unique RecordingDocument ID.                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Registration;

namespace Empiria.Land.Providers {

  /// <summary>Provides a service to generate a unique RecordingDocument ID.</summary>
  internal class RecordingDocumentIDGenerator {

    #region Constructor and fields

    private readonly string _customerID;

    public RecordingDocumentIDGenerator(string customerID) {
      Assertion.Require(customerID, nameof(customerID));
      Assertion.Require(customerID.Length == 2, "customerID must be two chars long.");

      _customerID = customerID;
    }

    #endregion Constructor and fields

    #region Service

    internal string GenerateID() {
      while (true) {
        string generatedID = BuildRecordingDocumentID();

        if (!ExistsRecordingDocumentID(generatedID)) {
          return generatedID;
        }
      }
    }

    #endregion Service

    #region Helpers

    private string BuildRecordingDocumentID() {
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

      temp = "RP-" + _customerID + "-" + temp.Substring(0, 4) + "-" +
                                         temp.Substring(4, 6) + "-" +
                                         temp.Substring(10, 4);

      hashCode = (hashCode * Convert.ToInt32(_customerID[0])) % 49;
      hashCode = (hashCode * Convert.ToInt32(_customerID[1])) % 53;

      temp += "ABCDEFGHJKMLNPQRSTUVWXYZ".Substring(hashCode % 24, 1);
      temp += "9A8B7C6D5E4F3G2H1JKR".Substring(hashCode % 20, 1);

      return temp;
    }


    static private bool ExistsRecordingDocumentID(string generatedID) {
      var document = RecordingDocument.TryParse(generatedID);

      return (document != null);
    }

    #endregion Helpers

  }  // class RecordingDocumentIDGenerator

}  // namespace Empiria.Land.Providers
