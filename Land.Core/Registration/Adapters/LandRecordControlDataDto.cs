/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Registration Services                      Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Transfer Object                    *
*  Type     : LandRecordControlDataDto                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO that holds land records control data flags.                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Registration.Adapters {

  /// <summary>Output DTO that holds land records control data flags.</summary>
  public class LandRecordControlDataDto {

    public LandRecordCanControlData Can {
      get; private set;
    } = new LandRecordCanControlData();


    public LandRecordShowControlData Show {
      get; private set;
    } = new LandRecordShowControlData();

  }  // class LandRecordControlDataDto


  /// <summary>Output DTO that holds the 'Can' part of control data flags.</summary>
  public class LandRecordCanControlData {

    public bool EditInstrument {
      get; internal set;
    }

    public bool OpenInstrument {
      get; internal set;
    }

    public bool CloseInstrument {
      get; internal set;
    }

    public bool DeleteInstrument {
      get; internal set;
    }

    public bool UploadInstrumentFiles {
      get; internal set;
    }

    public bool EditRecordingActs {
      get; internal set;
    }

    public bool CreateNextRecordingBookEntries {
      get; internal set;
    }

    public bool DeleteRecordingBookEntries {
      get; internal set;
    }

  }  // class LandRecordCanControlData



  /// <summary>Output DTO that holds the 'Show' part of control data flags.</summary>
  public class LandRecordShowControlData {

    public bool Files {
      get; internal set;
    }

    public bool RecordingActs {
      get; internal set;
    }

    public bool RecordingBookEntries {
      get; internal set;
    }

    public bool RegistrationStamps {
      get;
      internal set;
    }

  }  // class LandRecordShowControlData

}  // namespace Empiria.Land.Registration.Adapters
