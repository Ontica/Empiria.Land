/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Legal Instruments                          Component : Interface adapters                      *
*  Assembly : Empiria.Land.Registration.dll              Pattern   : Loose coupling interface                *
*  Type     : IInstrument                                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Temporal interface used to link land records (RecordinDocuments) with their Instrument.        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Contacts;
using Empiria.Geography;

namespace Empiria.Land.Instruments {

  /// <summary>Temporal interface used to link land records (RecordinDocuments) with their Instrument.</summary>
  public interface IInstrument {

    int Id { get; }

    IIdentifiable InstrumentType { get; }

    string Kind { get; }

    string Summary { get; }

    string BinderNo { get; }

    DateTime IssueDate { get; }

    int SheetsCount { get; }

    IIssuer Issuer { get; }

  }  // interface IInstrument



  public interface IIssuer {


    Contact RelatedContact { get; }

    Organization RelatedEntity { get; }

    GeographicRegion RelatedPlace { get; }

  }  // interface IIssuer

}  // namespace Empiria.Land.Instruments
