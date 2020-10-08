/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Filing Services                            Component : Filing Forms                            *
*  Assembly : Empiria.Land.Registration.dll              Pattern   : Information Holder                      *
*  Type     : DefinitiveNoteForm                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Definitive Note Registration Request data form.                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Json;

using Empiria.OnePoint.EFiling;

namespace Empiria.Land.Registration.Forms {

  /// <summary>Definitive Note Registration Request data form.</summary>
  public class DefinitiveNoteForm : NotaryForm, IRealPropertyForm {

    #region Constructors and parsers

    protected internal DefinitiveNoteForm(EFilingRequest request) : base(request) {
      // no-op
    }


    static internal DefinitiveNoteForm Parse(EFilingRequest request) {
      Assertion.AssertObject(request, "request");

      return new DefinitiveNoteForm(request);
    }


    protected override void LoadApplicationFormData(JsonObject appFormAsJson) {
      this.RealPropertyDescription = RealPropertyDescription.Parse(appFormAsJson.Slice("propertyData"));

      this.Operation = appFormAsJson.Get<string>("operation");
      this.Grantees = appFormAsJson.Get<string>("grantees");
      this.Grantors = appFormAsJson.Get<string>("grantors");
      this.Observations = appFormAsJson.Get("observations", String.Empty);
    }


    #endregion Constructors and parsers

    #region Properties

    public override LandSystemFormType FormType {
      get {
        return LandSystemFormType.DefinitiveNoteRegistrationForm;
      }
    }


    public RealPropertyDescription RealPropertyDescription {
      get;
      private set;
    }


    public string Operation {
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


    #endregion Properties

  }  // class DefinitiveNoteForm

}  // namespace Empiria.Land.Registration.Forms
