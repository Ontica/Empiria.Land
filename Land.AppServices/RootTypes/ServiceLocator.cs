using System;

using Empiria.Reflection;

using Empiria.OnePoint;

namespace Empiria.Land.AppServices {

  internal class ServiceLocator {

    internal static ITreasuryConnector GetTreasuryConnector() {
      Type type = ObjectFactory.GetType("Empiria.Land.Connectors", "Empiria.Land.Connectors.TreasuryConnector");

      return (ITreasuryConnector) ObjectFactory.CreateObject(type);
    }

  }  // class ServiceLocator

}  // namespace Empiria.Land.AppServices
