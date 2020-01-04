/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Documentation                     Assembly : Empiria.Land.Documentation          *
*  Type      : DocumentImageType                              Pattern  : Enumeration type                    *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Enumeration that describes the type of an imaging document.                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Documentation {

  /// <summary>Enumeration that describes the type of an imaging document.</summary>
  public enum DocumentImageType {
    Unknown = 'U',
    MainDocument = 'E',
    Appendix = 'A',
    Folder = 'F'
  }

 }  // namespace Empiria.Land.Documentation
