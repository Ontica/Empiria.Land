/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Legal Instruments                          Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Transfer Object                    *
*  Type     : InstrumentControlDataDto                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO that holds legal instrument control data flags.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Instruments.Adapters {

  /// <summary>Output DTO that holds legal instrument control data flags.</summary>
  public class InstrumentControlDataDto {

    public InstrumentCanControlData Can {
      get; private set;
    } = new InstrumentCanControlData();


    public InstrumentShowControlData Show {
      get; private set;
    } = new InstrumentShowControlData();

  }  // class InstrumentControlDataDto


  /// <summary>Output DTO that holds the 'Can' part of control data flags.</summary>
  public class InstrumentCanControlData {

    public bool Edit {
      get; internal set;
    }

    public bool Open {
      get; internal set;
    }

    public bool Close {
      get; internal set;
    }

    public bool Delete {
      get; internal set;
    }

    public bool UploadFiles {
      get; internal set;
    }

    public bool EditRecordingActs {
      get; internal set;
    }

    public bool CreatePhysicalRecordings {
      get; internal set;
    }

    public bool DeletePhysicalRecordings {
      get; internal set;
    }

    public bool LinkPhysicalRecordings {
      get; internal set;
    }

    public bool EditPhysicalRecordingActs {
      get; internal set;
    }

  }  // class InstrumentCanControlData



  /// <summary>Output DTO that holds the 'Show' part of control data flags.</summary>
  public class InstrumentShowControlData {

    public bool Files {
      get; internal set;
    }

    public bool RecordingActs {
      get; internal set;
    }

    public bool PhysicalRecordings {
      get; internal set;
    }

    public bool RegistrationStamps {
      get;
      internal set;
    }

  }  // class InstrumentShowControlData

}  // namespace Empiria.Land.Transactions.Adapters
