/* Empiria Land ***********************************************************************************************
*                                                                                                             *
*  Solution  : Empiria Land                                    System   : Land Registration System            *
*  Namespace : Empiria.Land.UI                                 Assembly : Empiria.Land.UI                     *
*  Type      : ResourceHistoryGrid                             Pattern  : Standard class                      *
*  Version   : 2.1                                             License  : Please read license.txt file        *
*                                                                                                             *
*  Summary   : Generates a grid HTML content that display the resource's history.                             *
*                                                                                                             *
********************************** Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

using Empiria.Land.Registration;

namespace Empiria.Land.UI {

  /// <summary>Auxiliary methods library to generate HTML content.</summary>
  static internal class HtmlFormatters {

    static internal string GetDateAsText(DateTime date) {
      if (date == ExecutionServer.DateMinValue ||
          date == ExecutionServer.DateMaxValue) {
        return "N/D";
      } else {
        return date.ToString(@"dd/MMM/yyyy");
      }
    }

    static internal string NoWrap(string text) {
      return "<span style='white-space:nowrap;'>" + text + "</span>";
    }

    static internal string SetPresentationAndAuthorizationDates(string row, RecordingDocument document) {
      if (document.PresentationTime == ExecutionServer.DateMinValue &&
          document.AuthorizationTime == ExecutionServer.DateMinValue) {
        row = row.Replace("{{PRESENTATION.DATE}}", "No constan");
        row = row.Replace("{{AUTHORIZATION.DATE}}", "&nbsp;");
      } else {
        row = row.Replace("{{PRESENTATION.DATE}}", GetDateAsText(document.PresentationTime));
        row = row.Replace("{{AUTHORIZATION.DATE}}", GetDateAsText(document.AuthorizationTime));
      }
      return row;
    }

    static internal string TableWrapper(string html) {
      return "<table class='details' style='width:96%'>" + html + "</table>";
    }

  } // class HtmlFormatters

} // namespace Empiria.Land.UI
