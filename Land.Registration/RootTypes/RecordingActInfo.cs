﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Empiria.Land.Registration {

  public class RecordingActInfo {

    public RecordingActInfo(int recordingActId) {
      Initialize();
      this.RecordingActId = recordingActId;
    }

    public RecordingActInfo(int recordingActTypeId, int physicalBookId,
                            int recordingId = -1, int recordingNo = -1,
                            string recordingSubNo = "", string recordingSuffixTag = "") {
      Initialize();
      this.RecordingActType = RecordingActType.Parse(recordingActTypeId);
      this.PhysicalBook = RecordingBook.Parse(physicalBookId);

      if (recordingId != -1) {
        this.PhysicalRecording = Recording.Parse(recordingId);
      } else if (recordingNo != -1) {
        this.PhysicalRecording = this.PhysicalBook.CreateQuickRecording(recordingNo, recordingSubNo, recordingSuffixTag);
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