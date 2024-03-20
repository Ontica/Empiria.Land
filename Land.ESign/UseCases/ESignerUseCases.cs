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

  } // class ESignerUseCases

} // namespace Empiria.Land.ESign.UseCases
