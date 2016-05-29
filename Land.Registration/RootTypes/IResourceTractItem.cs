/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : IResourceTractItem                             Pattern  : Loose coupling interface            *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Interface that represent a resource applied recording act or an emitted certificate.          *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land {

  public interface IResourceTractItem {

    string TractPrelationStamp {
      get;
    }

  }  // interface IResourceTractItem

}  // namespace Empiria.Land
