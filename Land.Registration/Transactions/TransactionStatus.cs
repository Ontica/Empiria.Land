/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Transactions                      Assembly : Empiria.Land.Registration           *
*  Type      : TransactionStatus                              Pattern  : Enumeration Type                    *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Represents a transaction status.                                                              *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Collections.Generic;

using Empiria.Contacts;
using Empiria.DataTypes;
using Empiria.Land.Registration.Data;
using Empiria.Security;

namespace Empiria.Land.Registration.Transactions {

  public enum TransactionStatus {
    Payment = 'Y',
    Received = 'R',
    Reentry = 'N',
    Control = 'K',
    Qualification = 'F',
    Recording = 'G',
    Elaboration = 'E',
    Revision = 'V',

    Juridic = 'J',

    Process = 'P',
    OnSign = 'S',

    Safeguard = 'A',
    ToDeliver = 'D',
    Delivered = 'C',
    ToReturn = 'L',
    Returned = 'Q',
    Deleted = 'X',

    Finished = 'H',

    Undefined = 'U',
    EndPoint = 'Z',
  }

} // namespace Empiria.Land.Registration.Transactions
