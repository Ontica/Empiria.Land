/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : CancelationAct                                 Pattern  : Empiria Object Type                 *
*  Version   : 2.0        Date: 23/Oct/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Recording act that serves for cancel other recording act.                                     *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Recording act that serves for cancel other recording act.</summary>
  public class CancelationAct : RecordingAct {

    #region Constructors and parsers

    private CancelationAct(RecordingActType powertype) : base(powertype) {
      // Required by Empiria Framework for all partitioned types.
    }

    static public new CancelationAct Parse(int id) {
      return BaseObject.ParseId<CancelationAct>(id);
    }

    #endregion Constructors and parsers

  } // class CancelationAct

} // namespace Empiria.Land.Registration
