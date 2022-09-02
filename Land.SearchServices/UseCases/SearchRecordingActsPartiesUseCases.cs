/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Search services                            Component : Use cases Layer                         *
*  Assembly : Empiria.Land.SearchServices.dll            Pattern   : Use case interactor class               *
*  Type     : SearchRecordingActsPartiesUseCases         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for recording acts parties searching.                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.Land.Registration;

namespace Empiria.Land.SearchServices.UseCases {

  /// <summary>Use cases for recording acts parties searching.</summary>
  public class SearchRecordingActsPartiesUseCases : UseCase {

    #region Constructors and parsers

    static public SearchRecordingActsPartiesUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<SearchRecordingActsPartiesUseCases>();
    }

    #endregion Constructors and parsers

    #region Services

    /// <summary>Searches recording acts parties for use by Land Recording offices.</summary>
    public FixedList<RecordingActPartyQueryResultDto> SearchForInternalUse(RecordingActsPartiesQuery query) {
      Assertion.Require(query, nameof(query));

      query.EnsureIsValid();

      string filter = query.MapToFilterString();
      string sort = query.MapToSortString();

      var searcher = new RecordingActsPartiesSearcher();

      FixedList<RecordingActParty> result = searcher.Search(filter, sort, query.PageSize);

      return RecordingActsPartiesQueryResultMapper.MapForInternalUse(result);
    }

    #endregion Services

  }  // class SearchRecordingActsPartiesUseCases

}  // namespace Empiria.Land.SearchServices.UseCases
