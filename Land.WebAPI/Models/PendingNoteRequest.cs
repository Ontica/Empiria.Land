/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Services                       *
*  Namespace : Empiria.Land.WebApi.Models                     Assembly : Empiria.Land.WebApi.dll             *
*  Type      : PendingNoteRequest                             Pattern  : External Interfacer                 *
*  Version   : 2.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Pending note request from an external transaction system.                                     *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Linq;

using Empiria.Contacts;
using Empiria.Json;

using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.WebApi.Models {

  /// <summary>Pending note request from an external transaction system.</summary>
  public class PendingNoteRequest : LRSExternalTransaction {

    #region Public properties

    /// <summary>The notary Id who send the pending note request.</summary>
    public int NotaryId {
      get;
      set;
    } = -1;

    /// <summary>The projected operation Id (recording act).</summary>
    public int ProjectedActId {
      get;
      set;
    } = -1;

    /// <summary>The real property grantee, receiver or transferee (future projected owner).</summary>
    public string ProjectedOwner {
      get;
      set;
    } = String.Empty;

    /// <summary>The real property unique ID.</summary>
    public string RealPropertyUID {
      get;
      set;
    } = String.Empty;

    public bool IsPartition {
      get;
      set;
    } = false;

    public string PartitionName {
      get;
      set;
    } = String.Empty;

    public decimal PartitionSize {
      get;
      set;
    } = 0m;

    public string PartitionLocation {
      get;
      set;
    } = String.Empty;

    public string PartitionMetesAndBounds {
      get;
      set;
    } = String.Empty;

    public string RecordingObservations {
      get;
      set;
    } = String.Empty;

    protected override LRSTransactionType TransactionType {
      get {
        return LRSTransactionType.Parse(699);
      }
    }

    protected override LRSDocumentType DocumentType {
      get {
        return LRSDocumentType.Parse(708);
      }
    }

    protected override Contact Agency {
      get {
        // TODO: Convert to notary.NotaryOffice
        return Contact.Parse(this.NotaryId);
      }
    }

    #endregion Public properties

    #region Public methods

    protected override void AssertIsValid() {
      base.AssertIsValid();

      this.CleanData();

      Assertion.Assert(this.IsNotaryValid(),
                       "NotaryId field has an invalid or unregistered value.");
      Assertion.Assert(this.IsProjectedActValid(),
                       "ProjectedActId field has an invalid or unregistered value.");

      Assertion.AssertObject(this.ProjectedOwner, "ProjectedOwner");
      Assertion.AssertObject(this.RealPropertyUID, "RealPropertyUID");

      Assertion.AssertObject(Property.TryParseWithUID(this.RealPropertyUID),
                             String.Format("There is not registered a real property with unique ID '{0}'.",
                                            this.RealPropertyUID));
      if (!this.IsPartition) {
        return;
      }

      Assertion.AssertObject(this.PartitionName, "PartitionName");
      Assertion.AssertObject(this.PartitionLocation, "PartitionLocation");
      Assertion.Assert(this.PartitionLocation.Length > 20,
                       "PartitionLocation field is too short.");
      Assertion.Assert(this.PartitionSize > 10m,
                       "PartitionSize field is too small.");
      Assertion.AssertObject(this.PartitionMetesAndBounds, "PartitionMetesAndBounds");
      Assertion.Assert(this.PartitionMetesAndBounds.Length > 50,
                       "PartitionMetesAndBounds field is too short.");
    }

    /// <summary>Creates a Land Transaction using the data of this certificate request.</summary>
    /// <returns>The Land Transaction created instance.</returns>
    internal LRSTransaction CreateTransaction() {
      this.AssertIsValid();

      return new LRSTransaction(this);
    }

    public override JsonObject ToJson() {
      var json = base.ToJson();

      json.Add(new JsonItem("NotaryId", this.NotaryId));
      json.Add(new JsonItem("ProjectedActId", this.ProjectedActId));
      json.Add(new JsonItem("ProjectedOwner", this.ProjectedOwner));
      json.Add(new JsonItem("PropertyUID", this.RealPropertyUID));
      json.Add(new JsonItem("IsPartition", this.IsPartition));
      if (this.IsPartition) {
        json.Add(new JsonItem("PartitionName", this.PartitionName));
        json.Add(new JsonItem("PartitionSize", this.PartitionSize));
        json.Add(new JsonItem("PartitionLocation", this.PartitionLocation));
        json.Add(new JsonItem("PartitionMetesAndBounds", this.PartitionMetesAndBounds));
      }
      json.AddIfValue(new JsonItem("RecordingObservations", this.RecordingObservations));

      return json;
    }

    #endregion Public methods

    #region Private methods

    private void CleanData() {
      this.RealPropertyUID = EmpiriaString.TrimAll(this.RealPropertyUID).ToUpperInvariant();
      this.ProjectedOwner = EmpiriaString.TrimAll(this.ProjectedOwner).ToUpperInvariant();

      this.PartitionName = EmpiriaString.TrimAll(this.PartitionName).ToUpperInvariant();
      this.PartitionLocation = EmpiriaString.TrimAll(this.PartitionLocation).ToUpperInvariant();
      this.PartitionMetesAndBounds = EmpiriaString.TrimAll(this.PartitionMetesAndBounds).ToUpperInvariant();

      this.RecordingObservations = EmpiriaString.TrimAll(this.RecordingObservations).ToUpperInvariant();
    }

    private bool IsNotaryValid() {
      string[] vector = ConfigurationData.GetString("PendingNoteRequest.NotariesArray").Split('~');

      return vector.Contains(this.NotaryId.ToString());
    }

    private bool IsProjectedActValid() {
      string[] vector = ConfigurationData.GetString("PendingNoteRequest.ProjectedActsArray").Split('~');

      return vector.Contains(this.ProjectedActId.ToString());
    }

    #endregion Private methods

  }  // class PendingNoteRequest

}  // namespace Empiria.Land.WebApi.Models
