/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Recordable Subjects                        Component : Use cases Layer                         *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Use case interactor class               *
*  Type     : RecordableSubjectsUseCases                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for get real estate data and their related information.                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.Land.Registration;

using Empiria.Land.RecordableSubjects.Adapters;

namespace Empiria.Land.RecordableSubjects.UseCases {

  /// <summary>Use cases for get real estate data and their related information.</summary>
  public partial class RecordableSubjectsUseCases : UseCase {

    #region Constructors and parsers

    protected RecordableSubjectsUseCases() {
      // no-op
    }

    static public RecordableSubjectsUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<RecordableSubjectsUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases


    public FixedList<string> RealEstateKinds() {
      var realEstateKinds = RealEstateType.GetList();

      return RealEstateMapper.MapRealEstateKindsList(realEstateKinds);
    }


    public FixedList<RecorderOfficeDto> RecorderOffices() {
      var recorderOffices = RecorderOffice.GetList();

      return RecorderOfficeMapper.Map(recorderOffices);
    }

    #endregion Use cases

  }  // class RecordableSubjectsUseCases

}  // namespace Empiria.Land.RecordableSubjects.UseCases
