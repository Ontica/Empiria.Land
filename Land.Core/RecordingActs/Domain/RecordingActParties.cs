/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Registration                          Component : Domain Layer                            *
*  Assembly : Empiria.Land.Registration.dll              Pattern   : Structurer                              *
*  Type     : RecordingActParties                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds recording act's parties.                                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Measurement;

using Empiria.Land.Data;
using Empiria.Land.Registration.Adapters;

namespace Empiria.Land.Registration {

  /// <summary>Holds recording act's parties.</summary>
  public class RecordingActParties {

    #region Fields

    private RecordingAct _recordingAct;

    #endregion Fields

    #region Constructors and parsers

    internal RecordingActParties(RecordingAct recordngAct) {
      _recordingAct = recordngAct;
    }


    #endregion Constructors and parsers

    #region Public properties

    public FixedList<RecordingActParty> List {
      get {
        return PartyData.GetRecordingPartyList(_recordingAct);
      }
    }


    public FixedList<RecordingActParty> PrimaryParties {
      get {
        return List.FindAll(x => x.RoleType == RecordingActPartyType.Primary);
      }
    }


    public FixedList<RecordingActParty> SecondaryParties {
      get {
        return List.FindAll(x => x.RoleType == RecordingActPartyType.Secondary);
      }
    }

    #endregion Public properties

    #region Public methods

    public RecordingActParty AppendParty(RecordingActPartyFields recordingActPartyFields) {
      Assertion.Require(recordingActPartyFields, nameof(recordingActPartyFields));
      Assertion.Require(_recordingAct.IsEditable,
                       "This recording act is not editable, so I cannot append a party to it.");

      recordingActPartyFields.EnsureValid();

      if (recordingActPartyFields.Type == RecordingActPartyType.Primary &&
          recordingActPartyFields.Party.UID.Length != 0) {
        return AppendExistingPrimaryParty(recordingActPartyFields);

      } else if (recordingActPartyFields.Type == RecordingActPartyType.Primary &&
                 recordingActPartyFields.Party.UID.Length == 0) {
        return AppendNewPrimaryParty(recordingActPartyFields);

      } else if (recordingActPartyFields.Type == RecordingActPartyType.Secondary &&
                 recordingActPartyFields.Party.UID.Length != 0 &&
                 recordingActPartyFields.AssociatedWithUID.Length != 0) {
        return AppendExistingSecondaryParty(recordingActPartyFields);

      } else if (recordingActPartyFields.Type == RecordingActPartyType.Secondary &&
                 recordingActPartyFields.Party.UID.Length == 0 &&
                 recordingActPartyFields.AssociatedWithUID.Length != 0) {
        return AppendNewSecondaryParty(recordingActPartyFields);

      } else {
        throw Assertion.EnsureNoReachThisCode();
      }
    }


    public void RemoveParty(RecordingActParty recordingActParty) {
      Assertion.Require(recordingActParty, nameof(recordingActParty));

      Assertion.Require(List.Exists(x => x.UID == recordingActParty.UID),
                        $"Party {recordingActParty.UID} do not belong to this recording act.");

      if (recordingActParty.RoleType == RecordingActPartyType.Primary) {
        var secondaryParties = GetSecondaryPartiesOf(recordingActParty.Party);
        foreach (var secondaryRecordingActParty in secondaryParties) {
          secondaryRecordingActParty.Delete();
        }
      }
      recordingActParty.Delete();
    }


    public FixedList<RecordingActParty> GetSecondaryPartiesOf(Party primaryParty) {
      return List.FindAll(x => x.RoleType == RecordingActPartyType.Secondary &&
                               x.PartyOf.Equals(primaryParty))
                  .Sort((x, y) => x.Id.CompareTo(y.Id))
                  .ToFixedList();
    }


    #endregion Public methods

    #region Private methods

