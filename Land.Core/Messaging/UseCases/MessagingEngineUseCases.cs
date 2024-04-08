/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Messaging Services                         Component : Use cases Layer                         *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Use case interactor class               *
*  Type     : MessagingEngineUseCases                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases to interact with the Empiria Land messaging engine.                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

namespace Empiria.Land.Messaging.UseCases {

  /// <summary>Use cases to interact with the Empiria Land messaging engine.</summary>
  public partial class MessagingEngineUseCases : UseCase {

    #region Constructors and parsers

    protected MessagingEngineUseCases() {
      // no-op
    }

    static public MessagingEngineUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<MessagingEngineUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public string EngineStatus() {
      if (LandMessenger.IsRunning) {
        return "Running";
      }
      return "Stopped";
    }


    public void StartEngine() {
      LandMessenger.Start();
    }


    public void StopEngine() {
      LandMessenger.Stop();
    }

    #endregion Use cases

  }  // class MessagingEngineUseCases

}  // namespace Empiria.Land.Messaging.UseCases
