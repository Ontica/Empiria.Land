/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RealEstateRule                                 Pattern  : Standard  Class                     *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Describes a real estate recording condition that serves as a recording registration rule.     *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

using Empiria.Json;

namespace Empiria.Land.Registration {

  /// <summary>Describes a real estate recording condition that serves as a
  /// recording registration rule.</summary>
  public class RealEstateRule {

    #region Constructors and parsers

    internal RealEstateRule() {
      this.Expire = false;
      this.IsInternalDivision = false;
      this.Name = String.Empty;
      this.RecordableObjectStatus = ResourceRecordingStatus.Undefined;
      this.UseNumbering = false;
    }

    static internal RealEstateRule Parse(JsonObject json) {
      RealEstateRule rule = new RealEstateRule();

      rule.Expire = json.Get<bool>("Expire", false);
      rule.IsInternalDivision = json.Get<bool>("IsInternalDivision", false);
      rule.Name = json.Get<string>("Name", String.Empty);
      rule.RecordableObjectStatus = json.Get<ResourceRecordingStatus>("RecordableObjectStatus",
                                                                      ResourceRecordingStatus.Undefined);
      rule.UseNumbering = json.Get<bool>("UseNumbering", false);

      return rule;
    }

    #endregion Constructors and parsers

    #region Properties

    public bool Expire {
      get;
      internal set;
    }

    public bool IsInternalDivision {
      get;
      internal set;
    }

    public string Name {
      get;
      internal set;
    }

    public ResourceRecordingStatus RecordableObjectStatus {
      get;
      internal set;
    }

    public bool UseNumbering {
      get;
      internal set;
    }

    #endregion Properties

  }  // class RealEstateRule

}  // namespace Empiria.Land.Registration
