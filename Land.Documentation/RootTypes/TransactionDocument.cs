/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Documentation                           Component : Transaction Document Management       *
*  Assembly : Empiria.Land.Documentation.dll               Pattern   : Information holder                    *
*  Type     : TransactionDocument                          License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Represents a transaction storable document like PDF, Word, ot text-based files.                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.IO;

using Empiria.Json;
using Empiria.Security;

using Empiria.Documents;

using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Documentation {

  /// <summary>Represents a transaction storable document like PDF, Word, ot text-based files.</summary>
  public class TransactionDocument : ImagingItem, IProtected {


    #region Fields

    static readonly string ROOT_FOLDER = ConfigurationData.GetString("TransactionDocument.RootFolder");

    #endregion Fields

    #region Constructors and parsers


    private TransactionDocument() {
      // Required by Empiria Framework
    }


    private TransactionDocument(LRSTransaction transaction, Stream inputStream,
                                FileContentType contentType, DocumentImageType documentType,
                                string sourceFileName = "") {

      this.Transaction = transaction;
      this.DocumentType = documentType;
      this.SetBaseFolder();
      base.ItemPath = this.GetRelativePath();
      base.FilesCount = 1;
      base.DigitalizedBy = EmpiriaUser.Current.AsContact();
      base.DigitalizationDate = DateTime.Now;

      CreateFile(inputStream, contentType);

      this.SetExtensionData(inputStream, contentType, sourceFileName);
    }


    private void SetExtensionData(Stream inputStream, FileContentType contentType,
                                  string sourceFileName = "") {

      byte[] array = Documents.IO.FileServices.StreamToArray(inputStream);
      var guid = Guid.NewGuid();

      string hash = Cryptographer.CreateHashCode(array, guid.ToString());

      var json = new JsonObject();

      json.AddIfValue("sourceFileName", sourceFileName);
      json.Add("length", array.LongLength);
      json.Add("contentType", contentType.ToString());
      json.Add("guid", guid.ToString());
      json.Add("hash", hash);

      this.ImagingItemExtData = json;
    }


    static public new TransactionDocument Empty {
      get {
        return BaseObject.ParseEmpty<TransactionDocument>();
      }
    }


    static internal TransactionDocument CreateMain(LRSTransaction transaction, Stream inputStream,
                                                   FileContentType contentType, string sourceFileName = "") {
      var document = new TransactionDocument(transaction, inputStream, contentType,
                                             DocumentImageType.MainDocument, sourceFileName);

      document.Save();

      return document;
    }


    static internal TransactionDocument CreateAuxiliary(LRSTransaction transaction, Stream inputStream,
                                                        FileContentType contentType, string sourceFileName = "") {
      var document = new TransactionDocument(transaction, inputStream, contentType,
                                             DocumentImageType.Appendix, sourceFileName);

      document.Save();

      return document;
    }


    #endregion Constructors and parsers

    #region Public properties


    [DataField("TransactionId")]
    public LRSTransaction Transaction {
      get;
      private set;
    }


    [DataField("ImageType", Default = DocumentImageType.Unknown)]
    public DocumentImageType DocumentType {
      get;
      private set;
    }


    [DataField("BaseFolderId")]
    public ImagingFolder BaseFolder {
      get;
      protected set;
    }

    public string UrlRelativePath {
      get {
        return base.ItemPath.Replace("~", this.BaseFolder.UrlRelativePath).Replace('\\', '/');
      }
    }

    #endregion Public properties

    #region Public methods


    protected override void OnSave() {
      DataServices.WriteTransactionDocument(this);
    }


    public void Delete() {
      base.Status = StateEnums.EntityStatus.Deleted;

      Save();
    }


    internal void Update(Stream inputStream, FileContentType contentType, string fileName) {
      SetExtensionData(inputStream, contentType, fileName);

      base.Save();
    }


    #endregion Public methods

    #region Integrity protection members


    int IProtected.CurrentDataIntegrityVersion {
      get {
        return 1;
      }
    }


    object[] IProtected.GetDataIntegrityFieldValues(int version) {
      if (version == 1) {
        return new object[] {
          1, "Id", this.Id, "TransactionUID", this.Transaction.UID, "DocumentType", (char) this.DocumentType,
          "BaseFolder", this.BaseFolder.Id, "ItemPath", base.ItemPath, "ExtData",
          this.ImagingItemExtData.ToString(), "FilesCount", this.FilesCount
        };
      }
      throw new SecurityException(SecurityException.Msg.WrongDIFVersionRequested, version);
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

    #region Private methods


    private string BuildFileName() {
      string fileName = this.Transaction.UID;

      if (this.DocumentType == DocumentImageType.MainDocument) {
        fileName += "_M";
      } else if (this.DocumentType == DocumentImageType.Appendix) {
        fileName += "_A";
      }

      fileName += ".pdf";

      return fileName;
    }


    private void CreateFile(Stream inputStream, FileContentType contentType) {
      string path = this.GetFullPath();

      Documents.IO.FileServices.AssureDirectoryForFile(path);

      using (var output = new FileStream(path, FileMode.Create)) {
        inputStream.CopyTo(output);
      }
    }


    private string GetBaseFolderPath() {
      return Path.Combine(ROOT_FOLDER, this.Transaction.PresentationTime.Year.ToString());
    }


    private string GetFullPath() {
      DateTime date = this.Transaction.PresentationTime;

      return GetBaseFolderPath() + @"\" + date.ToString("yyyy-MM") + @"\" +
             date.ToString("yyyy-MM-dd") + @"\" + this.Transaction.UID + @"\" + BuildFileName();
    }


    private string GetRelativePath() {
      string baseFolder = this.BaseFolder.ItemPath.Trim('\\');

      return GetFullPath().Replace(baseFolder, "~");
    }


    private void SetBaseFolder() {
      string folderPath = this.GetBaseFolderPath();

      var imagingFolder = ImagingFolder.TryParse(folderPath);

      if (imagingFolder != null) {
        this.BaseFolder = imagingFolder;

      } else {
        throw new LandDocumentationException(LandDocumentationException.Msg.ImagingFolderNotExists,
                                             folderPath);
      }
    }


    #endregion Private methods

  } // class TransactionDocument

} // namespace Empiria.Land.Documentation
