/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Registration                          Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Command payload                         *
*  Type     : RegistrationCommand                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Command payload used for recording acts registration.                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.Registration.Adapters {

  /// <summary>Command payload used for recording acts registration.</summary>
  public class RegistrationCommand {

    public RegistrationCommandType Type {
      get; set;
    } = RegistrationCommandType.Undefined;


    public RegistrationCommandPayload Payload {
      get; set;
    } = new RegistrationCommandPayload();


  }  // class RegistrationCommand



  public class RegistrationCommandPayload {


  }


}  // namespace Empiria.Land.Registration.Adapters
