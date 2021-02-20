using System;
using System.Drawing;
using System.Drawing.Imaging;

using C1.Win.C1BarCode;

namespace Empiria.Web.UI {

  public partial class BarCodeControl : System.Web.UI.Page {

    private void Page_Load(object sender, EventArgs e) {
      Response.Clear();
      Response.ContentType = "image/gif";
      PrintBarcode();
      Response.End();
    }


    private Image GetBarcode() {
      using (var bc = new C1BarCode()) {

        bc.CodeType = CodeTypeEnum.Code128;
        bc.Text = Request.QueryString["data"];

        int height = int.Parse(Request.QueryString["height"] ?? "-1");
        if (height > 0) {
          bc.BarHeight = height;
        } else {
          bc.BarHeight = 30;
        }

        bool showText = bool.Parse(Request.QueryString["show-text"] ?? "false");
        bc.ShowText = showText;

        return bc.GetImage(ImageFormat.Gif);
      }
    }


    private void PrintBarcode() {
      bool showVertical = bool.Parse(Request.QueryString["vertical"] ?? "false");

      using (Image barcodeImage = this.GetBarcode()) {
        if (showVertical) {
          barcodeImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
        }

        barcodeImage.Save(Response.OutputStream, ImageFormat.Gif);
      }

    }

  } // class BarCodeControl

} // namespace Empiria.Web.UI
