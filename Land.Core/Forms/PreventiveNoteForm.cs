/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Filing Services                            Component : Filing Forms                            *
*  Assembly : Empiria.Land.Registration.dll              Pattern   : Information Holder                      *
*  Type     : PreventiveNoteForm                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Preventive Note Registration Request data form.                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Json;

using Empiria.OnePoint.EFiling;

namespace Empiria.Land.Registration.Forms {

  /// <summary>Preventive Note Registration Request data form.</summary>
  public class PreventiveNoteForm : NotaryForm, IRealPropertyForm {

    #region Constructors and parsers

    protected internal PreventiveNoteForm(EFilingRequest request) : base(request) {
      // no-op
    }


    static internal PreventiveNoteForm Parse(EFilingRequest request) {
      Assertion.Require(request, "request");

      return new PreventiveNoteForm(request);
    }


    protected override void LoadApplicationFormData(JsonObject appFormAsJson) {
      this.RealPropertyDescription = RealPropertyDescription.Parse(appFormAsJson.Slice("propertyData"));

      this.ProjectedOperation = appFormAsJson.Get<string>("projectedOperation");
      this.Grantees = appFormAsJson.Get<string>("grantees");
      this.Grantors = appFormAsJson.Get<string>("grantors");
      this.ApplyToANewPartition = appFormAsJson.Get("createPartition", false);
      this.NewPartitionName = appFormAsJson.Get("partitionName", String.Empty);

      base.Observations = appFormAsJson.Get("observations", String.Empty);
    }


    #endregion Constructors and parsers

    #region Properties

    public override LandSystemFormType FormType {
      get {
        return LandSystemFormType.PreventiveNoteRegistrationForm;
      }
    }


    public RealPropertyDescription RealPropertyDescription {
      get;
      private set;
    }


    public string ProjectedOperation {
      get;
      private set;
    } = String.Empty;


    public string Grantors {
      get;
      private set;
    } = String.Empty;


    public string Grantees {
      get;
      private set;
    } = String.Empty;


    public bool ApplyToANewPartition {
      get;
      private set;
    }


    public string NewPartitionName {
      get;
      private set;
    } = String.Empty;


    #endregion Properties

  }  // class PreventiveNoteForm

}  // namespace Empiria.Land.Registration.Forms
