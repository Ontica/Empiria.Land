﻿/* Empiria® Land 2013 ****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria® Land                                  System   : Land Registration System            *
*  Namespace : Empiria.Government.LandRegistration            Assembly : Empiria.Government.LandRegistration *
*  Type      : LRSTransactionActList                          Pattern  : Empiria List Class                  *
*  Date      : 23/Oct/2013                                    Version  : 5.2     License: CC BY-NC-SA 3.0    *
*                                                                                                            *
*  Summary   : List of transaction concepts.                                                                 *
*                                                                                                            *
**************************************************** Copyright © La Vía Óntica SC + Ontica LLC. 1999-2013. **/
using System;
using System.Collections.Generic;
using System.Data;

namespace Empiria.Government.LandRegistration.Transactions {

  /// <summary>List of transaction concepts.</summary>
  public class LRSTransactionConceptsList : ObjectList<LRSTransactionAct> {

    #region Fields

    private LRSFee totalFee = new LRSFee();

    #endregion Fields

    #region Constructors and parsers

    public LRSTransactionConceptsList() {
      //no-op
    }

    public LRSTransactionConceptsList(int capacity)
      : base(capacity) {
      // no-op
    }

    public LRSTransactionConceptsList(string name, int capacity)
      : base(name, capacity) {
      //no-op
    }


    public LRSTransactionConceptsList(List<LRSTransactionAct> list)
      : this(list.Count) {
      this.AddRange(list);
      this.CalculateTotals();
    }

    public LRSTransactionConceptsList(System.Func<DataRow, LRSTransactionAct> parseMethod, DataView view)
      : this(view.Count) {
      for (int i = 0; i < view.Count; i++) {
        this.Add(parseMethod.Invoke(view[i].Row));
      }
      this.CalculateTotals();
    }

    public LRSTransactionConceptsList(System.Func<DataRow, LRSTransactionAct> parseMethod, DataTable table)
      : this(table.Rows.Count) {
      for (int i = 0; i < table.Rows.Count; i++) {
        this.Add(parseMethod.Invoke(table.Rows[i]));
      }
      this.CalculateTotals();
    }

    #endregion Constructors and parsers

    #region Public properties

    public LRSFee TotalFee {
      get { return this.totalFee; }
    }

    #endregion Public properties

    #region Public methods

    protected internal new void Add(LRSTransactionAct item) {
      base.Add(item);
      this.CalculateTotals();
    }

    public override LRSTransactionAct this[int index] {
      get {
        return (LRSTransactionAct) base[index];
      }
    }

    public new bool Contains(LRSTransactionAct item) {
      return base.Contains(item);
    }

    public new bool Contains(Predicate<LRSTransactionAct> match) {
      LRSTransactionAct result = base.Find(match);

      return (result != null);
    }

    public override void CopyTo(LRSTransactionAct[] array, int index) {
      for (int i = index, j = Count; i < j; i++) {
        array.SetValue(base[i], i);
      }
    }

    public new LRSTransactionAct Find(Predicate<LRSTransactionAct> match) {
      return base.Find(match);
    }

    public new List<LRSTransactionAct> FindAll(Predicate<LRSTransactionAct> match) {
      return base.FindAll(match);
    }

    protected internal new bool Remove(LRSTransactionAct item) {
      bool result = base.Remove(item);

      this.CalculateTotals();

      return result;
    }

    public new void Sort(Comparison<LRSTransactionAct> comparison) {
      base.Sort(comparison);
    }

    #endregion Public methods

    #region Private methods

    private void CalculateTotals() {
      this.totalFee = new LRSFee();

      for (int i = 0; i < base.Count; i++) {
        this.totalFee.Add(base[i].Fee);
      }
    }

    #endregion Private methods;

  } // class LRSTransactionActList

} // namespace Empiria.Government.LandRegistration.Transactions