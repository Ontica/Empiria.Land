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

    static internal readonly bool UseRecordingBookRegistation =
                                        ConfigurationData.Get("UseRecordingBookRegistation", false);

    private readonly RecordingDocument _instrumentRecording;
    private readonly Instrument _instrument;
    private readonly TransactionControlData _transactionControlData;
    private readonly bool _isHistoricRegistration;

    internal InstrumentRecordingControlData(RecordingDocument instrumentRecording,
                                            Instrument instrument,
                                            LRSTransaction transaction) {
      _instrumentRecording = instrumentRecording;
      _instrument = instrument;

      if (transaction == null) {
        transaction = instrument.GetTransaction();
      }

      _transactionControlData = transaction.ControlData;

      _isHistoricRegistration = transaction.IsEmptyInstance;
    }


    public bool CanDelete {
      get {
        return false;
      }
    }


    public bool CanEdit {
      get {
        if (_isHistoricRegistration) {
          return true;
        }
        return _transactionControlData.CanEditInstrument;
      }
    }


    public bool CanOpen {
      get {
        return this.CanEdit && _instrumentRecording.IsClosed &&
               !UseRecordingBookRegistation;
      }
    }


    public bool CanClose {
      get {
        return this.CanEdit && !_instrumentRecording.IsClosed &&
               !UseRecordingBookRegistation &&
               _instrumentRecording.HasRecordingActs;
      }
    }


    public bool CanMarkAsNoRecordable {
      get {
        return false;
      }
    }


    public bool CanEditRecordingActs {
      get {
        if (_isHistoricRegistration) {
          return true;
        }
        return (this.ShowRecordingActs && _transactionControlData.CanEditRecordingActs);
      }
    }


    public bool CanCreateRecordingBookEntries {
      get {
        if (!CanEdit) {
          return false;
        }
        return (_instrument.HasRecordingBookEntries || UseRecordingBookRegistation);
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
        return (_instrument.HasRecordingBookEntries || UseRecordingBookRegistation);
      }
    }


    public bool ShowRecordingActs {
      get {
        return (!_isHistoricRegistration &&
                _instrument.HasDocument &&
                !_instrument.GetTransaction().IsEmptyInstance &&
                !_instrument.HasRecordingBookEntries &&
                !UseRecordingBookRegistation);
      }
    }

    public bool ShowRegistrationStamps {
      get {
        return _instrument.HasRecordingBookEntries || _instrumentRecording.HasRecordingActs;
      }
    }

  }  // class InstrumentRecordingControlData

}  // namespace Empiria.Land.Registration
