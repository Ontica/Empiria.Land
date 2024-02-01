/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Registration                          Component : Use cases Layer                         *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Use case interactor class               *
*  Type     : TransactionLandRecordUseCases              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for transaction land record edition and retrieving.                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.Land.Instruments;
using Empiria.Land.Instruments.Adapters;

using Empiria.Land.Registration.Adapters;

using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Registration.UseCases {

  /// <summary>Use cases for transaction land record edition and retrieving.</summary>
  public class TransactionLandRecordUseCases : UseCase {

    #region Constructors and parsers

    protected TransactionLandRecordUseCases() {
      // no-op
    }

    static public TransactionLandRecordUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<TransactionLandRecordUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public LandRecordDto GetTransactionLandRecord(string transactionUID) {
      Assertion.Require(transactionUID, nameof(transactionUID));

      var transaction = LRSTransaction.Parse(transactionUID);

      RecordingDocument landRecord = transaction.Document;

      return LandRecordMapper.Map(landRecord);
    }


    public LandRecordDto CreateTransactionLandRecord(string transactionUID,
                                                     InstrumentFields fields) {
      Assertion.Require(transactionUID, nameof(transactionUID));
      Assertion.Require(fields, nameof(fields));
      Assertion.Require(fields.Type.HasValue, "Instrument.Type value is required.");

      var transaction = LRSTransaction.Parse(transactionUID);

      var instrumentType = InstrumentType.Parse(fields.Type.Value);

      var instrument = new Instrument(instrumentType, fields);

      instrument.Save();

      transaction.SetInstrument(instrument);

      var landRecord = RecordingDocument.CreateFromInstrument(instrument.Id, instrument.InstrumentType.Id, instrument.Kind);

      instrument.FillRecordingDocument(landRecord);

      landRecord.Save();

      transaction.AttachDocument(landRecord);

      return LandRecordMapper.Map(landRecord);
    }


    public LandRecordDto UpdateTransactionLandRecord(string transactionUID,
                                                     InstrumentFields fields) {
      Assertion.Require(transactionUID, nameof(transactionUID));
      Assertion.Require(fields, nameof(fields));

      var transaction = LRSTransaction.Parse(transactionUID);

      Instrument instrument = Instrument.Parse(transaction.InstrumentId);

      instrument.Update(fields);

      instrument.Save();

      var landRecord = transaction.Document;

      instrument.FillRecordingDocument(landRecord);

      landRecord.Save();

      return LandRecordMapper.Map(landRecord);
    }

    #endregion Use cases

  }  // class TransactionLandRecordUseCases

}  // namespace Empiria.Land.Registration.UseCases
