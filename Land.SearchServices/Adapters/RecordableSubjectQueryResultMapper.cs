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

using Empiria.Land.RecordableSubjects.Adapters;

namespace Empiria.Land.SearchServices {

  /// <summary>Methods to map recordable subjects like real estate, associations
  /// and no property subjects.</summary>
  static internal class RecordableSubjectQueryResultMapper {

    #region Mappers

    static internal FixedList<RecordableSubjectQueryResultDto> MapForInternalUse(FixedList<Resource> list) {
      return list.Select((x) => MapForInternalUse(x))
                 .ToFixedList();
    }

    static internal RecordableSubjectQueryResultDto MapForInternalUse(Resource resource) {
      Record record = LandRecordsSearcher.GetLastDomainActRecord(resource);

      return new RecordableSubjectQueryResultDto {
        UID = resource.GUID,
        Type = resource.GetEmpiriaType().NamedKey,
        Name = resource.AsText,
        ElectronicID = resource.UID,
        Kind = resource.Kind,
        Status = resource.Status.ToString(),
        RecordableSubject = RecordableSubjectsMapper.Map(resource),
        Record = RecordMapper.Map(record)
      };
    }

    #endregion Mappers

  }  // class RecordableSubjectQueryResultMapper

}  // namespace Empiria.Land.SearchServices
