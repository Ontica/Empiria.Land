/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Search services                            Component : Interface adapters                      *
*  Assembly : Empiria.Land.SearchServices.dll            Pattern   : Data Transfer Object                    *
*  Type     : RecordableSubjectQueryResultDto            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Search result DTO for all recordable subjects: real estate, associations and no-property.      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.RecordableSubjects.Adapters;
using Empiria.Land.Registration.Adapters;

namespace Empiria.Land.SearchServices {

  /// <summary>Search result DTO for all recordable subjects: real estate, associations and no-property.</summary>
  public class RecordableSubjectQueryResultDto {

    internal RecordableSubjectQueryResultDto() {
      // no-op
    }


    public string UID {
      get; internal set;
    }


    public string Type {
      get; internal set;
    }


    public string Name {
      get; internal set;
    }


    public string ElectronicID {
      get; internal set;
    }


    public string Kind {
      get; internal set;
    }


    public string Status {
      get; internal set;
    }


    public RecordableSubjectDto RecordableSubject {
      get; internal set;
    }


    public RecordDto Record {
      get; internal set;
    }

  }  // class RecordableSubjectQueryResultDto

 }  // namespace Empiria.Land.SearchServices
