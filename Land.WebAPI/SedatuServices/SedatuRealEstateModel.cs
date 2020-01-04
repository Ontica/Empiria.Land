/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Services                       *
*  Namespace : Empiria.Land.WebApi.SedatuServices             Assembly : Empiria.Land.WebApi.dll             *
*  Type      : SedatuRealEstateModel                          Pattern  : Information Holder                  *
*  Version   : 4.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : SEDATU Real Estate model with location, cadastral and owners data.                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Registration;

namespace Empiria.Land.WebApi.SedatuServices {

  /// <summary>SEDATU Real Estate model with location, cadastral and owners data.</summary>
  public class SedatuRealEstateModel {

    private RealEstate realEstate = RealEstate.Empty;
    private RecordingAct recordingAct = RecordingAct.Empty;
    private Party party = Party.Empty;
    private RecordingActParty recordingActParty = RecordingActParty.Empty;

    public SedatuRealEstateModel(RecordingAct recordingAct) {
      this.realEstate = recordingAct.Resource as RealEstate;
      this.recordingAct = recordingAct;

      var parties = this.recordingAct.GetParties();

      if (parties.Count != 0) {
        this.recordingActParty = parties[0];
        this.party = parties[0].Party;
      }
      this.COPROP_PORCEN = this.realEstate.CurrentOwners;
    }

    public SedatuRealEstateModel(RecordingActParty recordingActParty) {
      this.realEstate = recordingActParty.RecordingAct.Resource as RealEstate;
      this.recordingAct = recordingActParty.RecordingAct;
      this.party = recordingActParty.Party;
      this.recordingActParty = recordingActParty;
    }

    #region Properties

    public string NOM_RAZONSOC {
      get {
        return this.party.FullName;
      }
    }


    public string PATERNO {
      get;
      set;
    } = String.Empty;


    public string MATERNO {
      get;
      set;
    } = String.Empty;


    public string RFC {
      get {
        return this.party.OfficialIDType == "RFC" ? this.party.OfficialID : String.Empty;
      }
    }


    public string CURP {
      get {
        return this.party.OfficialIDType == "CURP" ? this.party.OfficialID : String.Empty;
      }
    }

    public string DOC_IDENTIF {
      get {
        return this.party.OfficialIDType != "None" ? this.party.OfficialIDType : String.Empty;
      }
    }


    public string NUM_IDENTIF {
      get {
        return this.party.OfficialID;
      }
    }


    public string TIPO_PROP {
      get {
        return recordingActParty.PartyRole.Name;
      }
    }


    public string COPROP_PORCEN {
      get;
      set;
    } = String.Empty;


    public string FOLIOREAL {
      get {
        return this.realEstate.UID;
      }
    }

    public string CVE_OFNAREG {
      get {
        return this.realEstate.District.Number;
      }
    }


    public string OFNAREG {
      get {
        return this.realEstate.District.Alias;
      }
    }


    public string INSCRIPCION {
      get {
        return this.recordingAct.Document.UID;
      }
    }


    //public string IMAGEN_INSCRIPCION {
    //  get {
    //    return this.recordingAct.Document.ImagingControlID;
    //  }
    //}


    public string CVE_MPIOINEGI {
      get {
        return this.realEstate.Municipality.Code;
      }
    }


    public string NOM_MPIOINEGI {
      get {
        return this.realEstate.Municipality.Name;
      }
    }


    public string CVE_MPIOENT {
      get {
        return this.realEstate.Municipality.Code;
      }
    }


    public string NOM_MPIOENT {
      get {
        return this.realEstate.Municipality.Name;
      }
    }


    public string DIRECCION {
      get {
        return this.realEstate.LocationReference;
      }
    }


    public string LOCALIDAD {
      get;
      set;
    } = String.Empty;


    public string COLONIA {
      get;
      set;
    } = String.Empty;


    public string COD_POSTAL {
      get;
      set;
    } = String.Empty;


    public string TIPO_VIALIDAD {
      get;
      set;
    } = String.Empty;


    public string NOM_VIALIDAD {
      get;
      set;
    } = String.Empty;


    public string NUM_EXTERIOR {
      get;
      set;
    } = String.Empty;


    public string NOM_EDIFPREDIO {
      get {
        return this.realEstate.Name;
      }
    }


    public string NIVEL {
      get;
      set;
    } = String.Empty;


    public string NUM_INTDEPTO {
      get;
      set;
    } = String.Empty;


    public string MEDCOLIN {
      get {
        return this.realEstate.MetesAndBounds;
      }
    }


    public string LOTE {
      get {
        return this.realEstate.PartitionNo;
      }
    }


    public string MANZANA {
      get;
      set;
    } = String.Empty;


    public string SUPERFICIE {
      get {
        if (this.realEstate.LotSize.Amount > 100000 && this.realEstate.LotSize.Unit.Abbr == "Ha") {
          return this.FormatSurfaceAsHectars(this.realEstate.LotSize.Amount).ToString() + " Ha";
        } else if (this.realEstate.LotSize.Amount > 0) {
          return this.realEstate.LotSize.ToString();
        } else {
          return "No registrada";
        }
      }
    }

    public string USOSUELO {
      get;
      set;
    } = String.Empty;


    public string CVE_CATASTRAL {
      get {
        return this.realEstate.CadastralKey;
      }
    }


    public string CTA_PREDIAL {
      get;
      set;
    } = String.Empty;


    #endregion Properties

    #region Methods

    private decimal FormatSurfaceAsHectars(decimal surface) {
      return surface / 10000000;
    }

    #endregion Methods

  }  // class SedatuRealEstateModel

} // namespace Empiria.Land.WebApi.SedatuServices
