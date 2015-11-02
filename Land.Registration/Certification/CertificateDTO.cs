/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Certification Services              *
*  Namespace : Empiria.Land.Certification                     Assembly : Empiria.Land.Certification.dll      *
*  Type      : CertificateDTO                                 Pattern  : Data Transfer Object                *
*  Version   : 2.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Holds certificate data.                                                                       *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.Certification {

  /// <summary>Holds certificate data.</summary>
  public class CertificateDTO {

    public CertificateDTO() {
      this.CertificateTypeUID = String.Empty;
      this.TransactionUID = String.Empty;
      this.RecorderOfficeId = -1;
      this.PropertyUID = String.Empty;
      this.PropertyCommonName = String.Empty;
      this.PropertyLocation = String.Empty;
      this.PropertyMetesAndBounds = String.Empty;
      this.Operation = String.Empty;
      this.OperationDate = ExecutionServer.DateMaxValue;
      this.SeekForName = String.Empty;
      this.FromOwnerName = String.Empty;
      this.ToOwnerName = String.Empty;
      this.MarginalNotes = String.Empty;
      this.UseMarginalNotesAsFullBody = false;
      this.UserNotes = String.Empty;
    }

    #region Properties

    public string CertificateTypeUID {
      get;
      set;
    }

    public string TransactionUID {
      get;
      set;
    }

    public int RecorderOfficeId {
      get;
      set;
    }

    public string PropertyUID {
      get;
      set;
    }

    public string PropertyCommonName {
      get;
      set;
    }

    public string PropertyLocation {
      get;
      set;
    }

    public string PropertyMetesAndBounds {
      get;
      set;
    }

    public string Operation {
      get;
      set;
    }

    public DateTime OperationDate {
      get;
      set;
    }

    public string SeekForName {
      get;
      set;
    }

    public string FromOwnerName {
      get;
      set;
    }

    public string ToOwnerName {
      get;
      set;
    }

    public string MarginalNotes {
      get;
      set;
    }

    public bool UseMarginalNotesAsFullBody {
      get;
      set;
    }

    public string UserNotes {
      get;
      set;
    }

    #endregion Properties

  } // class CertificateDTO

} // namespace Empiria.Land.Certification
