/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                     System   : Land Intranet Application         *
*  Namespace : Empiria.Land.WebApp                              Assembly : Empiria.Land.Intranet.dll         *
*  Type      : ObjectSearcher                                   Pattern  : Explorer Web Page                 *
*  Version   : 3.0                                              License  : Please read license.txt file      *
*                                                                                                            *
*  Summary   :                                                                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Pages {

  public partial class TransactionReceipt : System.Web.UI.Page {

    #region Fields

    protected static readonly string SEARCH_SERVICES_SERVER_BASE_ADDRESS =
                                        ConfigurationData.Get<string>("SearchServicesServerBaseAddress");

    private static readonly bool DISPLAY_VEDA_ELECTORAL_UI =
                                          ConfigurationData.Get<bool>("DisplayVedaElectoralUI", false);

    protected LRSTransaction transaction = null;

    #endregion Fields

    #region Constructors and parsers

    protected void Page_Load(object sender, EventArgs e) {
      Initialize();
    }

    #endregion Constructors and parsers

    #region Private methods

    private void Initialize() {
      transaction = LRSTransaction.TryParse(Request.QueryString["uid"], true);
    }

    protected string CustomerOfficeName() {
      return "Dirección de Catastro y Registro Público";
    }

    protected string GetDistrictName() {
      return $"Registro Público del Distrito de {transaction.RecorderOffice.ShortName}";
    }

    protected string QRCodeSourceImage() {
      var s = $"../user.controls/qrcode.aspx?size=120&amp;data={SEARCH_SERVICES_SERVER_BASE_ADDRESS}" +
              $"/?v=5.0%26s=qr%26type=transaction%26uid={transaction.UID}%26h={transaction.QRCodeSecurityHash()}";

      return s;
    }

    protected bool PaymentOrderWasGenerated {
      get {
        return (!transaction.PaymentOrder.IsEmpty &&
                 transaction.PaymentOrder.Issuer != "Empiria.Land");
      }
    }

    protected string GetDocumentLogo() {
      if (DISPLAY_VEDA_ELECTORAL_UI) {
        return "../themes/default/customer/horizontal.logo.veda.png";
      }
      return "../themes/default/customer/horizontal.logo.png";
    }

    protected string GetHeader() {
      if (ExecutionServer.LicenseName == "Tlaxcala") {
        return GetQuantitiesHeader();
      } else {
        return GetNoQuantitiesHeader();
      }
    }

    private string GetNoQuantitiesHeader() {
      const string aj = "<td style='white-space:nowrap'>#</td>" +
                          "<td style='width:30%'>Concepto</td>" +
                          "<td style='white-space:nowrap'>Base gravable</td>" +
                          "<td style='white-space:nowrap'>Cant</td>" +
                          "<td style='white-space:nowrap'>Unidad</td>" +
                          "<td style='white-space:nowrap'>Código</td>" +
                          "<td style='width:30%;'>Fundamento</td>" +
                          "<td style='width:30%;'>Observaciones</td>";
      return aj;
    }

    private string GetQuantitiesHeader() {
      const string aj = "<td style='white-space:nowrap'>#</td>" +
                          "<td style='white-space:nowrap'>Clave</td>" +
                          "<td style='white-space:nowrap;width:30%'>Acto jurídico / Concepto</td>" +
                          "<td style='white-space:nowrap'>Fundamento</td>" +
                          "<td style='white-space:nowrap' align='right'>Valor operac</td>" +
                          "<td align='right'>Derechos reg</td>" +
                          "<td align='right'>Cotejo</td>" +
                          "<td align='right'>Otros</td>" +
                          "<td align='right'>Subtotal</td>" +
                          "<td align='right'>Descuento</td>" +
                          "<td align='right'>Total</td>";

      const string cert = "<td style='white-space:nowrap'>#</td>" +
                          "<td style='white-space:nowrap'>Clave</td>" +
                          "<td style='white-space:nowrap;width:60%'>Concepto</td>" +
                          "<td style='white-space:nowrap'>Fundamento</td>" +
                          "<td align='right'>Subtotal</td>" +
                          "<td align='right'>Descuento</td>" +
                          "<td align='right'>Total</td>";
      if (transaction.TransactionType.Id == 702) {
        return cert;
      } else {
        return aj;
      }
    }

    protected string GetItems() {
      if (ExecutionServer.LicenseName == "Tlaxcala") {
        if (transaction.TransactionType.Id == 702) {
          return GetCertificate();
        } else {
          return GetRecordingActsWithTotals();
        }
      } else {
        return GetConcepts();
      }
    }

    protected string GetCertificate() {
      const string cert = "<tr width='24px'><td style='white-space:nowrap' valign='top'>{NUMBER}</td>" +
                      "<td style='white-space:nowrap' valign='top'>{CODE}</td>" +
                      "<td style='white-space:nowrap;width:30%' valign='top'>{CONCEPT}</td>" +
                      "<td style='white-space:nowrap' valign='top'>{LAW.ARTICLE}</td>" +
                      "<td align='right' valign='top'>{SUBTOTAL}</td>" +
                      "<td align='right' valign='top'>{DISCOUNTS}</td>" +
                      "<td align='right' valign='top'><b>{TOTAL}</b></td></tr>";
      FixedList<LRSTransactionService> services = transaction.Services;
      string html = String.Empty;

      for (int i = 0; i < services.Count; i++) {
        LRSTransactionService service = services[i];
        string temp = cert.Replace("{NUMBER}", (i + 1).ToString("00"));
        temp = temp.Replace("{CODE}", service.TreasuryCode.FinancialConceptCode);
        temp = temp.Replace("{CONCEPT}", service.ServiceType.DisplayName);
        temp = temp.Replace("{LAW.ARTICLE}", service.TreasuryCode.Name);
        if (!service.Quantity.Unit.IsEmptyInstance) {
          temp = temp.Replace("{QTY}", service.Quantity.Amount.ToString("N0"));
          temp = temp.Replace("{UNIT}", service.Quantity.Unit.Name);
        } else {
          temp = temp.Replace("{QTY}", "&nbsp;");
          temp = temp.Replace("{UNIT}", "&nbsp;");
        }
        temp = temp.Replace("{SUBTOTAL}", service.Fee.SubTotal.ToString("C2"));
        temp = temp.Replace("{DISCOUNTS}", service.Fee.Discount.Amount.ToString("C2"));
        temp = temp.Replace("{TOTAL}", service.Fee.Total.ToString("C2"));
        temp = temp.Replace("{NOTES}", service.Notes);
        html += temp;
      }
      return html;
    }

    protected string GetConcepts() {
      FixedList<LRSTransactionService> services = transaction.Services;

      const string template = "<tr width='24px'><td valign='top'>{NUMBER}</td>" +
                              "<td style='white-space:normal' valign='top'>{CONCEPT}&nbsp; &nbsp; &nbsp;</td>" +
                              "<td align='right' style='white-space:nowrap' valign='top'>{OPERATION.VALUE}</td>" +
                              "<td align='right' style='white-space:nowrap' valign='top'>{QTY}</td>" +
                              "<td style='white-space:nowrap' valign='top'>{UNIT}</td>" +
                              "<td style='white-space:nowrap' valign='top'>{CODE}</td>" +
                              "<td style='white-space:normal' valign='top'>{LAW.ARTICLE}</td>" +
                              "<td style='white-space:nowrap' valign='top'>{NOTES}</td></tr>";

      string html = String.Empty;
      for (int i = 0; i < services.Count; i++) {
        LRSTransactionService service = services[i];
        string temp = template.Replace("{NUMBER}", (i + 1).ToString("00"));
        temp = temp.Replace("{CONCEPT}", service.ServiceType.DisplayName);
        temp = temp.Replace("{OPERATION.VALUE}", service.OperationValue.Amount != decimal.Zero ? service.OperationValue.ToString() : "&nbsp;");

        if (!service.Quantity.Unit.IsEmptyInstance) {
          temp = temp.Replace("{QTY}", service.Quantity.Amount.ToString("N0"));
          temp = temp.Replace("{UNIT}", service.Quantity.Unit.Name);
        } else {
          temp = temp.Replace("{QTY}", "&nbsp;");
          temp = temp.Replace("{UNIT}", "&nbsp;");
        }
        temp = temp.Replace("{CODE}", service.TreasuryCode.FinancialConceptCode);
        temp = temp.Replace("{LAW.ARTICLE}", service.TreasuryCode.Name);
        temp = temp.Replace("{NOTES}", service.Notes);
        html += temp;
      }
      return html;
    }


    protected string GetPaymentTotal() {
      var payment = LRSPayment.Empty;

      if (transaction.Payments.Count > 0) {
        payment = transaction.Payments[0];
      }

      return payment.ReceiptTotal.ToString("C2");
    }


    protected string GetRecordingActsWithTotals() {
      const string template = "<tr width='24px'><td>{NUMBER}</td><td>{CONCEPT.CODE}</td>" +
                              "<td style='white-space:normal'>{RECORDING.ACT}&nbsp; &nbsp; &nbsp;</td>" +
                              "<td style='white-space:nowrap'>{LAW.ARTICLE}</td>" +
                              "<td align='right' style='white-space:nowrap'>{OPERATION.VALUE}</td>" +
                              "<td align='right' style='white-space:nowrap'>{RECORDING.RIGHTS}</td>" +
                              "<td align='right' style='white-space:nowrap'>{SHEETS.REVISION}</td>" +
                              "<td align='right' style='white-space:nowrap'>{OTHERS.FEE}</td>" +
                              "<td align='right' style='white-space:nowrap'>{SUBTOTAL}</td>" +
                              "<td align='right' style='white-space:nowrap'>{DISCOUNTS}</td>" +
                              "<td align='right' style='white-space:nowrap'><b>{TOTAL}</b></td></tr>";

      const string othersTemplate = "<tr width='24px'><td colspan='3'>&nbsp;</td><td><i>Otros conceptos:</i></td><td colspan='7'><i>{CONCEPTS}</i></td></tr>";

      const string totalsTemplate = "<tr width='24px' class='upperSeparatorRow'><td colspan='4'>{TOTAL_SPEECH}</td><td align='right'><b>Total</b>:</td>" +
                "<td align='right'><b>{0}</b></td><td align='right'><b>{1}</b></td><td align='right'><b>{2}</b></td>" +
                "<td align='right'><b>{3}</b></td><td align='right'><b>{4}</b></td><td align='right' style='font-size:11pt'><b>{5}</b></td></tr>";

      string html = String.Empty;

      FixedList<LRSTransactionService> services = transaction.Services;
      for (int i = 0; i < services.Count; i++) {
        LRSTransactionService service = services[i];
        string temp = template.Replace("{NUMBER}", (i + 1).ToString("00"));
        temp = temp.Replace("{RECORDING.ACT}", service.ServiceType.DisplayName);
        temp = temp.Replace("{LAW.ARTICLE}", service.TreasuryCode.Name);
        temp = temp.Replace("{CONCEPT.CODE}", service.TreasuryCode.FinancialConceptCode);
        temp = temp.Replace("{OPERATION.VALUE}", service.OperationValue.Amount.ToString("C2"));
        temp = temp.Replace("{RECORDING.RIGHTS}", service.Fee.RecordingRights.ToString("C2"));
        temp = temp.Replace("{SHEETS.REVISION}", service.Fee.SheetsRevision.ToString("C2"));
        decimal othersFee = service.Fee.ForeignRecordingFee;
        temp = temp.Replace("{OTHERS.FEE}", othersFee.ToString("C2"));
        temp = temp.Replace("{SUBTOTAL}", service.Fee.SubTotal.ToString("C2"));
        temp = temp.Replace("{DISCOUNTS}", service.Fee.Discount.Amount.ToString("C2"));
        temp = temp.Replace("{TOTAL}", service.Fee.Total.ToString("C2"));
        html += temp;
        if (othersFee != decimal.Zero) {
          temp = String.Empty;
          if (service.Fee.ForeignRecordingFee != decimal.Zero) {
            temp += " Trámite foráneo: " + service.Fee.ForeignRecordingFee.ToString("C2") + " &nbsp;";
          }
          html += othersTemplate.Replace("{CONCEPTS}", temp);
        }
      }

      LRSFee totalFee = transaction.Services.TotalFee;

      string temp1 = totalsTemplate.Replace("{0}", totalFee.RecordingRights.ToString("C2"));
      temp1 = temp1.Replace("{1}", totalFee.SheetsRevision.ToString("C2"));
      temp1 = temp1.Replace("{2}", totalFee.ForeignRecordingFee.ToString("C2"));
      temp1 = temp1.Replace("{3}", totalFee.SubTotal.ToString("C2"));
      temp1 = temp1.Replace("{4}", totalFee.Discount.Amount.ToString("C2"));
      temp1 = temp1.Replace("{5}", totalFee.Total.ToString("C2"));

      temp1 = temp1.Replace("{TOTAL_SPEECH}", EmpiriaSpeech.SpeechMoney(totalFee.Total));

      return html + temp1;
    }

    protected string GetCurrentUserInitials() {
      if (ExecutionServer.IsAuthenticated) {
        var user = ExecutionServer.CurrentContact;

        return user.Initials;
      } else {
        return "Interesado";
      }
    }

    protected string Encode(string source) {
      return HttpUtility.HtmlEncode(source);
    }

    #endregion Private methods

  } // class TransactionReceipt

} // namespace Empiria.Land.WebApp
