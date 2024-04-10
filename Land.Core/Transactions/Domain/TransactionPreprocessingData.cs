/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transactions Preprocessing                 Component : Domain Layer                            *
*  Assembly : Empiria.Land.Transactions.dll              Pattern   : Information Holder                      *
*  Type     : TransactionPreprocessingData               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides control data for transaction preprocessing tasks.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Instruments;
using Empiria.Land.Media;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Transactions.Preprocessing {

  /// <summary>Provides control data for transaction preprocessing tasks.</summary>
  internal class TransactionPreprocessingData {

    public TransactionPreprocessingData(LRSTransaction transaction) {
      Assertion.Require(transaction, nameof(transaction));

      this.Transaction = transaction;
    }


    public LRSTransaction Transaction {
      get;
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
        if (Transaction.HasInstrument) {
          return Instrument.Parse(Transaction.InstrumentId);
        }
        return Instrument.Empty;
      }
    }


    public FixedList<LandMediaPosting> TransactionMediaPostings {
      get {
        return LandMediaReadServices.TransactionFiles(Transaction);
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

}  // namespace Empiria.Land.Transactions.Preprocessing
