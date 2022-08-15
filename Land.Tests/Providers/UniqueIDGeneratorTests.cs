/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Empiria Land Providers                     Component : Test cases                              *
*  Assembly : Empiria.Land.Tests.dll                     Pattern   : Test class                              *
*  Type     : UniqueIDGeneratorTests                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Unit tests for the services provided by the unique ID generator. IDs are used to identify      *
*             transactions, certificates, recordings and recordable entities (real estate, associations).    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Xunit;

using Empiria.Tests;

using Empiria.Land.Providers;

namespace Empiria.Land.Tests.Providers {

  /// <summary>Unit tests for the services provided by the unique ID generator. IDs are used to identify
  /// transactions, certificates, recordings and recordable entities (real estate, associations).</summary>
  public class UniqueIDGeneratorTests {

    public UniqueIDGeneratorTests() {
      TestsCommonMethods.Authenticate();
    }


    [Fact]
    public void Should_Generate_An_AssociationID() {
      const int ASSOCIATION_ID_SIZE = 15;
      const string ASSOCIATION_ID_STARTS_WITH = "PM";

      IUniqueIDGeneratorProvider provider = ExternalProviders.GetUniqueIDGeneratorProvider();

      string sut = provider.GenerateAssociationID();

      AssertIDIsValid(sut, ASSOCIATION_ID_SIZE, ASSOCIATION_ID_STARTS_WITH);
    }



    [Fact]
    public void Should_Generate_A_CertificateID() {
      const int CERTIFICATE_ID_SIZE = 24;
      const string CERTIFICATE_ID_STARTS_WITH = "CE";

      IUniqueIDGeneratorProvider provider = ExternalProviders.GetUniqueIDGeneratorProvider();

      string sut = provider.GenerateCertificateID();

      AssertIDIsValid(sut, CERTIFICATE_ID_SIZE, CERTIFICATE_ID_STARTS_WITH);
    }


    [Fact]
    public void Should_Generate_A_NoPropertyID() {
      const int NO_PROPERTY_ID_SIZE = 15;
      const string NO_PROPERTY_ID_STARTS_WITH = "RD";

      IUniqueIDGeneratorProvider provider = ExternalProviders.GetUniqueIDGeneratorProvider();

      string sut = provider.GenerateNoPropertyID();

      AssertIDIsValid(sut, NO_PROPERTY_ID_SIZE, NO_PROPERTY_ID_STARTS_WITH);
    }


    [Fact]
    public void Should_Generate_A_RecordID() {
      const int DOCUMENT_ID_SIZE = 24;
      const string DOCUMENT_ID_STARTS_WITH = "RP";

      IUniqueIDGeneratorProvider provider = ExternalProviders.GetUniqueIDGeneratorProvider();

      string sut = provider.GenerateRecordID();

      AssertIDIsValid(sut, DOCUMENT_ID_SIZE, DOCUMENT_ID_STARTS_WITH);
    }


    [Fact]
    public void Should_Generate_A_RealEstateID() {
      const int REAL_ESTATE_ID_SIZE = 19;
      const string REAL_ESTATE_ID_STARTS_WITH = "FR";

      IUniqueIDGeneratorProvider provider = ExternalProviders.GetUniqueIDGeneratorProvider();

      string sut = provider.GenerateRealEstateID();

      AssertIDIsValid(sut, REAL_ESTATE_ID_SIZE, REAL_ESTATE_ID_STARTS_WITH);
    }


    [Fact]
    public void Should_Generate_A_TansactionID() {
      const int TRANSACTION_ID_SIZE = 19;
      const string TRANSACTION_ID_STARTS_WITH = "TR";

      IUniqueIDGeneratorProvider provider = ExternalProviders.GetUniqueIDGeneratorProvider();

      string sut = provider.GenerateTransactionID();

      AssertIDIsValid(sut, TRANSACTION_ID_SIZE, TRANSACTION_ID_STARTS_WITH);
    }


    #region Helpers

    private void AssertIDIsValid(string sut, int ID_SIZE, string STARTS_WITH) {
      string CUSTOMER_ID = ExecutionServer.LicenseName == "Zacatecas" ? "ZS" : "TL";

      Assert.NotNull(sut);
      Assert.Equal(ID_SIZE, sut.Length);
      Assert.StartsWith($"{STARTS_WITH}-{CUSTOMER_ID}", sut);
      Assert.DoesNotContain(" ", sut);
      Assert.Equal(EmpiriaString.Clean(sut), sut);
    }

    #endregion Helpers

  }  // class UniqueIDGeneratorTests

}  // namespace Empiria.Land.Tests.Providers
