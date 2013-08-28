/* Empiria® Land 2013 ****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria® Land                                  System   : Land Registration System            *
*  Namespace : Empiria.Government.LandRegistration            Assembly : Empiria.Government.LandRegistration *
*  Type      : PropertyRule                                   Pattern  : Standard  Class                     *
*  Date      : 23/Oct/2013                                    Version  : 5.2     License: CC BY-NC-SA 3.0    *
*                                                                                                            *
*  Summary   : Describes a property recording condition that serves as a rule for recording registration.    *
*                                                                                                            *
**************************************************** Copyright © La Vía Óntica SC + Ontica LLC. 1999-2013. **/
using System;

namespace Empiria.Government.LandRegistration {

  /// <summary>Describes a property recording condition that serves as a rule for 
  /// recording registration.</summary>
  public class PropertyRule {

    #region Constructors and parsers

    internal PropertyRule() {

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

    public PropertyRecordingStatus RecordingStatus {
      get;
      internal set;
    }

    public PropertyCount PropertyCount {
      get;
      internal set;
    }

    public bool UseNumbering {
      get;
      internal set;
    }

    #endregion Properties

  }  // class PropertyRule

}  // namespace Empiria.Government.LandRegistration
