<%@ Page Language="C#" AutoEventWireup="true" Inherits="Empiria.Land.Pages.RecordingStamp" Codebehind="recording.stamp.aspx.cs" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
  <title>&#160;</title>
  <meta http-equiv="Expires" content="-1" />
  <meta http-equiv="Pragma" content="no-cache" />
  <link href="../themes/default/css/official.document.css" type="text/css" rel="stylesheet" />
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
          <% if (landRecord.IsHistoricRecord) { %>
          <h2 style="padding-top:0">SELLO REGISTRAL DE PARTIDA HISTÓRICA</h2>
          <% } else if (!landRecord.IsClosed) { %>
          <h2 class="warning" style="padding-top:0">EL SELLO REGISTRAL NO HA SIDO CERRADO</h2>
          <% } else if (landRecord.SecurityData.UsesESign && landRecord.SecurityData.IsUnsigned) { %>
          <h2 class="warning" style="padding-top:0">EL SELLO REGISTRAL NO HA SIDO FIRMADO DIGITALMENTE</h2>
          <% } else if (!base.CanBePrinted()) { %>
          <h2 class="warning" style="padding-top:0">
            NO DEBE IMPRIMIRSE. NO TIENE VALOR OFICIAL.
          </h2>
          <h2 style="padding-top:0">SELLO REGISTRAL</h2>
          <% } else { %>
          <h2 style="padding-top:0">SELLO REGISTRAL</h2>
          <% } %>
          <h5><%=landRecord.UID%></h5>
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
          <img style="margin-left:8pt" alt="" title="" src="../user.controls/barcode.aspx?data=<%=landRecord.UID%>&#38;vertical=true&#38;show-text=true&#38;height=32" />
          <% } else { %>
          <img style="margin-left:8pt" alt="" title="" src="../user.controls/barcode.aspx?data=**SIN-VALOR**&#38;vertical=true&#38;show-text=true&#38;height=32" />
          <% } %>
        </td>
        <td>
          <div class="document-text">
            <p>
              <%=GetPrelationText()%>
            </p>
            <p>
              <%=base.GetRecordingActsText()%>
            </p>
            <% if (this.DocumentHasNotes) { %>
              <b>Observaciones registrales:</b>
                <p class="certificate-text">
                    <%=base.GetDocumentNotes()%>
                </p>
            <% } %>
            <p>
              <%=GetPaymentText()%>
            </p>
            <p>
              <%=GetRecordingPlaceAndDate()%>
            </p>
          </div>
        </td>
      </tr>
    </table>
    <div class="footNotes">
      <table>
        <% if (!landRecord.IsClosed) { %>
        <tr>
          <td colspan="3" style="text-align:center;font-size:10pt" >
            <br /><br />
            <b class="warning">*** ESTE DOCUMENTO AUN NO HA SIDO CERRADO. ***</b>
            <br />&#160;
          </td>
        </tr>
        <% } else if (landRecord.SecurityData.UsesESign && landRecord.SecurityData.IsSigned) { %>
         <tr>
          <td colspan="3" style="text-align:center;font-size:11pt" >
            <span style="font-size:8.5pt">
            Firmado y sellado electrónicamente de conformidad<br />
            con las leyes y regulaciones vigentes.</span><br />
            <br />
            <%=base.GetDigitalSignature()%>
            <br />
            <b><%=GetRecordingSignerName()%></b>
            <br />
            <%=GetRecordingSignerJobTitle()%>
            <br />
            &#160;
          </td>
          <td style="text-wrap:none">&#160;&#160;&#160;&#160;&#160;</td>
        </tr>
        <% } else if (landRecord.SecurityData.UsesESign && landRecord.SecurityData.IsUnsigned) { %>
         <tr>
          <td colspan="3" style="text-align:center;font-size:11pt" >
            <span style="font-size:8.5pt">
            Pendiente de firmar y sellar electrónicamente.</span><br />
            <br />
            <b class="warning">SIN FIRMA NI SELLO ELECTRÓNICO</b>
            <br />
            <b><%=GetRecordingSignerName()%></b>
            <br />
            <%=GetRecordingSignerJobTitle()%>
            <br />
            &#160;
          </td>
          <td style="text-wrap:none">&#160;&#160;&#160;&#160;&#160;</td>
        </tr>
        <% } else if (!landRecord.IsHistoricRecord) { %>
        <tr>
          <td colspan="3" style="text-align:center;font-size:11pt" >
            <br /><br /><br />
            <b><%=GetRecordingSignerName()%></b>
            <br />
            <%=GetRecordingSignerJobTitle()%>
            <br />&#160;
          </td>
          <td style="text-wrap:none">&#160;&#160;&#160;&#160;&#160;</td>
        </tr>
        <% } %>
        <tr>
          <td style="vertical-align:top;padding-right:12pt;width:100px">
            <% if (this.CanBePrinted()) { %>
            <img style="margin-left:-12pt;margin-top:-12pt" alt="" title=""
                 src="../user.controls/qrcode.aspx?size=120&#38;data=<%=SEARCH_SERVICES_SERVER_BASE_ADDRESS%>/?type=document%26uid=<%=landRecord.UID%>%26hash=<%=landRecord.SecurityData.SecurityHash%>" />
            <% } %>
            <div style="margin-top:-12pt;font-size:7pt;white-space:nowrap">
              Valide este documento<br />
              <b><%=landRecord.UID%></b>
            </div>
          </td>
          <td style="vertical-align:top;width:90%;white-space:nowrap">
            <b>Código de verificación:</b>
            <br />
              <% if (this.CanBePrinted()) { %>
              &#160;&#160;<%=base.landRecord.SecurityData.SecurityHash%>

              <% } else { %>
              <span class="warning">** SIN VALIDEZ **</span>

              <% } %>
            <br />
            <b>Sello digital:</b>
            <br />
             <% if (this.CanBePrinted()) { %>
              &#160;&#160; <%=base.GetDigitalSeal()%>
             <% } else { %>
             <span class="warning">** ESTE DOCUMENTO NO ES OFICIAL **</span>
             <% } %>
            <br />
            <b>Firma electrónica:</b>
            <br />
               &#160;&#160;<%=GetDigitalSignature()%>
            <br />
            <b>Registró:</b> <%=GetRecordingOfficialsInitials()%> &#160; &#160; <b>Impreso el:</b> <%=DateTime.Now.ToString("dd/MMM/yyyy HH:mm") %>
            <br />
            <div style="font-size:7pt;margin-top:4pt;text-align:left;">
              Verifique la <u>autenticidad</u> de este documento y el estado de su predio. Para ello lea los códigos QR con su<br />
              celular o dispositivo móvil, o visite nuestro sitio <b><%=Empiria.Land.Pages.CommonMethods.GovernmentWebPage%></b>.
            </div>
          </td>
          <td style="vertical-align:top">
            <% if (!base.UniqueInvolvedResource.IsEmptyInstance && landRecord.IsClosed) { %>
            <img style="margin-right:-12pt;margin-left:-12pt;margin-top:-12pt" alt="" title=""
                 src="../user.controls/qrcode.aspx?size=120&#38;data=<%=SEARCH_SERVICES_SERVER_BASE_ADDRESS%>/?type=resource%26uid=<%=base.UniqueInvolvedResource.UID%>%26hash=<%=base.UniqueInvolvedResource.QRCodeSecurityHash()%>" />
            <div style="margin-top:-12pt;font-size:7pt;white-space:nowrap">
              Consultar folio electrónico<br />
              <b><%=base.UniqueInvolvedResource.UID%></b>
            </div>
            <% } %>
          </td>
        </tr>
      </table>

    </div>

  </body>
</html>
