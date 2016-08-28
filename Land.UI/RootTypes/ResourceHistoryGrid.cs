/* Empiria Land ***********************************************************************************************
*                                                                                                             *
*  Solution  : Empiria Land                                    System   : Land Registration System            *
*  Namespace : Empiria.Land.UI                                 Assembly : Empiria.Land.UI                     *
*  Type      : ResourceHistoryGrid                             Pattern  : Standard class                      *
*  Version   : 2.1                                             License  : Please read license.txt file        *
*                                                                                                             *
*  Summary   : Generates a grid HTML content that displays the full resource's history.                       *
*                                                                                                             *
********************************** Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

using Empiria.Land.Registration;
using Empiria.Land.Certification;

namespace Empiria.Land.UI {

  /// <summary>Generates a grid HTML content that displays the full resource's history.</summary>
  public class ResourceHistoryGrid {

    #region Fields

    private Resource _resource = null;

    #endregion Fields

    #region Constructors and parsers

    private ResourceHistoryGrid(Resource resource) {
      _resource = resource;
    }

    static public string Parse(Resource resource) {
      var grid = new ResourceHistoryGrid(resource);

      return grid.GetHtml();
    }

    #endregion Constructors and parsers

    #region Private methods

    private string GetHtml() {
      FixedList<IResourceTractItem> resourceHistory = _resource.GetFullRecordingActsTractWithCertificates();

      string html = this.GetTitle() + this.GetHeader();
      for (int i = resourceHistory.Count - 1; 0 <= i; i--) {
        IResourceTractItem item = resourceHistory[i];

        if (item is RecordingAct) {
          html += this.GetRecordingActRow((RecordingAct) item, i);
        } else if (item is Certificate) {
          html += this.GetCertificateRow((Certificate) item, i);
        } else {
          Assertion.AssertNoReachThisCode("Invalid resource history tract item type.");
        }
      }
      return HtmlFormatters.TableWrapper(html);
    }

    private string GetTitle() {
      string template =
            "<tr class='detailsTitle'>" +
              "<td colspan='6'>Historia del predio <b>{{RESOURCE.UID}}</b></td>" +
            "</tr>";

      return template.Replace("{{RESOURCE.UID}}", this._resource.UID);
    }

    private string GetHeader() {
      string template =
            "<tr class='detailsHeader'>" +
              "<td>Present/Registro</td>" +
              "<td style='width:160px'>Acto jurídico</td>" +
              "<td style='white-space:nowrap'>Antecedente / Fracción</td>" +
              "<td style='width:200px'>Registrado en</td>" +
              "<td style='white-space:nowrap'>Img</td>" +
              "<td style ='width:160px'>Registró</ td >" +
            "</tr>";
      return template;
    }

    private string GetCertificateRow(Certificate certificate, int index) {
      const string template =
         "<tr class='{{CLASS}}'>" +
           "<td>{{PRESENTATION.DATE}}<br/>{{ISSUE.DATE}}</td>" +
           "<td style='white-space:normal'>Emisión de certificado</td>" +
           "<td>{{CERTIFICATE.TYPE}}</td>" +
           "<td style='white-space:nowrap;'>" +
             "<a href='javascript:doOperation(\"onSelectCertificate\", {{CERTIFICATE.ID}});'>" +
                 "{{CERTIFICATE.UID}}</a>" +
             "<br>{{TRANSACTION}}</td>" +
           "<td>&nbsp;</td>" +
           "<td>{{ISSUED.BY}}</td>" +
         "</tr>";

      string row = template.Replace("{{CLASS}}", (index % 2 == 0) ? "detailsItem" : "detailsOddItem");

      row = row.Replace("{{PRESENTATION.DATE}}",
                        HtmlFormatters.GetDateAsText(certificate.Transaction.PresentationTime));
      row = row.Replace("{{ISSUE.DATE}}",
                        HtmlFormatters.GetDateAsText(certificate.IssueTime));
      row = row.Replace("{{CERTIFICATE.TYPE}}", certificate.CertificateType.DisplayName);
      row = row.Replace("{{CERTIFICATE.ID}}", certificate.Id.ToString());
      row = row.Replace("{{CERTIFICATE.UID}}", certificate.UID);
      row = row.Replace("{{TRANSACTION}}", "Trámite:" + certificate.Transaction.UID);
      row = row.Replace("{{ISSUED.BY}}", certificate.IssuedBy.Nickname);

      return row;
    }

    private string GetRecordingActRow(RecordingAct recordingAct, int index) {
      const string template =
        "<tr class='{{CLASS}}'>" +
          "<td>{{PRESENTATION.DATE}}<br/>{{AUTHORIZATION.DATE}}</td>" +
          "<td style='white-space:normal;width:260px'>{{RECORDING.ACT}}</td>" +
          "<td style='white-space:normal;'>{{PARTITION}}</td>" +
          "<td style='white-space:{{WHITE-SPACE}};'>" +
            "<a href='javascript:doOperation(\"onSelectDocument\", {{DOCUMENT.ID}}, {{RECORDING.ACT.ID}});'>" +
                "{{DOCUMENT.OR.RECORDING}}</a>" +
            "<br>{{TRANSACTION}}</td>" +
          "<td style='white-space:nowrap'>{{IMAGING.LINKS}}</td>" +
          "<td>{{RECORDED.BY}}</td>" +
        "</tr>";

      string row = template.Replace("{{CLASS}}", (index % 2 == 0) ? "detailsItem" : "detailsOddItem");

      row = row.Replace("{{RECORDING.ACT}}", recordingAct.DisplayName);
      row = row.Replace("{{PARTITION}}", this.GetPartitionOrAntecedentCell(recordingAct));
      if (!recordingAct.PhysicalRecording.IsEmptyInstance) {
        row = row.Replace("{{DOCUMENT.OR.RECORDING}}", recordingAct.PhysicalRecording.AsText);
        row = row.Replace("{{TRANSACTION}}", this.OnSelectDocumentButton(recordingAct));
        row = row.Replace("{{WHITE-SPACE}}", "normal");
      } else {
        row = row.Replace("{{DOCUMENT.OR.RECORDING}}", recordingAct.Document.UID);
        row = row.Replace("{{TRANSACTION}}", "Trámite:" + recordingAct.Document.GetTransaction().UID);
        row = row.Replace("{{WHITE-SPACE}}", "nowrap");
      }
      row = HtmlFormatters.SetPresentationAndAuthorizationDates(row, recordingAct.Document);
      row = row.Replace("{{RECORDED.BY}}", recordingAct.RegisteredBy.Nickname);

      row = row.Replace("{{DOCUMENT.ID}}", recordingAct.Document.Id.ToString());

      row = row.Replace("{{IMAGING.LINKS}}", GetImagingLinks(recordingAct));

      row = row.Replace("{{RECORDING.ACT.ID}}", recordingAct.Id.ToString());

      return row;
    }

    private string GetImagingLinks(RecordingAct recordingAct) {
      RecordingDocument document = recordingAct.Document;

      if (!document.HasImageSet && !document.HasAuxiliarImageSet &&
           recordingAct.PhysicalRecording.IsEmptyInstance) {
        return "&nbsp";
      }
      string html = String.Empty;

      if (document.HasImageSet) {
        html = "<a href='javascript:doOperation(\"onSelectImageSet\", {{DOCUMENT.IMAGE.SET.ID}});'>" +
                  "<img src='../themes/default/bullets/scribble_doc_sm.gif' title='Instrumento registral'></a>";
        html = html.Replace("{{DOCUMENT.IMAGE.SET.ID}}", document.ImageSetId.ToString());
      }

      if (document.HasAuxiliarImageSet) {
        html += "<a href='javascript:doOperation(\"onSelectImageSet\", {{AUXILIAR.IMAGE.SET.ID}});'>" +
                   "<img src='../themes/default/bullets/clip.gif' title='Anexos al instrumento registral'></a>";
        html = html.Replace("{{AUXILIAR.IMAGE.SET.ID}}", document.AuxiliarImageSetId.ToString());
      }

      if (!recordingAct.PhysicalRecording.IsEmptyInstance &&
           recordingAct.PhysicalRecording.RecordingBook.HasImageSet) {
        html += "<a href='javascript:doOperation(\"onSelectImageSet\", {{RECORDING.BOOK.IMAGE.SET.ID}});'>" +
                   "<img src='../themes/default/bullets/book.gif' title='Libro registral'></a>";
        html = html.Replace("{{RECORDING.BOOK.IMAGE.SET.ID}}",
                            recordingAct.PhysicalRecording.RecordingBook.ImageSetId.ToString());
      }

      return html;
    }

    private string GetPartitionOrAntecedentCell(RecordingAct recordingAct) {
      if (!(this._resource is RealEstate)) {
        return "&nbsp;";
      }

      if (Resource.IsCreationalRole(recordingAct.ResourceRole)) {

        var realEstate = (RealEstate) recordingAct.Resource;

        if (recordingAct.ResourceRole == ResourceRole.Created) {
          return "Sin antecedente registral";
        } else if (realEstate.Equals(this._resource)) {
          var temp = "Creado como <b>" + realEstate.PartitionNo + "</b> del predio " +
                      "<a href='javascript:doOperation(\"displayResourcePopupWindow\", {{RESOURCE.ID}}, {{RECORDING.ACT.ID}});'>" +
                      "{{RESOURCE.UID}}</a>";
          temp = temp.Replace("{{RESOURCE.ID}}", recordingAct.RelatedResource.Id.ToString());
          temp = temp.Replace("{{RECORDING.ACT.ID}}", recordingAct.Id.ToString());
          temp = temp.Replace("{{RESOURCE.UID}}", HtmlFormatters.NoWrap(recordingAct.RelatedResource.UID));

          return temp;
        } else {
          var temp = "Sobre <b>" + realEstate.PartitionNo + "</b> con folio real " +
                      "<a href='javascript:doOperation(\"displayResourcePopupWindow\", {{RESOURCE.ID}}, {{RECORDING.ACT.ID}});'>" +
                      "{{RESOURCE.UID}}</a>";
          temp = temp.Replace("{{RESOURCE.ID}}", realEstate.Id.ToString());
          temp = temp.Replace("{{RECORDING.ACT.ID}}", recordingAct.Id.ToString());
          temp = temp.Replace("{{RESOURCE.UID}}", HtmlFormatters.NoWrap(realEstate.UID));

          return temp;
        }
      }
      return "&nbsp;";
    }

    private string OnSelectDocumentButton(RecordingAct recordingAct) {

      return String.Empty;

      //const string template =
      //    "<a href='javascript:doOperation(\"onSelectRecordingAct\", {{DOCUMENT.ID}}, {{RECORDING.ACT.ID}});'>" +
      //        "Editar este acto</a>";

      //string x = template.Replace("{{DOCUMENT.ID}}", recordingAct.Document.Id.ToString());
      //x = x.Replace("{{RECORDING.ACT.ID}}}", recordingAct.Id.ToString());

      //return x;
    }

    #endregion Private methods

  } // class ResourceHistoryGrid

} // namespace Empiria.Land.UI
