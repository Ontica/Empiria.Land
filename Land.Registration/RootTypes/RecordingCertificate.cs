/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land                                   Assembly : Empiria.Land                        *
*  Type      : RecordingCertificate                           Pattern  : Empiria Object Type                 *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Certificate emission and information search acts.                                             *
*                                                                                                            *
********************************* Copyright (c) 2009-2017. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Certificate emission and information search acts.</summary>
  public class RecordingCertificate : TransactionAct {

    #region Constructors and parsers

    private RecordingCertificate(RecordingActType powertype) : base(powertype) {
      // Required by Empiria Framework for all partitioned types.
    }

    static public new RecordingCertificate Parse(int id) {
      return BaseObject.ParseId<RecordingCertificate>(id);
    }

    #endregion Constructors and parsers

  } // class RecordingCertificate

} // namespace Empiria.Land.Registration
