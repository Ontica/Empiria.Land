/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Registration                          Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Command payload                         *
*  Type     : RegistrationCommand                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Command payload used for recording acts registration.                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;

namespace Empiria.Land.Registration.Adapters {

  /// <summary>Command used for recording acts registration.</summary>
  public class RegistrationCommand {

    public RegistrationCommandType Type {
      get; set;
    } = RegistrationCommandType.Undefined;


    public RegistrationCommandPayload Payload {
      get; set;
    } = new RegistrationCommandPayload();


    internal void EnsureIsValid() {
      Assertion.Assert(Type != RegistrationCommandType.Undefined,
                       "Unrecognized RegistrationCommand.Type");

      Assertion.AssertObject(Payload, "RegistrationCommand.Payload");

      Assertion.AssertObject(Payload.RecordingActTypeUID, "RegistrationCommand.Payload.RecordingActTypeUID");
    }

  }  // class RegistrationCommand


  /// <summary>Command payload used for recording acts registration.</summary>
  public class RegistrationCommandPayload {

    public string RecordingActTypeUID {
      get; set;
    } = string.Empty;


    public string RecordableSubjectUID {
      get; set;
    } = string.Empty;


  }  // class RegistrationCommandPayload


}  // namespace Empiria.Land.Registration.Adapters
