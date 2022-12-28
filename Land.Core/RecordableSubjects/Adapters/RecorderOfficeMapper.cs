/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Recordable Subjects                        Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Mapper class                            *
*  Type     : RecorderOfficeMapper                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Mapping methods for recorder offices.                                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Registration;

namespace Empiria.Land.RecordableSubjects.Adapters {

  static internal class RecorderOfficeMapper {

    #region Public methods

    static internal FixedList<RecorderOfficeDto> Map(FixedList<RecorderOffice> list) {
      return new FixedList<RecorderOfficeDto>(list.Select((x) => Map(x)));
    }


    static private RecorderOfficeDto Map(RecorderOffice recorderOffice) {
      return new RecorderOfficeDto {
        UID = recorderOffice.UID,
        Name = recorderOffice.ShortName,
        Municipalities = MapMunicipalities(recorderOffice),
        RecordingSections = MapRecordingSections(recorderOffice)
      };
    }


    #endregion Public methods

    #region Helper methods

    private static FixedList<NamedEntityDto> MapMunicipalities(RecorderOffice recorderOffice) {
      var municipalities = recorderOffice.GetMunicipalities();

      return new FixedList<NamedEntityDto>(municipalities.Select(x => new NamedEntityDto(x.UID, x.Name)));
    }


    private static FixedList<NamedEntityDto> MapRecordingSections(RecorderOffice recorderOffice) {
      var recordingSections = recorderOffice.GetRecordingSections();

      return new FixedList<NamedEntityDto>(recordingSections.Select(x => new NamedEntityDto(x.UID, x.Name)));
    }

    #endregion Helper methods

  }  // class RecorderOfficeMapper

}  // namespace Empiria.Land.RecordableSubjects.Adapters
