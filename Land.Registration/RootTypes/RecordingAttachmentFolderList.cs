/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingAttachmentFolderList                  Pattern  : Standard Class                      *
*  Version   : 1.5        Date: 28/Mar/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Stores directory information about recording attachments.                                     *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Collections.Generic;
using System.IO;

using Empiria.Collections;
using Empiria.Security;

namespace Empiria.Land.Registration {

  /// <summary>Stores directory information about recording attachments.</summary>
  public class RecordingAttachmentFolderList : EmpiriaList<RecordingAttachmentFolder> {

    #region Fields

    private const string thisTypeName = "ObjectType.RecordingAttachmentFolderList";

    #endregion Fields

    #region Constructors and parsers

    public RecordingAttachmentFolderList() {

    }

    #endregion Constructors and parsers

    #region Public methods

    public void Append(Recording recording) {
      Append(recording, String.Empty);
    }

    public void Append(Recording recording, string alias) {
      string impersonationToken = recording.RecordingBook.ImagingFilesFolder.ImpersonationToken;

      using (ImpersonationContext context = ImpersonationContext.Open(impersonationToken)) {
        if (Directory.Exists(this.GetRootDirectory(recording.RecordingBook))) {
          IEnumerable<string> directories = EnumerateDirectories(recording);

          foreach (string directory in directories) {
            RecordingAttachmentFolder folder = RecordingAttachmentFolder.Parse(recording, directory);
            folder.Alias = alias;
            base.Add(folder);
          }
        }
      } // using
    }

    private string BuildFolderPath(Recording recording) {
      int startIndex = recording.RecordingBook.ImagingFilesFolder.PhysicalPath.LastIndexOf(@"\");
      string recordingNamePath = recording.RecordingBook.ImagingFilesFolder.PhysicalPath.Substring(startIndex + 1);
      if (recordingNamePath.EndsWith(".A") || recordingNamePath.EndsWith(".B") || recordingNamePath.EndsWith(".C")
          || recordingNamePath.EndsWith(".D")) {
        recordingNamePath = recordingNamePath.Remove(recordingNamePath.Length - 2);
      }
      string path = recording.RecordingBook.ImagingFilesFolder.ParentFilesFolder.PhysicalPath;

      path += @".apendices\" + recordingNamePath + "-" + recording.Number.Replace("-", String.Empty) + "-000";

      return path;
    }

    private string GetRootDirectory(RecordingBook recordingBook) {
      return recordingBook.ImagingFilesFolder.ParentFilesFolder.PhysicalPath + @".apendices";
    }

    private string GetRecordingDirectoryPattern(Recording recording) {
      int startIndex = recording.RecordingBook.ImagingFilesFolder.PhysicalPath.LastIndexOf(@"\");

      string path = recording.RecordingBook.ImagingFilesFolder.PhysicalPath.Substring(startIndex + 1);
      if (path.EndsWith(".A") || path.EndsWith(".B") || path.EndsWith(".C") || path.EndsWith(".D")) {
        path = path.Remove(path.Length - 2);
      }
      return path + "-" + recording.Number.Replace("-", String.Empty).ToLowerInvariant() + "-*";
    }

    private IEnumerable<string> EnumerateDirectories(Recording recording) {
      return Directory.EnumerateDirectories(GetRootDirectory(recording.RecordingBook),
                                            GetRecordingDirectoryPattern(recording));
    }

    #endregion Public methods

  } // class RecordingAttachmentFolderList

} // namespace Empiria.Land.Registration
