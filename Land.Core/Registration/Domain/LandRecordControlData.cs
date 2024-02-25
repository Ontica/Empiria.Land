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

using Empiria.Land.Media;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Registration {

  internal class LandRecordControlData {

    static internal readonly bool UseRecordingBookRegistation =
                                        ConfigurationData.Get("UseRecordingBookRegistation", false);

    private readonly LandRecord _landRecord;
    private readonly LRSTransaction _transaction;

    private readonly FixedList<BookEntry> _bookEntries;

    private readonly bool _isHistoricRegistration;
    private readonly bool _isTransactionOnBooksRegistration;

    private readonly bool _isNewRegistration;

    internal LandRecordControlData(LandRecord landRecord) {
      Assertion.Require(landRecord, nameof(landRecord));

      _landRecord = landRecord;
      _transaction = landRecord.Transaction;

      _bookEntries = BookEntry.GetBookEntriesForLandRecord(_landRecord);

      _isHistoricRegistration = _transaction.IsEmptyInstance;

      _isTransactionOnBooksRegistration = !_transaction.IsEmptyInstance && _bookEntries.Count > 0;

      _isNewRegistration = _landRecord.Instrument.IsEmptyInstance || _landRecord.IsEmptyInstance || _landRecord.IsNew;
    }


    public bool CanDelete {
      get {
        return false;
      }
    }


    public bool CanEdit {
      get {
        if (_isNewRegistration) {
          return false;
        }
        if (_isHistoricRegistration && !_landRecord.IsClosed) {
          return true;
        }
        if (_isTransactionOnBooksRegistration && !UseRecordingBookRegistation &&
            !_landRecord.IsClosed) {
          return true;
        }
        return _transaction.ControlData.IsReadyForRecording;
      }
    }


    public bool CanOpen {
      get {
        if (_isNewRegistration) {
          return false;
        }
        if (_isHistoricRegistration && _landRecord.IsClosed) {
          return true;
        }
        if (_isTransactionOnBooksRegistration && !UseRecordingBookRegistation &&
            _landRecord.IsClosed) {
          return true;
        }
        return _transaction.ControlData.IsReadyForRecording && _landRecord.IsClosed &&
               !UseRecordingBookRegistation;
      }
    }


    public bool CanClose {
      get {
        if (_isNewRegistration) {
          return false;
        }
        if (_isHistoricRegistration && !_landRecord.IsClosed && _landRecord.HasRecordingActs) {
          return true;
        }
        if (_isTransactionOnBooksRegistration && !UseRecordingBookRegistation &&
            !_landRecord.IsClosed && _landRecord.HasRecordingActs) {
          return true;
        }
        return _transaction.ControlData.IsReadyForRecording && !_landRecord.IsClosed &&
               !UseRecordingBookRegistation && _landRecord.HasRecordingActs;
      }
    }


    public bool CanMarkAsNoRecordable {
      get {
        return false;
      }
    }


    public bool CanEditRecordingActs {
      get {
        if (_isNewRegistration) {
          return false;
        }
        if (_isHistoricRegistration && !_landRecord.IsClosed) {
          return true;
        }
        if (_isTransactionOnBooksRegistration && !UseRecordingBookRegistation && !_landRecord.IsClosed) {
          return true;
        }
        return CanEdit && ShowRecordingActs;
      }
    }


    public bool CanCreateRecordingBookEntries {
      get {
        if (_isNewRegistration) {
          return false;
        }
        if (!UseRecordingBookRegistation) {
          return false;
        }
        if (!CanEdit) {
          return false;
        }
        return _isTransactionOnBooksRegistration;
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
        return LandMediaReadServices.InstrumentFiles(_landRecord.Instrument).Count != 0;
      }
    }


    public bool ShowRecordingBookEntries {
      get {
        return _isTransactionOnBooksRegistration;
      }
    }


    public bool ShowRecordingActs {
      get {
        if (_isTransactionOnBooksRegistration) {
          return false;
        }
        return !_isNewRegistration;
      }
    }

    public bool ShowRegistrationStamps {
      get {
        return _bookEntries.Count > 0 || _landRecord.HasRecordingActs;
      }
    }

  }  // class LandRecordControlData

}  // namespace Empiria.Land.Registration
