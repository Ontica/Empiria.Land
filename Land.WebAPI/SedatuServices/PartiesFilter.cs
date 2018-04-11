/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Services                       *
*  Namespace : Empiria.Land.WebApi.SedatuServices             Assembly : Empiria.Land.WebApi.dll             *
*  Type      : PartiesFilterModel                             Pattern  : Information Holder                  *
*  Version   : 4.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Contains parties filter by name fields.                                                       *
*                                                                                                            *
********************************** Copyright(c) 2016-2017. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.WebApi.SedatuServices {

  /// <summary>Contains parties filter by name fields.</summary>
  public class PartiesFilterModel {

    #region Properties

    public string nombres {
      get;
      set;
    }

    public string paterno {
      get;
      set;
    }

    public string materno {
      get;
      set;
    }

    #endregion Properties

    #region Methods

    public void AssertValid() {
      this.nombres = EmpiriaString.TrimSpacesAndControl(this.nombres);
      this.paterno = EmpiriaString.TrimSpacesAndControl(this.paterno);
      this.materno = EmpiriaString.TrimSpacesAndControl(this.materno);

      Assertion.Assert(!String.IsNullOrWhiteSpace(this.nombres + this.paterno + this.materno),
            "Null search parameters. At least one name parameter (nombres, paterno, materno) must be supplied.");

    }

    public string GetAsKeywords() {
      return EmpiriaString.BuildKeywords(this.paterno, this.materno, this.nombres);
    }

    #endregion Methods

  }  // class PartiesFilterModel

} // namespace Empiria.Land.WebApi.SedatuServices
