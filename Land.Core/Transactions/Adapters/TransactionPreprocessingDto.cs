/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Transfer Object                    *
*  Type     : TransactionPreprocessingDto                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO with transaction preprocessing status, allowed actions and related entities.        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Land.Instruments.Adapters;
using Empiria.Land.Media.Adapters;

namespace Empiria.Land.Transactions.Adapters {

  /// <summary>Ouput DTO with transaction preprocessing status, allowed actions and related entities.</summary>
  public class TransactionPreprocessingDto {

    public InstrumentDto Instrument {
      get; internal set;
    }

    public object Antecedent {
      get; internal set;
    }

    public object AntecedentRecordingActs {
      get; internal set;
    }

    public FixedList<LandMediaFileDto> Media {
      get; internal set;
    }


    public TransactionPreprocessingActionsDto Actions {
      get; internal set;
    } = new TransactionPreprocessingActionsDto();

  }  // class TransactionPreprocessingDto



  /// <summary>Wraps both 'Can' and 'Show' parts for transaction preprocessing control data flags.</summary>
  public class TransactionPreprocessingActionsDto {

    public TransactionPreprocessingCanFlags Can {
      get; private set;
    } = new TransactionPreprocessingCanFlags();


    public TransactionPreprocessingShowFlags Show {
      get; private set;
    } = new TransactionPreprocessingShowFlags();

  }  // class TransactionPreprocessingActionsDto



  /// <summary>Output DTO that holds the 'Can' part of transaction preprocessing control data flags.</summary>
  public class TransactionPreprocessingCanFlags {

    public bool EditInstrument {
      get; internal set;
    }

    public bool UploadInstrumentFiles {
      get; internal set;
    }

    public bool SetAntecedent {
      get; internal set;
    }

    public bool CreateAntecedent {
      get; internal set;
    }

    public bool EditAntecedentRecordingActs {
      get; internal set;
    }

  }  // class TransactionPreprocessingCanFlags



  /// <summary>Output DTO that holds the 'Show' part of transaction preprocessing control data flags.</summary>
  public class TransactionPreprocessingShowFlags {

    public bool Instrument {
      get; internal set;
    }

    public bool InstrumentFiles {
      get; internal set;
    }

    public bool Antecedent {
      get; internal set;
    }

    public bool AntecedentRecordingActs {
      get; internal set;
    }

  }  //class TransactionPreprocessingShowFlags

}  // namespace Empiria.Land.Transactions.Adapters
