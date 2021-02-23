/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Workflow Management                        Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Aggregator class                        *
*  Type     : WorkflowRules                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides specific rules for Empiria Land micro workflow engine.                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Workflow {

  /// <summary>Provides specific rules for Empiria Land micro workflow engine.</summary>
  static public class WorkflowRules {

    static public FixedList<LRSTransactionStatus> GetNextStatusList(LRSTransaction transaction) {
      var currentStatus = transaction.Workflow.GetCurrentTask().CurrentStatus;

      return GetNextStatusList(transaction.TransactionType,
                               transaction.DocumentType,
                               currentStatus).ToFixedList();
    }


    static public List<LRSTransactionStatus> GetNextStatusList(LRSTransactionType type,
                                                               LRSDocumentType docType,
                                                               LRSTransactionStatus currentStatus) {
      List<LRSTransactionStatus> list = new List<LRSTransactionStatus>();

      switch (currentStatus) {

        case LRSTransactionStatus.Payment:
          list.Add(LRSTransactionStatus.Received);
          list.Add(LRSTransactionStatus.Deleted);
          break;

        case LRSTransactionStatus.Received:
        case LRSTransactionStatus.Reentry:
          list.Add(LRSTransactionStatus.Control);
          break;

        case LRSTransactionStatus.Process:
        case LRSTransactionStatus.Control:
          // Certificado || Cancelación || Copia simple
          if (type.Id == 701 || type.Id == 704 || docType.Id == 723 || docType.Id == 734) {
            list.Add(LRSTransactionStatus.Elaboration);
          } else if (type.Id == 700 || type.Id == 702 || type.Id == 703) {
            list.Add(LRSTransactionStatus.Qualification);
            list.Add(LRSTransactionStatus.Recording);
            list.Add(LRSTransactionStatus.Elaboration);
          }

          list.Add(LRSTransactionStatus.Revision);
          list.Add(LRSTransactionStatus.OnSign);

          list.Add(LRSTransactionStatus.ToReturn);

          if (LRSWorkflowRules.IsCertificateIssueCase(type, docType)) {
            list.Add(LRSTransactionStatus.ToDeliver);
          }
          break;

        case LRSTransactionStatus.Juridic:
          break;

        case LRSTransactionStatus.Qualification:       // Only used in Zacatecas

          list.Add(LRSTransactionStatus.Recording);
          list.Add(LRSTransactionStatus.Revision);
          list.Add(LRSTransactionStatus.Qualification);
          list.Add(LRSTransactionStatus.ToReturn);
          list.Add(LRSTransactionStatus.Control);

          break;

        case LRSTransactionStatus.Recording:
          list.Add(LRSTransactionStatus.Revision);
          list.Add(LRSTransactionStatus.Recording);
          list.Add(LRSTransactionStatus.Control);

          list.Add(LRSTransactionStatus.ToReturn);
          if (LRSWorkflowRules.IsArchivable(type, docType)) {
            list.Add(LRSTransactionStatus.Archived);
          }
          if (docType.Id == 728) {
            list.Add(LRSTransactionStatus.OnSign);
          }

          break;

        case LRSTransactionStatus.Elaboration:
          if (docType.Id == 734) {
            list.Add(LRSTransactionStatus.ToDeliver);
          } else if (type.Id == 704) {
            list.Add(LRSTransactionStatus.OnSign);
          } else {
            list.Add(LRSTransactionStatus.Revision);
          }

          list.Add(LRSTransactionStatus.OnSign);

          list.Add(LRSTransactionStatus.Elaboration);
          list.Add(LRSTransactionStatus.Control);

          list.Add(LRSTransactionStatus.ToReturn);

          break;

        case LRSTransactionStatus.Revision:

          list.Add(LRSTransactionStatus.OnSign);
          if (type.Id == 701 || docType.Id == 723) {
            list.Add(LRSTransactionStatus.Elaboration);
          } else if (type.Id == 700 || type.Id == 702 || type.Id == 703) {
            list.Add(LRSTransactionStatus.Recording);
          } else if (type.Id == 704) {
            list.Add(LRSTransactionStatus.Elaboration);
          }
          if (LRSWorkflowRules.IsArchivable(type, docType)) {
            list.Add(LRSTransactionStatus.Archived);
          }
          list.Add(LRSTransactionStatus.Revision);
          list.Add(LRSTransactionStatus.Control);
          list.Add(LRSTransactionStatus.ToReturn);

          break;

        case LRSTransactionStatus.OnSign:
          list.Add(LRSTransactionStatus.ToDeliver);
          list.Add(LRSTransactionStatus.Revision);
          if (type.Id == 701 || docType.Id == 723) {
            list.Add(LRSTransactionStatus.Elaboration);
          } else if (type.Id == 700 || type.Id == 702 || type.Id == 703) {
            list.Add(LRSTransactionStatus.Recording);
          } else if (type.Id == 704) {
            list.Add(LRSTransactionStatus.Elaboration);
          }
          list.Add(LRSTransactionStatus.Control);
          list.Add(LRSTransactionStatus.ToReturn);

          break;

        case LRSTransactionStatus.Digitalization:
          list.Add(LRSTransactionStatus.Delivered);
          if (LRSWorkflowRules.IsDigitalizable(type, docType)) {
            list.Add(LRSTransactionStatus.Digitalization);
          }
          list.Add(LRSTransactionStatus.Control);

          break;

        case LRSTransactionStatus.ToDeliver:
          list.Add(LRSTransactionStatus.Control);
          AddRecordingOrElaborationStatus(list, type, docType);
          list.Add(LRSTransactionStatus.Juridic);
          list.Add(LRSTransactionStatus.OnSign);

          break;

        case LRSTransactionStatus.Archived:
          list.Add(LRSTransactionStatus.Control);
          break;

        case LRSTransactionStatus.ToReturn:
          list.Add(LRSTransactionStatus.Returned);
          list.Add(LRSTransactionStatus.Control);
          break;
      }
      return list;
    }


    static private void AddRecordingOrElaborationStatus(List<LRSTransactionStatus> list,
                                                        LRSTransactionType type,
                                                        LRSDocumentType docType) {
      if (LRSWorkflowRules.IsRecordingDocumentCase(type, docType)) {
        list.Add(LRSTransactionStatus.Recording);

      } else if (LRSWorkflowRules.IsCertificateIssueCase(type, docType)) {
        list.Add(LRSTransactionStatus.Elaboration);

      } else {
        list.Add(LRSTransactionStatus.Elaboration);
        list.Add(LRSTransactionStatus.Recording);
      }
    }

  }  // class WorkflowRules

}  // namespace Empiria.Land.Workflow

