/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Recordable Subjects                        Component : Domain Layer                            *
*  Assembly : Empiria.Land.Registration.dll              Pattern   : Data Holder                             *
*  Type     : ResourceShapshotData                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds resource data in order to historically store and retrieve it in recording acts.          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Newtonsoft.Json;

using Empiria.Json;

namespace Empiria.Land.Registration {

  abstract public class ResourceShapshotData {

    #region Constructors and parsers

    internal ResourceShapshotData Parse(Resource resource, string data) {
      if (String.IsNullOrEmpty(data) ||
        !JsonObject.Parse(data).HasItems) {
          return ParseEmptyFor(resource);
      }

      ResourceShapshotData snapshot;

      if (resource is RealEstate) {
        snapshot = new RealEstateShapshotData();

      } else if (resource is Association) {
        snapshot = new AssociationShapshotData();

      } else if (resource is NoPropertyResource) {
        snapshot = new NoPropertyShapshotData();

      } else {
        snapshot = new NoPropertyShapshotData();

      }

      return Empiria.Json.JsonConverter.Merge(data, snapshot);
    }


    static public ResourceShapshotData ParseEmptyFor(Resource resource) {
      ResourceShapshotData snapshot;

      if (resource is RealEstate) {
        snapshot = new RealEstateShapshotData();

      } else if (resource is Association) {
        snapshot = new AssociationShapshotData();

      } else if (resource is NoPropertyResource) {
        snapshot = new NoPropertyShapshotData();

      } else {
        snapshot = new NoPropertyShapshotData();

      }

      snapshot.IsEmptyInstance = true;

      return snapshot;
    }


    static public ResourceShapshotData Empty {
      get {
        return new NoPropertyShapshotData {
          IsEmptyInstance = true
        };
      }
    }

    #endregion Constructors and parsers

    #region Properties

    public bool IsEmptyInstance {
      get; protected set;
    }


    [JsonProperty]
    public string Kind {
      get;
      internal set;
    } = string.Empty;


    [JsonProperty]
    public string Name {
      get;
      internal set;
    } = string.Empty;


    [JsonProperty]
    public string Description {
      get;
      internal set;
    } = string.Empty;


    [JsonProperty]
    public string Status {
      get;
      internal set;
    } = string.Empty;

    #endregion Properties

    public override string ToString() {
      var json = JsonObject.Parse(this);

      json.Remove("IsEmptyInstance");

      json.CleanAll();

      return json.ToString();
    }

  }  // class ResourceShapshotData


  public class RealEstateShapshotData : ResourceShapshotData {

    internal RealEstateShapshotData() {
      // no-op
    }


    [JsonProperty]
    public string Notes {
      get;
      internal set;
    } = string.Empty;


    [JsonProperty]
    public int MunicipalityId {
      get;
      internal set;
    } = -1;


    [JsonProperty]
    public string CadastralKey {
      get;
      internal set;
    } = string.Empty;


    [JsonProperty]
    public DateTime CadastreLinkingDate {
      get;
      internal set;
    } = ExecutionServer.DateMinValue;


    [JsonProperty]
    public decimal LotSize {
      get;
      internal set;
    }


    [JsonProperty]
    public int LotSizeUnitId {
      get;
      internal set;
    } = -1;


    [JsonProperty]
    public string PartitionNo {
      get;
      internal set;
    } = string.Empty;


    [JsonProperty]
    public string MetesAndBounds {
      get;
      internal set;
    } = string.Empty;

  }  // class RealEstateShapshotData


  public class AssociationShapshotData : ResourceShapshotData {

    internal AssociationShapshotData() {
      // no-op
    }

  }  // class AssociationShapshotData


  public class NoPropertyShapshotData : ResourceShapshotData {

    internal NoPropertyShapshotData() {
      // no-op
    }

  }  // class NoPropertyShapshotData


} // namespace Empiria.Land.Registration
