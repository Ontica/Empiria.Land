/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Certification Services              *
*  Namespace : Empiria.Land.Certification                     Assembly : Empiria.Land.Certification.dll      *
*  Type      : CertificateDTO                                 Pattern  : Data Transfer Object                *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Data transfer object that holds data to build land certificates.                              *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.Certification {

  /// <summary>Data transfer object that holds data to build land certificates.</summary>
  public class CertificateDTO {

    #region Public properties

    public string CertificateTypeUID {
      get;
      set;
    } = String.Empty;


    public string TransactionUID {
      get;
      set;
    } = String.Empty;


    public int RecorderOfficeId {
      get;
      set;
    } = -1;


    public string PropertyUID {
      get;
      set;
    } = String.Empty;


    public string PropertyCommonName {
      get;
      set;
    } = String.Empty;


    public string PropertyLocation {
      get;
      set;
    } = String.Empty;


    public string PropertyMetesAndBounds {
      get;
      set;
    } = String.Empty;


    public string Operation {
      get;
      set;
    } = String.Empty;


    public DateTime OperationDate {
      get;
      set;
    } = ExecutionServer.DateMaxValue;


    public string SeekForName {
      get;
      set;
    } = String.Empty;


    public int StartingYear {
      get;
      set;
    } = 0;


    public string FromOwnerName {
      get;
      set;
    } = String.Empty;


    public string ToOwnerName {
      get;
      set;
    } = String.Empty;


    public string MarginalNotes {
      get;
      set;
    } = String.Empty;


    public bool UseMarginalNotesAsFullBody {
      get;
      set;
    } = false;


    public string UserNotes {
      get;
      set;
    } = String.Empty;

    #endregion Public properties

  } // class CertificateDTO

} // namespace Empiria.Land.Certification
