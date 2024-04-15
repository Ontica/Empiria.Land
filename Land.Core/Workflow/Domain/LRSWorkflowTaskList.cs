/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Workflow                       Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Information Holder                      *
*  Type     : LRSWorkflowTaskList                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : List of workflow tasks in the context of a Recorder Office transaction.                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.Land.Transactions.Workflow.Data;

namespace Empiria.Land.Transactions.Workflow {

  /// <summary>List of workflow tasks in the context of a Recorder Office transaction.</summary>
  public class LRSWorkflowTaskList : FixedList<LRSWorkflowTask> {

    #region Constructors and parsers

    internal LRSWorkflowTaskList() {

    }

    private LRSWorkflowTaskList(List<LRSWorkflowTask> list) : base(list) {
      //no-op
    }

    static public LRSWorkflowTaskList Parse(LRSTransaction transaction) {
      List<LRSWorkflowTask> list = WorkflowData.GetWorkflowTrack(transaction);

      return new LRSWorkflowTaskList(list);
    }

    #endregion Constructors and parsers

    #region Public properties

    public override LRSWorkflowTask this[int index] {
      get {
        return base[index];
      }
    }

    #endregion Public properties

    #region Public methods

    protected internal new void Add(LRSWorkflowTask task) {
      base.Add(task);
    }

    public new bool Contains(LRSWorkflowTask task) {
      return base.Contains(task);
    }

    public new bool Contains(Predicate<LRSWorkflowTask> match) {
      return (base.Find(match) != null);
    }

    public override void CopyTo(LRSWorkflowTask[] array, int index) {
      for (int i = index, j = Count; i < j; i++) {
        array.SetValue(base[i], i);
      }
    }

    public new LRSWorkflowTask Find(Predicate<LRSWorkflowTask> match) {
      return base.Find(match);
    }

    public new FixedList<LRSWorkflowTask> FindAll(Predicate<LRSWorkflowTask> match) {
      return base.FindAll(match);
    }

    protected internal new bool Remove(LRSWorkflowTask task) {
      bool result = base.Remove(task);

      return result;
    }

    public new void Sort(Comparison<LRSWorkflowTask> comparison) {
      base.Sort(comparison);
    }

    #endregion Public methods

  } // class LRSWorkflowTaskList

} // namespace Empiria.Land.Transactions.Workflow
