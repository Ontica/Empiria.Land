/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Electronic Sign                            Component : Adpaters Layer                          *
*  Assembly : Empiria.Land.ESign.dll                     Pattern   : Mapper                                  *
*  Type     : SignableDocumentMapper                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Maps electronic signable documents to their data transfer objects.                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.ESign.Adapters {

  /// <summary>Maps electronic signable documents to their data transfer objects.</summary>
  static internal class SignableDocumentMapper {

    static internal FixedList<SignableDocumentDescriptor> MapToDescriptor(FixedList<SignableDocument> list) {
      return new FixedList<SignableDocumentDescriptor>();
    }

  }  // class SignableDocumentMapper

}  // namespace Empiria.Land.ESign.Adapters
