using System.Drawing;
using System.Drawing.Imaging;

using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;

namespace Empiria.Web.UI {

  public partial class QRCodeControl : System.Web.UI.Page {

    private void Page_Load(object sender, System.EventArgs e) {
      Response.Clear();
      Response.ContentType = "image/gif";

      string text = Request.QueryString["data"];
      int size = int.Parse(Request.QueryString["size"] ?? "150");

      PrintQRCode(text, size);
      Response.End();
    }

    private void PrintQRCode(string text, int size) {
      var encoder = new QrEncoder();

      QrCode qrCode = encoder.Encode(text);

      //ISize, QrCode will be draw at (size) 150 x 150 pixels with quiet zone size at two.
      var fCodeSize = new FixedCodeSize(size, QuietZoneModules.Two);

      var renderer = new GraphicsRenderer(fCodeSize, Brushes.Black, Brushes.White);

      renderer.WriteToStream(qrCode.Matrix, ImageFormat.Gif, Response.OutputStream);
    }

  } // class QRCodeControl

} // namespace Empiria.Web.UI
