/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : ESign Services                             Component : Domain Layer                            *
*  Assembly : Empiria.Land.ESign.dll                     Pattern   : Service provider                        *
*  Type     : ESignEngine                                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Generates the data for the ESign.                                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.ESign.Data;
using Empiria.OnePoint.ESign;

namespace Empiria.Land.ESign.Domain {

  /// <summary>Generates the data for the ESign.</summary>
  internal class ESignEngine {

    #region Public methods


    internal FixedList<SignedDocumentEntry> GetSignedDocuments(int recorderOfficeId) {

      FixedList<SignedDocumentEntry> requestedData = ESignEngineData.GetSignedDocuments(recorderOfficeId);

      return requestedData;
    }


    #endregion Public methods


    #region Private methods



    #endregion Private methods

  } // class ESignEngine

} // namespace Empiria.Land.ESign.Domain
