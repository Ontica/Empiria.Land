﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Transactions                      Assembly : Empiria.Land.Registration           *
*  Type      : CalculationRule                                Pattern  : Storage Item                        *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Describes a calculation rule for transaction items according to the law articles.             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Registration.Transactions {

  /// <summary>Describes a recorder office transaction type.</summary>
  public class CalculationRule : GeneralObject {

    #region Constructors and parsers

    private CalculationRule() {
      // Required by Empiria Framework.
    }

    static public CalculationRule Empty {
      get { return BaseObject.ParseEmpty<CalculationRule>(); }
    }

    static public CalculationRule Unknown {
      get { return BaseObject.ParseUnknown<CalculationRule>(); }
    }

    static public CalculationRule Parse(int id) {
      return BaseObject.ParseId<CalculationRule>(id);
    }

    static public FixedList<CalculationRule> GetList() {
      return GeneralObject.GetList<CalculationRule>();
    }

    #endregion Constructors and parsers

  } // class CalculationRule

} // namespace Empiria.Land.Registration.Transactions
