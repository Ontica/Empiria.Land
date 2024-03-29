﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Providers                             Component : UniqueID Generator Provider             *
*  Assembly : Empiria.Land.Providers.dll                 Pattern   : Service                                 *
*  Type     : RecordableSubjectIDGenerator               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides services to generate unique IDs for recordable subjects.                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.RecordableSubjects.UseCases;

namespace Empiria.Land.Providers {

  /// <summary>Provides services to generate unique IDs for recordable subjects.</summary>
  internal class RecordableSubjectIDGenerator {

    #region Constructor and fields

    private readonly string _recordingOfficeTag;

    internal RecordableSubjectIDGenerator(string recordingOfficeTag) {
      _recordingOfficeTag = recordingOfficeTag;
    }

    #endregion Constructor and fields

    #region Services

    internal string GenerateAssociationID(string associationsIDPrefix) {
      while (true) {
        string generatedID = BuildAssociationID(associationsIDPrefix);

        if (!ExistsRecordableSubjectID(generatedID)) {
          return generatedID;
        }
      }
    }


    internal string GenerateNoPropertyID(string noPropertiesIDPrefix) {
      while (true) {
        string generatedID = BuildNoPropertyID(noPropertiesIDPrefix);

        if (!ExistsRecordableSubjectID(generatedID)) {
          return generatedID;
        }
      }
    }


    internal string GenerateRealEstateID(string realEstatesIDPrefix) {
      while (true) {
        string generatedID = BuildRealEstateID(realEstatesIDPrefix);

        if (!ExistsRecordableSubjectID(generatedID)) {
          return generatedID;
        }
      }
    }

    #endregion Services

    #region Helpers

    private string BuildAssociationID(string associationsIDPrefix) {
      string temp = $"{associationsIDPrefix}-{_recordingOfficeTag}-";

      temp += EmpiriaMath.GetRandomDigit(temp);
      temp += EmpiriaMath.GetRandomCharacter(temp);
      temp += EmpiriaMath.GetRandomDigit(temp);
      temp += EmpiriaMath.GetRandomDigitOrCharacter(temp);
      temp += "-";
      temp += EmpiriaMath.GetRandomDigit(temp);
      temp += EmpiriaMath.GetRandomDigitOrCharacter(temp);
      temp += EmpiriaMath.GetRandomDigit(temp);

      int hashCode = GenerateHashCode(temp);

      temp += GetChecksumCharacterCode(hashCode);

      return temp;
    }


    private string BuildNoPropertyID(string noPropertiesIDPrefix) {
      string temp = $"{noPropertiesIDPrefix}-{_recordingOfficeTag}-";

      temp += EmpiriaMath.GetRandomDigit(temp);
      temp += EmpiriaMath.GetRandomCharacter(temp);
      temp += EmpiriaMath.GetRandomDigit(temp);
      temp += EmpiriaMath.GetRandomDigitOrCharacter(temp);
      temp += "-";
      temp += EmpiriaMath.GetRandomDigit(temp);
      temp += EmpiriaMath.GetRandomDigitOrCharacter(temp);
      temp += EmpiriaMath.GetRandomDigit(temp);

      int hashCode = GenerateHashCode(temp);

      temp += GetChecksumCharacterCode(hashCode);

      return temp;
    }


    private string BuildRealEstateID(string realEstatesIDPrefix) {
      string temp = $"{realEstatesIDPrefix}-{_recordingOfficeTag}-";

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

      int hashCode = GenerateHashCode(temp);

      temp += GetChecksumCharacterCode(hashCode);

      return temp;
    }


    static private bool ExistsRecordableSubjectID(string generatedID) {
      using (var usecase = RecordableSubjectsUseCases.UseCaseInteractor()) {

        return usecase.ExistsRecordableSubjectID(generatedID);
      }
    }


    static private int GenerateHashCode(string source) {
      int hashCode = 0;

      for (int i = 0; i < source.Length; i++) {
        hashCode += (Convert.ToInt32(source[i]) + Convert.ToInt32(i == 0 ? 0 : source[i - 1])) * (i + 1);
      }
      return hashCode;
    }


    static private string GetChecksumCharacterCode(int hashCode) {
      const string hashCodeConvertionRule = "FSKB8VANXM1TUCR9PG5WLZEH24QYD73J";

      return hashCodeConvertionRule.Substring(hashCode % hashCodeConvertionRule.Length, 1);
    }

    #endregion Helpers

  }  // class RecordableSubjectIDGenerator

}  // namespace Empiria.Land.Providers
