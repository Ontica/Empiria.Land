/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Registration                          Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Control data class                      *
*  Type     : LandRecordControlData                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides edition and other control data for a legal instrument for the current user.           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Instruments;
using Empiria.Land.Media;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Registration {

  internal class LandRecordControlData {

    static internal readonly bool UseRecordingBookRegistation =
                                        ConfigurationData.Get("UseRecordingBookRegistation", false);

    private readonly RecordingDocument _landRecord;
    private readonly Instrument _instrument;
    private readonly LRSTransaction _transaction;

    private readonly FixedList<BookEntry> _bookEntries;

    private readonly bool _isHistoricRegistration;
    private readonly bool _isNewRegistration;

    internal LandRecordControlData(RecordingDocument landRecord) {
      Assertion.Require(landRecord, nameof(landRecord));

      _landRecord = landRecord;
      _instrument = Instrument.Parse(landRecord.InstrumentId);
      _transaction = landRecord.GetTransaction();

      _bookEntries = BookEntry.GetBookEntriesForLandRecord(_landRecord);

      _isHistoricRegistration = _transaction.IsEmptyInstance;

      _isNewRegistration = _landRecord.IsEmptyInstance || _landRecord.IsNew;
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
        return _transaction.ControlData.CanEditInstrument;
      }
    }


    public bool CanOpen {
      get {
        return this.CanEdit && _landRecord.IsClosed &&
               !_isNewRegistration && !UseRecordingBookRegistation;
      }
    }


    public bool CanClose {
      get {
        return this.CanEdit && !_landRecord.IsClosed &&
               !_isNewRegistration && !UseRecordingBookRegistation &&
               _landRecord.HasRecordingActs;
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
        if (CanOpen) {
          return false;
        }
        return (this.ShowRecordingActs && _transaction.ControlData.CanEditRecordingActs);
      }
    }


    public bool CanCreateRecordingBookEntries {
      get {
        if (!CanEdit) {
          return false;
        }
        return (_bookEntries.Count > 0 || UseRecordingBookRegistation);
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
        return LandMediaReadServices.InstrumentFiles(_instrument).Count != 0;
      }
    }


    public bool ShowRecordingBookEntries {
      get {
        return (_bookEntries.Count > 0 || UseRecordingBookRegistation);
      }
    }


    public bool ShowRecordingActs {
      get {
        return (!UseRecordingBookRegistation &&
                !_isHistoricRegistration &&
                !_transaction.IsEmptyInstance &&
                !_transaction.LandRecord.IsEmptyInstance &&
                _bookEntries.Count == 0
                );
      }
    }

    public bool ShowRegistrationStamps {
      get {
        return _bookEntries.Count > 0 || _landRecord.HasRecordingActs;
      }
    }

  }  // class LandRecordControlData

}  // namespace Empiria.Land.Registration
