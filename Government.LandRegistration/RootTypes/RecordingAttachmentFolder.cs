/* Empiria® Land 2013 ****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria® Land                                  System   : Land Registration System            *
*  Namespace : Empiria.Government.LandRegistration            Assembly : Empiria.Government.LandRegistration *
*  Type      : RecordingAttachmentFolder                      Pattern  : Standard Class                      *
*  Date      : 25/Jun/2013                                    Version  : 5.1     License: CC BY-NC-SA 3.0    *
*                                                                                                            *
*  Summary   : Stores directory information about recording attachments.                                     *
*                                                                                                            *
**************************************************** Copyright © La Vía Óntica SC + Ontica LLC. 1999-2013. **/
using System;
using System.Collections;
using System.IO;

using Empiria.Documents.IO;
using Empiria.Security;

namespace Empiria.Government.LandRegistration {

  /// <summary>Stores directory information about recording attachments.</summary>
  public class RecordingAttachmentFolder {

    #region Fields

    private const string thisTypeName = "ObjectType.RecordingAttachmentFolder";

    static private readonly string attachmentFolderPostfixTag =
                            ConfigurationData.GetString("RecordingBook.AttachmentFolderPostfixTag");

    private Recording recording = Recording.Empty;
    private string alias = String.Empty;
    private string fullPath = String.Empty;
    private FileInfo[] filesCache = null;
    private string impersonationToken = String.Empty;

    #endregion Fields

    #region Constructors and parsers

    private RecordingAttachmentFolder() {

    }

    internal static RecordingAttachmentFolder Parse(Recording recording, string path) {
      RecordingAttachmentFolder folder = new RecordingAttachmentFolder();

      folder.recording = recording;
      folder.alias = recording.Number;
      folder.fullPath = path;
      folder.impersonationToken = recording.RecordingBook.ImagingFilesFolder.ImpersonationToken;

      return folder;
    }

    #endregion Constructors and parsers


    #region Public fields

    public string Alias {
      get { return alias + this.Postfix; }
      set {
        if (value.Length != 0) {
          alias = value;
        }
      }
    }

    public int FilesCount {
      get { return this.GetFiles().Length; }
    }

    public string ImpersonationToken {
      get { return impersonationToken; }
    }

    public string Name {
      get { return fullPath.Substring(fullPath.LastIndexOf(@"\") + 1); }
    }

    public string FullPath {
      get { return fullPath; }
    }

    public string Postfix {
      get {
        string temp = fullPath.Substring(fullPath.LastIndexOf("-") + 1);

        if (temp != "000") {
          return " - " + temp;
        } else {
          return String.Empty;
        }
      }
    }

    public Recording Recording {
      get { return recording; }
    }

    #endregion Public fields

    #region Public methods

    public string FileNameFilters {
      get { return "*.png"; }
    }

    public FileInfo[] GetFiles() {
      if (filesCache == null) {
        string[] searchPatternArray = this.FileNameFilters.Split('|');

        if (searchPatternArray.Length == 1) {
          filesCache = this.GetFiles(searchPatternArray[0]);
        } else {
          ArrayList filesArray = new ArrayList();
          for (int i = 0; i < searchPatternArray.Length; i++) {
            filesArray.AddRange(this.GetFiles(searchPatternArray[i]));
          }
          filesArray.Sort(new FileNameComparer());

          filesCache = (FileInfo[]) filesArray.ToArray(typeof(FileInfo));
        }
      }
      return filesCache;
    }

    public string GetImageURL(int position) {
      FileInfo fileInfo = this.GetFiles()[position];
      RecordBookDirectory directory = recording.RecordingBook.ImagingFilesFolder;
      string fileName = fileInfo.FullName.Replace(directory.PhysicalRootPath, directory.VirtualRootPath);

      return fileName.Replace(@"\", @"/");
    }

    private FileInfo[] GetFiles(string fileNameFilter) {
      DirectoryInfo directoryInfo = null;
      FileInfo[] files = null;
      using (ImpersonationContext context = ImpersonationContext.Open(this.ImpersonationToken)) {
        directoryInfo = new DirectoryInfo(this.FullPath);
        files = directoryInfo.GetFiles(fileNameFilter);
      }
      return files;
    }

    #endregion Public methods

  } // class RecordingAttachmentFolder

} // namespace Empiria.Government.LandRegistration