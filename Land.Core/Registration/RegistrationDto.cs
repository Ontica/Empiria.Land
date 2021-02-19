/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Registration Services                      Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Transfer Object                    *
*  Type     : RegistrationDto                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO with data representing a legal instrument registration.                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Media.Adapters;

using Empiria.Land.PhysicalBooks.Adapters;

namespace Empiria.Land.Registration.Adapters {

  /// <summary>Output DTO with data representing a legal instrument registration.</summary>
  public class RegistrationDto {

    public string UID {
      get; internal set;
    } = string.Empty;


    public string RegistrationID {
      get; internal set;
    } = string.Empty;


    public FixedList<PhysicalRecordingDto> PhysicalRecordings {
      get; internal set;
    } = new FixedList<PhysicalRecordingDto>();


    public MediaDto StampMedia {
      get; internal set;
    } = new MediaDto();

  }  // class RegistrationDto

}  // namespace Empiria.Land.Registration.Adapters
