﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : ESign Services                             Component : Use cases Layer                         *
*  Assembly : Empiria.Land.ESign.dll                     Pattern   : Use case interactor class               *
*  Type     : ESignEngineUseCases                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases that returns ESign data.                                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.OnePoint.ESign;

using Empiria.Land.ESign.Adapters;
using Empiria.Land.ESign.Domain;

namespace Empiria.Land.ESign.UseCases {

  /// <summary>Use cases that returns ESign data.</summary>
  public class ESignEngineUseCases : UseCase {

    #region Constructors and parsers

    protected ESignEngineUseCases() {
      // no-op
    }

    static public ESignEngineUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<ESignEngineUseCases>();
    }


    #endregion Constructors and parsers


    public FixedList<SignDocumentDto> GetSignedDocuments(int recorderOfficeId) {
      Assertion.Require(recorderOfficeId, nameof(recorderOfficeId));

      var build = new ESignEngine();
      FixedList<SignedDocumentEntry> requestedData = build.GetSignedDocuments(recorderOfficeId);

      return ESignEngineMapper.Mapper(requestedData);
    }


  } // class ESignEngineUseCases

} // namespace Empiria.Land.ESign.UseCases
