/* Empiria Land 2015 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Transactions                      Assembly : Empiria.Land.Registration           *
*  Type      : LRSFee                                         Pattern  : Standard Class                      *
*  Version   : 2.0        Date: 04/Jan/2015                   License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Fee payed in order to obtain a service in a Recorder Office.                                  *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System.Data;

using Empiria.DataTypes;

namespace Empiria.Land.Registration.Transactions {

  /// <summary>Fee payed in order to obtain a service in a Recorder Office.</summary>
  public class LRSFee {

    #region Constructors and parsers

    public LRSFee() {
      this.Discount = Discount.Empty;
    }

    static internal LRSFee Parse(DataRow row) {
      LRSFee fee = new LRSFee();

      fee.RecordingRights = (decimal) row["RecordingRightsFee"];
      fee.SheetsRevision = (decimal) row["SheetsRevisionFee"];
      fee.ForeignRecordingFee = (decimal) row["ForeignRecordingFee"];
      fee.Discount = Discount.Parse(DiscountType.Empty, (decimal) row["Discount"]);

      return fee;
    }

    static internal LRSFee Parse(FixedList<LRSTransactionItem> list) {
      LRSFee fee = new LRSFee();

      foreach (LRSTransactionItem act in list) {
        fee.Sum(act.Fee);
      }
      return fee;
    }

    #endregion Constructors and parsers

    #region Public properties

    public decimal RecordingRights {
      get;
      set;
    }

    public decimal SheetsRevision {
      get;
      set;
    }

    public decimal ForeignRecordingFee {
      get;
      set;
    }

    public decimal SubTotal {
      get {
        return this.RecordingRights + this.SheetsRevision + this.ForeignRecordingFee;
      }
    }

    public Discount Discount {
      get;
      set;
    }

    public decimal Total {
      get { return (this.SubTotal - this.Discount.Amount); }
    }

    #endregion Public properties

    #region Internal methods

    internal void Sum(LRSFee fee) {
      Assertion.AssertObject(fee, "fee");

      this.RecordingRights += fee.RecordingRights;
      this.SheetsRevision += fee.SheetsRevision;
      this.ForeignRecordingFee += fee.ForeignRecordingFee;   
      this.Discount += fee.Discount;
    }

    #endregion Internal methods

  } // class LRSFee

} // namespace Empiria.Land.Registration.Transactions
