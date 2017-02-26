/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : IResourceTractItem                             Pattern  : Loose coupling interface            *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Interface that represents a resource movement: recording act or an emitted certificate.       *
*                                                                                                            *
********************************* Copyright (c) 2009-2017. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Interface that represents a resource movement: a recording act or an emitted certificate.</summary>
  public interface IResourceTractItem {

    /// <summary>
    /// Returns the prelation stamp for the recording act or the certificate, in order to use
    /// it to sort the resource movements according to their prelation.</summary>
    string TractPrelationStamp {
      get;
    }

  }  // interface IResourceTractItem

}  // namespace Empiria.Land.Registration
