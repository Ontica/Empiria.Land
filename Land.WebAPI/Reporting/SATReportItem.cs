/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Reporting services                           Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Information Holder                    *
*  Type     : SATReportItem                                License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Represents a record or text line that are the building blocks of a SATReport.                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Text;

using Empiria.Contacts;
using Empiria.Land.Registration;

namespace Empiria.Land.WebApi.Reporting {

  /// <summary>Represents a record or text line that are the building blocks of a SATReport.</summary>
  internal class SATReportItem {

    internal SATReportItem(RecordingDocument document,
                           RecordingAct recordingAct,
                           Resource resource,
                           RecordingActParty party) {
      this.Document = document;
      this.RecordingAct = recordingAct;
      this.Resource = resource;
      this.RecordingActParty = party;
    }


    internal RecordingDocument Document {
      get;
    }


    internal RecordingAct RecordingAct {
      get;
    }


    internal Resource Resource {
      get;
    }


    internal RecordingActParty RecordingActParty {
      get;
    }

    internal string ToTextLine(DateTime emissionDate) {
      var text = new StringBuilder();

      var party = this.RecordingActParty.Party;

      RealEstate realEstate = null;
      if (this.Resource is RealEstate) {
        realEstate = (RealEstate) this.Resource;
      }

      //Datos para la identificación del contribuyente

      if (party.UIDType == "RFC") {       // RFC
        text.Append(party.UID);
      } else {
        text.Append("XAXX010101000");
      }
      text.Append('|');
      if (party.UIDType == "CURP") {      // CURP
        text.Append(party.UID);
      } else {
        text.Append("XEXX010101HNEXXXA4");
      }
      text.Append('|');
      if (!(party is HumanParty)) {
        text.Append(party.FullName);
      } else {
        text.Append("  ");
      }
      text.Append('|');
      if (party is HumanParty) {
        text.Append(party.FullName);
      } else {
        text.Append("  ");
      }
      text.Append('|');
      text.Append("  ");                  // Apellido paterno
      text.Append('|');
      text.Append("  ");                  // Apellido materno
      text.Append('|');
      text.Append("  ");                  // Fecha de nacimiento
      text.Append('|');

      // Datos del Domicilio del propietario
      text.Append("  ");                  // Calle
      text.Append('|');
      text.Append("  ");                  // Num exterior
      text.Append('|');
      text.Append("  ");                  // Num interior
      text.Append('|');
      text.Append("  ");                  // Colonia
      text.Append('|');
      text.Append("  ");                  // Localidad
      text.Append('|');
      text.Append("  ");                  // Entidad
      text.Append('|');
      text.Append("  ");                  // Municipio o Delegación
      text.Append('|');
      text.Append("  ");                  // Código Postal
      text.Append('|');
      text.Append("  ");                  // Teléfono
      text.Append('|');
      text.Append("  ");                  // Correo electrónico
      text.Append('|');

      // Datos para la identificación

      text.Append(this.Document.AuthorizationTime.ToString("ddMMyyyy"));  // Fecha de alta
      text.Append('|');
      text.Append(emissionDate.ToString("ddMMyyyy"));
      text.Append('|');

      // Datos del inmueble

      text.Append(this.Resource.Tract.FirstRecordingAct.RegistrationTime.ToString("ddMMyyyy"));  // Fecha de inscripción
      text.Append('|');
      if (this.Document.IssuedBy is Person) {
        var person = (Person) this.Document.IssuedBy;

        text.Append(person.FirstName);        // Nombre del fedatario
        text.Append('|');
        text.Append(person.LastName);         // Apellido paterno
        text.Append('|');
        text.Append(person.LastName2);        // Apellido materno
      } else {
        text.Append(this.Document.IssuedBy.FullName);        // Nombre del fedatario
        text.Append('|');
        text.Append("  ");                    // Apellido paterno fedatario
        text.Append('|');
        text.Append(" ");                     // Apellido materno fedatario
      }
      text.Append('|');
      text.Append(this.Document.Number);       // Número de escritura de la operación
      text.Append('|');

      if (realEstate != null) {
        text.Append(realEstate.LocationReference);    // Calle del inmueble
        text.Append("|");
        text.Append("  ");                            // Num exterior del inmueble
        text.Append("|");
        text.Append("  ");                            // Num interior del inmueble
        text.Append("|");
        text.Append("  ");                            // Colonia
        text.Append("|");
        text.Append(realEstate.UID);                  // Localidad (se optó por el folio real)
        text.Append("|");
        text.Append("Tlaxcala");                      // Entidad
        text.Append("|");
        text.Append(realEstate.Municipality.Name);    // Municipio
        text.Append("|");
        text.Append("  ");                            // Código postal
      } else {
        text.Append("  ");                            // Calle del inmueble
        text.Append("|");
        text.Append("  ");                            // Num exterior del inmueble
        text.Append("|");
        text.Append("  ");                            // Num interior del inmueble
        text.Append("|");
        text.Append("  ");                            // Colonia
        text.Append("|");
        text.Append("  ");                            // Localidad
        text.Append("|");
        text.Append("  ");                            // Entidad
        text.Append("|");
        text.Append("  ");                            // Municipio
        text.Append("|");
        text.Append("  ");                            // Código postal
      }
      text.Append('|');
      if (this.Resource is Association) {
        text.Append(this.Resource.UID);                // Folios mercantiles  (se optó por el folio real sociedad)
      } else {
        text.Append("  ");
      }

      text.Append('|');
      text.Append("  ");                              // Acta constitutiva
      text.Append('|');
      text.Append("  ");                              // Protocolo cambio situación sociedad
      text.Append('|');
      text.Append(this.RecordingAct.DisplayName);      // Datos del gravamen
      text.Append('|');

      if (realEstate != null) {
        text.Append(realEstate.LotSize.Amount);       // Superficie
        text.Append('|');
        text.Append(realEstate.CadastralKey);         // Número de cuenta catastral
        text.Append('|');
        text.Append(realEstate.MetesAndBounds);       // Linderos, rumbos y colindancias
      } else {
        text.Append("  ");                            // Superficie
        text.Append('|');
        text.Append("  ");                            // Número de cuenta catastral
        text.Append('|');
        text.Append("  ");                            // Linderos, rumbos y colindancias
      }

      text.Replace(Environment.NewLine, " ");
      text.Replace("\r\n", " ");
      text.Replace("?", "'");

      return text.ToString();
    }

  }  // class SATReportItem

}  // namespace Empiria.Land.WebApi.Reporting
