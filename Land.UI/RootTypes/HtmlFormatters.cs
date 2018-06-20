/* Empiria Land ***********************************************************************************************
*                                                                                                             *
*  Solution  : Empiria Land                                    System   : Land Registration System            *
*  Namespace : Empiria.Land.UI                                 Assembly : Empiria.Land.UI                     *
*  Type      : ResourceHistoryGrid                             Pattern  : Standard class                      *
*  Version   : 3.0                                             License  : Please read license.txt file        *
*                                                                                                             *
*  Summary   : Generates a grid HTML content that display the resource's history.                             *
*                                                                                                             *
********************************** Copyright (c) 2009-2017. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

using Empiria.Land.Registration;

namespace Empiria.Land.UI {

  /// <summary>Auxiliary methods library to generate HTML content.</summary>
  static public class HtmlFormatters {

    static internal string GetDateAsText(DateTime date) {
      if (date == ExecutionServer.DateMinValue ||
          date == ExecutionServer.DateMaxValue) {
        return "N/D";
      } else {
        return date.ToString(@"dd/MMM/yyyy");
      }
    }

    static public string GetImagingLinks(RecordingBook recordingBook) {
      if (!recordingBook.HasImageSet) {
        return "&nbsp;";
      }
      string html = String.Empty;

      html += "<a href='javascript:doOperation(\"onSelectImageSet\", {{RECORDING.BOOK.IMAGE.SET.ID}});'>" +
              "<img src='../themes/default/bullets/book.gif' title='Libro registral'></a>";

      html = html.Replace("{{RECORDING.BOOK.IMAGE.SET.ID}}", recordingBook.ImageSetId.ToString());

      return html;
    }

    static public string GetImagingLinks(RecordingDocument document) {
      if (!document.Imaging.HasImageSet && !document.Imaging.HasAuxiliarImageSet) {
        return "&nbsp;";
      }
      string html = String.Empty;

      if (document.Imaging.HasImageSet) {
        html = "<a href='javascript:doOperation(\"onSelectImageSet\", {{DOCUMENT.IMAGE.SET.ID}});'>" +
                  "<img src='../themes/default/bullets/scribble_doc_sm.gif' title='Instrumento registral'></a>";
        html = html.Replace("{{DOCUMENT.IMAGE.SET.ID}}", document.Imaging.ImageSetId.ToString());
      }

      if (document.Imaging.HasAuxiliarImageSet) {
        html += "<a href='javascript:doOperation(\"onSelectImageSet\", {{AUXILIAR.IMAGE.SET.ID}});'>" +
                   "<img src='../themes/default/bullets/clip.gif' title='Anexos al instrumento registral'></a>";
        html = html.Replace("{{AUXILIAR.IMAGE.SET.ID}}", document.Imaging.AuxiliarImageSetId.ToString());
      }

      return html;
    }

    static public string GetImagingLinks(RecordingAct recordingAct) {
      RecordingDocument document = recordingAct.Document;

      if (!document.Imaging.HasImageSet && !document.Imaging.HasAuxiliarImageSet &&
           recordingAct.PhysicalRecording.IsEmptyInstance) {
        return "&nbsp;";
      }
      string html = String.Empty;

      if (document.Imaging.HasImageSet) {
        html = "<a href='javascript:doOperation(\"onSelectImageSet\", {{DOCUMENT.IMAGE.SET.ID}});'>" +
                  "<img src='../themes/default/bullets/scribble_doc_sm.gif' title='Instrumento registral'></a>";
        html = html.Replace("{{DOCUMENT.IMAGE.SET.ID}}", document.Imaging.ImageSetId.ToString());
      }

      if (document.Imaging.HasAuxiliarImageSet) {
        html += "<a href='javascript:doOperation(\"onSelectImageSet\", {{AUXILIAR.IMAGE.SET.ID}});'>" +
                   "<img src='../themes/default/bullets/clip.gif' title='Anexos al instrumento registral'></a>";
        html = html.Replace("{{AUXILIAR.IMAGE.SET.ID}}", document.Imaging.AuxiliarImageSetId.ToString());
      }

      if (!recordingAct.PhysicalRecording.IsEmptyInstance &&
           recordingAct.PhysicalRecording.RecordingBook.HasImageSet) {
        html += "<a href='javascript:doOperation(\"onSelectImageSet\", {{RECORDING.BOOK.IMAGE.SET.ID}});'>" +
                   "<img src='../themes/default/bullets/book.gif' title='Libro registral'></a>";
        html = html.Replace("{{RECORDING.BOOK.IMAGE.SET.ID}}",
                            recordingAct.PhysicalRecording.RecordingBook.ImageSetId.ToString());
      }

      return html;
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
