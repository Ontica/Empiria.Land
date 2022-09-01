/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Search services                            Component : Use cases Layer                         *
*  Assembly : Empiria.Land.SearchServices.dll            Pattern   : Use case interactor class               *
*  Type     : RecordableSubjectsSearchUseCases           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for recordable subjects searching: real estate, associations and no-property.        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.Land.Registration;

namespace Empiria.Land.SearchServices.UseCases {

  /// <summary>Use cases for recordable subjects searching: real estate, associations and no-property.</summary>
  public class RecordableSubjectsSearchUseCases : UseCase {

    #region Constructors and parsers

    static public RecordableSubjectsSearchUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<RecordableSubjectsSearchUseCases>();
    }

    #endregion Constructors and parsers

    #region Services

    /// <summary>Internal search service for use by Land Recording offices.</summary>
    public FixedList<RecordableSubjectQueryResultDto> SearchForInternalUse(RecordableSubjectsQuery query) {
      Assertion.Require(query, nameof(query));

      query.EnsureIsValid();

      string filter = query.MapToFilterString();
      string sort = query.MapToSortString();

      var searcher = new RecordableSubjectsSearcher();

      FixedList<Resource> result = searcher.Search(filter, sort, query.PageSize);

      return RecordableSubjectQueryResultMapper.MapForInternalUse(result);
    }

    #endregion Services

  }  // class RecordableSubjectsSearchUseCases

}  // namespace Empiria.Land.SearchServices.UseCases
