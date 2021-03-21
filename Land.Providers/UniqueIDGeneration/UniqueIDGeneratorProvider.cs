/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Core                                  Component : Integration Layer                       *
*  Assembly : Empiria.Land.Providers.dll                 Pattern   : Provider implementation                 *
*  Type     : UniqueIDGeneratorProvider                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides unique ID generation services for land-related documents and objects.                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Certification;
using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Providers {

  /// <summary>Provides unique ID generation services for land-related documents and objects.</summary>
  public class UniqueIDGeneratorProvider : IUniqueIDGeneratorProvider {

    #region Fields

    private static readonly string PREFIX = ExecutionServer.LicenseName == "Zacatecas" ? "ZS" : "TL";

    #endregion Fields

    #region Public methods

    public string GenerateAssociationUID() {
      while (true) {
        string newAssociationUID = UniqueIDGeneratorProvider.CreateAssociationUID();

        var checkIfExistAssociation = Resource.TryParseWithUID(newAssociationUID);

        if (checkIfExistAssociation == null) {
          return newAssociationUID;
        }
      }
    }


    public string GenerateCertificateUID() {
      while (true) {
        string newCertificateUID = UniqueIDGeneratorProvider.CreateCertificateUID();

        var checkIfExistCertificate = Certificate.TryParse(newCertificateUID);

        if (checkIfExistCertificate == null) {
          return newCertificateUID;
        }
      }
    }


    public string GenerateDocumentUID() {
      while (true) {
        string newDocumentUID = UniqueIDGeneratorProvider.CreateDocumentUID();

        var checkIfExistDocument = RecordingDocument.TryParse(newDocumentUID);

        if (checkIfExistDocument == null) {
          return newDocumentUID;
        }
      }
    }


    public string GenerateNoPropertyResourceUID() {
      while (true) {
        string newNoPropertyUID = UniqueIDGeneratorProvider.CreateNoPropertyResourceUID();

        var checkIfExistNoPropertyResource = Resource.TryParseWithUID(newNoPropertyUID);

        if (checkIfExistNoPropertyResource == null) {
          return newNoPropertyUID;
        }
      }
    }


    public string GeneratePropertyUID() {
      while (true) {
        string newPropertyUID = UniqueIDGeneratorProvider.CreatePropertyUID();

        var checkIfExistProperty = Resource.TryParseWithUID(newPropertyUID);

        if (checkIfExistProperty == null) {
          return newPropertyUID;
        }
      }
    }


    public string GenerateTransactionUID() {
      while (true) {
        string newTransactionUID = UniqueIDGeneratorProvider.CreateTransactionUID();

        var checkIfExistTransaction = LRSTransaction.TryParse(newTransactionUID);

        if (checkIfExistTransaction == null) {
          return newTransactionUID;
        }
      }
    }


    static private string CreateAssociationUID() {
      string temp = "PM-" + PREFIX + "-";

      temp += EmpiriaMath.GetRandomDigit(temp);
      temp += EmpiriaMath.GetRandomCharacter(temp);
      temp += EmpiriaMath.GetRandomDigit(temp);
      temp += EmpiriaMath.GetRandomDigitOrCharacter(temp);
      temp += "-";
      temp += EmpiriaMath.GetRandomDigit(temp);
      temp += EmpiriaMath.GetRandomDigitOrCharacter(temp);
      temp += EmpiriaMath.GetRandomDigit(temp);

      int hashCode = 0;
      for (int i = 0; i < temp.Length; i++) {
        hashCode += (Convert.ToInt32(temp[i]) + Convert.ToInt32(i == 0 ? 0 : temp[i - 1])) * (i + 1);
      }
      temp += GetChecksumCharacterCode(hashCode);

      return temp;
    }


    static private string CreateCertificateUID() {
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
      string prefix = PREFIX;
      temp = "CE-" + PREFIX + "-" + temp.Substring(0, 4) + "-" + temp.Substring(4, 6) + "-" + temp.Substring(10, 4);

      temp += "ABCDEFHJKMNPRTWXYZ".Substring((hashCode * Convert.ToInt32(prefix[0])) % 17, 1);
      temp += "9A8B7CD5E4F2".Substring((hashCode * Convert.ToInt32(prefix[1])) % 11, 1);

      return temp;
    }


    static private string CreateDocumentUID() {
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
      string prefix = PREFIX;
      temp = "RP-" + PREFIX + "-" + temp.Substring(0, 4) + "-" + temp.Substring(4, 6) + "-" + temp.Substring(10, 4);

      hashCode = (hashCode * Convert.ToInt32(prefix[0])) % 49;
      hashCode = (hashCode * Convert.ToInt32(prefix[1])) % 53;

      temp += "ABCDEFGHJKMLNPQRSTUVWXYZ".Substring(hashCode % 24, 1);
      temp += "9A8B7C6D5E4F3G2H1JKR".Substring(hashCode % 20, 1);

      return temp;
    }


    static private string CreateNoPropertyResourceUID() {
      string temp = "RD-" + PREFIX + "-";

      temp += EmpiriaMath.GetRandomDigit(temp);
      temp += EmpiriaMath.GetRandomCharacter(temp);
      temp += EmpiriaMath.GetRandomDigit(temp);
      temp += EmpiriaMath.GetRandomDigitOrCharacter(temp);
      temp += "-";
      temp += EmpiriaMath.GetRandomDigit(temp);
      temp += EmpiriaMath.GetRandomDigitOrCharacter(temp);
      temp += EmpiriaMath.GetRandomDigit(temp);

      int hashCode = 0;
      for (int i = 0; i < temp.Length; i++) {
        hashCode += (Convert.ToInt32(temp[i]) + Convert.ToInt32(i == 0 ? 0 : temp[i - 1])) * (i + 1);
      }
      temp += GetChecksumCharacterCode(hashCode);

      return temp;
    }


    static private string CreatePropertyUID() {
      string temp = "FR-" + PREFIX + "-";

      temp += EmpiriaMath.GetRandomDigit(temp);
      temp += EmpiriaMath.GetRandomDigit(temp);
      temp += EmpiriaMath.GetRandomCharacter(temp);
      temp += EmpiriaMath.GetRandomDigitOrCharacter(temp);
      temp += EmpiriaMath.GetRandomDigit(temp);
      temp += EmpiriaMath.GetRandomDigitOrCharacter(temp);
      temp += "-";
      temp += EmpiriaMath.GetRandomDigit(temp);
      temp += EmpiriaMath.GetRandomCharacter(temp);
      temp += EmpiriaMath.GetRandomDigit(temp);
      temp += EmpiriaMath.GetRandomDigitOrCharacter(temp);
      temp += EmpiriaMath.GetRandomDigit(temp);

      int hashCode = 0;
      for (int i = 0; i < temp.Length; i++) {
        hashCode += (Convert.ToInt32(temp[i]) + Convert.ToInt32(i == 0 ? 0 : temp[i - 1])) * (i + 1);
      }
      temp += GetChecksumCharacterCode(hashCode);

      return temp;
    }


    static private string CreateTransactionUID() {
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
      string prefix = PREFIX;
      temp = "TR-" + PREFIX + "-" + temp.Substring(0, 5) + "-" + temp.Substring(5, 5);
      hashCode = (hashCode * Convert.ToInt32(prefix[0])) % 49;
      hashCode = (hashCode * Convert.ToInt32(prefix[1])) % 53;

      return temp + "-" + (hashCode % 10).ToString();
    }

    #endregion Public methods

    #region Private methods

    static private string GetChecksumCharacterCode(int hashCode) {
      string hashCodeConvertionRule = "FSKB8VANXM1TUCR9PG5WLZEH24QYD73J";

      return hashCodeConvertionRule.Substring(hashCode % hashCodeConvertionRule.Length, 1);
    }

    #endregion Private methods

  }  // class UniqueIDGeneratorProvider

}  //namespace Empiria.Land.Providers
