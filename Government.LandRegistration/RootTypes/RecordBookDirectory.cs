﻿/* Empiria® Land 2013 ****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria® Land                                  System   : Land Registration System            *
*  Namespace : Empiria.Government.LandRegistration            Assembly : Empiria.Government.LandRegistration *
*  Type      : RecordBookDirectory                            Pattern  : Storage Item                        *
*  Date      : 25/Jun/2013                                    Version  : 5.1     License: CC BY-NC-SA 3.0    *
*                                                                                                            *
*  Summary   : Describes a record book imaging directory.                                                    *
*                                                                                                            *
**************************************************** Copyright © La Vía Óntica SC + Ontica LLC. 1999-2013. **/
using System.IO;

using Empiria.Contacts;
using Empiria.Documents.IO;
using Empiria.Ontology;
using Empiria.Security;

namespace Empiria.Government.LandRegistration {

  /// <summary>Describes a record book imaging directory.</summary>
  public class RecordBookDirectory : FilesFolder {

    #region Fields

    private const string thisTypeName = "ObjectType.FilesFolder.RecordBookDirectory";

    static private readonly string emptyBookUrl = ConfigurationData.GetString("RecordingBook.Empty.Book.Image.Url");
    static private readonly string emptyImageFullPath = ConfigurationData.GetString("RecordingBook.Empty.Image.Path");
    static private readonly string emptyImageFileName = ConfigurationData.GetString("RecordingBook.Empty.Image.FileName");
    static private readonly bool processOnlyNewDirectories = ConfigurationData.GetBoolean("RecordBookDirectory.ProcessOnlyNewDirectories");

    private RecorderOffice recorderOffice = RecorderOffice.Empty;

    #endregion Fields

    #region Constructors and parsers

    public RecordBookDirectory()
      : base(thisTypeName) {

    }

    private RecordBookDirectory(string typeName)
      : base(typeName) {
      // Required by Empiria Framework. Do not delete. Protected in not sealed classes, private otherwise
    }

    static public new RecordBookDirectory Parse(int id) {
      return BaseObject.Parse<RecordBookDirectory>(thisTypeName, id);
    }

    static public new RecordBookDirectory Empty {
      get { return BaseObject.ParseEmpty<RecordBookDirectory>(thisTypeName); }
    }

    static public int ProcessDirectories(RecorderOffice office) {
      FilesFolder rootFolder = office.GetRootImagesFolder();

      if (rootFolder.IsEmptyInstance) {
        return 0;
      }
      if (!processOnlyNewDirectories) {
        DeleteNotUsedDirectories(rootFolder);
        UpdateUsedDirectories(rootFolder);
      }
      return CreateDirectories(rootFolder);
    }

    #endregion Constructors and parsers

    #region Public properties

    static public ObjectTypeInfo DirectoryType {
      get { return ObjectTypeInfo.Parse(thisTypeName); }
    }

    public string GetImageURL(int position) {
      if (this.IsEmptyInstance) {
        return emptyBookUrl;
      }
      System.IO.FileInfo[] fileInfo = base.GetFiles();
      if (position < 0) {
        position = 0;
      }
      if (0 <= position && position < fileInfo.Length) {
        return base.MapPath(fileInfo[position]);
      } else {
        throw new LandRegistrationException(LandRegistrationException.Msg.InvalidImagePosition, 
                                            this.Id, this.DisplayName, position, fileInfo.Length);
      }
    }

    public RecorderOffice RecorderOffice {
      get {
        if (recorderOffice.IsEmptyInstance) {
          recorderOffice = RecorderOffice.Parse(base.OwnerId);
        }
        return recorderOffice;
      }
    }

    #endregion Public properties

    #region Public methods

    public RecordingBook CreateRecordingBook(RecordingSection recordingSectionType,
                                             Contact imagesCapturedBy, Contact imagesReviewedBy,
                                             int recordingsControlCount,
                                             TimePeriod recordingsControlTimePeriod) {

      this.CapturedById = imagesCapturedBy.Id;
      this.ReviewedById = imagesReviewedBy.Id;
      this.Status = FilesFolderStatus.Opened;

      RecordingBook recordingBook = RecordingBook.Create(this, recordingSectionType, recordingsControlCount,
                                                         recordingsControlTimePeriod);
      this.ReferenceId = recordingBook.Id;
      this.Save();

      return recordingBook;
    }

    internal void DeleteImageAtIndex(int imageIndex) {
      base.DeleteFileAtIndex(imageIndex);
      base.Refresh();
      this.RenameDirectoryImages();
      base.UpdateStatistics();
      base.Save();
      base.Refresh();
    }

    public string GetRecordingBookTag(RecordingBookType recordingBookType) {
      if (RecordingBook.UseBookLevel) {
        return GetRecordingBookTagUsingBookLevel(recordingBookType);
      } else {
        return GetRecordingBookTagNotUsingBookLevel(recordingBookType);
      }
    }

    private string GetRecordingBookTagUsingBookLevel(RecordingBookType recordingBookType) {
      switch (recordingBookType) {
        case RecordingBookType.Section:
          return this.DisplayName.Split('-')[1];
        case RecordingBookType.Book:
          return this.DisplayName.Split('-')[2];
        case RecordingBookType.Volume:
          return this.DisplayName.Split('-')[3];
        default:
          return "00";
      }
    }

