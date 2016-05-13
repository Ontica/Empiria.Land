/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : SecondaryPartyRole                             Pattern  : Storage Item                        *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Describes the role that plays a party with respect to another party.                          *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Describes the role that plays a party with respect to another party.</summary>
  public class SecondaryPartyRole : BasePartyRole {

    #region Constructors and parsers

    private SecondaryPartyRole() {
      // Required by Empiria Framework.
    }

    static public FixedList<SecondaryPartyRole> GetList() {
      return GeneralObject.ParseList<SecondaryPartyRole>();
    }

    #endregion Constructors and parsers

    #region Public properties

    [DataField(GeneralObject.ExtensionDataFieldName + ".InverseRoleName", IsOptional = true)]
    public string InverseRoleName {
      get;
      private set;
    }

    #endregion Public properties

  } // class SecondaryPartyRole

} // namespace Empiria.Land.Registration
