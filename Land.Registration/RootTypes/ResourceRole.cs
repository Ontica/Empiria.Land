/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : ResourceRole                                   Pattern  : Enumeration Type                    *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Describes the role that plays a resource in the context of a recording act.                   *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Describes the role that plays a resource in the context of a recording act.</summary>
  public enum ResourceRole {

    /// <summary>The recording act created this resource, because is the first time
    /// registration or because it was created as a consequence of a partition or division,
    /// or it was created as a merge of two or more existing resources.
    /// Old registered resources could not have a 'Created' role entry.
    /// 'Created' always denotes the very first act in the recording tract.</summary>
    Created = 'C',

    /// <summary>The act modifies data about the resource but not it structure.
    /// The edited resource remains alive.</summary>
    Edited = 'E',

    /// <summary>The act divides the resource into two or more new resources.
    /// The resource won't be longer available. The recording act must have two
    /// or more role additional entries with 'Created' role value.</summary>
    Divided = 'D',

    /// <summary>The resource appear as a reference of the recording act by itself,
    /// but never is used to denote structure change. Informative roles can be used
    /// as a initial recording tract act in those cases when there are not complete
    /// historical information.</summary>
    Informative = 'I',

    /// <summary>The recording act merges the resource into another resource.
    /// The merged resource is no longer available. The recording act must have an
    /// additional role entry with the 'Extended' role value.</summary>
    Merged = 'M',

    /// <summary>The recording act merges one or more resources into this
    /// already created resource. This resource is extended but the merged ones
    /// are not longer available. The recording act must have one or more role
    /// additional entries with 'Merged' value.</summary>
    Extended = 'T',

    /// <summary>The recording act partitions this resource and creates a new resource.
    /// This resource is modified but remains alive. The recording act must have one
    /// or more role additional entries with the 'Created' role value.</summary>
    Partitioned = 'P',

    /// <summary>The recording act cancels this resource as a consequence of a
    /// recording mistake but not as a result of a merging or division.
    /// The resource won't be longer available.</summary>
    Canceled = 'X',

    ///// <summary>The act doesn't apply to resources (real estates or associations).</summary>
    //NotApply = 'N',

  }  // enum ResourceRole

} // namespace Empiria.Land.Registration
