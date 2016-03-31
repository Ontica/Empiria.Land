using System;

namespace Empiria.Land.Registration {

  public class RecordingActInfo {

    public RecordingActInfo(int recordingActId) {
      Initialize();
      this.RecordingActId = recordingActId;
      this.RecordingActType = RecordingAct.Parse(this.RecordingActId).RecordingActType;
    }

    public RecordingActInfo(int recordingActTypeId, int physicalBookId,
                            int physicalRecordingId = -1, string recordingNumber = "") {
      Initialize();
      this.RecordingActType = RecordingActType.Parse(recordingActTypeId);
      this.PhysicalBook = RecordingBook.Parse(physicalBookId);

      if (physicalRecordingId != -1) {
        this.PhysicalRecording = Recording.Parse(physicalRecordingId);
      } else if (recordingNumber != String.Empty) {
        this.PhysicalRecording = this.PhysicalBook.CreateQuickRecording(recordingNumber);
      } else {
        this.PhysicalRecording = Recording.Empty;
      }
    }

    private void Initialize() {
      this.RecordingActId = -1;
      this.RecordingActType = RecordingActType.Empty;
      this.PhysicalBook = RecordingBook.Empty;
      this.PhysicalRecording = Recording.Empty;
    }

    static public RecordingActInfo Empty {
      get {
        return new RecordingActInfo(-1, -1);
      }
    }

    public bool IsEmptyInstance {
      get {
        return (RecordingActId == -1 &&
                RecordingActType.Equals(RecordingActType.Empty) &&
                PhysicalBook.IsEmptyInstance);
      }
    }

    public int RecordingActId {
      get;
      private set;
    }

    public RecordingActType RecordingActType {
      get;
      private set;
    }

    public RecordingBook PhysicalBook {
      get;
      private set;
    }

    public Recording PhysicalRecording {
      get;
      private set;
    }

  }  // class RecordingActInfo

}  // namespace Empiria.Land.Registration
