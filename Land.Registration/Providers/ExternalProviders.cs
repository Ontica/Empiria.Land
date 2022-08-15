/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Core                                  Component : Integration Layer                       *
*  Assembly : Empiria.Land.dll                           Pattern   : Providers Factory                       *
*  Type     : ExternalProviders                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Factory that provide object instances used to access external services.                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Reflection;

using Empiria.OnePoint.EFiling;

namespace Empiria.Land.Providers {

  /// <summary>Factory that provide object instances used to access external services.</summary>
  static public class ExternalProviders {

    static public IFilingServices EFilingProvider {
      get {
        Type type = ObjectFactory.GetType("Empiria.OnePoint.EFiling",
                                          "Empiria.OnePoint.EFiling.EFilingServices");

        return (IFilingServices) ObjectFactory.CreateObject(type);
      }
    }


    static public IUniqueIDGeneratorProvider GetUniqueIDGeneratorProvider() {
        Type type = ObjectFactory.GetType("Empiria.Land.Providers",
                                          "Empiria.Land.Providers.UniqueIDGeneratorProvider");

        return (IUniqueIDGeneratorProvider) ObjectFactory.CreateObject(type);
    }

  }  // class ExternalProviders

}  // namespace Empiria.Land.Providers
