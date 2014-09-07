/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : StructureAct                                   Pattern  : Empiria Object Type                 *
*  Version   : 2.0        Date: 23/Oct/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Recording act that represents the changes on measures and limits of real estates,             *
*              as well as the creation of new properties through fusions and divisons.                       *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;

namespace Empiria.Land.Registration {

  /// <summary>Recording act that represents the changes on measures and limits of real estates, 
  ///as well as the creation of new properties through fusions and divisons.</summary>
  public class StructureAct : RecordingAct {

    #region Constructors and parsers

    private StructureAct(RecordingActType powertype) : base(powertype) {
      // Required by Empiria Framework for all partitioned types.
    }

    static public new StructureAct Parse(int id) {
      return BaseObject.ParseId<StructureAct>(id);
    }

    #endregion Constructors and parsers

  } // class StructureAct

} // namespace Empiria.Land.Registration
