/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Registration                          Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data holder                             *
*  Type     : ApplicableRecordingActType                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds a recording act type and its registration commands.                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

namespace Empiria.Land.Registration {

  /// <summary>Holds a recording act type and its registration commands.</summary>
  internal class ApplicableRecordingActType : IEquatable<ApplicableRecordingActType> {

    internal ApplicableRecordingActType(RecordingActType recordingActType,
                                        FixedList<RegistrationCommandType> commands,
                                        INamedEntity group) {
      Assertion.Require(recordingActType, nameof(recordingActType));
      Assertion.Require(commands, nameof(commands));
      Assertion.Require(group, nameof(group));

      this.RecordingActType = recordingActType;
      this.Commands = commands;
      this.Group = group;
    }


    #region Properties

    public RecordingActType RecordingActType {
      get;
    }

    public FixedList<RegistrationCommandType> Commands {
      get;
    }

    public INamedEntity Group {
      get;
    }

    #endregion Properties

    #region Methods

    public bool Equals(ApplicableRecordingActType applicableActType) {
      return applicableActType.GetHashCode() == this.GetHashCode();
    }


    public override bool Equals(object obj) {
      return obj is ApplicableRecordingActType applicableActType &&
             this.Equals(applicableActType);
    }


    public override int GetHashCode() {
      return this.RecordingActType.GetHashCode();
    }

    #endregion Methods

  }  // class ApplicableRecordingActTypes



  /// <summary>Holds a list of ApplicableRecordingActType instances.</summary>
  internal class ApplicableRecordingActTypeList {

    private readonly List<ApplicableRecordingActType> _list = new List<ApplicableRecordingActType>(128);

    internal void Add(ApplicableRecordingActType item) {
      _list.Add(item);
    }


    internal FixedList<INamedEntity> GetGroups() {
      return _list.Select(actType => actType.Group)
                  .Distinct()
                  .ToFixedList();
    }


    internal FixedList<ApplicableRecordingActType> GetGroupItems(INamedEntity group) {
      return _list.FindAll(actType => actType.Group == group)
                  .ToFixedList();
    }

    internal FixedList<ApplicableRecordingActType> GetList() {
      return _list.Distinct()
                  .ToFixedList();
    }

  }  // class ApplicableRecordingActTypeList

}  // namespace Empiria.Land.Registration
