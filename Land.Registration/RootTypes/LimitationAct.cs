/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : LimitationAct                                  Pattern  : Empiria Object Type                 *
*  Version   : 2.0        Date: 23/Oct/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Represents a property limitation or property assessment or mortgage act.                      *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System.Data;

namespace Empiria.Land.Registration {

  /// <summary>Represents a property limitation or property assessment or mortgage act.</summary>
  public class LimitationAct : RecordingAct {

    #region Constructors and parsers

    private LimitationAct(RecordingActType powertype) : base(powertype) {
      // Required by Empiria Framework for all partitioned types.
    }

    static public new LimitationAct Parse(int id) {
      return BaseObject.ParseId<LimitationAct>(id);
    }

    #endregion Constructors and parsers

  } // class LimitationAct

} // namespace Empiria.Land.Registration
