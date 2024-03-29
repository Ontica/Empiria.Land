﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Registration Services                      Component : Interface adapters                      *
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
      Assertion.Require(Type != RegistrationCommandType.Undefined,
                       "Unrecognized RegistrationCommand.Type");

      Assertion.Require(Payload, "RegistrationCommand.Payload");

      Assertion.Require(Payload.RecordingActTypeUID, "RegistrationCommand.Payload.RecordingActTypeUID");
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


    public string PartitionType {
      get; set;
    } = string.Empty;


    public string PartitionNo {
      get; set;
    } = string.Empty;


    public string RecordingBookUID {
      get; set;
    } = string.Empty;


    public string BookEntryUID {
      get; set;
    } = string.Empty;


    public string BookEntryNo {
      get; set;
    } = string.Empty;


    public DateTime PresentationTime {
      get; set;
    } = ExecutionServer.DateMinValue;


    public DateTime AuthorizationDate {
      get; set;
    } = ExecutionServer.DateMinValue;


    public string AmendedRecordingActUID {
      get; set;
    } = string.Empty;


  }  // class RegistrationCommandPayload

}  // namespace Empiria.Land.Registration.Adapters
