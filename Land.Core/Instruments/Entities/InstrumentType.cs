/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Legal Instruments                          Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Power type                              *
*  Type     : Instrument                                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Power type that defines a legal instrument type: deed, contract, court order, etc.             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Ontology;

namespace Empiria.Land.Instruments {

  /// <summary>Power type that defines a legal instrument type: deed, contract, court order, etc.</summary>
  [Powertype(typeof(Instrument))]
  internal class InstrumentType : Powertype {

    #region Constructors and parsers

    private InstrumentType() {
      // Empiria powertype types always have this constructor.
    }

    static public new InstrumentType Parse(int typeId) {
      if (typeId != -1) {
        return ObjectTypeInfo.Parse<InstrumentType>(typeId);
      } else {
        return InstrumentType.Empty;
      }
    }

    static internal new InstrumentType Parse(string typeName) {
      return InstrumentType.Parse<InstrumentType>(typeName);
    }


    static internal InstrumentType Parse(InstrumentTypeEnum instrumentTypeName) {
      Assertion.Assert(instrumentTypeName != InstrumentTypeEnum.All,
                       "instrumentTypeName can not be equal to 'All'.");

      var fullTypeName = $"ObjectTypeInfo.LegalInstrument.{instrumentTypeName}";

      return InstrumentType.Parse(fullTypeName);
    }


    static public InstrumentType Empty {
      get {
        return InstrumentType.Parse("ObjectType.LegalInstrument");
      }
    }

    #endregion Constructors and parsers


    public string[] InstrumentKinds {
      get {
        var json = base.ExtensionData;

        return json.GetList<string>("InstrumentKinds", false).ToArray();
      }
    }


    public IssuerType[] IssuerTypes {
      get {
        var json = base.ExtensionData;

        return json.GetList<IssuerType>("IssuerTypes", false).ToArray();
      }
    }


    public string AsEnumString {
      get {
        return this.Name.Replace("ObjectTypeInfo.LegalInstrument", string.Empty).Trim('.');
      }
    }


    #region Public methods

    /// <summary>Factory method to create instrument instances of this instrument type.</summary>
    internal Instrument CreateInstance() {
      return base.CreateObject<Instrument>();
    }

    #endregion Public methods

  }  // class InstrumentType

}  // namespace Empiria.Land.Instruments
