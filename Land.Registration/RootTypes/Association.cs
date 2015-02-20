/* Empiria Land 2015 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : Association                                    Pattern  : Empiria Object Type                 *
*  Version   : 2.0        Date: 04/Jan/2015                   License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Represents a social association or organization.                                              *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;

using Empiria.Contacts;
using Empiria.DataTypes;
using Empiria.Geography;
using Empiria.Json;
using Empiria.Security;
using Empiria.Land.Registration.Data;

namespace Empiria.Land.Registration {

  /// <summary>Represents a social association or organization.</summary>
  public class Association : Resource {

    #region Constructors and parsers
    
    private Association() {
      // Required by Empiria Framework
    }

    internal Association(string associationName) : base(associationName) {

    }

    #endregion Constructors and parsers

    #region Public methods

    protected override string CreatePropertyKey() {
      return TransactionData.GenerateAssociationKey();
    }

    protected override void OnSave() {
      PropertyData.WriteAssociation(this);
    }

    static public new Resource Parse(int id) {
      return BaseObject.ParseId<Resource>(id);
    }

    static public new Resource TryParseWithUID(string propertyUID) {
      DataRow row = PropertyData.GetPropertyWithUID(propertyUID);

      if (row != null) {
        return BaseObject.ParseDataRow<Resource>(row);
      } else {
        return null;
      }
    }

    #endregion Public methods

  }  // class Association

} // namespace Empiria.Land.Registration
