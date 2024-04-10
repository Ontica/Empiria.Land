/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Interface adapters                      *
*  Assembly : Empiria.Land.Transactions.dll              Pattern   : Mapper class                            *
*  Type     : ProvidedServiceDtoMapper                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains methods used to map to ProvidedServiceDto objects.                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Transactions {

  /// <summary>Contains methods used to map to ProvidedServiceDto objects.</summary>
  static internal class ProvidedServiceMapper {

    static internal FixedList<ProvidedServiceGroupDto> Map(FixedList<RecordingActTypeCategory> groups) {
      return new FixedList<ProvidedServiceGroupDto>(groups.Select((x) => Map(x)));
    }


    #region Private methods

    static private ProvidedServiceGroupDto Map(RecordingActTypeCategory group) {
      var dto = new ProvidedServiceGroupDto();

      dto.UID = group.UID;
      dto.Name = group.Name;
      dto.Services = GetProvidedServicesDtoArray(group.RecordingActTypes);

      return dto;
    }


    static private ProvidedServiceDto[] GetProvidedServicesDtoArray(FixedList<RecordingActType> servicesList) {
      ProvidedServiceDto[] array = new ProvidedServiceDto[servicesList.Count];

      for (int i = 0; i < servicesList.Count; i++) {
        array[i] = GetProvidedServiceDto(servicesList[i]);
      }

      return array;
    }


    static private ProvidedServiceDto GetProvidedServiceDto(RecordingActType service) {
      FixedList<LRSLawArticle> lawArticles = service.GetFinancialLawArticles();

      Empiria.DataTypes.Unit unit = Empiria.DataTypes.Unit.Empty;

      if (lawArticles.Count != 0) {
        unit = lawArticles[0].Unit;
      }

      var dto = new ProvidedServiceDto();

      dto.UID = service.UID;
      dto.Name = service.DisplayName;
      dto.Unit = unit.MapToNamedEntity();
      dto.FeeConcepts = GetApplicableFeeConceptsDtoArray(lawArticles);

      return dto;
    }


    static private FeeConceptDto[] GetApplicableFeeConceptsDtoArray(FixedList<LRSLawArticle> lawArticles) {
      FeeConceptDto[] array = new FeeConceptDto[lawArticles.Count];

      for (int i = 0; i < lawArticles.Count; i++) {
        array[i] = GetApplicableFeeConceptDto(lawArticles[i]);
      }

      return array;
    }


    static private FeeConceptDto GetApplicableFeeConceptDto(LRSLawArticle feeConcept) {
      return new FeeConceptDto {
        UID = feeConcept.UID,
        LegalBasis = feeConcept.Name,
        FinancialCode = feeConcept.FinancialConceptCode,
        RequiresTaxableBase = feeConcept.CalculatedOverTaxableBase
      };
    }

    #endregion Private methods

  }  // class ProvidedServiceMapper

}  // namespace Empiria.Land.Transactions
