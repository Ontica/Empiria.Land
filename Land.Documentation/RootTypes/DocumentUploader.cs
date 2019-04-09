/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Documentation                         Component : Document Uploader                       *
*  Assembly : Empiria.Land.Registration.dll              Pattern   : Service provider                        *
*  Type     : DocumentUploader                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Attaches documents to land transactions or recording documents.                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web;

using Empiria.Documents;

using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Documentation {

  /// <summary>Attaches documents to land transactions or recording documents.</summary>
  static public class DocumentUploader {

    #region Public methods


    static public TransactionDocument SetMainDocument(LRSTransaction transaction, HttpPostedFile uploadedFile) {
      Assertion.AssertObject(transaction, "transaction");
      Assertion.AssertObject(uploadedFile, "uploadedFile");
      Assertion.Assert(uploadedFile.ContentLength > 0, "uploadedFile is an empty file.");

      var documentSet = TransactionDocumentSet.ParseFor(transaction);

      documentSet.SetMainDocument(uploadedFile.InputStream, GetFileType(uploadedFile), uploadedFile.FileName);
      return documentSet.MainDocument;
    }


    static public TransactionDocument SetAuxiliaryDocument(LRSTransaction transaction, HttpPostedFile uploadedFile) {
      Assertion.AssertObject(transaction, "transaction");
      Assertion.AssertObject(uploadedFile, "uploadedFile");
      Assertion.Assert(uploadedFile.ContentLength > 0, "uploadedFile is an empty file.");

      var documentSet = TransactionDocumentSet.ParseFor(transaction);

      documentSet.SetAuxiliaryDocument(uploadedFile.InputStream, GetFileType(uploadedFile), uploadedFile.FileName);

      return documentSet.AuxiliaryDocument;
    }


    #endregion Public methods


    #region Private methods


    static private FileContentType GetFileType(HttpPostedFile uploadedFile) {
      switch (uploadedFile.ContentType) {
        case "application/pdf":
          return FileContentType.PDF;

        default:
          throw Assertion.AssertNoReachThisCode($"The system can't handle uploaded files " +
                                                $"with content type {uploadedFile.ContentType}.");
      }
    }

    #endregion Private methods


  }  // class DocumentUploader

}  // namespace Empiria.Land.Documentation
