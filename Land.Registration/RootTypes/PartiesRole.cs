/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : PartiesRole                                    Pattern  : Storage Item                        *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Describes the role that plays a party with respect to another party.                          *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Describes the role that plays a party with respect to another party.</summary>
  public class PartiesRole : GeneralObject {

    #region Constructors and parsers

    private PartiesRole() {
      // Required by Empiria Framework.
    }

    static public PartiesRole Empty {
      get { return BaseObject.ParseEmpty<PartiesRole>(); }
    }

    static public PartiesRole Unknown {
      get { return BaseObject.ParseUnknown<PartiesRole>(); }
    }

    static public PartiesRole Parse(int id) {
      return BaseObject.ParseId<PartiesRole>(id);
    }

    static public FixedList<PartiesRole> GetList() {
      return GeneralObject.ParseList<PartiesRole>();
    }

    #endregion Constructors and parsers

    #region Public properties

    [DataField(GeneralObject.ExtensionDataFieldName + ".InverseRoleName", IsOptional = true)]
    public string InverseRoleName {
      get;
      private set;
    }

    #endregion Public properties

  } // class PartiesRole

} // namespace Empiria.Land.Registration
