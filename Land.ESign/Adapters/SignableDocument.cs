/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Electronic Sign                            Component : Adapters Layer                          *
*  Assembly : Empiria.Land.ESign.dll                     Pattern   : Information holder                      *
*  Type     : SignableDocument                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents a signable document.                                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;

using Empiria.Contacts;

using Empiria.Land.Certificates;
using Empiria.Land.Registration;
using Empiria.Land.Transactions;

namespace Empiria.Land.ESign.Adapters {

  /// <summary>Represents a signable document.</summary>
  internal class SignableDocument {


    [DataField("DocumentId")]
    public int Id {
      get; private set;
    }


    [DataField("DocumentGuid")]
    public string Guid {
      get; private set;
    }


    [DataField("DocumentUID")]
    public string UID {
      get; private set;
    }


    [DataField("DocumentType")]
    public string DocumentType {
      get; private set;
    }


    public string DocumentSubtype {
      get {
        if (DocumentType == "Certificado") {
          return Certificate.Parse(Guid).CertificateType.DisplayName;
        }
        return LandRecord.ParseGuid(Guid).Instrument.InstrumentType.DisplayName;
      }
    }


    [DataField("TransactionId")]
    public LRSTransaction Transaction {
      get; private set;
    }


    [DataField("SignStatus", Default = SignStatus.Undefined)]
    public SignStatus SignStatus {
      get; private set;
    }


    [DataField("SignedById")]
    public Contact SignedBy {
      get; private set;
    }


    [DataField("SignedTime")]
    public DateTime SignedTime {
      get; private set;
    }

  }  // class SignableDocument



  /// <summary>Data transfer object used to return signable documents for use in lists.</summary>
  public class SignableDocumentDescriptor {

    public string UID {
      get; internal set;
    }

    public string Type {
      get; internal set;
    }

    public string Subtype {
      get; internal set;
    }

    public string DocumentID {
      get; internal set;
    }

    public string TransactionID {
      get; internal set;
    }

    public string RequestedBy {
      get; internal set;
    }

    public DateTime PresentationTime {
      get; internal set;
    }

    public DateTime RegistrationTime {
      get; internal set;
    }

    public string InternalControlNo {
      get; internal set;
    }

    public string Status {
      get; internal set;
    }

    public string StatusName {
      get; internal set;
    }

    public string NextStatus {
      get; internal set;
    }

    public string NextStatusName {
      get; internal set;
    }

    public string AssignedToUID {
      get; internal set;
    }

    public string AssignedToName {
      get; internal set;
    }

    public string NextAssignedToName {
      get; internal set;
    }

  }  // class SignableDocumentDescriptor

}  // namespace Empiria.Land.ESign.Adapters
