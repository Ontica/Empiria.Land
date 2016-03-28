/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : ModificationAct                                Pattern  : Empiria Object Type                 *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Recording act that modifies another recording act, resource, document or party.               *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Recording act that modifies another recording act, resource, document or party.</summary>
  public class ModificationAct : RecordingAct {

    #region Constructors and parsers

    private ModificationAct(RecordingActType powertype) : base(powertype) {
      // Required by Empiria Framework for all partitioned types.
    }

    static public new ModificationAct Parse(int id) {
      return BaseObject.ParseId<ModificationAct>(id);
    }

    #endregion Constructors and parsers

  } // class ModificationAct

} // namespace Empiria.Land.Registration
