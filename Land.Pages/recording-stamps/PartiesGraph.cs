/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Empiria Land Pages                         Component : Presentation Layer                      *
*  Assembly : Empiria.Land.Pages.dll                     Pattern   : Web Page                                *
*  Type     : RecordingStamp                             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Recording stamp for instruments with recording acts.                                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.Land.Registration;
using Empiria.Land.Registration.Adapters;

namespace Empiria.Land.WebApp {

  internal class PartiesGraph {

    private readonly FixedList<RecordingActParty> _parties;

    public PartiesGraph(RecordingAct recordingAct) {
      _parties = recordingAct.GetParties();
      Roots = GetRoots();
    }

    public IEnumerable<PartiesGraphNode> Roots {
      get;
      private set;
    }


    internal IEnumerable<PartiesGraphNode> GetChildren(PartiesGraphNode node) {
      var children = _parties.FindAll(x => x.PartyOf.Equals(node.RecordingActParty.Party));

      return children.Select(x => new PartiesGraphNode(node, x));
    }


    private IEnumerable<PartiesGraphNode> GetRoots() {
      var roots = _parties.FindAll(x => x.RoleType == RecordingActPartyType.Primary);

      return roots.Select(x => new PartiesGraphNode(x));
    }


  }  // class PartiesGraph


  internal class PartiesGraphNode {

    internal PartiesGraphNode(RecordingActParty party) {
      this.Parent = this;
      this.RecordingActParty = party;
    }

    internal PartiesGraphNode(PartiesGraphNode parent,
                              RecordingActParty party) {
      this.Parent = parent;
      this.RecordingActParty = party;
    }

    public RecordingActParty RecordingActParty {
      get;
    }

    public int Level {
      get {
        return IsRoot ? 1 : Parent.Level + 1;
      }
    }

    public bool IsRoot {
      get {
        return Parent == this;
      }
    }

    public PartiesGraphNode Parent {
      get;
    }

  }  // class PartiesGraphNode

}  // namespace Empiria.Land.WebApp
