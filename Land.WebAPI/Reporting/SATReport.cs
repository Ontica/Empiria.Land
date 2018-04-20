/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Reporting services                           Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Structurer                            *
*  Type     : SATReport                                    License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Generates an integration report for the SAT (Servicio de Administración Tributaria).           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Text;

using Empiria.Json;

using Empiria.Land.Registration;

namespace Empiria.Land.WebApi.Reporting {

  /// <summary>Generates an integration report for the SAT (Servicio de Administración Tributaria).</summary>
  public class SATReport {

    private readonly string reportsPath = ConfigurationData.GetString("Reporting.FilesPath");
    private readonly string reportsBaseAddress = ConfigurationData.GetString("Reporting.BaseAddress");

    #region Constructors and parsers

    internal SATReport(DateTime fromDate, DateTime toDate, string fileName) {
      this.FromDate = fromDate;
      this.ToDate = toDate;
      this.EmissionDate = DateTime.Today;
      this.FileName = fileName;

      this.Items = new List<SATReportItem>(16000);
    }

    #endregion Constructors and parsers

    #region Properties

    public DateTime FromDate {
      get;
    }

    public DateTime ToDate {
      get;
    }

    public DateTime EmissionDate {
      get;
    }

    public string FileName {
      get;
    }

    public string ReportUrl {
      get {
        return this.reportsBaseAddress + "/" + FileName;
      }
    }

    internal FixedList<RecordingDocument> Documents {
      get;
      private set;
    }

    internal List<SATReportItem> Items {
      get;
    }

    internal List<string> RowsAsText {
      get;
    }

    #endregion Properties

    #region Methods

    public void Build() {
      LoadDocuments();

      LoadReportRows();

      var text = new StringBuilder();

      foreach (var reportItem in this.Items) {
        text.AppendLine(reportItem.ToTextLine(this.EmissionDate));
      }

      Save(text.ToString());
    }

    public JsonObject ToJson() {
      var o = new {
        FromDate,
        ToDate,
        EmissionDate,
        ReportUrl
      };

      return JsonObject.Parse(o);
    }

    #endregion Methods

    #region Private methods
    private void LoadDocuments() {
      string filter = $"'{FromDate.ToString("yyyy-MM-dd")}' <= AuthorizationTime AND " +
                      $"AuthorizationTime <= '{ToDate.ToString("yyyy-MM-dd 23:59:59")}'";

      this.Documents = RecordingDocument.SearchClosed(filter);
    }


    private void LoadReportRows() {
      this.Items.Clear();

      foreach (var document in this.Documents) {
        var recordingActs =
            document.RecordingActs
                    .FindAll((x) => x.RecordingActType.IsDomainActType ||
                                    x.RecordingActType.IsLimitationActType ||
                                    x.RecordingActType.AppliesTo == RecordingRuleApplication.Association);

        foreach (var recordingAct in recordingActs) {

          var resource = recordingAct.Resource;

          var parties = recordingAct.GetParties();

          parties = parties.FindAll((x) => x.PartyRole.Id > 1200);

          if (parties.Count != 0) {

            foreach (var party in parties) {
              var row = new SATReportItem(document, recordingAct, resource, party);

              this.Items.Add(row);
            }

          }
        }
      }
    }

    private void Save(string contents) {

      var fullPath = System.IO.Path.Combine(reportsPath, this.FileName);

      System.IO.File.WriteAllText(fullPath, contents);
    }

    #endregion Private methods

  }  // class SATReport

} // namespace Empiria.Land.WebApi.Reporting
