/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Legal Instruments                          Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Command payload                         *
*  Type     : IssuersSearchCommand                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Command payload used to search instrument issuers.                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Instruments.Adapters {

  /// <summary>Command payload used to search instrument issuers.</summary>
  public class IssuersSearchCommand {

    #region Properties

    public InstrumentTypeEnum InstrumentType {
      get; set;
    } = InstrumentTypeEnum.All;


    public string InstrumentKind {
      get; set;
    } = String.Empty;


    public IssuerTypeEnum IssuerType {
      get; set;
    } = IssuerTypeEnum.All;


    public string OnDate {
      get; set;
    } = String.Empty;


    public string Keywords {
      get; set;
    } = string.Empty;


    public string OrderBy {
      get; set;
    } = String.Empty;


    public int PageSize {
      get; set;
    } = 50;


    public int Page {
      get; set;
    } = 1;

    #endregion Properties

  }  // class IssuersSearchCommand

}  // namespace Empiria.Land.Instruments.Adapters
