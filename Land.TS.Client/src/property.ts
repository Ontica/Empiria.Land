/**
 *  Solution : Empiria Land Client                             || v0.1.0104
 *  Type     : Empiria.Land.Property
 *  Summary  : Type to handle Empiria Land properties.
 *
 *  Author   : José Manuel Cota <https://github.com/jmcota>
 *  License  : GNU GPLv3. Other licensing terms are available. See <https://github.com/Ontica/Empiria.Land>
 *
 *  Copyright (c) 2015-2016. Ontica LLC, La Vía Óntica SC and contributors. <http://ontica.org>
*/

module Empiria.Land {

  /** Type to handle Empiria Land properties. */
  export class Property {

    // #region Constructor and parsers

    /**
    * Static method that returns true if a property with a unique ID exists, or false otherwise.
    * @param propertyUID A string with the unique ID of the property.
    */
    public static exists(propertyUID: string): boolean {
      var dataOperation = Empiria.DataOperation.parse("existsLandProperty", propertyUID);

      return dataOperation.existsData();
    }

    // #endregion Constructor and parsers

  }  // class Property

}  // module Empiria.Land
