/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Registration                          Component : Use cases Layer                         *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Use case interactor class               *
*  Type     : RegistrationRulesUseCases                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for get Empiria Land registration rules.                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.Land.Instruments;
using Empiria.Land.Registration.Adapters;

namespace Empiria.Land.Registration.UseCases {

  /// <summary>Use cases for get Empiria Land registration rules.</summary>
  public partial class RegistrationRulesUseCases : UseCase {

    #region Constructors and parsers

    protected RegistrationRulesUseCases() {
      // no-op
    }

    static public RegistrationRulesUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<RegistrationRulesUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public FixedList<RecordingActTypeGroupDto> RecordingActTypesForBookEntry(string recordingBookUID,
                                                                             string bookEntryUID) {
      Assertion.Require(recordingBookUID, nameof(recordingBookUID));
      Assertion.Require(bookEntryUID, nameof(bookEntryUID));

      var book = RecordingBook.Parse(recordingBookUID);

      var bookEntry = BookEntry.Parse(bookEntryUID);

      Assertion.Require(book.BookEntries.Contains(bookEntry),
          $"Book entry '{bookEntry.UID}' does not belong to recording book {book.AsText}");

      ApplicableRecordingActTypeList applicableActTypes = bookEntry.ApplicableRecordingActTypes();

      return RecordingActTypeMapper.Map(applicableActTypes);
    }


    public FixedList<RecordingActTypeGroupDto> RecordingActTypesForInstrument(string instrumentUID) {
      Assertion.Require(instrumentUID, nameof(instrumentUID));

      var instrument = Instrument.Parse(instrumentUID);

      ApplicableRecordingActTypeList applicableActTypes = instrument.ApplicableRecordingActTypes();

      return RecordingActTypeMapper.Map(applicableActTypes);
    }


    public FixedList<RecordingActTypeGroupDto> RecordingActTypesForRecordableSubject(string recordableSubjectUID) {
      Assertion.Require(recordableSubjectUID, nameof(recordableSubjectUID));

      var recordableSubject = Resource.ParseGuid(recordableSubjectUID);

      ApplicableRecordingActTypeList applicableActTypes = recordableSubject.ApplicableRecordingActTypes();

      return RecordingActTypeMapper.Map(applicableActTypes);
    }


    public FixedList<NamedEntityDto> RecordingActTypesList(string listUID) {
      Assertion.Require(listUID, nameof(listUID));

      var category = RecordingActTypeCategory.Parse(listUID);

      var list = category.RecordingActTypes;

      return new FixedList<NamedEntityDto>(list.Select(x => new NamedEntityDto(x.UID, x.DisplayName)));
    }


    #endregion Use cases

  }  // class RegistrationRulesUseCases

}  // namespace Empiria.Land.Registration.UseCases
