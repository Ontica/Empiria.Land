/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Services                       Component : Filing Forms                            *
*  Assembly : Empiria.Land.Registration.dll              Pattern   : Loose coupling interface                *
*  Type     : IRealPropertyForm                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes a Land System data form that works over an specific real property.                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.Registration.Forms {

  /// <summary>Describes a Land System data form that works over an specific real property.</summary>
  public interface IRealPropertyForm {

    RealPropertyDescription RealPropertyDescription { get; }

  }

}  // namespace Empiria.Land.Registration.Forms
