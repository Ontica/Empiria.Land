/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Extranet Services                            Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : InstrumentController                         License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Public API to read and write legal instruments.                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.Json;
using Empiria.WebApi;

using Empiria.Land.Instruments;

namespace Empiria.Land.WebApi.Extranet {

  /// <summary>Public API to read and write legal instruments.</summary>
  public class InstrumentController : WebApiController {

    #region Public methods

    [HttpGet]
    [Route("v2/extranet/instruments")]
    public PagedCollectionModel GetInstruments([FromUri] InstrumentStatus status = InstrumentStatus.Pending,
                                               [FromUri] string keywords = "") {
      try {
        var list = LegalInstrument.GetList(status, keywords);

        return new PagedCollectionModel(this.Request, list.ToResponse());

      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }


    [HttpGet]
    [Route("v2/extranet/instruments/{instrumentUID}")]
    public SingleObjectModel GetInstrument([FromUri] string instrumentUID) {
      try {
        base.RequireResource(instrumentUID, "instrumentUID");

        var instrument = LegalInstrument.TryParse(instrumentUID);

        if (instrument != null) {
          return new SingleObjectModel(this.Request, instrument.ToResponse(),
                                       instrument.GetType().FullName);
        } else {
          throw new ResourceNotFoundException("Instrument.UID.NotFound",
                                              $"No tenemos registrado ningún instrumento con folio real {instrumentUID}.");
        }
      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }


    [HttpPost]
    [Route("v2/extranet/instruments/create-preventive-note")]
    public SingleObjectModel CreatePreventiveNote([FromBody] object body) {
      try {
        base.RequireBody(body);
        var bodyAsJson = JsonObject.Parse(body);

        var instrument = new LegalInstrument(bodyAsJson);

        instrument.Save();

        return new SingleObjectModel(this.Request, instrument.ToResponse(),
                                     instrument.GetType().FullName);

      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }


    [HttpPost]
    [Route("v2/extranet/instruments/{instrumentUID}/sign")]
    public SingleObjectModel SignInstrument([FromUri] string instrumentUID, [FromBody] object body) {
      try {
        base.RequireResource(instrumentUID, "instrumentUID");
        base.RequireBody(body);
        var bodyAsJson = JsonObject.Parse(body);

        var instrument = LegalInstrument.TryParse(instrumentUID);

        if (instrument == null) {
          throw new ResourceNotFoundException("Instrument.UID.NotFound",
                                              $"No tenemos registrado ningún instrumento con identificador {instrumentUID}.");
        }

        instrument.Sign(bodyAsJson);

        instrument.Save();

        return new SingleObjectModel(this.Request, instrument.ToResponse(),
                                     instrument.GetType().FullName);

      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }


    [HttpPost]
    [Route("v2/extranet/instruments/{instrumentUID}/request-payment-order")]
    public SingleObjectModel RequestPaymentOrder([FromUri] string instrumentUID, [FromBody] object body) {
      try {
        base.RequireResource(instrumentUID, "instrumentUID");
        base.RequireBody(body);
        var bodyAsJson = JsonObject.Parse(body);

        var instrument = LegalInstrument.TryParse(instrumentUID);

        if (instrument == null) {
          throw new ResourceNotFoundException("Instrument.UID.NotFound",
                                              $"No tenemos registrado ningún instrumento con identificador {instrumentUID}.");
        }

        instrument.RequestFiling(bodyAsJson);

        instrument.Save();

        return new SingleObjectModel(this.Request, instrument.ToResponse(),
                                     instrument.GetType().FullName);

      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }


    [HttpPost]
    [Route("v2/extranet/instruments/{instrumentUID}/request-recording")]
    public SingleObjectModel RequestRecording([FromUri] string instrumentUID, [FromBody] object body) {
      try {
        base.RequireResource(instrumentUID, "instrumentUID");
        base.RequireBody(body);
        var bodyAsJson = JsonObject.Parse(body);

        var instrument = LegalInstrument.TryParse(instrumentUID);

        if (instrument == null) {
          throw new ResourceNotFoundException("Instrument.UID.NotFound",
                                              $"No tenemos registrado ningún instrumento con identificador {instrumentUID}.");
        }

        instrument.FileTransaction(bodyAsJson);

        instrument.Save();

        return new SingleObjectModel(this.Request, instrument.ToResponse(),
                                     instrument.GetType().FullName);

      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }


    [HttpPost]
    [Route("v2/extranet/instruments/{instrumentUID}/revoke-sign")]
    public SingleObjectModel RevokeInstrumentSign([FromUri] string instrumentUID, [FromBody] object body) {
      try {
        base.RequireResource(instrumentUID, "instrumentUID");
        base.RequireBody(body);
        var bodyAsJson = JsonObject.Parse(body);

        var instrument = LegalInstrument.TryParse(instrumentUID);

        if (instrument == null) {
          throw new ResourceNotFoundException("Instrument.UID.NotFound",
                                              $"No tenemos registrado ningún instrumento con identificador {instrumentUID}.");
        }

        instrument.RevokeSign(bodyAsJson);

        instrument.Save();

        return new SingleObjectModel(this.Request, instrument.ToResponse(),
                                     instrument.GetType().FullName);

      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }



    [HttpPut, HttpPatch]
    [Route("v2/extranet/instruments/{instrumentUID}")]
    public SingleObjectModel UpdateInstrument([FromUri] string instrumentUID, [FromBody] object body) {
      try {
        base.RequireResource(instrumentUID, "instrumentUID");

        base.RequireBody(body);
        var bodyAsJson = JsonObject.Parse(body);

        var instrument = LegalInstrument.TryParse(instrumentUID);

        if (instrument == null) {
          throw new ResourceNotFoundException("Instrument.UID.NotFound",
                                              $"No tenemos registrado ningún instrumento con identificador {instrumentUID}.");
        }

        instrument.Update(bodyAsJson);

        instrument.Save();

        return new SingleObjectModel(this.Request, instrument.ToResponse(),
                                     instrument.GetType().FullName);

      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }


    #endregion Public methods

  }  // class InstrumentController

}  // namespace Empiria.Land.WebApi.Extranet
