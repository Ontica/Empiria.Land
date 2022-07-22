/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Information Holder                      *
*  Type     : TransactionPreprocessingData               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides control data for transaction preprocessing tasks.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Instruments;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Transactions {

  /// <summary>Provides control data for transaction preprocessing tasks.</summary>
  internal class TransactionPreprocessingData {

    private readonly LRSTransaction _transaction;

    public TransactionPreprocessingData(LRSTransaction transaction) {
      _transaction = transaction;
    }

    public bool CanEditInstrument {
      get {
        return true;
      }
    }


    public bool CanUploadInstrumentFiles {
      get {
        return true;
      }
    }


    public bool CanSetAntecedent {
      get {
        return false;
      }
    }

    public bool CanCreateAntecedent {
      get {
        return false;
      }
    }

    public bool CanEditAntecedentRecordingActs {
      get {
        return false;
      }
    }


    public bool ShowInstrument {
      get {
        return true;
      }
    }


    public bool ShowInstrumentFiles {
      get {
        return true;
      }
    }


    public bool ShowAntecedent {
      get {
        return false;
      }
    }

    public bool ShowAntecedentRecordingActs {
      get {
        return false;
      }
    }


    public Instrument Instrument {
      get {
        if (_transaction.HasInstrument) {
          return Instrument.Parse(_transaction.InstrumentId);
        }
        return Instrument.Empty;
      }
    }


    public object Antecedent {
      get;
      internal set;
    } = new object();


    public object AntecedentRecordingActs {
      get;
      internal set;
    } = new object();


  }  // class TransactionPreprocessingData

}  // namespace Empiria.Land.Transactions
