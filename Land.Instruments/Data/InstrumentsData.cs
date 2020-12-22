/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Legal Instruments                          Component : Data Access Layer                       *
*  Assembly : Empiria.Land.Instruments.dll               Pattern   : Data Services                           *
*  Type     : InstrumentsData                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data read and write services for legal instruments.                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

namespace Empiria.Land.Instruments.Data {

  /// <summary>Data read and write services for legal instruments.</summary>
  static internal class InstrumentsData {

    static internal void WriteInstrument(Instrument o) {
      var op = DataOperation.Parse("writeLRSInstrument",
            o.Id, o.UID, o.InstrumentType.Id, o.Kind, o.ControlID, o.Issuer.Id,
            o.IssueDate, o.Summary, o.AsText, o.ExtData.ToString(), o.Keywords,
            o.SheetsCount, (char) o.Status, o.PostedById, o.PostingTime, String.Empty);

      DataWriter.Execute(op);
    }

  }  // class InstrumentsData

}  // namespace Empiria.Land.Instruments.Data
