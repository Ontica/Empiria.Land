/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Reporting services                           Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : ReportingController                          License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web services used to generate reports.                                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

namespace Empiria.Land.WebApi.Reporting {

  /// <summary>Web services used to generate reports.</summary>
  public class ReportingController : WebApiController {

    #region Public APIs

    [HttpGet]
    [AllowAnonymous]
    [Route("v1/reports/sat")]
    public SingleObjectModel BuildSATReport(DateTime fromDate, DateTime toDate, string fileName) {
      try {
        var report = new SATReport(fromDate, toDate, fileName);

        report.Build();

        return new SingleObjectModel(this.Request, report.ToJson());

      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }

    #endregion Public APIs

  }  // class ReportingController

}  // namespace Empiria.Land.WebApi.Reporting
