/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Filing Services                              Component : Html Forms                            *
*  Assembly : Empiria.Land.UI.dll                          Pattern   : Transform View                        *
*  Type     : LandHtmlFormTransformer                      License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Transforms data forms to their HTML representation.                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Registration;
using Empiria.Land.Registration.Forms;

namespace Empiria.Land.UI {

  /// <summary>Transforms data forms to their HTML representation.</summary>
  public class LandHtmlFormTransformer {

    private readonly IForm _form;


    public LandHtmlFormTransformer(IForm form) {
      this._form = form;
    }


    public string GetHtml() {
      switch (this._form.FormType) {

        case LandSystemFormType.PreventiveNoteRegistrationForm:
          return TransformPreventiveNoteRegistrationForm((PreventiveNoteForm) this._form);


        case LandSystemFormType.DefinitiveNoteRegistrationForm:
          return TransformDefinitiveNoteRegistrationForm((DefinitiveNoteForm) this._form);

        default:
          throw Assertion.AssertNoReachThisCode(
                $"There is not defined an HTML handler for forms of type {this._form.FormType}.");
      }
    }

    #region Private methods


    private string TransformDefinitiveNoteRegistrationForm(DefinitiveNoteForm form) {
      string html = GetTemplate(_form.FormType.ToString());

      html = html.Replace("{{PREPARED.BY.SECTION}}", TransformPreparedBySection(form));

      html = html.Replace("{{REAL.PROPERTY.SECTION}}", TransformRealPropertySection(form));

      html = html.Replace("{{OPERATION}}", form.Operation);
      html = html.Replace("{{GRANTORS}}", form.Grantors);
      html = html.Replace("{{GRANTEES}}", form.Grantees);
      html = html.Replace("{{OBSERVATIONS}}", form.Observations);

      return html;

    }

    private string TransformPreventiveNoteRegistrationForm(PreventiveNoteForm form) {
      string html = GetTemplate(_form.FormType.ToString());

      html = html.Replace("{{PREPARED.BY.SECTION}}", TransformPreparedBySection(form));

      html = html.Replace("{{REAL.PROPERTY.SECTION}}", TransformRealPropertySection(form));

      html = html.Replace("{{PROJECTED.OPERATION}}", form.ProjectedOperation);
      html = html.Replace("{{GRANTORS}}", form.Grantors);
      html = html.Replace("{{GRANTEES}}", form.Grantees);
      html = html.Replace("{{APPLY.TO.A.NEW.PARTITION}}", form.ApplyToANewPartition ? "Sí" : "No");
      html = html.Replace("{{NEW.PARTITION.NAME}}", form.NewPartitionName);
      html = html.Replace("{{OBSERVATIONS}}", form.Observations);

      return html;
    }


    private string TransformPreparedBySection(NotaryForm form) {
      string html = GetTemplate("PreparedBySection");

      html = html.Replace("{{NOTARY.NAME}}", form.Notary.FullName);
      html = html.Replace("{{NOTARY.OFFICE.NAME}}", form.NotaryOffice.FullName);
      html = html.Replace("{{AUTHORIZATION.TIME}}", form.AuthorizationTime.ToString("dd/MMM/yyyy HH:mm"));
      html = html.Replace("{{ELECTRONIC.SIGN}}", EmpiriaString.DivideLongString(form.ESign, 96, "&#8203;"));

      return html;
    }


    private string TransformRealPropertySection(IRealPropertyForm form) {
      if (form.RealPropertyDescription.OverRegistredPropertyUID) {
        var realEstate = RealEstate.TryParseWithUID(form.RealPropertyDescription.PropertyUID);

        return TransformRegisteredRealPropertySection(realEstate);
      } else {
        return TransformRealPropertyOnRecordingBookSection(form.RealPropertyDescription);
      }
    }


    private string TransformRealPropertyOnRecordingBookSection(RealPropertyDescription property) {
      string html = GetTemplate("RealPropertyOnRecordingBookSection");

      html = html.Replace("{{DISTRICT.NAME}}", property.RecorderOffice.Alias);
      html = html.Replace("{{MUNICIPALITY.NAME}}", property.Municipality.Name);
      html = html.Replace("{{RECORDING.BOOK.NAME}}", property.RecordingBook.AsText);
      html = html.Replace("{{RECORDING.NO}}", property.RecordingNo);
      html = html.Replace("{{PARTITION.NAME}}", property.RecordingFraction);
      html = html.Replace("{{CADASTRAL.KEY}}", property.CadastralKey);
      html = html.Replace("{{REAL.PROPERTY.TYPE}}", property.RealPropertyType.Name);
      html = html.Replace("{{REAL.PROPERTY.NAME}}", property.RealPropertyName);
      html = html.Replace("{{LOCATION}}", property.Location);
      html = html.Replace("{{METES.AND.BOUNDS}}", property.MetesAndBounds);
      html = html.Replace("{{SEARCH.NOTES}}", property.SearchNotes);

      return html;
    }


    private string TransformRegisteredRealPropertySection(RealEstate realProperty) {
      string html = GetTemplate("RegisteredRealPropertySection");

      html = html.Replace("{{REAL.PROPERTY.UID}}", realProperty.UID);

      html = html.Replace("{{DISTRICT.NAME}}", realProperty.RecorderOffice.Alias);
      html = html.Replace("{{MUNICIPALITY.NAME}}", realProperty.Municipality.Name);
      html = html.Replace("{{CADASTRAL.KEY}}", realProperty.CadastralKey);
      html = html.Replace("{{REAL.PROPERTY.TYPE}}", realProperty.Kind);
      html = html.Replace("{{REAL.PROPERTY.NAME}}", realProperty.Name);
      html = html.Replace("{{LOCATION}}", realProperty.Description);
      html = html.Replace("{{METES.AND.BOUNDS}}", realProperty.MetesAndBounds);
      return html;
    }


    private string GetTemplate(string formTemplateName) {
      string templatesPath = ConfigurationData.GetString("Templates.Path");
      string templateFileName = "template.form." + formTemplateName + ".txt";

      string fullPath = System.IO.Path.Combine(templatesPath, templateFileName);

      string template = System.IO.File.ReadAllText(fullPath);

      return template;
    }

    #endregion Private methods

  }  //class LandHtmlFormTransformer

}  // namespace Empiria.Land.UI
