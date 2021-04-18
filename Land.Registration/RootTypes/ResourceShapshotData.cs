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

using Empiria.Json;

namespace Empiria.Land.Registration {

  abstract public class ResourceShapshotData {

    #region Constructors and parsers

    internal ResourceShapshotData Parse(Resource resource, string data) {
      ResourceShapshotData snapshotData;

      if (resource is RealEstate) {
        snapshotData = new RealEstateShapshotData();

      } else if (resource is Association) {
        snapshotData = new AssociationShapshotData();

      } else if (resource is NoPropertyResource) {
        snapshotData = new NoPropertyShapshotData();

      } else {
        snapshotData = new NoPropertyShapshotData();

      }

      return JsonConverter.Merge(data, snapshotData);
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


    public string Kind {
      get;
      internal set;
    } = string.Empty;


    public string Name {
      get;
      internal set;
    } = string.Empty;


    public string Description {
      get;
      internal set;
    } = string.Empty;


    public string Status {
      get;
      internal set;
    }

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

    public string Notes {
      get;
      internal set;
    }


    public int MunicipalityId {
      get;
      internal set;
    }


    public string CadastralKey {
      get;
      internal set;
    }


    public DateTime CadastreLinkingDate {
      get;
      internal set;
    }


    public decimal LotSize {
      get;
      internal set;
    }


    public int LotSizeUnitId {
      get;
      internal set;
    } = -1;


    public string PartitionNo {
      get;
      internal set;
    }


    public string MetesAndBounds {
      get;
      internal set;
    }

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
