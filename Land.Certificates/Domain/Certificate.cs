/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Certificates Issuing                       Component : Domain Layer                            *
*  Assembly : Empiria.Land.Certificates.dll              Pattern   : Partitioned type                        *
*  Type     : Certificate                                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Partitioned type that represents a Land certificate.                                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Ontology;
using Empiria.Security;

using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Certificates {

  /// <summary>Partitioned type that represents a Land certificate.</summary>
  [PartitionedType(typeof(CertificateType))]
  internal partial class Certificate : BaseObject, IProtected {

    #region Constructors and parsers

    private Certificate(CertificateType powerType) : base(powerType) {
      // Required by Empiria Framework for all partitioned types.
    }

    static public Certificate Parse(int id) {
      return BaseObject.ParseId<Certificate>(id);
    }

    static internal Certificate Create(CertificateType certificateType,
                                       LRSTransaction transaction,
                                       Resource recordableSubject) {
      Assertion.Require(certificateType, nameof(certificateType));
      Assertion.Require(transaction, nameof(transaction));
      Assertion.Require(recordableSubject, nameof(recordableSubject));

      var certificate = new Certificate(certificateType);
      certificate.Transaction = transaction;
      certificate.RecordableSubject = recordableSubject;

      return certificate;
    }


    #endregion Constructors and parsers

    #region Properties

    public CertificateType CertificateType {
      get {
        return (CertificateType) base.GetEmpiriaType();
      }
    }

    public string CertificateID {
      get;
      private set;
    }


    public string Status {
      get;
      internal set;
    } = "Incomplete";


    public LRSTransaction Transaction {
      get;
      private set;
    }


    public Resource RecordableSubject {
      get;
      private set;
    }


    #endregion Properties

    #region IProtected implementation

    int IProtected.CurrentDataIntegrityVersion {
      get {
        return 1;
      }
    }


    private IntegrityValidator _validator = null;
    IntegrityValidator IProtected.Integrity {
      get {
        if (_validator == null) {
          _validator = new IntegrityValidator(this);
        }
        return _validator;
      }
    }


    object[] IProtected.GetDataIntegrityFieldValues(int version) {
      if (version == 1) {
        return new object[] {
          version, "Id", this.Id, "CertificateTypeId", this.CertificateType.Id,
          "UID", this.UID
        };
      }
      throw new SecurityException(SecurityException.Msg.WrongDIFVersionRequested, version);
    }


    public IntegrityValidator Integrity {
      get {
        if (_validator == null) {
          _validator = new IntegrityValidator(this);
        }
        return _validator;
      }
    }

    #endregion IProtected implementation

  } // class Certificate

} // namespace Empiria.Land.Certificates
