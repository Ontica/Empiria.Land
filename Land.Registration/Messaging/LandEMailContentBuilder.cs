/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Messaging services                    Component : EMail notification services             *
*  Assembly : Empiria.Land.Registration.dll              Pattern   : Information structurer                  *
*  Type     : LandEMailContentBuilder                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Builds email content for a land transaction.                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Messaging;

using Empiria.Land.Registration.Transactions;
using Empiria.Land.Registration;

namespace Empiria.Land.Messaging {

  internal class LandEMailContentBuilder {

    #region Constructors and parsers

    internal LandEMailContentBuilder() {
      // no-op
    }

    #endregion Constructors and parsers


    #region Public methods

    internal EMailContent BuildForRegisterForResourceChanges(Resource resource) {
      var content = new EMailContent($"El predio con folio real {resource.UID} ha sido registrado para su monitoreo",
                                     "Este es el cuerpo del recurso monitoreado.");

      return content;
    }


    internal EMailContent BuildForResourceChanged(Resource resource) {
      var content = new EMailContent($"Se han registrado nuevos movimientos en el predio con folio real {resource.UID}",
                                     "Este es el cuerpo del recurso con nuevo movimiento.");

      return content;
    }


    internal EMailContent BuildForTransactionDelayed(LRSTransaction transaction) {
      var content = new EMailContent($"Su trámite {transaction.UID} tiene una nueva fecha de entrega",
                                     "Este es el cuerpo del mensaje de trámite con nueva fecha de entrega.");

      return content;
    }


    internal EMailContent BuildForTransactionFinished(LRSTransaction transaction) {
      var content = new EMailContent($"Su trámite de inscripción {transaction.UID} está listo",
                                     "Este es el cuerpo del mensaje.");

      return content;
    }


    internal EMailContent BuildForTransactionReceived(LRSTransaction transaction) {
      var content = new EMailContent($"Su trámite {transaction.UID} fue recibido para su procesamiento",
                                     "Este es el cuerpo del mensaje de recibido.");

      return content;
    }


    internal EMailContent BuildForTransactionReentered(LRSTransaction transaction) {
      var content = new EMailContent($"Su trámite {transaction.UID} fue reingresado para corrección",
                                     "Este es el cuerpo del mensaje reingresado.");

      return content;
    }


    internal EMailContent BuildForTransactionReturned(LRSTransaction transaction) {
      var content = new EMailContent($"Su trámite {transaction.UID} le ha sido devuelto",
                                     "Este es el cuerpo del mensaje de devolución.");

      return content;
    }


    #endregion Public methods


    #region Private methods


    #endregion Private methods


  }  // class LandEMailContentBuilder

}  // namespace Empiria.Land.Messaging
