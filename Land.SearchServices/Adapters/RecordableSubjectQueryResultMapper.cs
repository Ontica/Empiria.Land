/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Search services                            Component : Interface adapters                      *
*  Assembly : Empiria.Land.SearchServices.dll            Pattern   : Mapper class                            *
*  Type     : RecordableSubjectQueryResultMapper         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods to map recordable subjects like real estate, associations and no property subjects.    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Registration;

namespace Empiria.Land.SearchServices {

  /// <summary>Methods to map recordable subjects like real estate, associations
  /// and no property subjects.</summary>
  static internal class RecordableSubjectQueryResultMapper {

    #region Mappers

    static internal FixedList<RecordableSubjectQueryResultDto> MapForInternalUse(FixedList<Resource> list) {
      return list.Select((x) => MapForInternalUse(x))
                 .ToFixedList();
    }

    #endregion Mappers

    #region Helpers

    static private RecordableSubjectQueryResultDto MapForInternalUse(Resource resource) {
      string resourceName = resource.Name.Length != 0 ?
                                        resource.Name : resource.Description;

      return new RecordableSubjectQueryResultDto {
        UID = resource.GUID,
        Type = resource.GetEmpiriaType().NamedKey,
        Name = resourceName,
        ElectronicID = resource.UID,
        Kind = resource.Kind,
        Status = resource.Status.ToString()
      };
    }

    #endregion Helpers

  }  // class RecordableSubjectQueryResultMapper

}  // namespace Empiria.Land.SearchServices
