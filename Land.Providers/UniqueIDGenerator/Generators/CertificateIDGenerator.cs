/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Providers                             Component : Integration Layer                       *
*  Assembly : Empiria.Land.Providers.dll                 Pattern   : Service                                 *
*  Type     : CertificateIDGenerator                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides a service to generate a unique Certificate ID.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Certificates.UseCases;

namespace Empiria.Land.Providers {

  /// <summary>Provides a service to generate a unique Certificate ID.</summary>
  internal class CertificateIDGenerator {

    #region Constructor and fields

    private readonly string _recordingOfficeTag;

    internal CertificateIDGenerator(string recordingOfficeTag) {
      _recordingOfficeTag = recordingOfficeTag;
    }

    #endregion Constructor and fields

    #region Service

    internal string GenerateID(string certificatesIDPrefix) {
      while (true) {
        string generatedID = BuildCertificateID(certificatesIDPrefix);

        if (!ExistsCertificateID(generatedID)) {
          return generatedID;
        }
      }
    }

    #endregion Service

    #region Helpers

    private string BuildCertificateID(string certificatesIDPrefix) {
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

      temp = $"{certificatesIDPrefix}-{_recordingOfficeTag}-" +
             temp.Substring(0, 4) + "-" +
             temp.Substring(4, 6) + "-" +
             temp.Substring(10, 4);

      temp += "ABCDEFHJKMNPRTWXYZ".Substring((hashCode * Convert.ToInt32(_recordingOfficeTag[0])) % 17, 1);
      temp += "9A8B7CD5E4F2".Substring((hashCode * Convert.ToInt32(_recordingOfficeTag[1])) % 11, 1);

      return temp;
    }


    static private bool ExistsCertificateID(string generatedID) {
      using (var usecase = CertificatesUseCases.UseCaseInteractor()) {

        return usecase.ExistsCertificateID(generatedID);
      }
    }

    #endregion Helpers

  }  // class CertificateIDGenerator

}  // namespace Empiria.Land.Providers
