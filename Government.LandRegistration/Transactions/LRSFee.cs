/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Transactions                      Assembly : Empiria.Land                        *
*  Type      : LRSFee                                         Pattern  : Standard Class                      *
*  Version   : 5.5        Date: 28/Mar/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Fee payed in order to obtain a service in a Recorder Office.                                  *
*                                                                                                            *
********************************* Copyright (c) 1999-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System.Data;

namespace Empiria.Land.Registration.Transactions {

  /// <summary>Fee payed in order to obtain a service in a Recorder Office.</summary>
  public class LRSFee {

    #region Fields

    private decimal recordingRights = decimal.Zero;
    private decimal sheetsRevision = decimal.Zero;
    private decimal aclaration = decimal.Zero;
    private decimal usufruct = decimal.Zero;
    private decimal easement = decimal.Zero;
    private decimal signCertification = decimal.Zero;
    private decimal foreignRecord = decimal.Zero;
    private decimal othersCharges = decimal.Zero;
    private decimal discount = decimal.Zero;

    #endregion Fields

    #region Constructors and parsers

    internal LRSFee() {
      //no-op
    }

    static internal LRSFee Parse(DataRow row) {
      LRSFee fee = new LRSFee();

      fee.recordingRights = (decimal) row["RecordingRightsFee"];
      fee.sheetsRevision = (decimal) row["SheetsRevisionFee"];
      fee.aclaration = (decimal) row["AclarationFee"];
      fee.usufruct = (decimal) row["UsufructFee"];
      fee.easement = (decimal) row["ServidumbreFee"];
      fee.signCertification = (decimal) row["SignCertificationFee"];
      fee.foreignRecord = (decimal) row["ForeignRecordFee"];
      fee.othersCharges = (decimal) row["OthersFee"];
      fee.discount = (decimal) row["Discount"];

      return fee;
    }

    static internal LRSFee Parse(ObjectList<LRSTransactionAct> list) {
      LRSFee fee = new LRSFee();

      foreach (LRSTransactionAct act in list) {
        fee.Add(act.Fee);
      }
      return fee;
    }

    #endregion Constructors and parsers

    #region Public properties

    public decimal RecordingRights {
      get { return recordingRights; }
      set { recordingRights = value; }
    }

    public decimal SheetsRevision {
      get { return sheetsRevision; }
      set { sheetsRevision = value; }
    }

    public decimal Aclaration {
      get { return aclaration; }
      set { aclaration = value; }
    }

    public decimal Usufruct {
      get { return usufruct; }
      set { usufruct = value; }
    }

    public decimal Easement {
      get { return easement; }
      set { easement = value; }
    }

    public decimal SignCertification {
      get { return signCertification; }
      set { signCertification = value; }
    }

    public decimal ForeignRecord {
      get { return foreignRecord; }
      set { foreignRecord = value; }
    }

    public decimal OthersCharges {
      get { return othersCharges; }
      set { othersCharges = value; }
    }

    public decimal SubTotal {
      get {
        return recordingRights + sheetsRevision + aclaration + usufruct + easement +
               signCertification + foreignRecord + othersCharges;
      }
    }

    public decimal Discount {
      get { return discount; }
      set { discount = value; }
    }

    public decimal Total {
      get { return (this.SubTotal - this.Discount); }
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
