/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Services                       Component : Filing Forms                            *
*  Assembly : Empiria.Land.Registration.dll              Pattern   : Abstract class                          *
*  Type     : NotaryForm                                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes a Land System data form emitted by a notary.                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Contacts;
using Empiria.Json;
using Empiria.OnePoint.EFiling;

namespace Empiria.Land.Registration.Forms {

  /// <summary>Describes a Land System data form emitted by a notary.</summary>
  public abstract class NotaryForm : IForm {

    #region Constructors and parsers

    protected NotaryForm(EFilingRequest request) {
      Load(request);
    }


    private void Load(EFilingRequest request) {
      this.Notary = request.Agent;
      this.NotaryOffice = request.Agency;
      this.ESign = request.SecurityData.ElectronicSign;
      this.AuthorizationTime = request.AuthorizationTime;

      this.LoadApplicationFormData(request.ApplicationForm);
    }

    #endregion Constructors and parsers

    #region Properties


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


    public string Observations {
      get;
      protected set;
    } = String.Empty;


    #endregion Properties


    #region Abstract members

    public abstract LandSystemFormType FormType {
      get;
    }

    protected abstract void LoadApplicationFormData(JsonObject appFormAsJson);

    #endregion Abstract members


  }  // class PreventiveNoteForm

}  // namespace Empiria.Land.Registration.Forms
