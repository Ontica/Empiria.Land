/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : InformationAct                                 Pattern  : Empiria Object Type                 *
*  Version   : 2.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Represents an information recording act that are not limitations and can be applied to        *
*              properties, persons or neither.                                                               *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Represents an information recording act that are not limitations and can be applied to
  /// properties, persons or neither.</summary>
  public class InformationAct : RecordingAct {

    #region Constructors and parsers

    private InformationAct(RecordingActType powertype) : base(powertype) {
      // Required by Empiria Framework for all partitioned types.
    }

    static public new InformationAct Parse(int id) {
      return BaseObject.ParseId<InformationAct>(id);
    }

    #endregion Constructors and parsers

  } // class InformationAct

} // namespace Empiria.Land.Registration
