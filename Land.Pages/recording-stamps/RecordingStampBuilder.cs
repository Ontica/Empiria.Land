/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Empiria Land Pages                         Component : Presentation Layer                      *
*  Assembly : Empiria.Land.Pages.dll                     Pattern   : Web Page                                *
*  Type     : RecordingStampBuilder                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Recording stamp builder methods.                                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Registration;

namespace Empiria.Land.Pages {

  /// <summary>Recording stamp builder methods.</summary>
  internal class RecordingStampBuilder {

    private readonly RecordingDocument _document;

    internal RecordingStampBuilder(RecordingDocument document) {
      Assertion.Require(document, nameof(document));

      _document = document;
    }


    internal string RecordingActsText(RecordingAct selectedRecordingAct, bool isMainDocument) {
      string html = String.Empty;

      int index = 0;

      foreach (RecordingAct recordingAct in _document.RecordingActs) {
        string temp = String.Empty;

        index++;

        var builder = new RecordingActTextBuilder(recordingAct);

        // If amendment act, process it and continue
        if (recordingAct.RecordingActType.IsAmendmentActType) {
          temp =  builder.GetAmendmentActText(index);
          html += builder.Decorate(selectedRecordingAct, isMainDocument, temp);
          html += builder.GetPartiesText();
          html += builder.GetNotesText();
          html += "<br/>";

          continue;
        }

        // If not amendment act, then process it by resource type application

        var resource = Reloaders.Reload(recordingAct.Resource);

        if (resource is RealEstate) {
          temp = builder.GetRealEstateActText(index);

        } else if (resource is Association) {
          temp = builder.GetAssociationActText((Association) resource, index);

        } else if (resource is NoPropertyResource) {
          temp = builder.GetNoPropertyActText(index);

        } else {
          throw Assertion.EnsureNoReachThisCode();

        }

        html += builder.Decorate(selectedRecordingAct, isMainDocument, temp);
        html += builder.GetPartiesText();
        html += builder.GetNotesText();
        html += "<br/>";
      }

      return html;
    }

  }  // class RecordingStampBuilder

}  // namespace Empiria.Land.WebApp
