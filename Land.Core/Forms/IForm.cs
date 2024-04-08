/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Services                       Component : Filing Forms                            *
*  Assembly : Empiria.Land.Registration.dll              Pattern   : Loose coupling interface                *
*  Type     : IForm                                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes a Land System data form.                                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.Registration.Forms {

  /// <summary>Describes a Land System data form.</summary>
  public interface IForm {

    LandSystemFormType FormType { get; }

  }

}  // namespace Empiria.Land.Registration.Forms
