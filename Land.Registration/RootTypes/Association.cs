﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : Association                                    Pattern  : Empiria Object Type                 *
*  Version   : 2.0                                            License  : Please read license.txt file        *
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

    static public new Association Parse(int id) {
      return BaseObject.ParseId<Association>(id);
    }

    static public new Resource TryParseWithUID(string propertyUID) {
      DataRow row = PropertyData.GetPropertyWithUID(propertyUID);

      if (row != null) {
        return BaseObject.ParseDataRow<Resource>(row);
      } else {
        return null;
      }
    }

    #endregion Constructors and parsers

    #region Public methods

    protected override string CreatePropertyKey() {
      return TransactionData.GenerateAssociationKey();
    }

    protected override void OnSave() {
      PropertyData.WriteAssociation(this);
    }

    public RecordingAct GetIncorporationAct() {
      FixedList<RecordingAct> tract = this.GetRecordingActsTractUntil(RecordingAct.Empty, false);

      if (tract.Count == 0) {         // Antecedent no registered
        return RecordingAct.Empty;
      }
      RecordingAct antecedent = tract.FindLast((x) => x.Id == 2750);    // Incorporation act
      if (antecedent != null) {
        return antecedent;
      } else if (tract[0].RecordingActType.Equals(RecordingActType.Empty)) {
        return tract[0];        // Incorporation act
      } else {
        return RecordingAct.Empty;
      }
    }

    #endregion Public methods

  }  // class Association

} // namespace Empiria.Land.Registration