/* Empiria Land ***********************************************************************************************
*                                                                                                             *
*  Solution  : Empiria Land                                    System   : Land Registration System            *
*  Namespace : Empiria.Land.UI                                 Assembly : Empiria.Land.UI                     *
*  Type      : CertificatesGrid                                Pattern  : Standard class                      *
*  Version   : 3.0                                             License  : Please read license.txt file        *
*                                                                                                             *
*  Summary   : Generates a grid HTML content that displays a list of certificates.                            *
*                                                                                                             *
********************************** Copyright (c) 2009-2017. La Vía Óntica SC, Ontica LLC and contributors.  **/
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
           "<td>{{PRESENTATION.DATE}}<br></br>{{ISSUE.DATE}}</td>" +
           "<td>" +
             "<a href='javascript:doOperation(\"onSelectCertificate\", {{CERTIFICATE.ID}});'>" +
                 "{{CERTIFICATE.UID}}</a>" +
             "<br></br>{{TRANSACTION}}</td>" +
           "<td>{{CERTIFICATE.TYPE}}<br></br>" +
              "<a href='javascript:doOperation(\"displayResourcePopupWindow\", {{RESOURCE.ID}}, {{CERTIFICATE.ID}});'>" +
                  "{{RESOURCE.UID}}</a></td>" +
           "<td>{{OWNER.NAME}}</td>" +
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


    private string GetHeaderRow() {
      string template = @"<tr>
                            <th class='tbHeader'>Prest/Reg</th>
                            <th class='tbHeader'>Certificado</th>
                            <th class='tbHeader'>Tipo / Folio real</th>
                            <th class='tbHeader'>Personas</th>
                            <th class='tbHeader'>Registró</th>
                          </tr>";

      return template;
    }

    private string GetColGroup() {
      string template = @"<colgroup>
                            <col width='70'/>
		                        <col width='145'/>
                            <col width='135'/>
		                        <col width='170'/>
		                        <col />
	                        </colgroup>";
      return template;
    }

    private string GetHtml() {
      string html = "<div class='tblContainer'>" +
                       "<div class='tbHeader'>" +
                          this.GetHeaderTable() +
                       "</div> " +
                       "<div class='tbBody'>" +
                          this.GetBodyTable() +
                       "</div>" +
                    "</div>";
      return html;
    }

    private string GetBodyTable() {
      string rows = String.Empty;

      for (int i = 0; i < _list.Count; i++) {
        Certificate certificate = _list[i];

        rows += this.GetCertificateRow(certificate, i);
      }

      return HtmlFormatters.TableWrapper(this.GetColGroup() +
                                          "<tbody>" +
                                            rows +
                                          "</tbody>", "details");
    }

    private string GetHeaderTable() {
      return HtmlFormatters.TableWrapper(this.GetColGroup() +
                                          "<tbody>" +
                                            this.GetTitleRow() +
                                            this.GetHeaderRow() +
                                          "</tbody>");
    }

    private string GetTitleRow() {
      string template = @"<tr>
                            <td class='tbTitle' colspan='5'>Resultado de la búsqueda de certificados</td>
                          </tr>";
      return template;
    }

    #endregion Private methods

  } // class CertificatesGrid

} // namespace Empiria.Land.UI
