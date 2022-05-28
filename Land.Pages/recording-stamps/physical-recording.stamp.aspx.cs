/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Empiria Land Pages                         Component : Presentation Layer                      *
*  Assembly : Empiria.Land.Pages.dll                     Pattern   : Web Page                                *
*  Type     : PhysicalRecordingStamp                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Recording stamp for instrument recording in physical books.                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using System.Web.UI;

using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Pages {

	/// <summary>Recording stamp for instrument recording in physical books.</summary>
	public partial class PhysicalRecordingStamp : Page {

    #region Fields

		protected LRSTransaction transaction = null;
		private FixedList<PhysicalRecording> recordings = null;
		private PhysicalRecording baseRecording = null;

    #endregion Fields

		#region Constructors and parsers

		protected void Page_Load(object sender, EventArgs e) {
			Initialize();
		}

		#endregion Constructors and parsers

		#region Private methods

		private void Initialize() {
			transaction = GetTransaction();

			Assertion.Ensure(!transaction.Document.IsEmptyInstance, "Transaction does not have a registration document.");

			recordings = PhysicalRecording.GetDocumentRecordings(transaction.Document.Id);

			Assertion.Ensure(recordings.Count > 0, "Document does not have recordings.");

			int recordingId = int.Parse(Request.QueryString["id"]);
			if (recordingId != -1) {
				baseRecording = recordings.Find((x) => x.Id == recordingId);
			} else {
				recordings.Sort((x, y) => x.RecordingTime.CompareTo(y.RecordingTime));
				baseRecording = recordings[recordings.Count - 1];
			}

			Assertion.Ensure(baseRecording, "We have a problem reading document recording data.");
		}


    private LRSTransaction GetTransaction() {
			if (String.IsNullOrWhiteSpace(Request.QueryString["transactionId"]) ||
					Request.QueryString["transactionId"] == "-1") {
				var physicalRecording = PhysicalRecording.Parse(int.Parse(Request.QueryString["id"]));

				return physicalRecording.MainDocument.GetTransaction();
			}

			return LRSTransaction.Parse(int.Parse(Request.QueryString["transactionId"]));
		}


    protected string CustomerOfficeName() {
			return "Dirección de Catastro y Registro Público";
    }


    protected string DistrictName {
      get {
        if (ExecutionServer.LicenseName == "Zacatecas") {
          return "Registro Público del Distrito de Zacatecas";
        }
        return String.Empty;
      }
    }


		protected bool ShowAllRecordings {
			get {
				return (int.Parse(Request.QueryString["id"]) == -1);
			}
		}


		protected string GetPaymentText() {
			const string t = "Derechos por <b>{AMOUNT}</b> según recibo <b>{RECEIPT}</b> expedido por la Secretaría de Finanzas del Estado, que se archiva.";

			var payment = LRSPayment.Empty;

			if (transaction.Payments.Count > 0) {
				payment = transaction.Payments[0];
			}
			string x = t.Replace("{AMOUNT}", payment.ReceiptTotal.ToString("C2"));
			x = x.Replace("{RECEIPT}", payment.ReceiptNo);

			return x;
		}


		protected string GetPrelationText() {
			const string t = "Presentado para su examen y registro en	{CITY}, el <b>{DATE} a las {TIME} horas</b>, bajo el número de trámite <b>{NUMBER}</b> - Conste";


			DateTime presentationTime = transaction.LastReentryTime == ExecutionServer.DateMaxValue ? transaction.PresentationTime : transaction.LastReentryTime;

			string x = t.Replace("{DATE}", presentationTime.ToString(@"dd \de MMMM \de yyyy"));
			x = x.Replace("{TIME}", presentationTime.ToString("HH:mm:ss"));
			x = x.Replace("{NUMBER}", transaction.UID);
			x = x.Replace("{CITY}", "Zacatecas, Zacatecas");

			return x;
		}


		protected string GetRecordingsText() {
			if (this.ShowAllRecordings) {
				return GetAllRecordingsText();
			}

			const string template = "Registrado bajo el número de <b>inscripción {NUMBER}</b> del <b>Volumen {VOL}</b> <b>{SECTION}</b> del <b>Distrito Judicial de {DISTRICT}</b>.";
			string x = String.Empty;

			x = template.Replace("{NUMBER}", baseRecording.Number);

			x = x.Replace("{VOL}", baseRecording.RecordingBook.BookNumber);
			x = x.Replace("{SECTION}", baseRecording.RecordingBook.RecordingSection.Name);
			x = x.Replace("{DISTRICT}", baseRecording.RecordingBook.RecorderOffice.Alias);
			x = x.Replace("{DOCUMENT}", transaction.Document.UID);

			return x;
		}


    protected string GetRecordingOfficialsInitials() {
      string temp = String.Empty;

      for (int i = 0; i < recordings.Count; i++) {
        string initials = recordings[i].RecordedBy.Nickname;
        if (initials.Length == 0) {
          continue;
        }
        if (!temp.Contains(initials)) {
          temp += initials + " ";
        }
      }
      temp = temp.Trim().ToLowerInvariant();
      return temp.Length != 0 ? "* " + temp : String.Empty;
    }


		protected string GetAllRecordingsText() {
			const string docMulti = "Registrado bajo las siguientes {COUNT} inscripciones:<br/><br/>";

			const string docOne = "Registrado bajo la siguiente inscripción:<br/><br/>";

			const string t1 = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Inscripción <b>{NUMBER}</b> del <b>Volumen {VOL}</b> <b>{SECTION}</b> del <b>Distrito Judicial de {DISTRICT}</b>.<br/>";

			string html = String.Empty;


			if (this.recordings.Count > 1) {
				html = docMulti.Replace("{DOCUMENT}", transaction.Document.UID);
				html = html.Replace("{COUNT}", this.recordings.Count.ToString() + " (" + EmpiriaString.SpeechInteger(this.recordings.Count).ToLower() + ")");
			} else if (this.recordings.Count == 1) {
				html = docOne.Replace("{DOCUMENT}", transaction.Document.UID);
			} else if (this.recordings.Count == 0) {
				throw new Exception("Document does not have recordings.");
			}

			for (int i = 0; i < recordings.Count; i++) {
				string x = t1.Replace("{NUMBER}", recordings[i].Number);
				x = x.Replace("{VOL}", recordings[i].RecordingBook.BookNumber);
				x = x.Replace("{SECTION}", recordings[i].RecordingBook.RecordingSection.Name);
				x = x.Replace("{DISTRICT}", recordings[i].RecordingBook.RecorderOffice.Alias);
				html += x;
			}
			return html;
		}


		protected string GetRecordingPlaceAndDate() {
			const string t = "Registrado en {CITY}, a las {TIME} horas del {DATE}. Doy Fe.";

			string x = t.Replace("{DATE}", baseRecording.RecordingTime.ToString(@"dd \de MMMM \de yyyy"));
			x = x.Replace("{TIME}", baseRecording.RecordingTime.ToString(@"HH:mm"));

			x = x.Replace("{CITY}", "Zacatecas, Zacatecas");

			return x;
		}


		protected string GetRecordingSignerPosition() {
			return "C. Oficial Registrador del Distrito Judicial de Zacatecas";
		}


		protected string GetRecordingSignerName() {
			if (baseRecording.RecordingTime >= new DateTime(2022, 4, 1)) {
				return "Lic. Roberto López Arellano";
			} else {
				return "Lic. Teresa de Jesús Alvarado Ortiz";
			}
		}


		protected string GetDigitalSeal() {
			string s = "||" + transaction.UID + "|" + transaction.Document.UID;
			if (this.ShowAllRecordings) {
				for (int i = 0; i < recordings.Count; i++) {
					s += "|" + recordings[i].Id.ToString();
				}
			} else {
				s += "|" + this.baseRecording.Id.ToString();
			}
			s += "||";
			return Empiria.Security.Cryptographer.SignTextWithSystemCredentials(s);
		}


		protected string GetDigitalSignature() {
			string s = "||" + transaction.UID + "|" + transaction.Document.UID;
			if (this.ShowAllRecordings) {
				for (int i = 0; i < recordings.Count; i++) {
					s += "|" + recordings[i].Id.ToString();
				}
			} else {
				s += "|" + this.baseRecording.Id.ToString();
			}
			return Empiria.Security.Cryptographer.SignTextWithSystemCredentials(s + "eSign");
		}


		protected string GetUpperMarginPoints() {
			decimal centimeters = Math.Max(5.0m, 1.0m);   // transaction.Document.SealUpperPosition

			return (centimeters * 28.3464657m).ToString("G4");
		}

		#endregion Private methods

	} // class RecordingSeal

} // namespace Empiria.Web.UI.FSM
