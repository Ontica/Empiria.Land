/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Recordable Subjects                        Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Input Data Holder                       *
*  Type     : RecordableSubjectsFields                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Fields input data holder for real estate, associations and no-property recordable subjects.    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Land.Registration;

namespace Empiria.Land.RecordableSubjects.Adapters {

  /// <summary>Base input data fields for all recordable subjects.</summary>
  public class RecordableSubjectFields {

    public string UID {
      get; set;
    } = string.Empty;


    public RecordableSubjectType Type {
      get; set;
    } = RecordableSubjectType.None;


    public string ElectronicID {
      get; set;
    } = string.Empty;


    public string RecorderOfficeUID {
      get; set;
    } = string.Empty;


    public string Kind {
      get; set;
    } = string.Empty;


    public RecordableObjectStatus Status {
      get; set;
    } = RecordableObjectStatus.Incomplete;


  }  // RecordableSubjectFields



  /// <summary>Fields input data holder for associations or societies.</summary>
  public class AssociationFields : RecordableSubjectFields {

    public string Name {
      get; set;
    } = string.Empty;


    public string Description {
      get; set;
    } = string.Empty;


  }  // class AssociationFields


  /// <summary>Fields input data holder for no-property entities like
  /// recordable documents or things.</summary>
  public class NoPropertyFields : RecordableSubjectFields {

    public string Name {
      get; set;
    } = string.Empty;


    public string Description {
      get; set;
    } = string.Empty;


  }  // class NoPropertyFields


  /// <summary>Fields input data holder for real estate.</summary>
  public class RealEstateFields : RecordableSubjectFields {

    public string MunicipalityUID {
      get; set;
    } = string.Empty;


    public string CadastralID {
      get; set;
    } = string.Empty;


    public string Description {
      get; set;
    } = string.Empty;


    public string MetesAndBounds {
      get; set;
    } = string.Empty;


    public decimal LotSize {
      get; set;
    }

    public string LotSizeUnitUID {
      get; set;
    } = string.Empty;


  }  // class RealEstateFields

}  // namespace Empiria.Land.RecordableSubjects.Adapters
