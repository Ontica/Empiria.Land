/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Recordable Subjects                        Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Command payload                         *
*  Type     : SearchRecordableSubjectsCommand            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Command payload used for recordable subjects searching.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.RecordableSubjects.Adapters {

  public class SearchRecordableSubjectsCommand {

    public RecordableSubjectType Type {
      get; set;
    } = RecordableSubjectType.None;


    public string Keywords {
      get;
      set;
    } = string.Empty;


    public string OrderBy {
      get;
      set;
    } = String.Empty;


    public int PageSize {
      get;
      set;
    } = 50;


    public int Page {
      get;
      set;
    } = 1;


  }  // class // SearchRecordableSubjectsCommand

}  // namespace Empiria.Land.RecordableSubjects.Adapters
