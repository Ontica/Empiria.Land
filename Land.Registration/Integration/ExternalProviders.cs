/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Integration Services                       Component : Empiria Land Transaction Services       *
*  Assembly : Empiria.Land.Registration.dll              Pattern   : Service provider                        *
*  Type     : ExternalProviders                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Plugin factory methods that provide access to external services.                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Reflection;

using Empiria.OnePoint.EFiling;

namespace Empiria.Land.Integration {

  /// <summary>Plugin factory methods that provide access to external services.</summary>
  static internal class ExternalProviders {

    static internal IFilingServices GetEFilingProvider() {
      Type type = ObjectFactory.GetType("Empiria.OnePoint.EFiling",
                                        "Empiria.OnePoint.EFiling.EFilingServices");

      return (IFilingServices) ObjectFactory.CreateObject(type);
    }


  }  // class ExternalProviders

}  // namespace Empiria.Land.Integration
