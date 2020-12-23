/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Legal Instruments                          Component : Domain Layer                            *
*  Assembly : Empiria.Land.Instruments.dll               Pattern   : Power typed Information Holder          *
*  Type     : Issuer                                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents a legal instrument issuer or attester, like a notary, judge or official authority.  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Contacts;
using Empiria.Geography;
using Empiria.Json;
using Empiria.StateEnums;

using Empiria.Ontology;
using Empiria.Land.Instruments.Data;

namespace Empiria.Land.Instruments {

  /// <summary>Represents a legal instrument issuer or attester, like a notary, judge or
  /// official authority.</summary>
  [PartitionedType(typeof(IssuerType))]
  internal class Issuer : BaseObject {

    #region Constructors and parsers

    protected Issuer(IssuerType issuerType) : base(issuerType) {
      // Required by Empiria Framework for all partitioned types.
    }

    internal Issuer(IssuerType issuerType, IssuerFields data) : base(issuerType) {
      this.LoadData(data);
    }

    static public Issuer Parse(int id) => BaseObject.ParseId<Issuer>(id);

    static public Issuer Parse(string uid) => BaseObject.ParseKey<Issuer>(uid);

    static public Issuer Empty => BaseObject.ParseEmpty<Issuer>();

    static public FixedList<Issuer> GetList(IssuersSearchCommand searchCommand) {
      return IssuersData.GetList(searchCommand);
    }

    #endregion Constructors and parsers

    #region Properties

    public IssuerType IssuerType {
      get {
        return (IssuerType) base.GetEmpiriaType();
      }
    }


    [DataField("IssuerName")]
    internal string Name {
      get; private set;
    }


    [DataField("EntityName")]
    internal string EntityName {
      get; private set;
    }


    [DataField("OfficialPosition")]
    internal string OfficialPosition {
      get; private set;
    }


    [DataField("OfficeName")]
    internal string OfficeName {
      get; private set;
    }


    [DataField("PlaceName")]
    internal string PlaceName {
      get; private set;
    }


    [DataField("IssuerExtData")]
    internal JsonObject ExtData {
      get; private set;
    }


    internal string Keywords {
      get {
        return EmpiriaString.BuildKeywords(this.Name, this.EntityName, this.OfficeName,
                                           this.PlaceName, this.OfficialPosition);
      }
    }


    [DataField("RelatedContactId")]
    internal Contact RelatedContact {
      get; private set;
    }


    [DataField("RelatedEntityId")]
    internal Organization RelatedEntity {
      get; private set;
    }


    [DataField("RelatedOfficeId")]
    internal Organization RelatedOffice {
      get; private set;
    }


    [DataField("RelatedPlaceId")]
    internal GeographicRegion RelatedPlace {
      get; private set;
    }


    [DataField("IssuerFromDate")]
    internal DateTime FromDate {
      get; private set;
    } = ExecutionServer.DateMinValue;


    [DataField("IssuerToDate")]
    internal DateTime ToDate {
      get; private set;
    } = ExecutionServer.DateMaxValue;


    [DataField("IssuerStatus", Default = EntityStatus.Active)]
    internal EntityStatus Status {
      get; private set;
    }


    [DataField("PostedById")]
    internal int PostedById {
      get; private set;
    }


    [DataField("PostingTime")]
    internal DateTime PostingTime {
      get; private set;
    }


    #endregion Properties


    #region Methods

    private void LoadData(IssuerFields data) {
      throw new NotImplementedException();
    }


    internal void Update(IssuerFields data) {
      throw new NotImplementedException();
    }


    protected override void OnSave() {
      IssuersData.WriteIssuer(this);
    }


    #endregion Methods

  }  // class Issuer

}  // namespace Empiria.Land.Instruments
