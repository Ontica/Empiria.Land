/* Empiria Land ***********************************************************************************************
*                                                                                                             *
*  Solution  : Empiria Land                                    System   : Land Registration System            *
*  Namespace : Empiria.Land.UI                                 Assembly : Empiria.Land.UI                     *
*  Type      : TransactionDocumentAndCertificatesGrid          Pattern  : Standard class                      *
*  Version   : 3.0                                             License  : Please read license.txt file        *
*                                                                                                             *
*  Summary   : Generates a grid HTML content that displays the document and certificates of a transaction.    *
*                                                                                                             *
********************************** Copyright (c) 2009-2017. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

using Empiria.Land.Certification;
using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.UI {

  /// <summary>Generates a grid HTML content that displays the document and
  /// certificates of a transaction.</summary>
  public class TransactionDocumentAndCertificatesGrid {

    #region Fields

    private LRSTransaction _transaction = null;

    #endregion Fields

    #region Constructors and parsers

    private TransactionDocumentAndCertificatesGrid(LRSTransaction transaction) {
      _transaction = transaction;
    }

    static public string Parse(LRSTransaction transaction) {
      var grid = new TransactionDocumentAndCertificatesGrid(transaction);

      return grid.GetHtml();
    }

    #endregion Constructors and parsers

    #region Private methods

    private string GetDocumentRow(RecordingDocument document, int index) {
      const string template =
         "<tr class='{{CLASS}}'>" +
           "<td style='white-space:nowrap;'>" +
             "<a href='javascript:doOperation(\"onSelectDocument\", {{DOCUMENT.ID}});'>" +
                 "{{DOCUMENT.UID}}</a></td>" +
           "<td>{{DOCUMENT.TYPE}}</td>" +
           "<td style='white-space:nowrap'>{{IMAGING.LINKS}}</td>" +
           "<td>&nbsp;</td>" +
           "<td>{{RECORDING.DATE}}</td>" +
           "<td>{{ISSUED.BY}}</td>" +
         "</tr>";

      string row = template.Replace("{{CLASS}}", (index % 2 == 0) ? "detailsItem" : "detailsOddItem");


      row = row.Replace("{{DOCUMENT.TYPE}}", document.DocumentType.DisplayName);
      row = row.Replace("{{DOCUMENT.ID}}", document.Id.ToString());

      row = row.Replace("{{IMAGING.LINKS}}", HtmlFormatters.GetImagingLinks(document));

      row = row.Replace("{{DOCUMENT.UID}}", document.UID);
      row = row.Replace("{{ISSUED.BY}}", document.PostedBy.Nickname);
      row = row.Replace("{{RECORDING.DATE}}", HtmlFormatters.GetDateAsText(document.AuthorizationTime));

      return row;
    }

    private string GetCertificateRow(Certificate certificate, int index) {
      const string template =
         "<tr class='{{CLASS}}'>" +
           "<td style='white-space:nowrap;'>" +
             "<a href='javascript:doOperation(\"onSelectCertificate\", {{CERTIFICATE.ID}});'>" +
                 "{{CERTIFICATE.UID}}</a></td>" +
           "<td>{{CERTIFICATE.TYPE}}<br/>" +
              "<a href='javascript:doOperation(\"displayResourcePopupWindow\", {{RESOURCE.ID}}, {{CERTIFICATE.ID}});'>" +
                  "{{RESOURCE.UID}}</a></td>" +
           "<td style='white-space:nowrap'>&nbsp;</td>" +
           "<td style='width:300px;white-space:normal;'>{{OWNER.NAME}}</td>" +
           "<td>{{RECORDING.DATE}}</td>" +
           "<td>{{ISSUED.BY}}</td>" +
         "</tr>";

      string row = template.Replace("{{CLASS}}", (index % 2 == 0) ? "detailsItem" : "detailsOddItem");

      row = row.Replace("{{CERTIFICATE.TYPE}}", certificate.CertificateType.DisplayName);
      row = row.Replace("{{CERTIFICATE.ID}}", certificate.Id.ToString());
      row = row.Replace("{{CERTIFICATE.UID}}", certificate.UID);
      row = row.Replace("{{OWNER.NAME}}", certificate.OwnerName);

      if (!certificate.Property.IsEmptyInstance) {
        row = row.Replace("{{RESOURCE.UID}}", certificate.Property.UID);
        row = row.Replace("{{RESOURCE.ID}}", certificate.Property.Id.ToString());
      } else {
        row = row.Replace("{{RESOURCE.UID}}", String.Empty);
        row = row.Replace("{{RESOURCE.ID}}", "-1");
      }
      row = row.Replace("{{RECORDING.DATE}}", HtmlFormatters.GetDateAsText(certificate.IssueTime));

      row = row.Replace("{{ISSUED.BY}}", certificate.IssuedBy.Nickname +
                                         (certificate.Status == CertificateStatus.Pending ?
                                          " Pendiente" : String.Empty));

      return row;
    }


    private string GetHeader() {
      string template =
            "<tr class='detailsHeader'>" +
              "<td style='width:200px'>Documento/Certificado</td>" +
              "<td style='width:200px'>Tipo / Folio real</td>" +
              "<td style='white-space:nowrap'>Img</td>" +
              "<td style='white-space:nowrap'>Personas</td>" +
              "<td style='width:160px'>Fecha</td>" +
              "<td style ='width:160px'>Registró</td>" +
            "</tr>";
      return template;
    }


    private string GetHtml() {
      string html = this.GetTitle() + this.GetHeader();

      if (!_transaction.Document.IsEmptyDocument) {
        html += this.GetDocumentRow(_transaction.Document, 0);
      }
      FixedList<Certificate> certificates = _transaction.GetIssuedCertificates();
      for (int i = 0; i < certificates.Count; i++) {
        Certificate certificate = certificates[i];

        html += this.GetCertificateRow(certificate, i + 1);
      }

      if (_transaction.Document.IsEmptyDocument && certificates.Count == 0) {
        html += this.NoRecordsFoundRow();
      }

      return HtmlFormatters.TableWrapper(html);
    }

    private string GetTitle() {
      string template =
            "<tr class='detailsTitle'>" +
              "<td colspan='6'>Documento y certificados del trámite <b>{{TRANSACTION.UID}}</b></td>" +
            "</tr>";

      return template.Replace("{{TRANSACTION.UID}}", _transaction.UID);
    }


    private string NoRecordsFoundRow() {
      const string template =
        "<tr class='detailsItem'>" +
          "<td colspan='6'>Este trámite no tiene un documento registrado ni certificados emitidos</td>" +
        "<tr>";

      return template;
    }

    #endregion Private methods

  } // class TransactionDocumentAndCertificatesGrid

} // namespace Empiria.Land.UI
