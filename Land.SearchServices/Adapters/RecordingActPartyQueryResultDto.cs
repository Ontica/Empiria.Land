/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Search services                            Component : Interface adapters                      *
*  Assembly : Empiria.Land.SearchServices.dll            Pattern   : Data Transfer Object                    *
*  Type     : RecordingActPartyQueryResultDto            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Search result DTO for a recording act party.                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Registration.Adapters;
using Empiria.Land.RecordableSubjects.Adapters;

namespace Empiria.Land.SearchServices {

  /// <summary>Search result DTO for a recording act party.</summary>
  public class RecordingActPartyQueryResultDto {

    internal RecordingActPartyQueryResultDto() {
      // no-op
    }


    public string UID {
      get; internal set;
    }


    public string Type {
      get; internal set;
    }


    public string Role {
      get; internal set;
    }


    public string RecordingActType {
      get; internal set;
    }


    public string Status {
      get; internal set;
    }


    public PartyDto Party {
      get; internal set;
    }


    public RecordableSubjectDto RecordableSubject {
      get; internal set;
    }


    public RecordDto Record {
      get; internal set;
    }

  }  // class RecordingActPartyQueryResultDto

}  // namespace Empiria.Land.SearchServices
