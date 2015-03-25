/* Empiria Land 2015 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : ResourceTarget                                 Pattern  : Association Class                   *
*  Version   : 2.0        Date: 04/Jan/2015                   License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Application of a recording act with a resource (property or association).                     *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;

using Empiria.Contacts;
using Empiria.DataTypes;
using Empiria.Security;

using Empiria.Land.Registration.Data;

namespace Empiria.Land.Registration {

  public enum ResourceRole {
    Created = 'C',
    Divided = 'D',
    Edited = 'E',
    Merged = 'M',
    NotApply = 'N',
    Partitioned = 'P',
    Informative = 'I',
  }

  /// <summary>Application of a recording act with another entity that can be a resource (property
  /// or association), a document, a party (person or organization) or another recording act.</summary>
  public class ResourceTarget : RecordingActTarget {

    #region Constructors and parsers

    protected ResourceTarget() {
      // Required by Empiria Framework.
    }

    internal ResourceTarget(RecordingAct recordingAct, Resource resource, ResourceRole resourceRole)
         : base(recordingAct) {
      Assertion.AssertObject(resource, "resource");
      Assertion.Assert(resourceRole != Registration.ResourceRole.NotApply,
                       "resourceRole wrong value");

      //this.Resource = resource;
      this.ResourceRole = resourceRole;
    }

    static public new ResourceTarget Parse(int id) {
      return BaseObject.ParseId<ResourceTarget>(id);
    }

    #endregion Constructors and parsers

    #region Public properties


    //[DataField("TargetPropertyId", Default = "Empiria.Land.Registration.Property.Empty")]
    //public Resource Resource {
    //  get;
    //  private set;
    //}

    [DataField("TargetPropertyRole", Default = ResourceRole.Informative)]
    public ResourceRole ResourceRole {
      get;
      private set;
    }

    #endregion Public properties

    #region Public methods

    protected override void OnSave() {
      if (this.Resource.IsNew) {
        this.Resource.Save();
      }
      RecordingActsData.WriteResourceTarget(this);
    }

    #endregion Public methods

  } // class ResourceTarget

} // namespace Empiria.Land.Registration
