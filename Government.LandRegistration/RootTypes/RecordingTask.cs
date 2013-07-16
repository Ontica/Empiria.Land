using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Empiria.Government.LandRegistration.Transactions;

namespace Empiria.Government.LandRegistration {

  public class RecordingTask {

    #region Constructors and parsers

    public RecordingTask(int transactionId = -1, int documentId = -1,
                         int recordingId = -1, int recorderOfficeId = -1,
                         int recordingActTypeCategoryId = -1, int recordingActTypeId = -1,
                         string propertyType = "", int precedentRecordingBookId = -1,
                         int precedentRecordingId = -1, int precedentPropertyId = -1,
                         int targetRecordingActId = -1, int quickAddRecordingNumber = -1,
                         string quickAddBisRecordingSuffixTag = "") {

      this.Transaction = LRSTransaction.Parse(transactionId);
      this.Document = RecordingDocument.Parse(documentId);
      this.Recording = Recording.Parse(recordingId);
      this.RecorderOffice = RecorderOffice.Parse(recorderOfficeId);
      this.RecordingActTypeCategory = RecordingActTypeCategory.Parse(recordingActTypeCategoryId);
      this.RecordingActType = RecordingActType.Parse(recordingActTypeId);

      this.PropertyType = propertyType;
      this.PrecedentRecordingBook = RecordingBook.Parse(precedentRecordingBookId);
      this.PrecedentRecording = Recording.Parse(precedentRecordingId);
      this.PrecedentProperty = Property.Parse(precedentPropertyId);

      this.TargetRecordingAct = RecordingAct.Parse(targetRecordingActId);
      this.QuickAddRecordingNumber = quickAddRecordingNumber;
      this.QuickAddBisRecordingSuffixTag = quickAddBisRecordingSuffixTag;
    }

    #endregion Constructors and parsers

    #region Properties

    public LRSTransaction Transaction { get; set; }
    public RecordingDocument Document { get; set; }
    public Recording Recording { get; set; }
    public RecorderOffice RecorderOffice { get; set; }
    public RecordingActTypeCategory RecordingActTypeCategory { get; set; }
    public RecordingActType RecordingActType { get; set; }

    public string PropertyType { get; set; }
    public RecordingBook PrecedentRecordingBook { get; set; }
    public Recording PrecedentRecording { get; set; }
    public Property PrecedentProperty { get; set; }

    public RecordingAct TargetRecordingAct { get; set; }
    public int QuickAddRecordingNumber { get; set; }
    public string QuickAddBisRecordingSuffixTag { get; set; }

    #endregion Properties

    #region Methods

    public RecordingAct DoRecording() {
      throw new EntryPointNotFoundException();
    }

    public void Validate() {
      throw new EntryPointNotFoundException();
    }

    #endregion Methods

  }  // class RecordingTask

}  // namespace Empiria.Government.LandRegistration