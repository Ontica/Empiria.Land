/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transactions Preprocessing                 Component : Interface adapters                      *
*  Assembly : Empiria.Land.Transactions.dll              Pattern   : Mapper class                            *
*  Type     : TransactionPreprocessingMapper             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains methods to map TransactionPreprocessingData instances to their DTOs.                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Instruments.Adapters;

using Empiria.Land.Media.Adapters;

namespace Empiria.Land.Transactions.Preprocessing {

  /// <summary>Contains methods to map TransactionPreprocessingData instances to their DTOs.</summary>
  static internal class TransactionPreprocessingMapper {

    static internal TransactionPreprocessingDto Map(TransactionPreprocessingData data) {
      var dto = new TransactionPreprocessingDto();

      dto.Actions.Can.EditInstrument = data.CanEditInstrument;
      dto.Actions.Can.UploadInstrumentFiles = data.CanUploadInstrumentFiles;
      dto.Actions.Can.SetAntecedent = data.CanSetAntecedent;
      dto.Actions.Can.CreateAntecedent = data.CanCreateAntecedent;
      dto.Actions.Can.EditAntecedentRecordingActs = data.CanEditAntecedentRecordingActs;

      dto.Actions.Show.Instrument = data.ShowInstrument;
      dto.Actions.Show.InstrumentFiles = data.ShowInstrumentFiles;
      dto.Actions.Show.Antecedent = data.ShowAntecedent;
      dto.Actions.Show.AntecedentRecordingActs = data.ShowAntecedentRecordingActs;

      dto.Media = LandMediaFileMapper.Map(data.TransactionMediaPostings);
      dto.Instrument = InstrumentMapper.Map(data.Instrument, data.Transaction);
      dto.Antecedent = data.Antecedent;
      dto.AntecedentRecordingActs = data.AntecedentRecordingActs;

      return dto;
    }

  }  // class TransactionPreprocessingMapper

}  // namespace Empiria.Land.Transactions.Preprocessing
