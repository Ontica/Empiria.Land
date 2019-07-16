/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                 System   : Land Registration System              *
*  Namespace : Empiria.Land.Data                            Assembly : Empiria.Land.Registration             *
*  Type      : ResourceData                                 Pattern  : Data Services                         *
*  Version   : 3.0                                          License  : Please read license.txt file          *
*                                                                                                            *
*  Summary   : Provides database methods for recordable resources: real estate and associations.             *
*                                                                                                            *
********************************* Copyright (c) 2009-2017. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

using Empiria.Data;


namespace Empiria.Land.Instruments {

  /// <summary>Provides database methods for recordable resources: real estate and associations.</summary>
  static public class InstrumentData {

    #region Internal methods
    static internal void WriteInstrument(LegalInstrument o) {
      var operation = DataOperation.Parse("writeLRSInstrument", o.Id, o.UID, o.GetEmpiriaType().Id,
                                          o.RequestedBy, o.Number, o.IssueOffice.Id,
                                          o.IssuedBy.Id, o.IssueDate,
                                          o.ExtensionData.ToString(), o.Summary, o.Keywords,
                                          (char) o.Status, o.PostingTime, o.PostedBy.Id,
                                          o.Integrity.GetUpdatedHashCode());

      DataWriter.Execute(operation);
    }


    #endregion Internal methods

  } // class InstrumentData

} // namespace Empiria.Land.Instruments
