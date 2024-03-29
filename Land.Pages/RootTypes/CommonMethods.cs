﻿using System;


namespace Empiria.Land.Pages {

  static public class CommonMethods {

    static public readonly bool DISPLAY_VEDA_ELECTORAL_UI =
                                    ConfigurationData.Get<bool>("DisplayVedaElectoralUI", false);

    static public string CustomerOfficeName => "Dirección de Catastro y Registro Público";

    static public string GovernmentName => "GOBIERNO DEL ESTADO DE ZACATECAS";

    static public string GovernmentWebPage => "https://registropublico.zacatecas.gob.mx";


    static internal string GetDateAsText(DateTime date) {
      if (date == ExecutionServer.DateMinValue || date == ExecutionServer.DateMaxValue) {
        return "No consta";
      } else {
        return date.ToString(@"dd \de MMMM \de yyyy");
      }
    }

    static internal string AsWarning(string text) {
      return "<span style='color:red;'><strong>*****" + text + "*****</strong></span>";
    }

  } // class CommonMethods

} // namespace Empiria.Land.Pages

