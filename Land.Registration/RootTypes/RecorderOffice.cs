/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecorderOffice                                 Pattern  : Storage Item                        *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : A recorder of deeds office.                                                                   *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;

using Empiria.Contacts;
using Empiria.DataTypes;
using Empiria.Documents.IO;
using Empiria.Geography;

using Empiria.Land.Registration.Data;

namespace Empiria.Land.Registration {

  /// <summary>A recorder of deeds office.</summary>
  public class RecorderOffice : Organization {

    #region Constructors and parsers

    private RecorderOffice() {
      // Required by Empiria Framework.
    }

    static public new RecorderOffice Empty {
      get { return RecorderOffice.ParseEmpty<RecorderOffice>(); }
    }

    static public new RecorderOffice Parse(int id) {
      return BaseObject.ParseId<RecorderOffice>(id);
    }

    static public FixedList<RecorderOffice> GetList() {
      return MainRecorderOffice.GetLinks<RecorderOffice>("RecorderOffice->SubRecorderOffices",
                                                         (x, y) => x.Number.CompareTo(y.Number));
    }

    static public RecorderOffice MainRecorderOffice {
      get { return RecorderOffice.Parse(ExecutionServer.OrganizationId); }
    }

    #endregion Constructors and parsers

    #region Public properties

    [DataField("NickName")]
    public string Number {
      get;
      private set;
    }

    #endregion Public properties

    #region Public methods

    public RecordingBook AddRootRecordingBook(string rootTag) {
      FixedList<RecordingBook> roots = this.GetRootRecordingBooks();
      if (!roots.Contains((x) => x.BookNumber.Equals(rootTag))) {
        var recordingBook = new RecordingBook(this, rootTag);
        recordingBook.Save();
        return recordingBook;
      } else {
        throw new LandRegistrationException(LandRegistrationException.Msg.RecorderOfficeRootRecordingBookAlreadyExists,
                                            this.Alias, rootTag);
      }
    }

    public FixedList<GeographicRegion> GetNotaryOfficePlaces() {
      return MainRecorderOffice.GetLinks<GeographicRegion>("RecorderOffice->NotaryOfficePlaces",
                                                           (x, y) => x.Name.CompareTo(y.Name));
    }

    public FixedList<GeographicRegion> GetPrivateDocumentIssuePlaces() {
      return MainRecorderOffice.GetLinks<GeographicRegion>("RecorderOffice->PrivateDocumentIssuePlace",
                                                           (x, y) => x.Name.CompareTo(y.Name));
    }

    public FixedList<GeographicRegion> GetJudicialDocumentIssuePlaces() {
      return MainRecorderOffice.GetLinks<GeographicRegion>("RecorderOffice->JudicialDocumentIssuePlace",
                                                               (x, y) => x.Name.CompareTo(y.Name));
    }

    public FixedList<Municipality> GetMunicipalities() {
      return this.GetLinks<Municipality>("RecorderOffice->Municipalities",
                                         (x, y) => x.Name.CompareTo(y.Name));
    }

    public FixedList<Person> GetRecorderOfficials() {
      return this.GetLinks<Person>("RecorderOffice->RecorderOfficials",
                                   (x, y) => x.FamilyFullName.CompareTo(y.FamilyFullName));
    }

    public FixedList<Person> GetRecorderOfficials(TimeFrame period) {
      return this.GetLinks<Person>("RecorderOffice->RecorderOfficials", period,
                                   (x, y) => x.FamilyFullName.CompareTo(y.FamilyFullName));
    }

    public FixedList<Organization> GetPropertyTitleOffices() {
      return MainRecorderOffice.GetLinks<Organization>("RecorderOffice->PropertyTitleOffices",
                                                       (x, y) => x.FullName.CompareTo(y.FullName));

    }

    public FixedList<Person> GetPropertyTitleSigners() {
      return MainRecorderOffice.GetLinks<Person>("RecorderOffice->PropertyTitleSigners",
                                                  (x, y) => x.FamilyFullName.CompareTo(y.FamilyFullName));
    }

    public FixedList<RecordingBook> GetRecordingBooks(RecordingSection sectionType) {
      return RecordingBooksData.GetRecordingBooksInSection(this, sectionType);
    }

    public FixedList<RecordingBook> GetRootRecordingBooks() {
      return RecordingBooksData.GetRootRecordingBooks(this);
    }

    internal FilesFolder GetRootImagesFolder() {
      var rootFolders = RootFilesFolder.GetRootFilesFolders();

      foreach (FilesFolder filesFolder in rootFolders) {
        if (filesFolder.Owner == this) {
          return filesFolder;
        }
      }
      return FilesFolder.Empty;
    }

    #endregion Public methods

  } // class RecorderOffice

} // namespace Empiria.Land.Registration
