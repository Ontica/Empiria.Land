﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : ESign Services                             Component : Domain Layer                            *
*  Assembly : Empiria.Land.ESign.dll                     Pattern   : Service provider                        *
*  Type     : ESignEngine                                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Generates the data for the ESign.                                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.Land.ESign.Adapters;
using Empiria.Land.ESign.Data;

namespace Empiria.Land.ESign.Domain {

  /// <summary>Generates the data for the ESign.</summary>
  internal class ESignEngine {


    private readonly ESignQuery _query;

    public ESignEngine(ESignQuery query) {
      _query = query;
    }


    #region Public methods


    internal FixedList<SignRequestEntry> BuildRequest() {

      FixedList<SignRequestEntry> requestedData = ESignEngineData.GetSignRequests(_query);

      return requestedData;
    }


    #endregion Public methods


    #region Private methods



    #endregion Private methods

  } // class ESignEngine

} // namespace Empiria.Land.ESign.Domain
