﻿/* Empiria Land **********************************************************************************************
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

    public NoPropertyResource() {
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
      IUniqueIDGeneratorProvider provider = ExternalProviders.GetUniqueIDGeneratorProvider();

      return provider.GenerateNoPropertyID();
    }

    public override ResourceShapshotData GetSnapshotData() {
      return new NoPropertyShapshotData {
        Kind = this.Kind,
        Name = this.Name,
        Description = this.Description,
        Status = ((char) this.Status).ToString()
      };
    }


    protected override void OnSave() {
      ResourceData.WriteNoPropertyResource(this);
    }

    #endregion Public methods

  }  // class NoPropertyResource

} // namespace Empiria.Land.Registration
