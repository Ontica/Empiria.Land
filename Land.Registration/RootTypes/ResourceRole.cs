/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : ResourceRole                                   Pattern  : Enumeration Type                    *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Describes the role that a resource plays in the context of a recording act.                   *
*                                                                                                            *
********************************* Copyright (c) 2009-2017. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Describes the role that a resource plays in the context of a recording act.</summary>
  public enum ResourceRole {

    /// <summary>The recording act created this resource, because it is its first
    /// time registration. There are not related resource.
    /// 'Created' always denotes the first act in the recording tract.</summary>
    //   Example:
    //   Act 1    P  Created  Empty      -- P is a new resource
    Created = 'C',


    /// <summary>The recording act creates this resource as a partition of
    /// the RelatedResource. RelatedResource remains alive.
    /// 'Partition' always denotes the first act in the recording tract.</summary>
    //   Example:
    //   Act 1  P1  PartitionOf  P      -- P1 is a new resource. P remains alive.
    PartitionOf = 'P',


    /// <summary>The act creates the resource as a division of the RelatedResource.
    /// The document must have one or more additional acts with 'DivisionOf'.
    /// At the end, the document must have an act applied to the RelatedResource
    /// with a 'Split' role. 'DivisionOf' denotes the first act in the resource's tract.</summary>
    //  Please see 'Split' for an example.
    DivisionOf = 'D',


    /// <summary>The act splits the resource into two or more resources and ends
    /// the life of that resource. Prior, the document must have two or more
    /// additional acts with role 'DivisionOf' applied to new resources using this
    /// resource as a RelatedResource on them.</summary>
    //   Example:
    //   Act 1  D1  DivisionOf  P      -- D1 is a kind of partition of P
    //   Act 2  D2  DivisionOf  P      -- D2 is a kind of partition of P
    //   Act 3  P   Split       Empty  -- P is no longer available
    Split = 'S',


    /// <summary>The recording act creates a new resource as a consequence of a merge
    /// of two or more resources. The RelatedResource is empty, but the document should have
    /// two or more acts with 'MergedInto' roles using as their RelatedResource the new resource
    /// created in this act. 'Extended' denotes the first act in the resource's tract.</summary>
    //  Please see 'MergedInto' for an example.
    Extended = 'T',


    /// <summary>The recording act merges the resource into the RelatedResource.
    /// The resource won't be longer available. The document must have one or more
    /// addditional acts with role 'MergedInto' with the same RelatedResource.
    /// At the begining, a recording act with role 'Extended' must appear in
    /// the document applied to this act RelatedResource.</summary>
    //   Example:
    //   Act 1  Resource P  Extended   Empty     -- P is a new resource
    //   Act 2  Resource M1 MergedInto P         -- M1 is no longer available
    //   Act 3  Resource M2 MergedInto P         -- M2 is no longer available
    MergedInto = 'M',


    /// <summary>The act is a kind of 'Informative' act but, -like creational roles does-,
    /// allows data modification over the resource (e.g, its metes and bounds) but not
    /// over its structure. The edited resource remains alive.
    /// RelatedResource is always empty.</summary>
    Edited = 'E',


    /// <summary>The resource appear as a reference of the recording act by itself,
    /// but not is used to denote structure change or resource data changes.
    /// RelatedResource is always empty. Informative roles can be used as a initial
    /// recording tract act in those cases when there are not complete
    /// historical information.</summary>
    Informative = 'I',


    /// <summary>The recording act cancels this resource as a consequence of a
    /// recording mistake, or when it cancels the unique domain act of a resource.
    /// Other cancelation acts must be recorded with 'Informative' role.
    /// Don't use this role for merging or divisions. The resource won't be longer
    /// available and RelatedResource must be empty.</summary>
    Canceled = 'X',


  }  // enum ResourceRole

} // namespace Empiria.Land.Registration