    private string GetRecordingBookTagNotUsingBookLevel(RecordingBookType recordingBookType) {
      switch (recordingBookType) {
        case RecordingBookType.Section:
          return this.DisplayName.Split('-')[1];
        case RecordingBookType.Book:
          throw new LandRegistrationException(LandRegistrationException.Msg.UnrecognizedRecordingBookType, RecordingBookType.Book.ToString());
        case RecordingBookType.Volume:
          return this.DisplayName.Split('-')[2];
        default:
          return "00";
      }
    }

    internal void InsertEmptyImageAtIndex(int imageIndex) {
      this.RenameDirectoryImages();
      FileInfo[] files = base.GetFiles();
      using (ImpersonationContext context = ImpersonationContext.Open(base.ImpersonationToken)) {
        for (int i = files.Length - 1; imageIndex <= i; i--) {
          string newFileName = this.DisplayName + "-" + (i + 2).ToString("0000") + files[i].Extension;
          File.Move(files[i].FullName, this.PhysicalPath + @"\" + newFileName);
        }
        FileInfo emptyImage = new FileInfo(emptyImageFullPath + emptyImageFileName);
        emptyImage.CopyTo(this.PhysicalPath + @"\" + this.DisplayName + "-" + (imageIndex + 1).ToString("0000") + emptyImage.Extension);
      }
      base.UpdateStatistics();
      base.Save();
      base.Refresh();
    }

    public void RenameDirectoryImages() {
      FileInfo[] files = base.GetFiles();
      using (ImpersonationContext context = ImpersonationContext.Open(base.ImpersonationToken)) {
        for (int i = 0; i < files.Length; i++) {
          string newFileName = "temp" + "-" + (i + 1).ToString("0000") + files[i].Extension;
          File.Move(files[i].FullName, this.PhysicalPath + @"\" + newFileName);
        }
      }
      base.Refresh();
      files = base.GetFiles();
      using (ImpersonationContext context = ImpersonationContext.Open(base.ImpersonationToken)) {
        for (int i = 0; i < files.Length; i++) {
          string newFileName = this.DisplayName + "-" + (i + 1).ToString("0000") + files[i].Extension;
          File.Move(files[i].FullName, this.PhysicalPath + @"\" + newFileName);
        }
      }
      base.Refresh();
    }

    internal string[] GetNameParts() {
      return this.DisplayName.Split('-');
    }

    #endregion Public methods

    #region Private methods

    static private bool AlreadyExists(FilesFolder findFilesFolder) {
      FilesFolderList rootFolders = RootFilesFolder.GetRootFilesFolders();

      foreach (FilesFolder filesFolder in rootFolders) {
        FilesFolderList childs = filesFolder.GetChilds();
        foreach (FilesFolder childFolder in childs) {
          if ((childFolder.DisplayName == findFilesFolder.DisplayName) ||
              (childFolder.PhysicalPath == findFilesFolder.PhysicalPath)) {
            return true;
          }
        }
      }
      return false;
    }

    static private int CreateDirectories(FilesFolder rootFolder) {
      int counter = 0;
      if (rootFolder.Status != FilesFolderStatus.Opened) {
        return 0;
      }
      RecorderOffice recorderOffice = RecorderOffice.Parse(rootFolder.OwnerId);

      FilesFolderList filesFolderList = FilesFolder.CreateAllFromPath(RecordBookDirectory.DirectoryType, rootFolder,
                                                                      rootFolder.PhysicalPath);
      for (int j = 0; j < filesFolderList.Count; j++) {
        RecordBookDirectory newDirectory = (RecordBookDirectory) filesFolderList[j];
        if (AlreadyExists(newDirectory)) {
          continue;
        }
        if (IsValidName(recorderOffice, newDirectory.DisplayName)) {
          newDirectory.UpdateStatistics();
          if (newDirectory.FilesCount != 0) {
            newDirectory.OwnerId = recorderOffice.Id;
            newDirectory.Save();
            counter++;
          }
        }
      }
      return counter;
    }

    static private void DeleteNotUsedDirectories(FilesFolder rootFolder) {
      FilesFolderList childs = rootFolder.GetChilds();
      foreach (FilesFolder childFolder in childs) {
        if (childFolder.Status == FilesFolderStatus.Pending) {
          childFolder.Delete();
        }
      }
    }

    static private bool IsValidName(RecorderOffice recorderOffice, string filesFolderName) {
      string[] parts = filesFolderName.Split('-');

      if (RecordingBook.UseBookLevel && parts.Length != 4) {
        return false;
      } else if (!RecordingBook.UseBookLevel && parts.Length != 3) {
        return false;
      } else if (parts[0] != recorderOffice.Tag) {
        return false;
      } else {
        return true;
      }
    }

    static private void UpdateUsedDirectories(FilesFolder rootFolder) {
      FilesFolderList childs = rootFolder.GetChilds();
      foreach (FilesFolder childFolder in childs) {
        if (childFolder.Status == FilesFolderStatus.Opened) {
          childFolder.UpdateStatistics();
          childFolder.Save();
        }
      }
    }

    static public void UpdateFilesCount(RecorderOffice office) {
      FilesFolder rootFolder = office.GetRootImagesFolder();

      FilesFolderList childs = rootFolder.GetChilds();
      foreach (FilesFolder childFolder in childs) {
        if (childFolder.Status == FilesFolderStatus.Opened || childFolder.Status == FilesFolderStatus.Pending) {
          childFolder.UpdateFilesCount();
          childFolder.Save();
        }
      }
    }

    #endregion Private methods

  } // class RecordBookDirectory

} // namespace Empiria.Government.LandRegistration