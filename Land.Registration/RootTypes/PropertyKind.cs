/* Empiria Land 2015 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : PropertyKind                                   Pattern  : Value object                        *
*  Version   : 2.0        Date: 04/Jan/2015                   License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : String classificator for properties.                                                          *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Linq;

using Empiria.Ontology;

namespace Empiria.Land.Registration {

  /// <summary>String classificator for properties.</summary>
  public class PropertyKind : ValueObject<string> {

    #region Fields

    private const string thisTypeName = "ValueType.ListItem.PropertyKind";

    static private FixedList<PropertyKind> valuesList =
           PropertyKind.ValueTypeInfo.GetValuesList<PropertyKind, string>((x) => new PropertyKind(x));

    #endregion Fields

    #region Constructors and parsers

    private PropertyKind(string value) : base(value) {

    }

    static public PropertyKind Parse(string value) {
      Assertion.AssertObject(value, "value");

      if (value == PropertyKind.Empty.Value) {
        return PropertyKind.Empty;
      }
      if (value == PropertyKind.Unknown.Value) {
        return PropertyKind.Unknown;
      }
      return valuesList.First((x) => x.Value == value);
    }

    static public PropertyKind Empty {
      get {
        PropertyKind empty = new PropertyKind("No determinado");
        empty.MarkAsEmpty();

        return empty;
      }
    }

    static public PropertyKind Unknown {
      get {
        PropertyKind unknown = new PropertyKind("No proporcionado");
        unknown.MarkAsUnknown();

        return unknown;
      }
    }

    static public ValueTypeInfo ValueTypeInfo {
      get {
        return ValueTypeInfo.Parse(thisTypeName);
      }
    }

    static public FixedList<PropertyKind> GetList() {
      return valuesList;
    }

    #endregion Constructors and parsers

  } // class PropertyKind

} // namespace Empiria.Geography
