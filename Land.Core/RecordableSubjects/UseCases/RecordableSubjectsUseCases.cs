/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Recordable Subjects                        Component : Use cases Layer                         *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Use case interactor class               *
*  Type     : RecordableSubjectsUseCases                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for get real estate data and their related information.                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.Land.Registration;

using Empiria.Land.RecordableSubjects.Adapters;

namespace Empiria.Land.RecordableSubjects.UseCases {

  /// <summary>Use cases for get real estate data and their related information.</summary>
  public partial class RecordableSubjectsUseCases : UseCase {

    #region Constructors and parsers

    protected RecordableSubjectsUseCases() {
      // no-op
    }

    static public RecordableSubjectsUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<RecordableSubjectsUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases


    public FixedList<string> AssociationKinds() {
      return Association.AssociationKinds();
    }


    public FixedList<string> NoPropertyKinds() {
      return NoPropertyResource.NoPropertyKinds();
    }


    public FixedList<string> RealEstateKinds() {
      return RealEstate.RealEstateKinds();
    }


    public FixedList<string> RealEstatePartitionKinds() {
      return RealEstate.RealEstatePartitionKinds();
    }


    public FixedList<NamedEntityDto> RealEstateLotSizeUnits() {
      var units = RealEstate.LotSizeUnits();

      return new FixedList<NamedEntityDto>(units.Select(x => x.MapToNamedEntity()));
    }


    public FixedList<RecorderOfficeDto> RecorderOffices() {
      var recorderOffices = RecorderOffice.GetList();

      return RecorderOfficeMapper.Map(recorderOffices);
    }

    public FixedList<RecordableSubjectShortDto> SearchRecordableSubjects(SearchRecordableSubjectsCommand searchCommand) {
      Assertion.AssertObject(searchCommand, "searchCommand");

      searchCommand.EnsureIsValid();

      string filter = searchCommand.MapToFilterString();
      string sort = searchCommand.MapToSortString();

      switch (searchCommand.Type) {
        case RecordableSubjectType.Association:
          var associations = Resource.GetList<Association>(filter, sort, searchCommand.PageSize);

          return RecordableSubjectsMapper.Map(associations);

        case RecordableSubjectType.RealEstate:
          var realEstates = Resource.GetList<RealEstate>(filter, sort, searchCommand.PageSize);

          return RecordableSubjectsMapper.Map(realEstates);

        case RecordableSubjectType.NoProperty:
          var noProperties = Resource.GetList<NoPropertyResource>(filter, sort, searchCommand.PageSize);

          return RecordableSubjectsMapper.Map(noProperties);

        default:
          var list = Resource.GetList<Resource>(filter, sort, searchCommand.PageSize);

          return RecordableSubjectsMapper.Map(list);
      }
    }

    #endregion Use cases

  }  // class RecordableSubjectsUseCases

}  // namespace Empiria.Land.RecordableSubjects.UseCases
