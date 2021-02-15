<%@ Page language="c#" Inherits="Empiria.Web.UI.PrinterControl" EnableViewState="false" EnableSessionState="true" Codebehind="printer.aspx.cs" %>
<%@ OutputCache Location="None" NoStore="true" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en">
	<head>
		<title>Impresión de documentos</title>
	  <link href="../themes/default/css/secondary.master.page.css" type="text/css" rel="stylesheet" />
		<script type="text/javascript" language="javascript">
    <!--
      function window_onload() {
        document.ifraPrintPage.focus();
        document.ifraPrintPage.print();
        addEvent(document.ifraPrintPage.window, 'onafterprint', printPage_onafterprint);
      }

      function printPage_onafterprint() {
        self.close();
      }

    //-->
		</script>
	</head>
	<body onload="window_onload();">
		<form id="frmEditor" method="post" runat="server">
       <br /><br /><br />
      &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b style="color:#3a3a3a">Impresión de documentos</b>
			<iframe id="ifraPrintPage" name="ifraPrintPage" src="text.mode.print.aspx?<%=printPageQueryString%>" width="0px" height="0px"></iframe>
		</form>
	</body>
</html>
