<%@ Page Language="C#" AutoEventWireup="true" Inherits="Empiria.Land.Pages.PaymentOrderPage" Codebehind="payment.order.aspx.cs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" lang="es">
<head id="Head1" runat="server">
  <title>Orden de pago</title>
  <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
  <meta http-equiv="Expires" content="-1" />
  <meta http-equiv="Pragma" content="no-cache" />
	<link href="../themes/default/css/to.printer.css" type="text/css" rel="stylesheet" />
</head>
<body style="margin-top:0">
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
								      <h1 style="white-space:normal;font-size:14pt">ORDEN DE PAGO</h1>
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
          <table style="width:100%;white-space:nowrap">
            <tr>
              <td style="vertical-align:top;white-space:nowrap">Interesado:</td>
              <td style='white-space:normal;border-bottom: 1px solid' colspan="3">
                <b style='font-size:9pt'><%=Encode(base.transaction.RequestedBy)%></b>
              </td>
              <td style="white-space:nowrap">Distrito:</td>
              <td><b><%=transaction.RecorderOffice.Alias%></b></td>
            </tr>
            <tr>
              <td style="white-space:nowrap;">Notaría/Gestor:</td><td style="white-space:nowrap;width:30%"><b><%=transaction.Agency.Alias%></b></td>
              <td style="white-space:nowrap">Tipo de trámite:</td><td style="white-space:nowrap;width:30%"><b><%=transaction.TransactionType.Name%></b></td>
              <td style="white-space:nowrap">Emitió:</td><td style="white-space:nowrap;width:30%"><b><%=transaction.PostedBy.Alias%>&nbsp;&nbsp;&nbsp;&nbsp;</b></td>
            </tr>
            <tr>
              <td style="white-space:nowrap">Instrumento:</td><td><b><%=Encode(transaction.DocumentDescriptor)%></b></td>
              <td style="white-space:nowrap">Tipo de documento:</td><td style="white-space:nowrap"><b><%=transaction.DocumentType.Name%></b></td>
              <td style="white-space:nowrap">Emisión:</td><td style="white-space:nowrap"><b><%=transaction.PostingTime.ToString("dd/MMM/yyyy HH:mm")%>&nbsp;&nbsp;&nbsp;&nbsp;</b></td>
            </tr>
            <tr>
              <td style="vertical-align:top;white-space:nowrap">Observaciones:</td>
              <td style='white-space:normal' colspan="5"><%=Encode(transaction.ExtensionData.RequesterNotes)%></td>
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
        <td>
          <table style="width:100%" cellpadding="4px" cellspacing="0px">
            <tr>
              <td style="border-top: 3px solid #3a3a3a;font-size:7pt">
                <b>Cadena original:</b><br /><%=Empiria.EmpiriaString.DivideLongString(transaction.GetDigitalString(), 132, "&#8203;")%>
                <br /><br />
                <b>Sello electrónico:</b><br /><%=Empiria.EmpiriaString.DivideLongString(transaction.GetDigitalSign(), 132, "&#8203;")%>
              </td>
            </tr>
          </table>
        </td>
      </tr>
      <tr>
        <td style="border-top: 1px dotted #3a3a3a">
        <br />
        <%=GetPaymentOrderFooter()%>
        </td>
      </tr>
    </table>

    <div class="breakpage"></div>

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
								      <h1 style="white-space:normal;font-size:14pt">BOLETA DE CONTROL</h1>
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
          <table style="width:100%;white-space:nowrap">
            <tr>
              <td style="vertical-align:top;white-space:nowrap">Interesado:</td>
              <td style='white-space:normal;border-bottom: 1px solid' colspan="3">
                <b style='font-size:9pt'><%=Encode(transaction.RequestedBy)%></b>
              </td>
              <td style="white-space:nowrap">Distrito:</td>
              <td><b><%=transaction.RecorderOffice.Alias%></b></td>
            </tr>
            <tr>
              <td style="white-space:nowrap;">Notaría/Gestor:</td><td style="white-space:nowrap;width:30%"><b><%=transaction.Agency.Alias%></b></td>
              <td style="white-space:nowrap">Tipo de trámite:</td><td style="white-space:nowrap;width:30%"><b><%=transaction.TransactionType.Name%></b></td>
              <td style="white-space:nowrap">Emitió:</td><td style="white-space:nowrap;width:30%"><b><%=transaction.PostedBy.Alias%>&nbsp;&nbsp;&nbsp;&nbsp;</b></td>
            </tr>
            <tr>
              <td style="white-space:nowrap">Instrumento:</td><td><b><%=Encode(transaction.DocumentDescriptor)%></b></td>
              <td style="white-space:nowrap">Tipo de documento:</td><td style="white-space:nowrap"><b><%=transaction.DocumentType.Name%></b></td>
              <td style="white-space:nowrap">Emisión:</td><td style="white-space:nowrap"><b><%=transaction.PostingTime.ToString("dd/MMM/yyyy HH:mm")%>&nbsp;&nbsp;&nbsp;&nbsp;</b></td>
            </tr>
            <tr>
              <td style="vertical-align:top;white-space:nowrap">Observaciones:</td>
              <td style='white-space:normal' colspan="5"><%=Encode(transaction.ExtensionData.RequesterNotes)%></td>
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
        <td>
          <table style="width:100%" cellpadding="4px" cellspacing="0px">
            <tr>
              <td style="border-top: 3px solid #3a3a3a;font-size:7pt">
                <b>Cadena original:</b><br /><%=Empiria.EmpiriaString.DivideLongString(transaction.GetDigitalString(), 132, "&#8203;")%>
                <br /><br />
                <b>Sello electrónico:</b><br /><%=Empiria.EmpiriaString.DivideLongString(transaction.GetDigitalSign(), 132, "&#8203;")%>
              </td>
            </tr>
          </table>
        </td>
      </tr>
      <tr>
        <td style="border-top: 1px dotted #3a3a3a">
        <br />
        * Este documento deberá <b>ENTREGARSE en la <u>Ventanilla de Recepción de Documentos</u></b> junto con su recibo de pago.
        </td>
      </tr>
    </table>

  </form>
  </body>
</html>
