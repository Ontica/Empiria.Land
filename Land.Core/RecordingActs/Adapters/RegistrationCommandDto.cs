/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Registration Services                      Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Transfer Object                    *
*  Type     : RegistrationCommandRuleDto                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO with recording act data.                                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.RecordableSubjects.Adapters;

namespace Empiria.Land.Registration.Adapters {

  public class RegistrationCommandDto {

    public string UID {
      get; internal set;
    }

    public string Name {
      get; internal set;
    }

    public RegistrationCommandRuleDto Rules {
      get; internal set;
    } = new RegistrationCommandRuleDto();


  }  // internal class RegistrationCommandDto



  public class RegistrationCommandRuleDto {


    public RecordableSubjectType SubjectType {
      get; internal set;
    } = RecordableSubjectType.None;


    public bool SelectSubject {
      get; internal set;
    }

    public bool SelectBookEntry {
      get; internal set;
    }

    public bool SelectTargetAct {
      get; internal set;
    }

    public bool NewPartition {
      get; internal set;
    }

  }  // class RegistrationCommandRuleDto

}  // namespace Empiria.Land.Registration.Adapters
