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
using Empiria.Geography;

using Empiria.Land.Registration;

namespace Empiria.Land.RecordableSubjects.Adapters {

  /// <summary>Methods to map recordable subjects like real estate, associations
  /// and no property subjects.</summary>
  static public class RecordableSubjectsMapper {

    static public RecordableSubjectDto Map(Resource resource) {
      if (resource is RealEstate realEstate) {
        return Map(realEstate);

      } else if (resource is Association association) {
        return Map(association);

      } else if (resource is NoPropertyResource noPropertyResource) {
        return Map(noPropertyResource);

      } else {
        throw Assertion.EnsureNoReachThisCode($"Unrecognized recordable subject type " +
                                              $"'{resource.GetEmpiriaType().NamedKey}'.");
      }
    }

    static internal RecordableSubjectDto Map(Resource resource,
                                             ResourceShapshotData snapshot) {
      if (resource is RealEstate realEstate) {
        return MapRealEstateWithSnapshotData(realEstate, (RealEstateShapshotData) snapshot);

      } else if (resource is Association association) {
        return MapAssociationWithSnapshotData(association, (AssociationShapshotData) snapshot);

      } else if (resource is NoPropertyResource noPropertyResource) {
        return MapNoPropertyResourceWithSnapshotData(noPropertyResource, (NoPropertyShapshotData) snapshot);

      } else {
        throw Assertion.EnsureNoReachThisCode($"Unrecognized recordable subject type " +
                                              $"'{resource.GetEmpiriaType().NamedKey}'.");
      }
    }


    static internal RealEstateDto Map(RealEstate realEstate) {
      var dto = new RealEstateDto();

      MapCommonFields(dto, realEstate);

      dto.CadastralID = realEstate.CadastralKey;
      dto.CadastreLinkingDate = realEstate.CadastreLinkingDate;
      dto.CadastralCardMedia = MediaData.Empty;

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


    #region Private methods

    static private void MapCommonFields(RecordableSubjectDto dto, Resource resource) {
      dto.UID = resource.GUID;
      dto.Type = (RecordableSubjectType) Enum.Parse(typeof(RecordableSubjectType),
                                                    resource.GetEmpiriaType().NamedKey);
      dto.RecorderOffice = resource.RecorderOffice.MapToNamedEntity();
      dto.ElectronicID = resource.UID;
      dto.Kind = resource.Kind;
      dto.Name = resource.Name;
      dto.Description = resource.Description;
      dto.Status = resource.Status.ToString();
    }


    static private NoPropertyDto MapNoPropertyResourceWithSnapshotData(NoPropertyResource noPropertyResource,
                                                                       NoPropertyShapshotData snapshot) {
      return Map(noPropertyResource);
    }


    static private AssociationDto MapAssociationWithSnapshotData(Association association,
                                                                 AssociationShapshotData snapshot) {
      return Map(association);
    }


    static private RealEstateDto MapRealEstateWithSnapshotData(RealEstate realEstate,
                                                               RealEstateShapshotData snapshot) {

      RealEstateDto dto = Map(realEstate);

      dto.CadastralID = snapshot.CadastralKey;
      dto.CadastreLinkingDate = snapshot.CadastreLinkingDate;
      // dto.CadastralCardMedia = MediaData.Empty;
      dto.Kind = snapshot.Kind;

      var municipality = Municipality.Parse(snapshot.MunicipalityId);

      dto.Municipality = new NamedEntityDto(municipality.UID,
                                            municipality.Name);

      dto.LotSize = snapshot.LotSize;
      dto.LotSizeUnit = Unit.Parse(snapshot.LotSizeUnitId)
                            .MapToNamedEntity();

      dto.Description = snapshot.Description;
      dto.MetesAndBounds = snapshot.MetesAndBounds;

      return dto;
    }

    #endregion Private methods

  }  // class RecordableSubjectsMapper

}  // namespace Empiria.Land.RecordableSubjects.Adapters
