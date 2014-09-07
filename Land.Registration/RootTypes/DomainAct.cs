/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : DomainAct                                      Pattern  : Empiria Object Type                 *
*  Version   : 2.0        Date: 23/Oct/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Represents a domain traslative recording act.                                                 *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Represents a domain traslative recording act.</summary>
  public class DomainAct : RecordingAct {

    #region Constructors and parsers

    private DomainAct(RecordingActType powertype) : base(powertype) {
      // Required by Empiria Framework for all partitioned types.
    }

    static public new DomainAct Parse(int id) {
      return BaseObject.ParseId<DomainAct>(id);
    }

    #endregion Constructors and parsers

  } // class DomainAct

} // namespace Empiria.Land.Registration
