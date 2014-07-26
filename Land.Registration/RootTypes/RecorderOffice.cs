﻿/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecorderOffice                                 Pattern  : Storage Item                        *
*  Version   : 2.0        Date: 23/Oct/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : A recorder of deeds office.                                                                   *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;

using Empiria.Contacts;
using Empiria.Documents.IO;
using Empiria.Geography;

using Empiria.Land.Registration.Data;

namespace Empiria.Land.Registration {

  /// <summary>A recorder of deeds office.</summary>
  public class RecorderOffice : Organization {

    #region Fields

    private const string thisTypeName = "ObjectType.Contact.Organization.RecorderOffice";

    private string tag = String.Empty;

    #endregion Fields

    #region Constructors and parsers

    public RecorderOffice()
      : base(thisTypeName) {

    }

    protected RecorderOffice(string typeName)
      : base(typeName) {
      // Required by Empiria Framework. Do not delete. Protected in not sealed classes, private otherwise
    }

    static public new RecorderOffice Empty {
      get { return RecorderOffice.ParseEmpty<RecorderOffice>(thisTypeName); }
    }

    static public new RecorderOffice Parse(int id) {
      return BaseObject.Parse<RecorderOffice>(thisTypeName, id);
    }

    static public FixedList<RecorderOffice> GetList() {
      return MainRecorderOffice.GetLinks<RecorderOffice>("MainRecorderOffice_RecorderOffices",
                                                         (x, y) => x.Tag.CompareTo(y.Tag));
    }

    static public RecorderOffice MainRecorderOffice {
      get { return RecorderOffice.Parse(ExecutionServer.OrganizationId); }
    }

    #endregion Constructors and parsers

    #region Public properties

    public string Tag {
      get { return tag; }
    }

    #endregion Public properties

    #region Public methods

    public RecordingBook AddRootRecordingBook(string rootTag) {
      FixedList<RecordingBook> roots = this.GetRootRecordingBooks();
      if (!roots.Contains((x) => x.BookNumber.Equals(rootTag))) {
        RecordingBook recordingBook = new RecordingBook(this, rootTag);
        recordingBook.Save();
        return recordingBook;
      } else {
        throw new LandRegistrationException(LandRegistrationException.Msg.RecorderOfficeRootRecordingBookAlreadyExists,
                                            this.Alias, rootTag);
      }
    }

    public FixedList<GeographicRegionItem> GetNotaryOfficePlaces() {
      return MainRecorderOffice.GetLinks<GeographicRegionItem>("MainRecorderOffice_NotaryOfficePlaces",
                                                               (x, y) => x.Name.CompareTo(y.Name));
    }

    public FixedList<GeographicRegionItem> GetPrivateDocumentIssuePlaces() {
      return MainRecorderOffice.GetLinks<GeographicRegionItem>("MainRecorderOffice_PrivateDocumentIssuePlace",
                                                               (x, y) => x.Name.CompareTo(y.Name));
    }

    public FixedList<GeographicRegionItem> GetJudicialDocumentIssuePlaces() {
      return MainRecorderOffice.GetLinks<GeographicRegionItem>("MainRecorderOffice_JudicialDocumentIssuePlace",
                                                               (x, y) => x.Name.CompareTo(y.Name));
    }

    public FixedList<GeographicRegionItem> GetMunicipalities() {
      return this.GetLinks<GeographicRegionItem>("RecorderOffice_Municipalities",
                                                 (x, y) => x.Name.CompareTo(y.Name));
    }

    public FixedList<Person> GetRecorderOfficials() {
      return this.GetLinks<Person>("RecorderOffice_RecorderOfficials",
                                   (x, y) => x.FamilyFullName.CompareTo(y.FamilyFullName));
    }

    public FixedList<Person> GetRecorderOfficials(TimePeriod period) {
      return this.GetLinks<Person>("RecorderOffice_RecorderOfficials", period,
                                   (x, y) => x.FamilyFullName.CompareTo(y.FamilyFullName));
    }

    public FixedList<Organization> GetPropertyTitleOffices() {
      return MainRecorderOffice.GetLinks<Organization>("MainRecorderOffice_PropertyTitleOffices",
                                                       (x, y) => x.FullName.CompareTo(y.FullName));

    }

    public FixedList<Contact> GetPropertyTitleSigners(TimePeriod period) {
      return MainRecorderOffice.GetLinks<Contact>("MainRecorderOffice_PropertyTitleSigners", period,
                                                  (x, y) => x.FullName.CompareTo(y.FullName));
    }

    public FixedList<RecordingBook> GetRecordingBooks(RecordingSection sectionType) {
      return RecordingBooksData.GetRecordingBooksInSection(this, sectionType);
    }

    public FixedList<RecordingBook> GetRootRecordingBooks() {
      return RecordingBooksData.GetRootRecordingBooks(this);
    }

    internal FilesFolder GetRootImagesFolder() {
      FilesFolderList rootFolders = RootFilesFolder.GetRootFilesFolders();

      foreach (FilesFolder filesFolder in rootFolders) {
        if (filesFolder.OwnerId == this.Id) {
          return filesFolder;
        }
      }
      return FilesFolder.Empty;
    }

    protected override void ImplementsLoadObjectData(DataRow row) {
      base.ImplementsLoadObjectData(row);
      this.tag = (string) row["NickName"];
    }

    protected override void ImplementsSave() {
      base.ImplementsSave();
    }

    #endregion Public methods

  } // class RecorderOffice

} // namespace Empiria.Land.Registration