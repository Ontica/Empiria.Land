/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Empiria Land Tests                         Component : Test Helpers                            *
*  Assembly : Empiria.Land.Tests.dll                     Pattern   : Testing constants                       *
*  Type     : TestingConstants                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides Empiria Land testing constants.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.Tests {

  /// <summary>Provides Empiria Land testing constants.</summary>
  static public class TestingConstants {

    static public string CREATE_ASSOCIATION_RECORDING_ACT_TYPE_UID => "ObjectType.RecordingAct.InformationAct.08";

    static public string CREATE_NO_PROPERTY_RECORDING_ACT_TYPE_UID => "ObjectType.RecordingAct.InformationAct.06";

    static public string CREATE_REAL_ESTATE_RECORDING_ACT_TYPE_UID => "ObjectType.RecordingAct.DomainAct.01";

    static public string EFILING_UID => ConfigurationData.Get<string>("Testing.FilingUID");

    static public string INSTRUMENT_UID => ConfigurationData.Get<string>("Testing.InstrumentUID");

    public static string INSTRUMENT_RECORDING_UID => "2638ff1f-bea6-40f2-827c-4fa17ed5184d";

    static public int PAYMENT_ORDER_ROUTE_NUMBER_LENGTH => 20;

    static internal string SESSION_TOKEN => ConfigurationData.GetString("Testing.SessionToken");

    static public string SIGNED_DOCUMENT_UID => ConfigurationData.Get<string>("Testing.SignedDocumentUID");

    static public string TRANSACTION_UID => ConfigurationData.Get<string>("Testing.TransactionUID");


  }  // class TestingConstants

}  // namespace Empiria.Land.Tests
