<%@ Page Language="C#" AutoEventWireup="true" Inherits="Empiria.Land.Pages.TransactionReceipt" Codebehind="transaction.receipt.aspx.cs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" lang="es">
<head id="Head1" runat="server">
  <title>Boleta de recepción de trámite</title>
  <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
  <meta http-equiv="Expires" content="-1" />
  <meta http-equiv="Pragma" content="no-cache" />
	<link href="../themes/default/css/to.printer.css" type="text/css" rel="stylesheet" />
</head>
<body style="margin-right:0">
	<form id="frmEditor" method="post" runat="server">
		<table cellspacing="0" cellpadding="0" width="100%">
      <tr style="vertical-align:top">
	      <td style="white-space:nowrap;width:100%">
		      <table style="width:100%" cellspacing="0" cellpadding="2">
			      <tr>
				      <td style="vertical-align:middle">
                <img src="<%=GetDocumentLogo()%>" alt="" title="" style="width:120px;padding-top:6px" />
				      </td>
              <td style="vertical-align:top;width:80%;white-space:nowrap">
			          <table width="100%" cellpadding="0" cellspacing="0">
						      <tr>
							      <td style="text-align:center">
								      <h1 style="white-space:normal;font-size:12pt"><%=CustomerOfficeName()%></h1>
							      </td>
						      </tr>
                  <% if (base.GetDistrictName().Length != 0) { %>
						      <tr>
							      <td style="text-align:center;margin-top:-16px">
								      <h2 style="white-space:normal;font-size:10pt"><%=base.GetDistrictName()%></h2>
							      </td>
						      </tr>
                  <% } %>
						      <tr>
							      <td style="text-align:center;">
								      <h1 style="white-space:normal;font-size:14pt">BOLETA DE RECEPCIÓN</h1>
							      </td>
						      </tr>
                </table>
              </td>
				      <td style="vertical-align:top;white-space:nowrap">
					      <table width="100%">
						      <tr>
							      <td style="vertical-align:top;text-align:right">
								      <h3><%=transaction.UID%></h3>
							      </td>
						      </tr>
						      <tr>
							      <td align="right">
                      <img style="margin-left:8pt" alt="" title="" src="../user.controls/barcode.aspx?data=<%=transaction.UID%>" />
                        <br />Control: <b><%=transaction.InternalControlNoFormatted%></b>
							      </td>
						      </tr>
					      </table>
				      </td>
			      </tr>
		      </table>
	      </td>
      </tr>
      <tr>
        <td style="border-top: 3px solid #3a3a3a;padding-top:8pt;padding-bottom:4pt">
			    <table style="width:100%;white-space:nowrap" cellpadding="3px" cellspacing="0px">
            <tr>
              <td valign="top" style="white-space:nowrap">Interesado:</td>
              <td style='white-space:normal;border-bottom: 1px solid' colspan="3">
                <b style='font-size:9pt'><%=Encode(transaction.RequestedBy)%></b>
              </td>
							<td style="white-space:nowrap">Distrito:</td>
              <td><b><%=transaction.RecorderOffice.Alias%></b></td>
            </tr>
            <tr>
              <td style="white-space:nowrap">Notaría/Gestor:</td>
              <td style="white-space:nowrap;width:30%"><b><%=transaction.Agency.Alias%></b></td>
							<td style="white-space:nowrap">Tipo de trámite:</td>
              <td style="white-space:nowrap;width:30%"><b><%=transaction.TransactionType.Name%></b></td>
              <td style="white-space:nowrap">Importe:</td>
              <td style="white-space:nowrap;width:30%"><b><%=GetPaymentTotal()%></b>
                &nbsp; (R: <b><%=transaction.Payments.ReceiptNumbers%>)</b>
              </td>
            </tr>
            <tr>
							<td style="white-space:nowrap">Fecha de presentación:</td>
              <td style="white-space:nowrap"><b><%=transaction.PresentationTime.ToString("dd/MMM/yyyy HH:mm:ss")%></b></td>
							<td style="white-space:nowrap">Instrumento:</td>
              <td><b><%=Encode(transaction.DocumentDescriptor)%></b></td>
              <td style="white-space:nowrap">Recibió:</td>
              <td style="white-space:nowrap"><b><%=transaction.ReceivedBy.Alias%></b></td>
            </tr>
            <tr>
              <td style="white-space:nowrap;vertical-align:top">Observaciones:</td>
              <td style="white-space:normal" colspan="5"><%=Encode(transaction.ExtensionData.RequesterNotes)%></td>
            </tr>
          </table>
        </td>
      </tr>
			<tr>
				<td nowrap="nowrap" style="width:100%;height:10px;padding-bottom:10pt">
			    <table style="width:100%" cellpadding="4px" cellspacing="0px">
				    <tr class="borderHeaderRow" style="padding-bottom:8pt">
              <%=GetHeader()%>
				    </tr>
            <%=GetItems()%>
           </table>
         </td>
      </tr>
      <tr>
        <td class="upperSeparatorRow" style="font-size:8pt">
          <table style="width:100%" cellpadding="4px" cellspacing="0px">
            <tr>
              <td style="vertical-align:top;width:100px">
                <img style="margin-left:6pt;margin-top:0" alt="" title=""
                     src="<%=base.QRCodeSourceImage()%>" />
                <div style="margin-left:0;margin-top:-8px;font-size:8pt;white-space:nowrap;text-align:center">
                  Consulte este trámite<br />
                  <b><%=base.transaction.UID%></b>
                </div>
              </td>
              <td style="vertical-align:top;padding-left:12pt;font-size:8pt">
                <% if (base.PaymentOrderWasGenerated) { %>
                <b>Línea de captura del pago:</b>
                <br />
                <span style="font-family:'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;font-size:12pt">
                  <%=base.transaction.PaymentOrder.UID%>
                </span>
                <% } %>
                <br />
                <b>Sello electrónico:</b>
                <br />
                <%=transaction.GetDigitalSign()%>
                <br />
