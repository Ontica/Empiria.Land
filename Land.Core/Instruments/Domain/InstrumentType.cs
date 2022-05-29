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

using Empiria.Land.Instruments.Adapters;

namespace Empiria.Land.Instruments {

  /// <summary>Power type that defines a legal instrument type: deed, contract, court order, etc.</summary>
  [Powertype(typeof(Instrument))]
  public class InstrumentType : Powertype {

    #region Constructors and parsers

    private InstrumentType() {
      // Empiria powertype types always have this constructor.
    }

    static public new InstrumentType Parse(int typeId) => ObjectTypeInfo.Parse<InstrumentType>(typeId);

    static internal new InstrumentType Parse(string typeName) => InstrumentType.Parse<InstrumentType>(typeName);

    static internal InstrumentType Parse(InstrumentTypeEnum instrumentTypeName) {
      Assertion.Require(instrumentTypeName != InstrumentTypeEnum.All,
                      "instrumentTypeName can not be equal to 'All'.");

      var fullTypeName = $"ObjectTypeInfo.LegalInstrument.{instrumentTypeName}";

      return InstrumentType.Parse(fullTypeName);
    }


    internal static FixedList<InstrumentType> GetListForRecordingBooks() {
      var listType = GeneralList.Parse("RecordingBooks.InstrumentTypes");

      return listType.GetItems<InstrumentType>();
    }


    static public InstrumentType Empty => InstrumentType.Parse("ObjectType.LegalInstrument");

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


    public InstrumentTypeEnum ToInstrumentTypeEnum() {
      string typeNamedKey = this.NamedKey;

      InstrumentTypeEnum result;

      if (Enum.TryParse<InstrumentTypeEnum>(typeNamedKey, out result)) {
        return result;
      } else {
        throw Assertion.EnsureNoReachThisCode($"Cannot convert type name '{typeNamedKey}' to InstrumentTypeEnum.");
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
