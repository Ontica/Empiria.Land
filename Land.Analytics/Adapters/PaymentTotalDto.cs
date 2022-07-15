/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Analytics Services                         Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Transfer Object                    *
*  Type     : PaymentTotalDto                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases that perform parties registration over recording acts.                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.Analytics.Adapters {

  public class PaymentTotalDto {

    [DataField("ObjectId")]
    public int Id {
      get;
      private set;
    }


    [DataField("cantidad")]
    public int Cantidad {
      get;
      private set;
    }


    [DataField("tipo")]
    public string Tipo {
      get;
      private set;
    }


    [DataField("Total")]
    public decimal Total {
      get;
      private set;
    }

  }

}