<%--                <br />
                <b>Códigos de seguridad:</b>
                <br />
                ALFA 453 <b>&nbsp;||&nbsp;</b> BETA 128 <b>&nbsp;||&nbsp;</b> GAMMA 412 <b>&nbsp;||&nbsp;</b> DELTA 341 <b>&nbsp;||&nbsp;</b> SIGMA 123847
                <br />--%>
                <br />
                <b>Recibió:</b> <%=transaction.ReceivedBy.FullName%>  &nbsp; | &nbsp; <b>Imprimió:</b> <%=GetCurrentUserInitials()%>, <%=DateTime.Now.ToString("dd/MMM/yyyy HH:mm") %>
                <div style="font-size:7pt;margin-top:8pt;">
                  Consulte el <b>estado</b> de este trámite leyendo el código QR con su teléfono o dispositivo móvil,
                  o visite nuestro sitio web <b>registropublico.zacatecas.gob.mx</b>, donde también podrá consultar
                  documentos, certificados y el estado de los predios.
                  <br /><br />
                  <% if (transaction.ExtensionData.SendTo.Address != "") { %>
                    Le enviaremos información sobre este trámite a su cuenta de correo: <b><%=transaction.ExtensionData.SendTo.Address%></b>
                    <br />
                    Conserve este comprobante. Pero <u>NO necesitará regresar al RPP</u> en caso de que este trámite pueda
                    ENTREGÁRSELE DE FORMA ELECTRÓNICA.
                  <%  } else { %>
                    Este comprobante deberá <b>PRESENTARSE en la <u>Ventanilla de Entregas</u></b> al recoger su documento o certificado.
                    Si en trámites futuros nos proporciona su correo electrónico, podemos enviarle sus documentos
                    VIA ELECTRÓNICA. Así no necesitará regresar a recogerlos.
                  <% } %>
                </div>
              </td>
              <td valign="top">
                <% if (!base.transaction.BaseResource.IsEmptyInstance) { %>
                <img alt="" title="" src="../user.controls/barcode.aspx?data=<%=base.transaction.BaseResource.UID%>" />
                <div><b>FOLIO REAL:</b><%=base.transaction.BaseResource.UID%></div>
                <% } %>
              </td>
            </tr>
          </table>
        </td>
      </tr>
    </table>
	</form>
	</body>
</html>
