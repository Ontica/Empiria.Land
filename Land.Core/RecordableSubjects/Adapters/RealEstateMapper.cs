/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Recordable Subjects                        Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Mapper class                            *
*  Type     : RealEstateMapper                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Mapping methods for real estate.                                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.Land.Registration;

namespace Empiria.Land.RecordableSubjects.Adapters {

  /// <summary>Mapping methods for real estate.</summary>
  static internal class RealEstateMapper {

    static internal FixedList<string> MapRealEstateKindsList(FixedList<RealEstateType> list) {
      return new FixedList<string>(list.Select(x => x.Name));
    }

  }  // internal class RealEstateMapper

}  // namespace Empiria.Land.RecordableSubjects.Adapters
