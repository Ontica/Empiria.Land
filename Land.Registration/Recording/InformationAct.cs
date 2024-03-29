﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : InformationAct                                 Pattern  : Empiria Object Type                 *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Represents an information recording act that are not limitations and can be applied to        *
*              properties, persons or neither.                                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Represents an information recording act that are not limitations and can be applied to
  /// properties, persons or neither.</summary>
  public class InformationAct : RecordingAct {

    #region Constructors and parsers

    private InformationAct(RecordingActType powertype) : base(powertype) {
      // Required by Empiria Framework for all partitioned types.
    }

    internal InformationAct(RecordingActType recordingActType, LandRecord landRecord,
                            Resource resource) : base(recordingActType, landRecord) {
      recordingActType.AssertIsApplicableResource(resource);

      this.AttachResource(resource);
    }

    public InformationAct(RecordingActType recordingActType, LandRecord landRecord,
                          Resource resource, BookEntry bookEntry)
                                      : base(recordingActType, landRecord, bookEntry) {
      recordingActType.AssertIsApplicableResource(resource);

      this.AttachResource(resource);
    }

    static public new InformationAct Parse(int id) {
      return BaseObject.ParseId<InformationAct>(id);
    }

    #endregion Constructors and parsers

    #region Private methods

    private void AttachResource(Resource resource) {
      if (resource is RealEstate) {
        this.SetRealEstate((RealEstate) resource);
        return;
      }

      var tract = resource.Tract.GetRecordingActs();

      if (tract.Count != 0) {     // This is not the first act of the real estate
        base.SetResource(resource, ResourceRole.Informative);
      } else {
        base.SetResource(resource, ResourceRole.Created);
      }
    }


    private void SetRealEstate(RealEstate property) {
      Assertion.Require(property, "property");

      var tract = property.Tract.GetRecordingActs();

      if (tract.Count != 0) {     // This is not the first act of the real estate
        base.SetResource(property, ResourceRole.Informative);
        return;
      }

      if (property.IsPartition) {
        base.SetResource(property, ResourceRole.PartitionOf, property.IsPartitionOf);
      } else {
        base.SetResource(property, ResourceRole.Created);
      }
    }

    #endregion Private methods

  } // class InformationAct

} // namespace Empiria.Land.Registration
