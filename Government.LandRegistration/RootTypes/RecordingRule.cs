/* Empiria® Land 2013 ****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria® Land                                  System   : Land Registration System            *
*  Namespace : Empiria.Government.LandRegistration            Assembly : Empiria.Government.LandRegistration *
*  Type      : RecordingRule                                  Pattern  : Empiria Structure Type              *
*  Date      : 25/Jun/2013                                    Version  : 5.1     License: CC BY-NC-SA 3.0    *
*                                                                                                            *
*  Summary   : Describes the conditions and business rules that have to be fulfilled when a                  *
*              RecordingAct is registered.                                                                   *  
*                                                                                                            *
**************************************************** Copyright © La Vía Óntica SC + Ontica LLC. 1999-2013. **/
using System;

namespace Empiria.Government.LandRegistration {

  public enum PropertyCount {
    Undefined,
    One,
    OneOrMore,
    Two,
    TwoOrMore,
  }

  public enum PropertyRecordingStatus {
    NotApply,
    Both,
    Registered,
    Unregistered,
  }

  public enum RecordingRuleApplication {
    Undefined,
    None,
    Property,
    RecordingAct,
    Strucuture,
  }

  /// <summary>Describes the conditions and business rules that have to be fulfilled when a
  /// RecordingAct is registered.</summary>
  public class RecordingRule {   /// : Structure

    #region Fields

    //private const string thisTypeName = "StructureType.RecordingRule";

    #endregion Fields

    #region Constructors and parsers

    public RecordingRule() {   //: base(thisTypeName)

    }

    internal static RecordingRule Parse(string json) {      
      RecordingRule rule = Empiria.Data.DataConverter.ToObject<RecordingRule>(json);
      rule.Text = json;
      //= new RecordingRule();

      //rule.IsAnnotation = o["IsAnnotation"];

      return rule;
    }

    #endregion Constructors and parsers

    #region Properties

    public RecordingRuleApplication AppliesTo {
      get;
      private set;
    }

    public bool AutoCancel {
      get;
      private set;
    }

    public bool IsAnnotation {
      get;
      private set;
    }

    public bool IsCancelation {
      get;
      private set;
    }

    public PropertyRecordingStatus PropertyStatus {
      get;
      private set;
    }

    public PropertyCount PropertyCount {
      get;
      private set;
    }

    public RecordingSection RecordingSection {
      get;
      private set;
    }

    public RecordingActType[] RecordingActTypes {
      get;
      private set;
    }

    public PropertyRule[] InputProperties {
      get;
      private set;
    }

    public PropertyRule[] NewProperties {
      get;
      private set;
    }

    public string Text {
      get;
      private set;
    }

    #endregion Properties

  }  // class RecordingRule

}  // namespace Empiria.Government.LandRegistration