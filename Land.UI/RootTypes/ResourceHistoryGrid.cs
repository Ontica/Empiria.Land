/* Empiria Land ***********************************************************************************************
*                                                                                                             *
*  Solution  : Empiria Land                                    System   : Land Registration System            *
*  Namespace : Empiria.Land.UI                                 Assembly : Empiria.Land.UI                     *
*  Type      : ResourceHistoryGrid                             Pattern  : Standard class                      *
*  Version   : 3.0                                             License  : Please read license.txt file        *
*                                                                                                             *
*  Summary   : Generates a grid HTML content that displays the full resource's history.                       *
*                                                                                                             *
********************************** Copyright (c) 2009-2017. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;
using Empiria.Land.Certification;

namespace Empiria.Land.UI {

  /// <summary>Generates a grid HTML content that displays the full resource's history.</summary>
  public class ResourceHistoryGrid {

    #region Constructors and parsers

    private ResourceHistoryGrid(Resource resource, LRSTransaction selectedTransaction) {
      this.Resource = resource;
      this.SelectedTransaction = selectedTransaction;
      this.SelectedDocument = selectedTransaction.Document;
    }

    private ResourceHistoryGrid(Resource resource, RecordingDocument selectedDocument) {
      this.Resource = resource;
      this.SelectedDocument = selectedDocument;
      this.SelectedTransaction = selectedDocument.GetTransaction();
    }

    static public string Parse(Resource resource, LRSTransaction selectedTransaction) {
      var grid = new ResourceHistoryGrid(resource, selectedTransaction);

      return grid.GetHtml();
    }

    static public string Parse(Resource resource, RecordingDocument selectedDocument) {
      var grid = new ResourceHistoryGrid(resource, selectedDocument);

      return grid.GetHtml();
    }

    #endregion Constructors and parsers

    #region Public properties

    public Resource Resource {
      get;
    }

    public LRSTransaction SelectedTransaction {
      get;
    }

    public RecordingDocument SelectedDocument {
      get;
    }

    #endregion Public properties


    #region Private methods

    private string GetHtml() {
      FixedList<IResourceTractItem> resourceHistory = Resource.Tract.GetFullRecordingActsWithCertificates();

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

      return template.Replace("{{RESOURCE.UID}}", this.Resource.UID);
    }

    private string GetHeader() {
      string template =
            "<tr class='detailsHeader'>" +
              "<td>Present/Registro</td>" +
              "<td style='width:160px'>Acto jurídico</td>" +
              "<td style='white-space:nowrap'>Antecedente / Fracción</td>" +
              "<td style='width:200px'>Registrado en</td>" +
              "<td style='white-space:nowrap'>Img</td>" +
              "<td style ='width:160px'>Registró</td>" +
            "</tr>";
      return template;
    }

    private string GetCertificateRow(Certificate certificate, int index) {
      const string template =
         "<tr class='{{CLASS}}'>" +
           "<td>{{PRESENTATION.DATE}}<br></br>{{ISSUE.DATE}}</td>" +
           "<td style='white-space:normal'>Emisión de certificado</td>" +
           "<td>{{CERTIFICATE.TYPE}}</td>" +
           "<td style='white-space:nowrap;'>" +
             "<a href='javascript:doOperation(\"onSelectCertificate\", {{CERTIFICATE.ID}});'>" +
                 "{{CERTIFICATE.UID}}</a>" +
             "<br></br>{{TRANSACTION}}</td>" +
           "<td>&#160;</td>" +
           "<td>{{ISSUED.BY}}</td>" +
         "</tr>";

      string className = (index % 2 == 0) ? "detailsItem" : "detailsOddItem";
      if (certificate.Transaction.Equals(this.SelectedTransaction)) {
        className = "selectedItem";
      } else if (!certificate.IsClosed) {
        className = "warningItem";
      }
      string row = template.Replace("{{CLASS}}", className);

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
          "<td>{{PRESENTATION.DATE}}<br></br>{{AUTHORIZATION.DATE}}</td>" +
          "<td style='white-space:normal;width:260px'>{{RECORDING.ACT}}</td>" +
          "<td style='white-space:normal;'>{{PARTITION}}</td>" +
          "<td style='white-space:{{WHITE-SPACE}};'>" +
            "<a href='javascript:doOperation(\"onSelectDocument\", {{DOCUMENT.ID}}, {{RECORDING.ACT.ID}});'>" +
                "{{DOCUMENT.OR.RECORDING}}</a>" +
            "<br></br>{{TRANSACTION}}</td>" +
          "<td style='white-space:nowrap'>{{IMAGING.LINKS}}</td>" +
          "<td>{{RECORDED.BY}}</td>" +
        "</tr>";

      string className = (index % 2 == 0) ? "detailsItem" : "detailsOddItem";
      if (recordingAct.Document.Equals(this.SelectedDocument)) {
        className = "selectedItem";
      } else if (!recordingAct.Document.IsClosed) {
        className = "warningItem";
      }

      string row = template.Replace("{{CLASS}}", className);

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

      row = row.Replace("{{IMAGING.LINKS}}", HtmlFormatters.GetImagingLinks(recordingAct));

      row = row.Replace("{{RECORDING.ACT.ID}}", recordingAct.Id.ToString());

      return row;
    }

    private string GetPartitionOrAntecedentCell(RecordingAct recordingAct) {
      if (!(this.Resource is RealEstate)) {
        return "&#160;";
      }

      if (Resource.IsCreationalRole(recordingAct.ResourceRole)) {

        var realEstate = (RealEstate) recordingAct.Resource;

        if (recordingAct.ResourceRole == ResourceRole.Created) {
          return "Sin antecedente registral";
        } else if (realEstate.Equals(this.Resource)) {
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
      return "&#160;";
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
