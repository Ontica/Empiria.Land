/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecorderOffice                                 Pattern  : Storage Item                        *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : A recorder of deeds office.                                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Contacts;
using Empiria.DataTypes.Time;
using Empiria.Documents.IO;
using Empiria.Geography;

using Empiria.Land.Data;

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

    static public new RecorderOffice Parse(string uid) {
      return BaseObject.ParseKey<RecorderOffice>(uid);
    }

    static public FixedList<RecorderOffice> GetList() {
      return BaseObject.GetList<RecorderOffice>("ContactStatus = 'A'")
                       .ToFixedList();
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


    public FixedList<GeographicRegion> GetNotaryOfficePlaces() {
      throw new NotImplementedException("GetNotaryOfficePlaces()");

      //return MainRecorderOffice.GetLinks<GeographicRegion>("RecorderOffice->NotaryOfficePlaces",
      //                                                     (x, y) => x.Name.CompareTo(y.Name));
    }

    public FixedList<GeographicRegion> GetPrivateDocumentIssuePlaces() {
      throw new NotImplementedException("GetPrivateDocumentIssuePlaces()");
      //return MainRecorderOffice.GetLinks<GeographicRegion>("RecorderOffice->PrivateDocumentIssuePlace",
      //                                                     (x, y) => x.Name.CompareTo(y.Name));
    }

    public FixedList<GeographicRegion> GetJudicialDocumentIssuePlaces() {
      throw new NotImplementedException("GetJudicialDocumentIssuePlaces()");

      //return MainRecorderOffice.GetLinks<GeographicRegion>("RecorderOffice->JudicialDocumentIssuePlace",
      //                                                         (x, y) => x.Name.CompareTo(y.Name));
    }


    public FixedList<Municipality> GetMunicipalities() {
      return base.ExtendedData.GetList<Municipality>("municipalities", false)
                              .ToFixedList();
    }


    public FixedList<RecordingSection> GetRecordingSections() {
      return RecordingSection.GetList(this);
    }


    public FixedList<Person> GetRecorderOfficials() {
      throw new NotImplementedException("GetRecorderOfficials()");

      //return this.GetLinks<Person>("RecorderOffice->RecorderOfficials",
      //                             (x, y) => x.FamilyFullName.CompareTo(y.FamilyFullName));
    }

    public FixedList<Person> GetRecorderOfficials(TimeFrame period) {
      throw new NotImplementedException("GetRecorderOfficials(TimeFrame)");

      //return this.GetLinks<Person>("RecorderOffice->RecorderOfficials", period,
      //                             (x, y) => x.FamilyFullName.CompareTo(y.FamilyFullName));
    }

    public FixedList<Organization> GetPropertyTitleOffices() {
      throw new NotImplementedException("GetPropertyTitleOffices()");

      //return MainRecorderOffice.GetLinks<Organization>("RecorderOffice->PropertyTitleOffices",
      //                                                 (x, y) => x.FullName.CompareTo(y.FullName));

    }

    public FixedList<Person> GetPropertyTitleSigners() {
      throw new NotImplementedException("GetPropertyTitleSigners()");

      //return MainRecorderOffice.GetLinks<Person>("RecorderOffice->PropertyTitleSigners",
      //                                            (x, y) => x.FamilyFullName.CompareTo(y.FamilyFullName));
    }


    public Person GetSigner() {
      return ExtendedData.Get("recorderOfficerSigner", Person.Empty);
    }


    public string GetPlace() {
      return ExtendedData.Get("recorderOfficePlace", "Lugar no determinado");
    }

    public FixedList<RecordingBook> GetRecordingBooks(RecordingSection sectionType) {
      return RecordingBooksData.GetRecordingBooksInSection(this, sectionType);
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
