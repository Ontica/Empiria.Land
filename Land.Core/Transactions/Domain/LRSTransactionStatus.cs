/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Workflow                       Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Enumeration                             *
*  Type     : TransactionStatus                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Enumerates the possible statuses of a transaction with respect of the office workflow.         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Transactions {

  /// <summary>Enumerates the possible statuses of a transaction with respect of the office workflow.</summary>
  public enum TransactionStatus {

    Undefined = 'U',

    Payment = 'Y',

    Received = 'R',

    Reentry = 'N',

    Control = 'K',

    Qualification = 'F',

    Recording = 'G',

    Elaboration = 'E',

    Revision = 'V',

    Juridic = 'J',

    OnSign = 'S',

    Digitalization = 'A',

    ToDeliver = 'D',

    Delivered = 'C',

    ToReturn = 'L',

    Returned = 'Q',

    Deleted = 'X',

    Archived = 'H',

    EndPoint = 'Z',

    All = '@',

  }  // enum TransactionStatus



  static public class TransactionStatusExtensionMethods {

    static public string GetStatusName(this TransactionStatus status) {
      switch (status) {
        case TransactionStatus.Payment:
          return "Precalificación";
        case TransactionStatus.Received:
          return "Trámite recibido";
        case TransactionStatus.Reentry:
          return "Trámite reingresado";
        case TransactionStatus.Control:
          return "En mesa de control";
        case TransactionStatus.Qualification:
          return "En calificación";
        case TransactionStatus.Recording:
          return "En registro en libros";
        case TransactionStatus.Elaboration:
          return "En elaboración";
        case TransactionStatus.Revision:
          return "En revisión";
        case TransactionStatus.Juridic:
          return "En área jurídica";
        case TransactionStatus.OnSign:
          return "En firma";
        case TransactionStatus.Digitalization:
          return "En digitalización y resguardo";
        case TransactionStatus.ToDeliver:
          return "En ventanilla de entregas";
        case TransactionStatus.Delivered:
          return "Entregado al interesado";
        case TransactionStatus.ToReturn:
          return "En ventanilla de devoluciones";
        case TransactionStatus.Returned:
          return "Devuelto al interesado";
        case TransactionStatus.Deleted:
          return "Trámite eliminado";
        case TransactionStatus.Archived:
          return "Archivado/Concluido";
        default:
          return "No determinado";
      }
    }

  }  // class TransactionStatusExtensionMethods

} // namespace Empiria.Land.Transactions
