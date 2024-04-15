/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Search services                            Component : Domain Layer                            *
*  Assembly : Empiria.Land.SearchServices.dll            Pattern   : Domain object                           *
*  Type     : Record                                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents a land recording system official record.                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Contacts;

using Empiria.Land.Instruments;
using Empiria.Land.Transactions;

namespace Empiria.Land.Registration {

  /// <summary>Represents a land recording system official record.</summary>
  public class Record {

    #region Fields

    private readonly LandRecord _landRecord;
    private readonly BookEntry _bookEntry;
    private readonly LRSTransaction _transaction;

    #endregion Fields

    #region Constructors and parsers

    internal Record(LandRecord landRecord) {
      Assertion.Require(landRecord, nameof(landRecord));

      _landRecord = landRecord;

      _bookEntry = LoadBookEntry(_landRecord);
      _transaction = LoadTransaction(_landRecord);
    }

    #endregion Constructors and parsers

    #region Properties

    public int Id => _landRecord.Id;

    public string UID => _landRecord.GUID;

    public string RecordingID => _landRecord.UID;

    public Instrument Instrument => _landRecord.Instrument;

    public RecorderOffice RecorderOffice => _landRecord.RecorderOffice;

    public DateTime RecordingTime => _landRecord.AuthorizationTime;

    public DateTime PresentationTime => _landRecord.PresentationTime;

    public Contact RecordedBy => _landRecord.PostedBy;

    public Contact AuthorizedBy => _landRecord.AuthorizedBy;

    public bool HasBookEntry => !_bookEntry.IsEmptyInstance;

    public BookEntry BookEntry => _bookEntry;

    public bool HasTransaction => !_transaction.IsEmptyInstance;

    public LRSTransaction Transaction => _transaction;

    #endregion Properties

    #region Methods

    static private BookEntry LoadBookEntry(LandRecord landRecord) {
      BookEntry bookEntry = landRecord.TryGetBookEntry();

      if (bookEntry == null) {
        return BookEntry.Empty;
      }

      return bookEntry;
    }


    static private LRSTransaction LoadTransaction(LandRecord landRecord) {
      if (landRecord.HasTransaction) {
        return landRecord.Transaction;
      }

      return LRSTransaction.Empty;
    }

    #endregion Methods

  }  // class Record

}  // namespace Empiria.Land.Registration