    private RecordingActParty AppendPrimaryParty(Party party, DomainActPartyRole role) {

      if (SecondaryParties.Contains(x => x.Party.Equals(party))) {
        Assertion.RequireFail($"No es posible agregar a {party.FullName} en un rol primario " +
                              $"ya que juega un rol secundario en el mismo acto jurídico.");
      }

      return RecordingActParty.Create(_recordingAct, party, role);
    }


    private RecordingActParty AppendSecondaryParty(Party party, SecondaryPartyRole role, Party partyOf) {

      if (PrimaryParties.Contains(x => x.Party.Equals(party))) {
        Assertion.RequireFail($"No es posible agregar a {party.FullName} en un rol secundario " +
                              $"ya que juega un rol primario en el mismo acto jurídico.");
      }

      return RecordingActParty.Create(_recordingAct, party, role, partyOf);
    }


    private RecordingActParty AppendExistingPrimaryParty(RecordingActPartyFields recordingActPartyFields) {
      var party = Party.Parse(recordingActPartyFields.Party.UID);

      var role = DomainActPartyRole.Parse(recordingActPartyFields.RoleUID);

      var recordingActParty = this.AppendPrimaryParty(party, role);

      LoadRecordingActPartyFields(recordingActParty, recordingActPartyFields);

      recordingActParty.Save();

      return recordingActParty;
    }


    private RecordingActParty AppendExistingSecondaryParty(RecordingActPartyFields recordingActPartyFields) {
      var party = Party.Parse(recordingActPartyFields.Party.UID);
      var partyOf = Party.Parse(recordingActPartyFields.AssociatedWithUID);

      Assertion.Require(!party.Equals(partyOf), "Primary and secondary parties must be distinct persons.");

      var role = SecondaryPartyRole.Parse(recordingActPartyFields.RoleUID);

      var recordingActParty = this.AppendSecondaryParty(party, role, partyOf);

      LoadRecordingActPartyFields(recordingActParty, recordingActPartyFields);

      recordingActParty.Save();

      return recordingActParty;
    }


    private RecordingActParty AppendNewPrimaryParty(RecordingActPartyFields recordingActPartyFields) {
      Party party = CreateParty(recordingActPartyFields);

      var role = DomainActPartyRole.Parse(recordingActPartyFields.RoleUID);

      var recordingActParty = this.AppendPrimaryParty(party, role);

      LoadRecordingActPartyFields(recordingActParty, recordingActPartyFields);

      recordingActParty.Save();

      return recordingActParty;
    }


    private RecordingActParty AppendNewSecondaryParty(RecordingActPartyFields recordingActPartyFields) {
      Party party = CreateParty(recordingActPartyFields);

      var role = SecondaryPartyRole.Parse(recordingActPartyFields.RoleUID);

      var partyOf = Party.Parse(recordingActPartyFields.AssociatedWithUID);

      var recordingActParty = this.AppendSecondaryParty(party, role, partyOf);

      LoadRecordingActPartyFields(recordingActParty, recordingActPartyFields);

      recordingActParty.Save();

      return recordingActParty;
    }

    private Party CreateParty(RecordingActPartyFields recordingActPartyFields) {
      Party party;

      if (recordingActPartyFields.Party.Type == PartyType.Organization) {
        party = new OrganizationParty(recordingActPartyFields.Party);
      } else {
        party = new HumanParty(recordingActPartyFields.Party);
      }
      party.Save();

      return party;
    }


    private void LoadRecordingActPartyFields(RecordingActParty recordingActParty,
                                             RecordingActPartyFields recordingActPartyFields) {
      if (recordingActParty.RoleType == RecordingActPartyType.Primary) {
        recordingActParty.OwnershipPart = Quantity.Parse(Unit.Parse(recordingActPartyFields.PartUnitUID),
                                                         recordingActPartyFields.ToDecimalPartAmount());
      }
      recordingActParty.Notes = recordingActPartyFields.Notes;
    }

    #endregion Private methods

  } // class RecordingActParties

} // namespace Empiria.Land.Registration
