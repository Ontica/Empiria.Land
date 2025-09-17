/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : SecondaryPartyRole                             Pattern  : Storage Item                        *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Describes the role that plays a party with respect to another party.                          *
*              Secondary roles are of the form: 'X plays the role R with respect of Y'.                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
namespace Empiria.Land.Registration {

  /// <summary>Describes the role that plays a party with respect to another party.
  /// Secondary roles are of the form: 'X plays the role R with respect of Y'.</summary>
  public class SecondaryPartyRole : BasePartyRole {

    #region Constructors and parsers

    private SecondaryPartyRole() {
      // Required by Empiria Framework.
    }


    static public SecondaryPartyRole Parse(string uid) {
      return BaseObject.ParseKey<SecondaryPartyRole>(uid);
    }


    static public FixedList<SecondaryPartyRole> GetList() {
      return GeneralObject.GetList<SecondaryPartyRole>();
    }

    #endregion Constructors and parsers

    #region Public properties

    public string InverseRoleName {
      get {
        return base.ExtendedDataField.Get<string>("InverseRoleName", string.Empty);
      }
    }

    #endregion Public properties

  } // class SecondaryPartyRole

} // namespace Empiria.Land.Registration
