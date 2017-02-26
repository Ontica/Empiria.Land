/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : NoPropertyResource                             Pattern  : Empiria Object Type                 *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Represents a no-property or document resource. Wills, testaments and some contracts are       *
*              kinds of No-property resources.                                                               *
*                                                                                                            *
********************************* Copyright (c) 2009-2017. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

using Empiria.Land.Registration.Data;

namespace Empiria.Land.Registration {

  /// <summary>Represents a no-property or document resource. Wills, testaments and some contracts are
  /// kinds of No-property resources.</summary>
  public class NoPropertyResource : Resource {

    #region Constructors and parsers

    internal NoPropertyResource() {
      // Required by Empiria Framework
    }

    static public new NoPropertyResource Parse(int id) {
      return BaseObject.ParseId<NoPropertyResource>(id);
    }

    #endregion Constructors and parsers

    #region Public methods

    protected override string GenerateResourceUID() {
      return TransactionData.GenerateNoPropertyResourceUID();
    }

    protected override void OnSave() {
      ResourceData.WriteNoPropertyResource(this);
    }

    #endregion Public methods

  }  // class NoPropertyResource

} // namespace Empiria.Land.Registration
