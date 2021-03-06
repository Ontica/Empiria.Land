﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : TransactionAct                                 Pattern  : Empiria Object Type                 *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Transaction act that serves only for payment and control functions.                           *
*              Transaction are not recordable.                                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Data;

namespace Empiria.Land.Registration {

  /// <summary>Transaction acts serves only for payment and control functions.
  ///  Transaction acts are not recordable.</summary>
  public class TransactionAct : RecordingAct {

    #region Constructors and parsers

    protected TransactionAct(RecordingActType powertype) : base(powertype) {
      // Required by Empiria Framework for all partitioned types.
    }

    static public new TransactionAct Parse(int id) {
      return BaseObject.ParseId<TransactionAct>(id);
    }

    #endregion Constructors and parsers

  } // class TransactionAct

} // namespace Empiria.Land.Registration
