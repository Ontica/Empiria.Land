/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Transactions                      Assembly : Empiria.Land.Registration           *
*  Type      : LRSFee                                         Pattern  : Standard Class                      *
*  Version   : 2.0        Date: 23/Oct/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Fee payed in order to obtain a service in a Recorder Office.                                  *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System.Data;

using Empiria.DataTypes;

namespace Empiria.Land.Registration.Transactions {

  /// <summary>Fee payed in order to obtain a service in a Recorder Office.</summary>
  public class LRSFee {

    #region Constructors and parsers

    internal LRSFee() {

    }

    static internal LRSFee Parse(DataRow row) {
      LRSFee fee = new LRSFee();

      fee.RecordingRights = (decimal) row["RecordingRightsFee"];
      fee.SheetsRevision = (decimal) row["SheetsRevisionFee"];
      fee.Aclaration = (decimal) row["AclarationFee"];
      fee.Usufruct = (decimal) row["UsufructFee"];
      fee.Easement = (decimal) row["ServidumbreFee"];
      fee.SignCertification = (decimal) row["SignCertificationFee"];
      fee.ForeignRecord = (decimal) row["ForeignRecordFee"];
      fee.OthersCharges = (decimal) row["OthersFee"];
      fee.Discount = Discount.Parse(DiscountType.Parse((int) row["DiscountTypeId"]), 
                                                       (decimal) row["Discount"]);

      return fee;
    }

    static internal LRSFee Parse(FixedList<LRSTransactionItem> list) {
      LRSFee fee = new LRSFee();

      foreach (LRSTransactionItem act in list) {
        fee.Add(act.Fee);
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

    public decimal Aclaration {
      get;
      set;
    }

    public decimal Usufruct {
      get;
      set;
    }

    public decimal Easement {
      get;
      set;
    }

    public decimal SignCertification {
      get;
      set;
    }

    public decimal ForeignRecord {
      get;
      set;
    }

    public decimal OthersCharges {
      get;
      set;
    }

    public decimal SubTotal {
      get {
        return this.RecordingRights + this.SheetsRevision + this.Aclaration + this.Usufruct +
               this.Easement + this.SignCertification + this.ForeignRecord + this.OthersCharges;
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

    internal void Add(LRSFee fee) {
      this.RecordingRights += fee.RecordingRights;
      this.SheetsRevision += fee.SheetsRevision;
      this.Aclaration += fee.Aclaration;
      this.Usufruct += fee.Usufruct;
      this.Easement += fee.Easement;
      this.SignCertification += fee.SignCertification;
      this.ForeignRecord += fee.ForeignRecord;
      this.OthersCharges += fee.OthersCharges;
      this.Discount += fee.Discount;
    }

    #endregion Internal methods

  } // class LRSFee

} // namespace Empiria.Land.Registration.Transactions
