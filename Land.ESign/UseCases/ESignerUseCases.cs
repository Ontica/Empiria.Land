/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Electronic Sign                            Component : Use cases Layer                         *
*  Assembly : Empiria.Land.ESign.dll                     Pattern   : Use case interactor class               *
*  Type     : ESignerUseCases                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases that performs electronic sign of documents.                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.Land.ESign.Adapters;

namespace Empiria.Land.ESign.UseCases {

  /// <summary>Use cases that performs electronic sign of documents.</summary>
  public class ESignerUseCases : UseCase {

    #region Constructors and parsers

    protected ESignerUseCases() {
      // no-op
    }

    static public ESignerUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<ESignerUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public void RefuseMyTransactionDocuments(ESignCommand command) {
      Assertion.Require(command, nameof(command));

      command.EnsureIsValid(ESignCommandType.Refuse, true);

      throw new NotImplementedException();
    }


    public void RevokeMyTransactionDocuments(ESignCommand command) {
      Assertion.Require(command, nameof(command));

      command.EnsureIsValid(ESignCommandType.Revoke, true);

      throw new NotImplementedException();
    }


    public void SignMyTransactionDocuments(ESignCommand command) {
      Assertion.Require(command, nameof(command));

      command.EnsureIsValid(ESignCommandType.Sign, true);

      throw new NotImplementedException();
    }


    public void UnrefuseMyTransactionDocuments(ESignCommand command) {
      Assertion.Require(command, nameof(command));

      command.EnsureIsValid(ESignCommandType.Unrefuse, true);

      throw new NotImplementedException();
    }

    #endregion Use cases

  } // class ESignerUseCases

} // namespace Empiria.Land.ESign.UseCases
