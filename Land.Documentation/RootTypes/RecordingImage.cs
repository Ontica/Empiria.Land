/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Documentation                     Assembly : Empiria.Land.Documentation          *
*  Type      : RecordingImage                                 Pattern  : Empiria Object Type                 *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Represents a recording book/recording document image.                                         *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

using Empiria.Land.Registration;

namespace Empiria.Land.Documentation {

  /// <summary>Represents a recording book/recording document image.</summary>
  public class RecordingImage : DocumentImage {

    #region Constructors and parsers

    internal RecordingImage(RecordingCandidateImage candidateImage) : base(candidateImage) {
      this.PhysicalBook = candidateImage.RecordingBook;
      this.PhysicalRecording = candidateImage.Recording;
      this.RecordingNo = candidateImage.RecordingNo;
    }

    #endregion Constructors and parsers

    #region Public properties

    [DataField("PhysicalBookId")]
    public RecordingBook PhysicalBook {
      get;
      private set;
    }

    [DataField("PhysicalRecordingId")]
    public Recording PhysicalRecording {
      get;
      private set;
    }

    [DataField("RecordingNo")]
    public string RecordingNo {
      get;
      private set;
    }

    #endregion Public properties

    #region Public methods

    protected override void OnSave() {
      DataServices.WriteImagingItem(this);
    }

    #endregion Public methods

  }  // class RecordingImage

}  // namespace Empiria.Land.Documentation
