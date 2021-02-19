/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Physical Registration                      Component : Use cases Layer                         *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Use cases class                         *
*  Type     : PhysicalRecordingUseCases                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for registration of documents in physical books.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.Land.Instruments;
using Empiria.Land.Instruments.Adapters;

using Empiria.Land.PhysicalBooks.Adapters;

using Empiria.Land.Registration;

namespace Empiria.Land.PhysicalBooks.UseCases {

  /// <summary>Use cases for registration of documents in physical books.</summary>
  public class PhysicalRecordingUseCases : UseCase {

    #region Constructors and parsers

    protected PhysicalRecordingUseCases() {
      // no-op
    }

    static public PhysicalRecordingUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<PhysicalRecordingUseCases>();
    }


    #endregion Constructors and parsers

    #region Command Use cases

    public InstrumentDto CreateNextPhysicalRecording(string instrumentUID,
                                                     CreateNextPhysicalRecordingFields fields) {
      Assertion.AssertObject(instrumentUID, "instrumentUID");
      Assertion.AssertObject(fields, "fields");

      var instrument = Instrument.Parse(instrumentUID);

      instrument.EnsureHasRecordingDocument();

      var recordingDocument = instrument.TryGetRecordingDocument();

      Assertion.AssertObject(recordingDocument, "recordingDocument");

      var office = RecorderOffice.Parse(fields.RecorderOfficeUID);
      var section = RecordingSection.Parse(fields.SectionUID);

      var book = RecordingBook.GetAssignedBookForRecording(office, section,
                                                           instrument.SheetsCount);

      var physicalRecording = book.CreateNextRecording(recordingDocument);

      physicalRecording.Save();

      return InstrumentMapper.Map(instrument);
    }


    public FixedList<NamedEntityDto> GetRecordingSections() {
      FixedList<RecordingSection> list = RecordingSection.GetListForRecording();

      return list.MapToNamedEntityList();
    }


    public InstrumentDto RemovePhysicalRecording(string instrumentUID, string physicalRecordingUID) {
      Assertion.AssertObject(instrumentUID, "instrumentUID");
      Assertion.AssertObject(physicalRecordingUID, "physicalRecordingUID");

      var instrument = Instrument.Parse(instrumentUID);

      var document = instrument.TryGetRecordingDocument();

      var instrumentRecordings = PhysicalRecording.GetDocumentRecordings(document.Id);

      var physicalRecording = instrumentRecordings.Find(x => x.UID == physicalRecordingUID);

      Assertion.AssertObject(physicalRecording,
            $"Physical recording '{physicalRecordingUID}' does not belong to legal instrument '{instrumentUID}'.");

      physicalRecording.Delete();

      return InstrumentMapper.Map(instrument);
    }

    #endregion Command Use cases

  }  // class PhysicalRecordingUseCases

}  // namespace Empiria.Land.PhysicalBooks.UseCases
