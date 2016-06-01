/* Empiria Land ***********************************************************************************************
*                                                                                                             *
*  Solution  : Empiria Land                                    System   : Land Registration System            *
*  Namespace : Empiria.Land.UI                                 Assembly : Empiria.Land.UI                     *
*  Type      : CertificatesGrid                                Pattern  : Standard class                      *
*  Version   : 2.1                                             License  : Please read license.txt file        *
*                                                                                                             *
*  Summary   : Generates a grid HTML content that displays a list of certificates.                            *
*                                                                                                             *
********************************** Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

using Empiria.Land.Certification;

namespace Empiria.Land.UI {

  /// <summary>Generates a grid HTML content that displays a list of certificates.</summary>
  public class CertificatesGrid {

    #region Fields

    private FixedList<Certificate> _list = null;

    #endregion Fields

    #region Constructors and parsers

    private CertificatesGrid(FixedList<Certificate> list) {
      _list = list;
    }

    static public string Parse(FixedList<Certificate> list) {
      var grid = new CertificatesGrid(list);

      return grid.GetHtml();
    }

    #endregion Constructors and parsers

    #region Private methods

    private string GetCertificateRow(Certificate certificate, int index) {
      const string template =
         "<tr class='{{CLASS}}'>" +
           "<td>{{PRESENTATION.DATE}}<br/>{{ISSUE.DATE}}</td>" +
           "<td style='white-space:nowrap;'>" +
             "<a href='javascript:doOperation(\"onSelectCertificate\", {{CERTIFICATE.ID}});'>" +
                 "{{CERTIFICATE.UID}}</a>" +
             "<br>{{TRANSACTION}}</td>" +
           "<td>{{CERTIFICATE.TYPE}}<br/>" +
              "<a href='javascript:doOperation(\"displayResourcePopupWindow\", {{RESOURCE.ID}}, {{CERTIFICATE.ID}});'>" +
                  "{{RESOURCE.UID}}</a></td>" +
           "<td style='width:300px;white-space:normal;'>{{OWNER.NAME}}</td>" +
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
      row = row.Replace("{{OWNER.NAME}}", certificate.OwnerName);

      if (!certificate.Property.IsEmptyInstance) {
        row = row.Replace("{{RESOURCE.UID}}", certificate.Property.UID);
        row = row.Replace("{{RESOURCE.ID}}", certificate.Property.Id.ToString());
      } else {
        row = row.Replace("{{RESOURCE.UID}}", String.Empty);
        row = row.Replace("{{RESOURCE.ID}}", "-1");
      }

      row = row.Replace("{{TRANSACTION}}", "Trámite:" + certificate.Transaction.UID);
      row = row.Replace("{{ISSUED.BY}}", certificate.IssuedBy.Nickname +
                                         (certificate.Status == CertificateStatus.Pending ?
                                          " Pendiente" : String.Empty));

      return row;
    }


    private string GetHeader() {
      string template =
            "<tr class='detailsHeader'>" +
              "<td>Present/Registro</td>" +
              "<td style='width:200px'>Certificado</td>" +
              "<td style='white-space:nowrap'>Tipo / Folio real</td>" +
              "<td style='width:300px'>Personas</td>" +
              "<td style ='width:160px'>Registró</ td >" +
            "</tr>";
      return template;
    }


    private string GetHtml() {
      string html = this.GetTitle() + this.GetHeader();
      for (int i = 0; i < _list.Count; i++) {
        Certificate certificate = _list[i];

        html += this.GetCertificateRow(certificate, i);
      }
      return HtmlFormatters.TableWrapper(html);
    }


    private string GetTitle() {
      string template =
            "<tr class='detailsTitle'>" +
              "<td colspan='5'>Resultado de la búsqueda de certificados</td>" +
            "</tr>";

      return template;
    }

    #endregion Private methods

  } // class CertificatesGrid

} // namespace Empiria.Land.UI
