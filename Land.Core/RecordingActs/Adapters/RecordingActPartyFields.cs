/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Registration Services                      Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Input Data Holder                       *
*  Type     : PartyFields                                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data structure used to update and create parties in the conext of a recording act.             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Registration.Adapters {

  /// <summary>Enumerates a recording act party type.</summary>
  public enum RecordingActPartyType {

    Undefined,

    /// <summary>A party playing a primary role.</summary>
    Primary,

    /// <summary>A party playing a secondary role and associated with another party.</summary>
    Secondary

  }  // enum RecordingActPartyType



  /// <summary>Enumerates a party type.</summary>
  public enum PartyType {

    Undefined,

    Person,

    Organization

  }  // enum PartyType



  /// <summary>Data structure used to update and create parties in the context of a recording act.</summary>
  public class RecordingActPartyFields {

    public string UID {
      get; set;
    } = string.Empty;


    public RecordingActPartyType Type {
      get; set;
    } = RecordingActPartyType.Undefined;


    public PartyFields Party {
      get; set;
    }

    public string RoleUID {
      get; set;
    } = string.Empty;


    public string PartAmount {
      get; set;
    } = "1";


    public string PartUnitUID {
      get; set;
    } = string.Empty;


    public string AssociatedWithUID {
      get; set;
    } = string.Empty;


    public string Notes {
      get; set;
    } = string.Empty;


    internal void EnsureValid() {

    }

    internal decimal ToDecimalPartAmount() {
      if (this.PartUnitUID == "Unit.Fraction") {
        var fractionParts = this.PartAmount.Split('/');

        return decimal.Parse(fractionParts[0] + "." + fractionParts[1]);

      }

      return this.PartAmount.Length != 0 ? decimal.Parse(this.PartAmount) : 1m;
    }

  }  // class RecordingActPartyFields



  /// <summary>Fields to create or update a person or organization.</summary>
  public class PartyFields {

    public string UID {
      get; set;
    } = string.Empty;


    public PartyType Type {
      get; set;
    } = PartyType.Undefined;


    public string FullName {
      get; set;
    } = string.Empty;


    public string CURP {
      get; set;
    } = string.Empty;


    public string RFC {
      get; set;
    } = string.Empty;


    public string Notes {
      get; set;
    } = string.Empty;

  }  // class PartyFields



  /// <summary>Command used to search parties.</summary>
  public class SearchPartiesCommand {

    public string Keywords {
      get; set;
    } = string.Empty;

  }  // class SearchPartiesCommand


}  // namespace Empiria.Land.Registration.Adapters
