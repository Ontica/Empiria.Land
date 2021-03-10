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
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Land.Data;
using Empiria.Land.Providers;

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


    static public FixedList<string> NoPropertyKinds() {
      var kindsList = GeneralList.Parse("Land.NoProperty.Kinds");

      return kindsList.GetItems<string>();
    }


    #endregion Constructors and parsers

    #region Public methods

    protected override string GenerateResourceUID() {
      return ExternalProviders.UniqueIDGeneratorProvider.GenerateNoPropertyResourceUID();
    }

    protected override void OnSave() {
      ResourceData.WriteNoPropertyResource(this);
    }

    #endregion Public methods

  }  // class NoPropertyResource

} // namespace Empiria.Land.Registration
