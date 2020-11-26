/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Services                       *
*  Namespace : Empiria.Land.WebApi.Models                     Assembly : Empiria.Land.WebApi.dll             *
*  Type      : PropertyBagItem                                Pattern  : Information Holder                  *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Holds information about a property item included in a property bag.                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.WebApi.Models {

  /// <summary>Holds information about a property item included in a property bag.</summary>
  public class PropertyBagItem {

    #region Constructors and parsers

    public PropertyBagItem(string name, object value) {
      Assertion.AssertObject(name, "name");
      Assertion.AssertObject(value, "value");

      this.Name = name;
      this.Value = value;
    }


    public PropertyBagItem(string name, object value, string style) {
      Assertion.AssertObject(name, "name");
      Assertion.AssertObject(value, "value");
      Assertion.AssertObject(style, "style");

      this.Name = name;
      this.Value = value;
      this.Style = style;
    }

    #endregion Constructors and parsers

    #region Properties

    public string Name {
      get;
      private set;
    } = String.Empty;


    public object Value {
      get;
      private set;
    } = String.Empty;


    public string Style {
      get;
      private set;
    } = String.Empty;


    public string DataType {
      get {
        return Value.GetType().Name;
      }
    }

    #endregion Properties

  }  // class PropertyBagItem

}  // namespace Empiria.Land.WebApi.Models
