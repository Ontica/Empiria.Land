/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Certification Services              *
*  Namespace : Empiria.Land.Certification                     Assembly : Empiria.Land.Certification.dll      *
*  Type      : FormerCertificateDTO                           Pattern  : Data Transfer Object                *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Former Data transfer object that holds data used to build certificates from external sources. *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Certification {

  /// <summary>Former Data transfer object that holds data used to build
  /// certificates from external sources.</summary>
  public class FormerCertificateDTO {

    #region Public properties

    /// <summary>The unique ID of the certificate type.</summary>
    public string CertificateTypeUID {
      get;
      set;
    } = String.Empty;


    /// <summary>The transaction unique ID related to the certificate.</summary>
    public string TransactionUID {
      get;
      set;
    } = String.Empty;


    /// <summary>The recorder office Id that issues the certificate.</summary>
    public int RecorderOfficeId {
      get;
      set;
    } = -1;


    /// <summary>The property unique ID. Used only for certificates related
    /// to real estates or associations.</summary>
    public string PropertyUID {
      get;
      set;
    } = String.Empty;


    /// <summary>The common name of the real estate.</summary>
    public string PropertyCommonName {
      get;
      set;
    } = String.Empty;


    /// <summary>The location of the real estate.</summary>
    public string PropertyLocation {
      get;
      set;
    } = String.Empty;


    /// <summary>The metes and bounds description of the real estate.</summary>
    public string PropertyMetesAndBounds {
      get;
      set;
    } = String.Empty;


    /// <summary>The operation applied to the real estate.
    /// Used when there are not enough real estate historic information.</summary>
    public string Operation {
      get;
      set;
    } = String.Empty;

    /// <summary>The date of the operation applied to the real estate.
    /// Used when there are not enough real estate historic information.</summary>
    public DateTime OperationDate {
      get;
      set;
    } = ExecutionServer.DateMaxValue;


    /// <summary>The name of the person or persons seeked to issue the certificate.</summary>
    public string SeekForName {
      get;
      set;
    } = String.Empty;


    /// <summary>The year when the issuer seeked information about.</summary>
    public int StartingYear {
      get;
      set;
    } = 0;


    /// <summary>The name of the former owner of the real estate.
    /// Used when there are not enough real estate historic information.</summary>
    public string FromOwnerName {
      get;
      set;
    } = String.Empty;

    /// <summary>The name of the current owner of the real estate.
    /// Used when there are not enough real estate historic information.</summary>
    public string ToOwnerName {
      get;
      set;
    } = String.Empty;


    /// <summary>The marginal notes that appear in recording books.
    /// Used when there are not enough real estate historic information.</summary>
    public string MarginalNotes {
      get;
      set;
    } = String.Empty;


    /// <summary>When true, the full text of the marginal notes are used as the body
    /// of the certificate. Used for text-free certificates.</summary>
    public bool UseMarginalNotesAsFullBody {
      get;
      set;
    } = false;


    /// <summary>Issuer notes about the certificate.</summary>
    public string UserNotes {
      get;
      set;
    } = String.Empty;

    #endregion Public properties

  } // class CertificateDTO

} // namespace Empiria.Land.Certification
