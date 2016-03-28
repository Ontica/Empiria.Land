/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RealEstateKind                                 Pattern  : Value object                        *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : String classificator for real estate properties.                                              *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Linq;

using Empiria.Ontology;

namespace Empiria.Land.Registration {

  /// <summary>String classificator for real estate properties.</summary>
  public class RealEstateKind : ValueObject<string> {

    #region Fields

    private const string thisTypeName = "ValueType.ListItem.RealEstateKind";

    static private FixedList<RealEstateKind> valuesList =
           RealEstateKind.ValueTypeInfo.GetValuesList<RealEstateKind, string>((x) => new RealEstateKind(x));

    #endregion Fields

    #region Constructors and parsers

    private RealEstateKind(string value) : base(value) {

    }

    static public RealEstateKind Parse(string value) {
      Assertion.AssertObject(value, "value");

      if (value == RealEstateKind.Empty.Value) {
        return RealEstateKind.Empty;
      }
      if (value == RealEstateKind.Unknown.Value) {
        return RealEstateKind.Unknown;
      }
      return valuesList.First((x) => x.Value == value);
    }

    // User-defined conversion from string to PropertyKind
    public static implicit operator RealEstateKind(string value) {
      return RealEstateKind.Parse(value);
    }

    // User-defined conversion from PropertyKind to string
    public static implicit operator string(RealEstateKind propertyKind) {
      return propertyKind.Value;
    }

    static public RealEstateKind Empty {
      get {
        RealEstateKind empty = new RealEstateKind("No determinado");
        empty.MarkAsEmpty();

        return empty;
      }
    }

    static public RealEstateKind Unknown {
      get {
        RealEstateKind unknown = new RealEstateKind("No proporcionado");
        unknown.MarkAsUnknown();

        return unknown;
      }
    }

    static public ValueTypeInfo ValueTypeInfo {
      get {
        return ValueTypeInfo.Parse(thisTypeName);
      }
    }

    static public FixedList<RealEstateKind> GetList() {
      return valuesList;
    }

    #endregion Constructors and parsers

  } // class RealEstateKind

} // namespace Empiria.Land.Registration
