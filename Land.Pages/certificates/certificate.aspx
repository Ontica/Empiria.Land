<%@ Page Language="C#" AutoEventWireup="true" Inherits="Empiria.Land.Pages.CertificatePage" Codebehind="certificate.aspx.cs" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
  <title>&#160;</title>
  <meta http-equiv="Expires" content="-1" />
  <meta http-equiv="Pragma" content="no-cache" />
  <link href="../themes/default/css/certificate.css" type="text/css" rel="stylesheet" />
  </head>
  <body>
    <table>
      <tr>
        <td style="vertical-align:top">
          <br />
          <img style="margin-left:-22pt" class="logo" src="<%=base.GetDocumentLogo()%>" alt="" title="" width="280" />
        </td>
        <td style="vertical-align:top;text-align:center;width:95%">
	      <h3><%=Empiria.Land.Pages.CommonMethods.CustomerOfficeName%></h3>
          <h4><%=Empiria.Land.Pages.CommonMethods.GovernmentName%></h4>
          <% if (!certificate.IsClosed) { %>
          <h2 class="warning" style="padding-top:0">EL CERTIFICADO NO HA SIDO CERRADO</h2>
          <% } else if (landRecord.SecurityData.IsUnsigned) { %>
          <h2 class="warning" style="padding-top:0">EL CERTIFICADO NO HA SIDO FIRMADO DIGITALMENTE</h2>
          <% } else if (!base.CanBePrinted()) { %>
          <h2 class="warning" style="padding-top:0">
            NO DEBE IMPRIMIRSE. NO TIENE VALOR OFICIAL.
          </h2>
          <h2 style="padding-top:0"><%=GetCertificateType()%></h2>
          <% } else { %>
          <h2 style="padding-top:0"><%=GetCertificateType()%></h2>
          <% } %>
          <h5><%=certificate.CertificateID%></h5>
          <% if (transaction.IsReentry) { %>
            <h5><b>(Reingreso)</b></h5>
          <% } %>
        </td>
        <td style="vertical-align:top;padding-left:20px">
          <br />
          <img style="margin-left:-22pt" class="logo" src="../themes/default/customer/government.seal.right.jpg" alt="" title="" />
        </td>
      </tr>
    </table>

    <table>
      <tr>
        <td style="vertical-align:top">
          <% if (this.CanBePrinted()) { %>
          <img style="margin-left:8pt" alt="" title="" src="../user.controls/barcode.aspx?data=<%=certificate.CertificateID%>&#38;vertical=true&#38;show-text=true&#38;height=32" />
          <% } else { %>
          <img style="margin-left:8pt" alt="" title="" src="../user.controls/barcode.aspx?data=**SIN-VALOR**&#38;vertical=true&#38;show-text=true&#38;height=32" />
          <% } %>
        </td>
        <td style="vertical-align:top">
          <div class="document-text">
           <p>
              <%=GetCertificatePlaceAndDate()%>
           </p>
          </div>
          <div class="document-text">
            <h2 style="padding-top:0;text-align:center">C E R T I F I C A</h2>
          </div>
          <div class="document-text">
            <p>
              <%=GetCertificateText()%>
            </p>
            <p>
                <b>DOY FE.---------------------------------------------------------------------------------------------------------------------------</b>
            </p>
          </div>
        </td>
      </tr>
    </table>
    <div class="footNotes">
      <table>
        <% if (!certificate.IsClosed) { %>
        <tr>
          <td colspan="3" style="text-align:center;font-size:10pt" >
            <br /><br />
            <b class="warning">*** ESTE CERTIFICADO AUN NO HA SIDO CERRADO. ***</b>
            <br />&#160;
          </td>
        </tr>
        <% } else if (landRecord.SecurityData.IsSigned) { %>
         <tr>
          <td colspan="3" style="text-align:center;font-size:11pt" >
            <span style="font-size:8.5pt">
            Firmado y sellado electrónicamente de conformidad con las leyes y regulaciones vigentes,<br />
            utilizando los servicios de la Autoridad Certificadora del Gobierno del Estado.</span><br />
            <br />
            <b><%=GetCertificateSignerName()%></b>
            <br />
            <%=GetCertificateSignerJobTitle()%>
            <br />
            &#160;
          </td>
          <td style="text-wrap:none">&#160;&#160;&#160;&#160;&#160;</td>
        </tr>
        <% } else { %>
         <tr>
          <td colspan="3" style="text-align:center;font-size:11pt" >
            <span style="font-size:8.5pt">
            Pendiente de firmar y sellar electrónicamente.</span><br />
            <br />
            <b class="warning">SIN FIRMA NI SELLO ELECTRÓNICO</b>
            <br />
            <b><%=GetCertificateSignerName()%></b>
            <br />
            <%=GetCertificateSignerJobTitle()%>
            <br />
            &#160;
          </td>
          <td style="text-wrap:none">&#160;&#160;&#160;&#160;&#160;</td>
        </tr>
        <% } %>
        <tr>
          <td style="vertical-align:top;padding-right:12pt;width:100px">
            <% if (this.CanBePrinted()) { %>
            <img style="margin-left:-12pt;margin-top:-12pt" alt="" title=""
                 src="../user.controls/qrcode.aspx?size=120&#38;data=<%=SEARCH_SERVICES_SERVER_BASE_ADDRESS%>/?type=certificate%26uid=<%=certificate.CertificateID%>%26hash=<%=landRecord.SecurityData.SecurityHash%>" />
            <% } %>
            <div style="margin-top:-12pt;font-size:7pt;white-space:nowrap">
              Valide este certificado<br />
              <b><%=certificate.CertificateID%></b>
              <br />
              <%=landRecord.SecurityData.SignDocumentID%>
            </div>
          </td>
          <td style="vertical-align:top;width:90%;white-space:nowrap">
            <b>Código de verificación:</b>
            <br />
              <% if (this.CanBePrinted()) { %>
              <%=base.landRecord.SecurityData.SecurityHash%>

              <% } else { %>
              <span class="warning">** SIN VALIDEZ **</span>

              <% } %>
            <br />
            <b>Sello digital:</b>
            <br />
             <% if (this.CanBePrinted()) { %>
              <%=base.GetDigitalSeal()%>
             <% } else { %>
             <span class="warning">** ESTE DOCUMENTO NO ES OFICIAL **</span>
             <% } %>
            <br />
            <%=GetSignatureGuid()%>
            <%=GetDigest()%>
            <b>Firma electrónica:</b>
            <br />
            <%=GetDigitalSignature()%>
            <br />
            <b>Elaboró:</b> <%=GetElaboratedByInitials()%> &#160; &#160; <b>Impreso el:</b> <%=DateTime.Now.ToString("dd/MMM/yyyy HH:mm") %>
            <br />
            <div style="font-size:7pt;margin-top:4pt;text-align:left;">
              Verifique la <u>autenticidad</u> de este documento y el estado de su predio. Para ello lea los códigos QR con su<br />
              celular o dispositivo móvil, o visite nuestro sitio <b><%=Empiria.Land.Pages.CommonMethods.GovernmentWebPage%></b>.
            </div>
          </td>
          <td style="vertical-align:top">
            <% if (!base.UniqueInvolvedResource.IsEmptyInstance && certificate.IsClosed) { %>
            <img style="margin-right:-12pt;margin-left:-12pt;margin-top:-12pt" alt="" title=""
                 src="<%=GetUniqueInvolvedResourceQRCodeUrl()%>" />
              <div style="margin-top:-12pt;font-size:7pt;white-space:nowrap">
              <%=GetUniqueInvolvedResourceQRCodeText()%><br />
              <b><%=base.UniqueInvolvedResource.UID%></b>
            </div>
            <% } %>
          </td>
        </tr>
      </table>

    </div>

  </body>
</html>
