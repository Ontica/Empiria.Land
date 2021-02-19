/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Legal Instruments                          Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Control data class                      *
*  Type     : InstrumentControlData                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides edition and other control data for a legal instrument for the current user.           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.Instruments {

  internal class InstrumentControlData {

    private readonly Instrument _instrument;

    internal InstrumentControlData(Instrument instrument) {
      _instrument = instrument;
    }

    public bool CanDelete {
      get {
        return false;
      }
    }


    public bool CanEdit {
      get {
        return true;
      }
    }


    public bool CanOpen {
      get {
        return false;
      }
    }


    public bool CanClose {
      get {
        return false;
      }
    }


    public bool CanMarkAsNoRecordable {
      get {
        return false;
      }
    }


    public bool CanProceedWithRecording {
      get {
        return false;
      }
    }


    public bool CanEditRecordingActs {
      get {
        return false;
      }
    }


    public bool CanCreatePhysicalRecordings {
      get {
        return (_instrument.TryGetRecordingDocument() != null);
      }
    }


    public bool CanDeletePhysicalRecordings {
      get {
        return (_instrument.HasPhysicalRecordings);
      }
    }


    public bool CanLinkPhysicalRecordings {
      get {
        return false;
      }
    }


    public bool CanEditPhysicalRecordingActs {
      get {
        return false;
      }
    }


    public bool CanUploadFiles {
      get {
        return false;
      }
    }


    public bool ShowFiles {
      get {
        return false;
      }
    }


    public bool ShowPhysicalRecordings {
      get {
        return (_instrument.TryGetRecordingDocument() != null);
      }
    }


    public bool ShowRecordingActs {
      get {
        return false;
      }
    }

    public bool ShowRegistrationStamps {
      get {
        return _instrument.HasPhysicalRecordings;
      }
    }

  }  // class InstrumentControlData

}  // namespace Empiria.Land.Instruments
