﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Media Files Management                Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Service provider                        *
*  Type     : LandMediaReadServices                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides file read services for Empiria Land entities.                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Storage;

using Empiria.Land.Instruments;
using Empiria.Land.Transactions;

namespace Empiria.Land.Media {

  static public class LandMediaReadServices {

    static internal bool HasFileReferences(StorageFile file) {
      FixedList<LandMediaPosting> postings = LandMediaPostingsData.GetFilePostings(file);

      return postings.Count != 0;
    }

    static internal FixedList<LandMediaPosting> InstrumentFiles(Instrument instrument) {
      return LandMediaPostingsData.GetMediaPostings(instrument);
    }


    static public FixedList<LandMediaPosting> TransactionFiles(LRSTransaction transaction) {
      return LandMediaPostingsData.GetMediaPostings(transaction);
    }

  }  // class LandMediaReadServices

}  // namespace Empiria.Land.Media
