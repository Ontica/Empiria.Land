/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Registration                            Component : Interface adapters                    *
*  Assembly : Empiria.Land.Registration.dll                Pattern   : Data Transfer Object                  *
*  Type     : BookEntryDto                                 License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : DTO that holds data about a physical book recording entry.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.Contacts;

namespace Empiria.Land.Registration {

  /// <summary>DTO that holds data about a physical book recording entry.</summary>
  public class BookEntryDto {

    #region Constructors and parsers

    public BookEntryDto(RecordingBook book, LandRecord landRecord, string number) {
      Assertion.Require(book, nameof(book));
      Assertion.Require(landRecord, nameof(landRecord));
      Assertion.Require(number, nameof(number));

      Assertion.Require(!book.IsEmptyInstance, "book can't be the empty instance.");
      Assertion.Require(!landRecord.IsEmptyInstance, "landRecord can't be the empty instance.");

      this.RecordingBook = book;
      this.LandRecord = landRecord;
      this.Number = RecordingBook.FormatBookEntryNumber(number);
    }

    #endregion Constructors and parsers

    #region Public properties

    public RecordingBook RecordingBook {
      get;
    }


    public string Number {
      get;
    }


    public LandRecord LandRecord {
      get;
    }


    public string Notes {
      get; set;
    } = String.Empty;


    public int StartImageIndex {
      get; set;
    } = -1;


    public int EndImageIndex {
      get; set;
    } = -1;


    public DateTime PresentationTime {
      get; set;
    } = ExecutionServer.DateMinValue;


    public DateTime AuthorizationDate {
      get; set;
    } = ExecutionServer.DateMinValue;


    public Contact AuthorizedBy {
      get; set;
    } = Contact.Empty;


    public RecordableObjectStatus Status {
      get; set;
    } = RecordableObjectStatus.Incomplete;


    #endregion Public properties

  } // class BookEntryDto

} // namespace Empiria.Land.Registration
