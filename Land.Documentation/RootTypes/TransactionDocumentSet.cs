/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Recording Services                      Component : Transaction Document Management       *
*  Assembly : Empiria.Land.Documentation.dll               Pattern   : Aggregate root                        *
*  Type     : TransactionDocumentSet                       License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Manages references to transaction's recordable documents.                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.IO;

using Empiria.Documents;

using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Documentation {

  /// <summary>Manages references to transaction's recordable documents.</summary>
  public class TransactionDocumentSet {


    #region Constructors and parsers

    private TransactionDocumentSet(LRSTransaction transaction) {
      this.Transaction = transaction;
      this.LoadDocuments();
    }


    static public TransactionDocumentSet ParseFor(LRSTransaction transaction) {
      Assertion.AssertObject(transaction, "transaction");
      Assertion.AssertObject(transaction.IsEmptyInstance, "transaction can't be the empty instance.");

      return new TransactionDocumentSet(transaction);
    }


    internal void SetAuxiliaryDocument(Stream inputStream, FileContentType contentType, string fileName = "") {
      if (this.AuxiliaryDocument.IsEmptyInstance) {
        this.AuxiliaryDocument = TransactionDocument.CreateAuxiliary(this.Transaction, inputStream,
                                                                     contentType, fileName);
      } else {
        this.MainDocument.Update(inputStream, contentType, fileName);
      }
    }


    internal void SetMainDocument(Stream inputStream, FileContentType contentType, string fileName = "") {
      if (this.MainDocument.IsEmptyInstance) {
        this.MainDocument = TransactionDocument.CreateMain(this.Transaction, inputStream,
                                                           contentType, fileName);
      } else {
        this.MainDocument.Update(inputStream, contentType, fileName);
      }
    }


    #endregion Constructors and parsers

    #region Public properties


    public LRSTransaction Transaction {
      get;
    }


    public TransactionDocument MainDocument {
      get;
      private set;
    } = TransactionDocument.Empty;


    public TransactionDocument AuxiliaryDocument {
      get;
      private set;
    } = TransactionDocument.Empty;


    public bool HasMainDocument {
      get {
        return !this.MainDocument.IsEmptyInstance;
      }
    }


    public bool HasAuxiliaryDocument {
      get {
        return !this.AuxiliaryDocument.IsEmptyInstance;
      }
    }


    #endregion Public properties


    #region Public methods


    #endregion Public methods


    #region Private methods


    private void LoadDocuments() {
      var documents = DataServices.GetTransactionDocuments(this.Transaction);

      this.MainDocument = documents.Find(x => x.DocumentType == DocumentImageType.MainDocument);
      if (this.MainDocument == null) {
        this.MainDocument = TransactionDocument.Empty;
      }

      this.AuxiliaryDocument = documents.Find(x => x.DocumentType == DocumentImageType.Appendix);
      if (this.AuxiliaryDocument == null) {
        this.AuxiliaryDocument = TransactionDocument.Empty;
      }
    }


    #endregion Private methods

  } // class TransactionDocumentSet

} // namespace Empiria.Land.Documentation
