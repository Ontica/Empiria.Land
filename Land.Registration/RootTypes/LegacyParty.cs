/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Registration                          Component : Search Services                         *
*  Assembly : Empiria.Land.Registration.dll              Pattern   : Empiria Object Type                     *
*  Type     : LegacyParty                                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains data abaout legacy registered parties.                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Contains data abaout legacy registered parties.</summary>
  public class LegacyParty: BaseObject {


    #region Constructors and parsers

    private LegacyParty() {
      // Required by Empiria Framework.
    }

    #endregion Constructors and parsers


    #region Fields


    [DataField("FullName")]
    public string FullName {
      get;
      private set;
    }


    [DataField("Volumen")]
    public string Volumen {
      get;
      private set;
    }


    [DataField("Partida")]
    public string Partida {
      get;
      private set;
    }


    [DataField("Seccion")]
    public string Seccion {
      get;
      private set;
    }


    [DataField("Distrito")]
    public string Distrito {
      get;
      private set;
    }


    [DataField("Fecha")]
    public string Fecha {
      get;
      private set;
    }


    #endregion Fields

  } // class LegacyParty

} // namespace Empiria.Land.Registration
