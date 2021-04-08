/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Recordable Subjects                        Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Mapper class                            *
*  Type     : RecordableSubjectsMapper                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods to map recordable subjects like real estate, associations and no property subjects.    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.DataTypes;

using Empiria.Land.Registration;

namespace Empiria.Land.RecordableSubjects.Adapters {

  /// <summary>Methods to map recordable subjects like real estate, associations
  /// and no property subjects.</summary>
  static internal class RecordableSubjectsMapper {


    static internal FixedList<RecordableSubjectShortDto> Map<T>(FixedList<T> list) where T: Resource {
      return new FixedList<RecordableSubjectShortDto>(list.Select((x) => MapToShortDto(x)));
    }


    static internal RecordableSubjectDto Map(Resource resource) {
      if (resource is RealEstate realEstate) {
        return Map(realEstate);

      } else if (resource is Association association) {
        return Map(association);

      } else if (resource is NoPropertyResource noPropertyResource) {
        return Map(noPropertyResource);

      } else {
        throw Assertion.AssertNoReachThisCode($"Unrecognized recordable subject type " +
                                              $"'{resource.GetEmpiriaType().NamedKey}'.");
      }
    }


    static internal RealEstateDto Map(RealEstate realEstate) {
      var dto = new RealEstateDto();

      MapCommonFields(dto, realEstate);

      dto.CadastralID = realEstate.CadastralKey;
      dto.CadastreLinkingDate = realEstate.CadastreLinkingDate;
      dto.CadastralCardMedia = MediaData.Empty;
      dto.RecorderOffice = realEstate.RecorderOffice.MapToNamedEntity();
      dto.Municipality = new NamedEntityDto(realEstate.Municipality.UID,
                                            realEstate.Municipality.Name);
      dto.LotSize = realEstate.LotSize.Amount;
      dto.LotSizeUnit = realEstate.LotSize.Unit.MapToNamedEntity();
      dto.Description = realEstate.Description;
      dto.MetesAndBounds = realEstate.MetesAndBounds;

      return dto;
    }


    static internal AssociationDto Map(Association association) {
      var dto = new AssociationDto();

      MapCommonFields(dto, association);

      return dto;
    }


    static internal NoPropertyDto Map(NoPropertyResource noProperty) {
      var dto = new NoPropertyDto();

      MapCommonFields(dto, noProperty);

      return dto;
    }


    static private void MapCommonFields(RecordableSubjectDto dto, Resource resource) {
      dto.UID = resource.GUID;
      dto.Type = (RecordableSubjectType) Enum.Parse(typeof(RecordableSubjectType),
                                                    resource.GetEmpiriaType().NamedKey);
      dto.Name = resource.Name;
      dto.ElectronicID = resource.UID;
      dto.Kind = resource.Kind;
    }


    private static RecordableSubjectShortDto MapToShortDto<T>(T resource) where T : Resource {
      var dto = new RecordableSubjectShortDto();

      dto.UID = resource.GUID;
      dto.Type = (RecordableSubjectType) Enum.Parse(typeof(RecordableSubjectType),
                                                    resource.GetEmpiriaType().NamedKey);

      dto.Name = resource.Name.Length != 0 ? resource.Name : resource.Description;
      dto.ElectronicID = resource.UID;
      dto.Kind = resource.Kind;

      return dto;
    }


  }  // class RecordableSubjectsMapper

}  // namespace Empiria.Land.RecordableSubjects.Adapters
