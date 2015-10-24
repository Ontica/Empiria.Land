/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Transactions                      Assembly : Empiria.Land.Registration           *
*  Type      : LRSTransactionTaskList                         Pattern  : Empiria List Class                  *
*  Version   : 2.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : List of transaction tasks or workflow items.                                                  *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Collections.Generic;
using System.Data;

using Empiria.Land.Registration.Data;

namespace Empiria.Land.Registration.Transactions {

  /// <summary>List of transaction items or lines. Can be used to hold
  /// lines of one or many transactions.</summary>
  public class LRSTransactionTaskList : FixedList<LRSTransactionTask> {

    #region Constructors and parsers

    private LRSTransactionTaskList(List<LRSTransactionTask> list) : base(list) {
      //no-op
    }

    static public LRSTransactionTaskList Parse(LRSTransaction transaction) {
      List<LRSTransactionTask> list = TransactionData.GetLRSTransactionTaskList(transaction);

      return new LRSTransactionTaskList(list);
    }

    #endregion Constructors and parsers

    #region Public properties

    public override LRSTransactionTask this[int index] {
      get {
        return (LRSTransactionTask) base[index];
      }
    }

    #endregion Public properties

    #region Public methods

    protected internal new void Add(LRSTransactionTask task) {
      base.Add(task);
    }

    public new bool Contains(LRSTransactionTask task) {
      return base.Contains(task);
    }

    public new bool Contains(Predicate<LRSTransactionTask> match) {
      return (base.Find(match) != null);
    }

    public override void CopyTo(LRSTransactionTask[] array, int index) {
      for (int i = index, j = Count; i < j; i++) {
        array.SetValue(base[i], i);
      }
    }

    public new LRSTransactionTask Find(Predicate<LRSTransactionTask> match) {
      return base.Find(match);
    }

    public new List<LRSTransactionTask> FindAll(Predicate<LRSTransactionTask> match) {
      return base.FindAll(match);
    }

    protected internal new bool Remove(LRSTransactionTask task) {
      bool result = base.Remove(task);

      return result;
    }

    public new void Sort(Comparison<LRSTransactionTask> comparison) {
      base.Sort(comparison);
    }

    #endregion Public methods

  } // class LRSTransactionTaskList

} // namespace Empiria.Land.Registration.Transactions
