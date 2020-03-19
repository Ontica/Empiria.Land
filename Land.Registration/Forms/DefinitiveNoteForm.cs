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

using Empiria.Contacts;
using Empiria.OnePoint.EFiling;

namespace Empiria.Land.Registration.Forms {

  /// <summary>Definitive Note Registration Request data form.</summary>
  public class DefinitiveNoteForm : IForm, INotaryForm {

    #region Constructors and parsers


    private DefinitiveNoteForm(EFilingRequest request) {
      Load(request);
    }


    static internal DefinitiveNoteForm Parse(EFilingRequest request) {
      Assertion.AssertObject(request, "request");

      return new DefinitiveNoteForm(request);
    }


    private void Load(EFilingRequest request) {
      JsonObject json = request.ApplicationForm;

      this.RealPropertyDescription = RealPropertyDescription.Parse(json.Slice("propertyData"));

      this.Operation = json.Get<string>("operation");
      this.Grantees = json.Get<string>("grantees");
      this.Grantors = json.Get<string>("grantors");
      this.Observations = json.Get("observations", String.Empty);

      this.Notary = request.Agent;
      this.NotaryOffice = request.Agency;
      this.ESign = request.ElectronicSign;
      this.AuthorizationTime = request.AuthorizationTime;
    }

    #endregion Constructors and parsers


    #region Properties


    public LandSystemFormType FormType {
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



    public string Observations {
      get;
      private set;
    } = String.Empty;


    public Contact NotaryOffice {
      get;
      private set;
    }


    public Contact Notary {
      get;
      private set;
    }


    public string ESign {
      get;
      private set;
    }


    public DateTime AuthorizationTime {
      get;
      private set;
    }


    #endregion Properties

  }  // class DefinitiveNoteForm

}  // namespace Empiria.Land.Registration.Forms
