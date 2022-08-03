/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Media Files Management                Component : Use cases Layer                         *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Use case interactor class               *
*  Type     : RecordingBookMediaUseCases                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases to upload and manage media files related to Empiria Land entities.                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.Land.Media.Adapters;

using Empiria.Land.Registration;

namespace Empiria.Land.Media.UseCases {

  /// <summary>Use cases to upload media files related to Empiria Land entities.</summary>
  public class RecordingBookMediaUseCases : UseCase {

    #region Constructors and parsers

    protected RecordingBookMediaUseCases() {
      // no-op
    }

    static public RecordingBookMediaUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<RecordingBookMediaUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public FixedList<LandMediaFileDto> GetRecordingBookImages(string recordingBookUID) {
      Assertion.Require(recordingBookUID, "recordingBookUID");

      var book = RecordingBook.Parse(recordingBookUID);

      throw new NotImplementedException();
    }

    #endregion Use cases

  }  // class RecordingBookMediaUseCases

}  // namespace Empiria.Land.Media.UseCases
