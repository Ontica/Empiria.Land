/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  System   : Empiria Land                                 Module  : Recording Services                      *
*  Assembly : Empiria.Land.Registration.dll                Pattern : Utility methods                         *
*  Type     : UIDGenerators                                License : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Utility methods used generate entities' unique IDs.                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Data {

  /// <summary>Utility methods used generate entities' unique IDs.</summary>
  static internal class UIDGenerators {

    #region Public methods

    static internal string CreateAssociationUID() {
      string temp = "TL-SC-";

      temp += EmpiriaMath.GetRandomCharacter(temp);
      temp += EmpiriaMath.GetRandomDigit(temp);
      temp += EmpiriaMath.GetRandomCharacter(temp);
      temp += EmpiriaMath.GetRandomDigit(temp);
      temp += EmpiriaMath.GetRandomCharacter(temp);
      temp += EmpiriaMath.GetRandomDigit(temp);
      temp += EmpiriaMath.GetRandomDigit(temp);

      int hashCode = 0;
      for (int i = 0; i < temp.Length; i++) {
        hashCode += (Convert.ToInt32(temp[i]) + Convert.ToInt32(i == 0 ? 0 : temp[i - 1])) * (i + 1);
      }
      temp += GetChecksumCharacterCode(hashCode);

      return temp;
    }


    static internal string CreateCertificateUID() {
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
      string prefix = "TL";
      temp = "CE" + temp.Substring(0, 4) + "-" + temp.Substring(4, 6) + "-" + temp.Substring(10, 4);

      temp += "ABCDEFHJKMNPRTWXYZ".Substring((hashCode * Convert.ToInt32(prefix[0])) % 17, 1);
      temp += "9A8B7CD5E4F2".Substring((hashCode * Convert.ToInt32(prefix[1])) % 11, 1);

      return temp;
    }


    static internal string CreateDocumentUID() {
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
      string prefix = "TL";
      temp = "RP" + temp.Substring(0, 4) + "-" + temp.Substring(4, 6) + "-" + temp.Substring(10, 4);

      hashCode = (hashCode * Convert.ToInt32(prefix[0])) % 49;
      hashCode = (hashCode * Convert.ToInt32(prefix[1])) % 53;

      temp += "ABCDEFGHJKMLNPQRSTUVWXYZ".Substring(hashCode % 24, 1);
      temp += "9A8B7C6D5E4F3G2H1JKR".Substring(hashCode % 20, 1);

      return temp;
    }


    static internal string CreateNoPropertyResourceUID() {
      string temp = "TL-DOC-";

      temp += EmpiriaMath.GetRandomDigit(temp);
      temp += EmpiriaMath.GetRandomCharacter(temp);
      temp += EmpiriaMath.GetRandomDigit(temp);
      temp += EmpiriaMath.GetRandomCharacter(temp);
      temp += EmpiriaMath.GetRandomDigit(temp);
      temp += EmpiriaMath.GetRandomCharacter(temp);
      temp += EmpiriaMath.GetRandomDigit(temp);

      int hashCode = 0;
      for (int i = 0; i < temp.Length; i++) {
        hashCode += (Convert.ToInt32(temp[i]) + Convert.ToInt32(i == 0 ? 0 : temp[i - 1])) * (i + 1);
      }
      temp += GetChecksumCharacterCode(hashCode);

      return temp;
    }


    static internal string CreatePropertyUID() {
      string temp = "TL";
      temp += EmpiriaMath.GetRandomDigit(temp);
      temp += EmpiriaMath.GetRandomDigit(temp);
      temp += EmpiriaMath.GetRandomCharacter(temp);
      temp += EmpiriaMath.GetRandomDigit(temp);
      temp += EmpiriaMath.GetRandomCharacter(temp);
      temp += EmpiriaMath.GetRandomDigit(temp);
      temp += EmpiriaMath.GetRandomCharacter(temp);
      temp += EmpiriaMath.GetRandomDigitOrCharacter(temp);
      temp += EmpiriaMath.GetRandomDigit(temp);
      temp += EmpiriaMath.GetRandomCharacter();
      temp += EmpiriaMath.GetRandomDigit(temp);
      temp += EmpiriaMath.GetRandomCharacter(temp);
      temp += EmpiriaMath.GetRandomDigit(temp);

      int hashCode = 0;
      for (int i = 0; i < temp.Length; i++) {
        hashCode += (Convert.ToInt32(temp[i]) + Convert.ToInt32(i == 0 ? 0 : temp[i - 1])) * (i + 1);
      }
      temp += GetChecksumCharacterCode(hashCode);

      return temp.Substring(0, 4) + "-" + temp.Substring(4, 4) + "-" + temp.Substring(8, 4) + "-" + temp.Substring(12, 4);
    }


    static internal string CreateTransactionUID() {
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
      string prefix = "TL";
      temp = "TR-" + temp.Substring(0, 5) + "-" + temp.Substring(5, 5);
      hashCode = (hashCode * Convert.ToInt32(prefix[0])) % 49;
      hashCode = (hashCode * Convert.ToInt32(prefix[1])) % 53;

      return temp + "-" + (hashCode % 10).ToString();
    }

    #endregion Public methods

    #region Private methods

    static private string GetChecksumCharacterCode(int hashCode) {
      string hashCodeConvertionRule = "NAXMT1C5WZ7J3HE489RLGV6F2PUQKYD0BS";
      return hashCodeConvertionRule.Substring(hashCode % hashCodeConvertionRule.Length, 1);
    }

    #endregion Private methods

  } // class DocumentsData

} // namespace Empiria.Land.Data
