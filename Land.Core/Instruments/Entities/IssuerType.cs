/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Legal Instruments                          Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Power type                              *
*  Type     : IssuerType                                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Power type that defines a legal instrument issuer: notary, judge, local authority, etc.        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Ontology;

namespace Empiria.Land.Instruments {

  /// <summary>Power type that defines a legal instrument issuer:
  /// notary, judge, local authority, etc.</summary>
  [Powertype(typeof(Issuer))]
  internal class IssuerType : Powertype {

    #region Constructors and parsers

    private IssuerType() {
      // Empiria powertype types always have this constructor.
    }

    static public new IssuerType Parse(int typeId) => ObjectTypeInfo.Parse<IssuerType>(typeId);

    static public new IssuerType Parse(string typeName) => ObjectTypeInfo.Parse<IssuerType>(typeName);

    static internal IssuerType Parse(IssuerTypeEnum issuerTypeName) {
      Assertion.Assert(issuerTypeName != IssuerTypeEnum.All,
                       "issuerTypeName can not be equal to 'All'.");

      var fullTypeName = $"ObjectTypeInfo.LegalInstrumentIssuer.{issuerTypeName}";

      return IssuerType.Parse(fullTypeName);
    }

    #endregion Constructors and parsers

    #region Public methods

    /// <summary>Factory method to create issuers instances of this issuer type.</summary>
    internal Issuer CreateInstance() {
      return base.CreateObject<Issuer>();
    }

    #endregion Public methods

  }  // class IssuerType

}  // namespace Empiria.Land.Instruments
