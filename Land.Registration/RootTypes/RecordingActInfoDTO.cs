/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingActInfoDTO                            Pattern  : Data Transfer Object                *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Data transfer object used to hold information about a recording act and send it               *
*              to the recorder expert.                                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Data transfer object used to hold information about a recording act
  /// and send it to the recorder expert.</summary>
  public class RecordingActInfoDTO {

    public RecordingActInfoDTO(int recordingActId) {
      Initialize();
      this.RecordingActId = recordingActId;
      this.RecordingActType = RecordingAct.Parse(this.RecordingActId).RecordingActType;
    }


    public RecordingActInfoDTO(string recordingActUID) {
      Initialize();
      var recordingAct = RecordingAct.Parse(recordingActUID);

      this.RecordingActType = recordingAct.RecordingActType;
      this.RecordingActId = recordingAct.Id;
    }


    public RecordingActInfoDTO(int recordingActTypeId, int physicalBookId,
                               int physicalRecordingId = -1, string recordingNumber = "") {
      Initialize();
      this.RecordingActType = RecordingActType.Parse(recordingActTypeId);
      this.PhysicalBook = RecordingBook.Parse(physicalBookId);

      if (physicalRecordingId != -1) {
        this.PhysicalRecording = PhysicalRecording.Parse(physicalRecordingId);
      } else if (recordingNumber != String.Empty) {
        this.PhysicalRecording = this.PhysicalBook.AddBookEntry(recordingNumber);
        this.PhysicalRecordingWasCreated = true;
      } else {
        this.PhysicalRecording = PhysicalRecording.Empty;
      }
    }

    private void Initialize() {
      this.RecordingActId = -1;
      this.RecordingActType = RecordingActType.Empty;
      this.PhysicalBook = RecordingBook.Empty;
      this.PhysicalRecording = PhysicalRecording.Empty;
    }

    static public RecordingActInfoDTO Empty {
      get {
        return new RecordingActInfoDTO(-1, -1);
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

    public PhysicalRecording PhysicalRecording {
      get;
      private set;
    }

    public bool PhysicalRecordingWasCreated {
      get;
      private set;
    }

  }  // class RecordingActInfoDTO

}  // namespace Empiria.Land.Registration
