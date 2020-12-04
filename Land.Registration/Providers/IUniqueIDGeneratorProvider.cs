/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Core                                  Component : Integration Layer                       *
*  Assembly : Empiria.Land.dll                           Pattern   : Dependency Inversion Interface          *
*  Type     : IUniqueIDGeneratorProvider                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides unique ID generation services for transactions, documents, parties, and certificates. *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.Providers {

  /// <summary>Provides unique ID generation services for transactions, documents,
  /// parties, and certificates.</summary>
  public interface IUniqueIDGeneratorProvider {

    string GenerateAssociationUID();

    string GenerateCertificateUID();

    string GenerateDocumentUID();

    string GenerateNoPropertyResourceUID();

    string GeneratePropertyUID();

    string GenerateTransactionUID();

  }

}
