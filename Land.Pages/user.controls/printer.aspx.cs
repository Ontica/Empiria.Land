using System;

namespace Empiria.Web.UI {

  public partial class PrinterControl : System.Web.UI.Page {

    protected string printPageQueryString = String.Empty;

    private void Page_Load(object sender, System.EventArgs e) {
      printPageQueryString = Request.Url.Query.Substring(1);
    }

  } // class CalendarControl

} // namespace Empiria.Web.UI
