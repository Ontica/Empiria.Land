/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Electronic Sign                            Component : Interface adapters                      *
*  Assembly : Empiria.Land.ESign.dll                     Pattern   : Command payload                         *
*  Type     : ESignCommand                               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Command payload used for sign documents.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.Security;

namespace Empiria.Land.ESign.Adapters {

  /// <summary>Describes a sign task or command type.</summary>
  public enum ESignCommandType {

    None = 'N',

    Sign = 'S',

    Revoke = 'K',

    Refuse = 'F',

    Unrefuse = 'U'

  }



  /// <summary>Command payload used for sign documents.</summary>
  public class ESignCommand {

    public ESignCommandType CommandType {
      get; set;
    } = ESignCommandType.None;


    public SignCredentialsDto Credentials {
      get; set;
    }


    public FixedList<string> DocumentUIDs {
      get; set;
    }


    public FixedList<string> TransactionUIDs {
      get; set;
    }

  }  // class ESignTaskFields



  /// <summary>Sign credentials</summary>
  public class SignCredentialsDto : ISecurityTokenData {

    public string AppKey {
      get; set;
    }

    public string UserID {
      get; set;
    }

    public string Password {
      get; set;
    }

    public string UserHostAddress {
      get; set;
    }

  }  // class SignCredentialsDto



  /// <summary>Extension methods for ESignCommand class.</summary>
  static internal class ESignCommandExtensions {

    #region Extension methods

    static internal void EnsureIsValid(this ESignCommand command, ESignCommandType commandType, bool forTransactions) {
      Assertion.Require(command.CommandType != ESignCommandType.None, $"Undefined command type value.");

      Assertion.Require(command.CommandType == commandType, $"Invalid command type. Expected '{commandType}'.");

      Assertion.Require(command.Credentials, nameof(command.Credentials));
      Assertion.Require(command.Credentials.UserID, "credentials.userID");
      Assertion.Require(command.Credentials.Password, "credentials.password");
      Assertion.Require(command.Credentials.AppKey, "credentials.AppKey");
      Assertion.Require(command.Credentials.UserHostAddress, "credentials.UserHostAddress");

      if (forTransactions) {
        Assertion.Require(command.TransactionUIDs, nameof(command.TransactionUIDs));
        Assertion.Require(command.TransactionUIDs.Count > 0, "transactionUIDs can't be an empty list.");
      } else {
        Assertion.Require(command.DocumentUIDs, nameof(command.DocumentUIDs));
        Assertion.Require(command.DocumentUIDs.Count > 0, "documentUIDs can't be an empty list.");
      }
    }

    #endregion Extension methods

  }  // class ESignCommandExtensions

}  // namespace Empiria.Land.ESign.Adapters
