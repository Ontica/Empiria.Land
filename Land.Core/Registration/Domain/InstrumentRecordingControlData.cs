/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Registration                          Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Control data class                      *
*  Type     : InstrumentRecordingControlData             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides edition and other control data for a legal instrument for the current user.           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Instruments;
using Empiria.Land.Transactions;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Registration {

  internal class InstrumentRecordingControlData {

    private readonly Instrument _instrument;
    private readonly TransactionControlData _transactionControlData;

    internal InstrumentRecordingControlData(Instrument instrument, LRSTransaction transaction) {
      _instrument = instrument;

      if (transaction != null) {
        _transactionControlData = transaction.ControlData;
      } else {
        _transactionControlData = instrument.GetTransaction().ControlData;
      }
    }


    public bool CanDelete {
      get {
        return false;
      }
    }


    public bool CanEdit {
      get {
        return _transactionControlData.CanEditInstrument;
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


    public bool CanEditRecordingActs {
      get {
        return false;
      }
    }


    public bool CanCreateRecordingBookEntries {
      get {
        if (!CanEdit) {
          return false;
        }
        return (_instrument.HasDocument);
      }
    }


    public bool CanDeleteRecordingBookEntries {
      get {
        return CanCreateRecordingBookEntries;
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


    public bool ShowRecordingBookEntries {
      get {
        return (_instrument.HasDocument);
      }
    }


    public bool ShowRecordingActs {
      get {
        return false;
      }
    }

    public bool ShowRegistrationStamps {
      get {
        return _instrument.HasRecordingBookEntries;
      }
    }

  }  // class InstrumentRecordingControlData

}  // namespace Empiria.Land.Registration
