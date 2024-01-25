/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : RecordableSubjects                         Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Information Holder                      *
*  Type     : SubjectEditionRules                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Flags holder with recordable subject's edition rules.                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.RecordableSubjects.Adapters {

  /// <summary>Flags holder with recordable subject's edition rules.</summary>
  public class SubjectEditionRules {

    internal SubjectEditionRules() {
      // no-op
    }

    public bool CanBeClosed {
      get; internal set;
    }


    public bool CanBeOpened {
      get; internal set;
    }


    public bool CanBeUpdated {
      get; internal set;
    }


    public bool CanBeDeleted {
      get; internal set;
    }

  }  // class SubjectEditionRules

}  // namespace Empiria.Land.RecordableSubjects.Adapters
