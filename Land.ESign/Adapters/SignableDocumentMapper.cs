/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Electronic Sign                            Component : Adapters Layer                          *
*  Assembly : Empiria.Land.ESign.dll                     Pattern   : Mapper                                  *
*  Type     : SignableDocumentMapper                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Maps electronic signable documents to their data transfer objects.                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.ESign.Adapters {

  /// <summary>Maps electronic signable documents to their data transfer objects.</summary>
  static internal class SignableDocumentMapper {

    static internal FixedList<SignableDocumentDescriptor> MapToDescriptor(FixedList<SignableDocument> list) {
      return list.Select(x => MapToDescriptor(x)).ToFixedList();
    }


    static private SignableDocumentDescriptor MapToDescriptor(SignableDocument document) {
      var transaction = document.Transaction;
      var currentTask = transaction.Workflow.GetCurrentTask();

      return new SignableDocumentDescriptor {
        UID = document.Guid,
        DocumentID = document.UID,
        TransactionID = transaction.UID,
        Type = document.DocumentType,
        Subtype = document.DocumentSubtype,
        RequestedBy = transaction.RequestedBy,
        PresentationTime = transaction.PresentationTime,
        RegistrationTime = transaction.LandRecord.AuthorizationTime,
        InternalControlNo = transaction.InternalControlNumberFormatted,
        Status = currentTask.CurrentStatus.ToString(),
        StatusName = currentTask.CurrentStatusName,
        AssignedToUID = currentTask.Responsible.UID,
        AssignedToName = currentTask.Responsible.ShortName,
        NextStatus = currentTask.NextStatus.ToString(),
        NextStatusName = currentTask.NextStatusName,
        NextAssignedToName = currentTask.NextContact.ShortName
      };
    }

  }  // class SignableDocumentMapper

}  // namespace Empiria.Land.ESign.Adapters
