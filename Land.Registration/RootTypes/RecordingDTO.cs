/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : Recording                                      Pattern  : Data Transfer Object                *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : DTO that holds data used to edit recording books with physical recording entries.             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.Contacts;

namespace Empiria.Land.Registration {

  /// <summary>DTO that holds data used to edit recording books with physical recording entries.</summary>
  public class RecordingDTO {

    #region Constructors and parsers

    public RecordingDTO(RecordingBook book, string number) {
      this.RecordingBook = book;
      this.Number = RecordingBook.FormatRecordingNumber(number);
    }

    #endregion Constructors and parsers

    #region Public properties

    public RecordingBook RecordingBook {
      get;
      private set;
    }


    public string Number {
      get;
      private set;
    }


    public RecordingDocument MainDocument {
      get;
      set;
    } = new RecordingDocument(RecordingDocumentType.Empty);


    public string Notes {
      get;
      set;
    } = String.Empty;


    public int StartImageIndex {
      get;
      set;
    } = -1;


    public int EndImageIndex {
      get;
      set;
    } = -1;


    public DateTime PresentationTime {
      get;
      set;
    }


    public DateTime AuthorizationDate {
      get;
      set;
    }


    public Contact AuthorizedBy {
      get;
      set;
    }


    public RecordableObjectStatus Status {
      get;
      set;
    }


    #endregion Public properties

  } // class RecordingDTO

} // namespace Empiria.Land.Registration
