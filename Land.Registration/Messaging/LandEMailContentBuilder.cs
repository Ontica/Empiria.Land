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
using Empiria.Messaging.EMailDelivery;

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

    internal EmailContent BuildForRegisteredForResourceChanges(Resource resource) {
      var body = GetTemplate(NotificationType.SubscribedForResourceChanges);

      return new EmailContent($"El predio con folio electrónico {resource.UID} " +
                              $"ha sido registrado para su monitoreo", body);
    }


    internal EmailContent BuildForResourceChanged(Resource resource) {
      var body = GetTemplate(NotificationType.ResourceWasChanged);

      return new EmailContent($"Se han registrado nuevos movimientos " +
                              $"en el predio con folio electrónico {resource.UID}", body);
    }


    internal EmailContent BuildForTransactionDelayed(LRSTransaction transaction) {
      var body = GetTemplate(NotificationType.TransactionDelayed);

      return new EmailContent($"Su trámite {transaction.UID} tiene una nueva fecha de entrega", body);
    }


    internal EmailContent BuildForTransactionReadyToDelivery(FormerMessage message) {
      var transaction = GetTransaction(message);

      var body = GetTemplate(NotificationType.TransactionReadyToDelivery);

      body = SetTransactionFields(body, transaction);
      body = SetMessageFields(body, message);

      return new EmailContent($"Su trámite {transaction.UID} está listo", body);
    }


    internal EmailContent BuildForTransactionReceived(FormerMessage message) {
      var transaction = GetTransaction(message);

      var body = GetTemplate(NotificationType.TransactionReceived);

      body = SetTransactionFields(body, transaction);
      body = SetMessageFields(body, message);

      return new EmailContent($"Su trámite {transaction.UID} fue ingresado", body);
    }


    internal EmailContent BuildForTransactionReentered(FormerMessage message) {
      var transaction = GetTransaction(message);

      var body = GetTemplate(NotificationType.TransactionReentered);

      body = SetTransactionFields(body, transaction);
      body = SetMessageFields(body, message);

      return new EmailContent($"Su trámite {transaction.UID} fue reingresado. " +
                              $"Sus documentos impresos están sujetos a cambios.", body);
    }


    internal EmailContent BuildForTransactionReturned(FormerMessage message) {
      var transaction = GetTransaction(message);

      var body = GetTemplate(NotificationType.TransactionReturned);

      body = SetTransactionFields(body, transaction);
      body = SetMessageFields(body, message);

      return new EmailContent($"Su trámite {transaction.UID} ha sido devuelto", body);
    }


    #endregion Public methods


    #region Private methods

    static private string GetTemplate(NotificationType notificationType) {
      string templatesPath = ConfigurationData.GetString("Templates.Path");

      string fileName = System.IO.Path.Combine(templatesPath, $"template.email.{notificationType}.txt");

      return System.IO.File.ReadAllText(fileName);
    }


    static private LRSTransaction GetTransaction(FormerMessage message) {
      var transaction = LRSTransaction.TryParse(message.UnitOfWorkUID);

      Assertion.Require(transaction,
                       $"Unrecognized transaction with UID {message.UnitOfWorkUID}.");

      return transaction;
    }


    static private string SetMessageFields(string body, FormerMessage message) {
      body = body.Replace("{{MESSAGE-UID}}", message.UID);

      return body;
    }


    static private string SetTransactionFields(string body, LRSTransaction transaction) {
      body = body.Replace("{{TRANSACTION-UID}}", transaction.UID);
      body = body.Replace("{{TRANSACTION-HASH}}", transaction.QRCodeSecurityHash());
      body = body.Replace("{{PRESENTATION-TIME}}", transaction.PresentationTime.ToString("dd/MMM/yyyy") +
                                                   " a las " + transaction.PresentationTime.ToShortTimeString());
      body = body.Replace("{{REQUESTED_BY}}", transaction.RequestedBy);

      return body;
    }

    #endregion Private methods

  }  // class LandEMailContentBuilder

}  // namespace Empiria.Land.Messaging
