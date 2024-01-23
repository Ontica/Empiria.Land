﻿/* Empiria Land **********************************************************************************************
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
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Registration {

  /// <summary>Represents a land recording system official record.</summary>
  public class Record {

    #region Fields

    private readonly RecordingDocument _document;
    private readonly Instrument _instrument;
    private readonly BookEntry _bookEntry;
    private readonly LRSTransaction _transaction;

    #endregion Fields

    #region Constructors and parsers

    internal Record(RecordingDocument document) {
      Assertion.Require(document, nameof(document));

      _document = document;
      _instrument = Instrument.Parse(_document.InstrumentId);
      _bookEntry = LoadBookEntry(_document);
      _transaction = LoadTransaction(document);
    }

    #endregion Constructors and parsers

    #region Properties

    public int Id => _document.Id;

    public string UID => _document.GUID;

    public string RecordingID => _document.UID;

    public Instrument Instrument => _instrument;

    public RecorderOffice RecorderOffice => _document.RecorderOffice;

    public DateTime RecordingTime => _document.AuthorizationTime;

    public DateTime PresentationTime => _document.PresentationTime;

    public Contact RecordedBy => _document.PostedBy;

    public Contact AuthorizedBy => _document.AuthorizedBy;

    public bool HasBookEntry => !_bookEntry.IsEmptyInstance;

    public BookEntry BookEntry => _bookEntry;

    public bool HasTransaction => !_transaction.IsEmptyInstance;

    public LRSTransaction Transaction => _transaction;

    #endregion Properties

    #region Methods

    static private BookEntry LoadBookEntry(RecordingDocument document) {
      BookEntry bookEntry = document.TryGetBookEntry();

      if (bookEntry == null) {
        return BookEntry.Empty;
      }

      return bookEntry;
    }


    static private LRSTransaction LoadTransaction(RecordingDocument document) {
      if (document.HasTransaction) {
        return document.GetTransaction();
      }

      return LRSTransaction.Empty;
    }

    #endregion Methods

  }  // class Record

}  // namespace Empiria.Land.Registration
