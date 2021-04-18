/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Registration Services                      Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Service provider                        *
*  Type     : RecordableSubjectUpdater                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides services to update recordable subjects.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.DataTypes;
using Empiria.Geography;

using Empiria.Land.RecordableSubjects.Adapters;

namespace Empiria.Land.Registration {

  /// <summary>Provides services to update recordable subjects.</summary>
  internal class RecordableSubjectUpdater {

    internal RecordableSubjectUpdater() {
      // no-op
    }


    internal void Update(RecordingAct recordingAct,
                         RecordableSubjectFields fields) {
      EnsureIsEditable(recordingAct);
      EnsureValidFields(recordingAct, fields);

      if (fields is AssociationFields) {
        Update(recordingAct.Resource as Association, fields as AssociationFields);

      } else if (fields is NoPropertyFields) {
        Update(recordingAct.Resource as NoPropertyResource, fields as NoPropertyFields);

      } else if (fields is RealEstateFields) {
        RealEstate realEstate = (RealEstate) recordingAct.Resource;

        Update(realEstate, fields as RealEstateFields);

        recordingAct.OnResourceUpdated(realEstate);

      } else {
        throw Assertion.AssertNoReachThisCode($"Unhandled RecordableSubjectFields type {fields.GetType()}.");
      }
    }


    private void EnsureIsEditable(RecordingAct recordingAct) {
      Assertion.Assert(recordingAct.IsEditable,
          "The associated recording act is not editable, so is not possible to modify its linked resource.");
    }


    private void EnsureValidFields(RecordingAct recordingAct, RecordableSubjectFields fields) {
      Resource resource = recordingAct.Resource;

      Assertion.Assert(resource.GUID == fields.UID,
          $"The UID of the resource that should be modified '{fields.UID}' is not equal to " +
          $"the resource linked to the recording act '{resource.GUID}'.");

      Assertion.Assert(resource.UID == fields.ElectronicID,
          $"The ElectronicID of the resource that should be modified '{fields.ElectronicID}' " +
          $"is not equal to the ID of the resource linked to the recording act {resource.UID}.");


      Assertion.Assert(resource.GetEmpiriaType().NamedKey == fields.Type.ToString(),
          $"The resource linked to the recording act is of type '{resource.GetEmpiriaType().NamedKey}', " +
          $"but the type of the resource that should be modified is marked as '{fields.Type}'.");

    }


    static private void Update(Association association,
                               AssociationFields fields) {
      association.RecorderOffice = RecorderOffice.Parse(fields.RecorderOfficeUID);
      association.Kind = fields.Kind;
      association.Name = fields.Name;
      association.Description = fields.Description;

      association.Save();
    }


    static private void Update(NoPropertyResource noPropertyResource,
                               NoPropertyFields fields) {
      noPropertyResource.RecorderOffice = RecorderOffice.Parse(fields.RecorderOfficeUID);
      noPropertyResource.Kind = fields.Kind;
      noPropertyResource.Name  = fields.Name;
      noPropertyResource.Description = fields.Description;

      noPropertyResource.Save();
    }


    static private void Update(RealEstate realEstate,
                               RealEstateFields fields) {
      realEstate.Kind = fields.Kind;
      realEstate.RecorderOffice = RecorderOffice.Parse(fields.RecorderOfficeUID);
      realEstate.Municipality = Municipality.Parse(fields.MunicipalityUID);
      realEstate.LotSize = Quantity.Parse(Unit.Parse(fields.LotSizeUnitUID), fields.LotSize);
      realEstate.Description = fields.Description;

      var data = new RealEstateExtData();

      data.CadastralKey = fields.CadastralID;
      data.MetesAndBounds = fields.MetesAndBounds;

      realEstate.SetExtData(data);

      realEstate.SetStatus(fields.Status);

      realEstate.Save();
    }

  }  // class RecordableSubjectUpdater

}  // namespace Empiria.Land.Registration
