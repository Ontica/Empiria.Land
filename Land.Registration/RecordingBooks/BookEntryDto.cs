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

    public BookEntryDto(RecordingBook book, string number) {
      this.RecordingBook = book;
      this.Number = RecordingBook.FormatBookEntryNumber(number);
    }

    #endregion Constructors and parsers

    #region Public properties

    public RecordingBook RecordingBook {
      get; private set;
    }


    public string Number {
      get; private set;
    }


    public RecordingDocument MainDocument {
      get; set;
    } = new RecordingDocument(RecordingDocumentType.Empty);


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
