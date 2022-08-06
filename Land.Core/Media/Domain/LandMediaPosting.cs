/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Media Files Management                Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Object Posting                          *
*  Type     : LandMediaPosting                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Stores a link between an Empiria Land entity (instrument, transaction, recording book)         *
*             and a media file.                                                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Contacts;
using Empiria.Json;
using Empiria.Storage;

using Empiria.StateEnums;
using Empiria.Security;
using Empiria.Land.Registration.Transactions;

using Empiria.Land.Media.Adapters;
using Empiria.Land.Instruments;
using Empiria.Land.Registration;

namespace Empiria.Land.Media {

  /// <summary>A media file related to an Empiria Land entity like instrument,
  /// transaction or recording book.</summary>
  internal class LandMediaPosting : BaseObject, IProtected {

    #region Constructors and parsers

    private LandMediaPosting() {
      // Required by Empiria Framework
    }


    internal protected LandMediaPosting(LandMediaContent mediaContent, StorageFile storageFile) {
      Assertion.Require(storageFile, nameof(storageFile));

      this.MediaContent = mediaContent;
      this.StorageItem = storageFile;
    }


    static public LandMediaPosting Parse(int id) {
      return BaseObject.ParseId<LandMediaPosting>(id);
    }


    static public LandMediaPosting Parse(string uid) {
      return BaseObject.ParseKey<LandMediaPosting>(uid);
    }

    static public LandMediaPosting Empty {
      get {
        return BaseObject.ParseEmpty<LandMediaPosting>();
      }
    }

    #endregion Constructors and parsers

    #region Properties


    [DataField("StorageItemId")]
    internal StorageItem StorageItem {
      get;
      private set;
    }


    [DataField("MediaContentType", Default = LandMediaContent.Unknown)]
    internal LandMediaContent MediaContent {
      get;
      private set;
    }


    [DataField("ImagingControlID")]
    internal string ImagingControlID {
      get;
      private set;
    }


    internal string Keywords {
      get {
        return String.Empty;
      }
    }


    [DataField("MediaPostingExtData")]
    protected internal JsonObject ExtensionData {
      get;
      private set;
    }


    [DataField("TransactionId")]
    internal LRSTransaction Transaction {
      get;
      private set;
    }


    [DataField("InstrumentId")]
    internal Instrument Instrument {
      get;
      private set;
    }


    [DataField("DocumentId")]
    internal RecordingDocument InstrumentRecording {
      get;
      private set;
    }


    [DataField("RecordingBookId")]
    internal RecordingBook RecordingBook {
      get;
      private set;
    }


    [DataField("BookEntryId")]
    internal PhysicalRecording BookEntry {
      get;
      private set;
    }


    [DataField("BookEntryNo")]
    internal string BookEntryNo {
      get;
      private set;
    }


    [DataField("ExternalTransactionId", Default = -1)]
    internal int ExternalTransactionId {
      get;
      private set;
    } = -1;


    [DataField("PostingTime")]
    internal DateTime PostingTime {
      get;
      private set;
    }


    [DataField("PostedById")]
    internal Contact PostedBy {
      get;
      private set;
    }


    [DataField("MediaPostingStatus", Default = EntityStatus.Active)]
    internal EntityStatus Status {
      get;
      private set;
    }


    #endregion Properties

    #region Integrity protection members

    int IProtected.CurrentDataIntegrityVersion {
      get {
        return 1;
      }
    }


    object[] IProtected.GetDataIntegrityFieldValues(int version) {
      if (version == 1) {
        return new object[] {
          1, "Id", Id, "StorageItemId", StorageItem.Id, "ImagingControlID", ImagingControlID,
          "ExtensionData", ExtensionData.ToString(),
          "Transaction", Transaction.Id, "Instrument", Instrument.Id,
          "InstrumentRecording", InstrumentRecording.Id,
          "RecordingBook", RecordingBook.Id, "BookEntry", BookEntry.Id, "BookEntryNo", BookEntryNo,
          "ExternalTransaction", ExternalTransactionId,
          "PostingTime", PostingTime, "PostedBy", PostedBy.Id,
          "MediaStatus", (char) Status
        };
      }
      throw new SecurityException(SecurityException.Msg.WrongDIFVersionRequested, version);
    }


    internal void LinkToTransaction(LRSTransaction transaction) {
      this.Transaction = transaction;
    }


    private IntegrityValidator _validator = null;
    public IntegrityValidator Integrity {
      get {
        if (_validator == null) {
          _validator = new IntegrityValidator(this);
        }
        return _validator;
      }
    }


    #endregion Integrity protection members

    #region Methods

    internal void Delete() {
      Assertion.Require(this.Status == EntityStatus.Active,
                       "MediaObject must be in 'Active' status.");

      this.Status = EntityStatus.Deleted;
    }


    protected override void OnSave() {
      if (base.IsNew) {
        this.PostedBy = EmpiriaUser.Current.AsContact();
        this.PostingTime = DateTime.Now;
      }

      LandMediaPostingsData.WriteMediaPosting(this);
    }

    #endregion Methods

  }  // class LandMediaPosting

}  // namespace Empiria.Land.Media
