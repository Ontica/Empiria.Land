/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Services                       *
*  Namespace : Empiria.Land.WebApi.Models                     Assembly : Empiria.Land.WebApi.dll             *
*  Type      : PendingNoteRequest                             Pattern  : External Interfacer                 *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Data transfer object that holds information about a Pending note request from                 *
*              an external transaction system.                                                               *
*                                                                                                            *
********************************* Copyright (c) 2009-2017. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Linq;

using Empiria.Contacts;
using Empiria.Json;

using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.WebApi.Models {

  /// <summary>Data transfer object that holds information about a Pending note request from
  /// an external transaction system.</summary>
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

    /// <summary>Indicates if the Pending note is over a new projected partition.</summary>
    public bool IsPartition {
      get;
      set;
    } = false;

    /// <summary>Optionally indicates the name of the partition when IsPartition is true.</summary>
    public string PartitionName {
      get;
      set;
    } = String.Empty;


    /// <summary>Optionally indicates the partition's size when IsPartition is true.</summary>
    public decimal PartitionSize {
      get;
      set;
    } = 0m;

    /// <summary>Optionally indicates the partition's location when IsPartition is true.</summary>
    public string PartitionLocation {
      get;
      set;
    } = String.Empty;

    /// <summary>Optionally indicates the partition's metes and bounds when IsPartition is true.</summary>
    public string PartitionMetesAndBounds {
      get;
      set;
    } = String.Empty;

    /// <summary>A note about the recording conditions or notary's needs.</summary>
    public string RecordingObservations {
      get;
      set;
    } = String.Empty;

    /// <summary>Returns the transaction type used for all pending note requests.</summary>
    protected override LRSTransactionType TransactionType {
      get {
        return LRSTransactionType.Parse(699);
      }
    }

    /// <summary>Returns the document type used for all pending note requests.</summary>
    protected override LRSDocumentType DocumentType {
      get {
        return LRSDocumentType.Parse(708);
      }
    }

    /// <summary>Returns agency or notary information responsible of the request.</summary>
    protected override Contact Agency {
      get {
        // TODO: Convert to notary.NotaryOffice
        return Contact.Parse(this.NotaryId);
      }
    }

    #endregion Public properties

    #region Public methods

    public override void AssertIsValid() {
      base.AssertIsValid();

      this.CleanData();

      Assertion.Assert(this.IsNotaryValid(),
        "No tengo registrado al notario que envía el aviso: '{0}'.", this.NotaryId);
      Assertion.Assert(this.IsProjectedActValid(),
        "No reconozco la operación proyectada: '{0}'", this.ProjectedActId);

      Assertion.AssertObject(this.ProjectedOwner,
        "Necesito que el campo 'A favor de:' no sea vacío.");
      Assertion.AssertObject(this.RealPropertyUID,
        "Requiero se proporcione el folio electrónico del predio.");

      Assertion.AssertObject(RealEstate.TryParseWithUID(this.RealPropertyUID),
        "No tengo registrado ningún predio con folio real '{0}'.", this.RealPropertyUID);

      if (!this.IsPartition) {
        return;
      }

      Assertion.AssertObject(this.PartitionName,
        "Necesito la denominación o nombre de la fracción (si no consta debe teclearse 'Sin número')");
      Assertion.AssertObject(this.PartitionLocation,
        "Requiero conocer la ubicación de la fracción.");
      Assertion.Assert(this.PartitionLocation.Length >= 20,
        "Favor de especificar la ubicación de la fracción en 20 o más caracteres: " +
        "calle, número, colonia, etc.");
      Assertion.Assert(this.PartitionSize >= 10m,
        "Las fracciones no pueden ser menores a 10 metros cuadrados.");
      Assertion.AssertObject(this.PartitionMetesAndBounds,
        "Necesito se proporcionen las medidas y colindancias de la fracción.");
      Assertion.Assert(this.PartitionMetesAndBounds.Length >= 50,
        "Las medidas y colindancias deberían ser más específicas (50 o más caracteres).");
    }

    /// <summary>Creates a Land Transaction using the data of this certificate request.</summary>
    /// <returns>The Land Transaction created instance.</returns>
    internal LRSTransaction CreateTransaction() {
      this.AssertIsValid();

      return base.CreateLRSTransaction();
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
