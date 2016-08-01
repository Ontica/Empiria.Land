/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Transactions                      Assembly : Empiria.Land.Registration           *
*  Type      : LRSWorkflowTaskList                            Pattern  : Empiria List Class                  *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : List of workflow items. Can be used to hold work items from one or many transactions.         *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Collections.Generic;

using Empiria.Land.Registration.Data;

namespace Empiria.Land.Registration.Transactions {

  /// <summary>List of workflow items. Can be used to hold work items from one or many transactions.</summary>
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

} // namespace Empiria.Land.Registration.Transactions
