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


    public RecordingActInfoDTO(int recordingActTypeId, int recordingBookId,
                               int bookEntryId = -1, string bookEntryNumber = "") {
      Initialize();

      this.RecordingActType = RecordingActType.Parse(recordingActTypeId);
      this.RecordingBook = RecordingBook.Parse(recordingBookId);

      if (bookEntryId != -1) {
        this.BookEntry = BookEntry.Parse(bookEntryId);

      } else if (bookEntryNumber != String.Empty) {
        this.BookEntry = this.RecordingBook.AddBookEntry(bookEntryNumber);
        this.BookEntryWasCreated = true;

      } else {
        this.BookEntry = BookEntry.Empty;

      }
    }

    private void Initialize() {
      this.RecordingActId = -1;
      this.RecordingActType = RecordingActType.Empty;
      this.RecordingBook = RecordingBook.Empty;
      this.BookEntry = BookEntry.Empty;
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
                RecordingBook.IsEmptyInstance);
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

    public RecordingBook RecordingBook {
      get;
      private set;
    }

    public BookEntry BookEntry {
      get;
      private set;
    }

    public bool BookEntryWasCreated {
      get;
      private set;
    }

  }  // class RecordingActInfoDTO

}  // namespace Empiria.Land.Registration
