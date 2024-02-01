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

namespace Empiria.Land.Instruments {

  /// <summary>Temporal interface used to link land records (RecordinDocuments) with their Instrument.</summary>
  public interface IInstrument {

    int Id { get; }

    IIdentifiable InstrumentType { get; }

    string Kind { get; }

  }  // public class IInstrument

}  // namespace Empiria.Land.Instruments
