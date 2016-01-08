/**
 *  Solution : Empiria Land Client                             || v0.1.0104
 *  Module   : Empiria.Land.Certificates
 *  Summary  : Gets data and performs operations over land certificates.
 *
 *  Author   : José Manuel Cota <https://github.com/jmcota>
 *  License  : GNU GPLv3. Other licensing terms are available. See <https://github.com/Ontica/Empiria.Land>
 *
 *  Copyright (c) 2015-2016. Ontica LLC, La Vía Óntica SC and contributors. <http://ontica.org>
*/

module Empiria.Land.Certificates {

  /** Holds data for Empiria Land certificates. */
  export interface CertificateData {
    uid?: string;

    certificateTypeUID: string;
    transactionUID: string;

    recorderOfficeId?: number;

    propertyUID?: string;
    propertyCommonName?: string;
    propertyLocation?: string;
    propertyMetesAndBounds?: string;

    operation?: string;
    operationDate?: Date;
    seekForName?: string;
    startingYear?: number;
    fromOwnerName?: string;
    toOwnerName?: string;

    marginalNotes?: string;
    useMarginalNotesAsFullBody: boolean;

    status?: string;
    statusName?: string;
    certificateTypeName?: string;

    transactionRequestedBy?: string;
    transactionTime?: Date;
    transactionReceipt?: string;

  }  // interface CertificateData


  /** Type to handle Empiria Land certificates. */
  export class Certificate {

    // #region Fields

    private _data: CertificateData = Certificate.empty;

    get data(): CertificateData {
      return this._data;
    }

    // #endregion Fields

    // #region Constructor and parsers

    constructor(data: CertificateData) {
      this._data = data;
    }

    /** Gets the empty instance for CertificateData */
    static empty: CertificateData = {
      uid: "",
      certificateTypeUID: "",
      transactionUID: "",
      transactionRequestedBy: "",
      transactionTime: new Date("2078-12-31"),
      transactionReceipt: "",

      recorderOfficeId: -1,

      propertyUID: "",
      propertyCommonName: "",
      propertyLocation: "",
      propertyMetesAndBounds: "",

      operation: "",
      operationDate: new Date("2078-12-31"),
      seekForName: "",
      startingYear : 0,
      fromOwnerName: "",
      toOwnerName: "",
      marginalNotes: "",
      useMarginalNotesAsFullBody: false,

      status: "",
      statusName: "",
      certificateTypeName: "",
    };

    /**
      * Static method that parses an existing certificate given its unique ID.
      * @param certificateUID A string with the unique ID of the certificate.
      */
    static parse(certificateUID: string): Certificate {
      var dataOperation = Empiria.DataOperation.parse("getLandCertificate", certificateUID);

      var data = dataOperation.getData();

      //Empiria.Assertion.hasValue(data, "There was a problem reading data for certificate {0}.", certificateUID);

      return new Certificate(this.prototype.convertToCertificateData(data));
    }

    /** Static method that creates a new certificate with the supplied data.
      * @param newCertificateData The new certificate's data.
      */
    static create(newCertificateData: CertificateData): Certificate {
      var dataOperation = Empiria.DataOperation.parse("createLandCertificate");

      var responseData = dataOperation.writeData(newCertificateData);

      //Empiria.Assertion.hasValue(responseData, "There was a problem writing data for the new certificate.");

      return new Certificate(this.prototype.convertToCertificateData(responseData));
    }

    // #endregion Constructor and parsers

    // #region Public methods

    /** Closes the certificate, preventing any further changes. */
    public close() {
      var dataOperation = Empiria.DataOperation.parse("closeLandCertificate", this.data.uid);

      var responseData = dataOperation.executeAndGetData();

      this._data = this.convertToCertificateData(responseData);
    }

    /** Deletes this certificate if it is opened, otherwise throws an exception. */
    public delete(): void {
      var dataOperation = Empiria.DataOperation.parse("deleteLandCertificate", this.data.uid);

      var responseData = dataOperation.executeAndGetData();

      //Empiria.Assertion.hasValue(responseData, "There was a problem retriving the new state for certificate {0}.", this.data.uid);

      this._data = this.convertToCertificateData(responseData);
    }

    /** Reopens this certificate if it is closed, otherwise throws an exception. */
    public reOpen(): void {
      var dataOperation = Empiria.DataOperation.parse("openLandCertificate", this.data.uid);

      var responseData = dataOperation.executeAndGetData();

      //Empiria.Assertion.hasValue(responseData, "There was a problem retriving the new state for certificate {0}.", this.data.uid);

      this._data = this.convertToCertificateData(responseData);
    }

    /** Gets the HTML text representation of this certificate. */
    public getHtmlText(): string {
      var dataOperation = Empiria.DataOperation.parse("getLandCertificateText", this.data.uid);

      var responseData = dataOperation.getData();

      //Empiria.Assertion.hasValue(responseData, "There was a problem retriving the text for certificate {0}.", this.data.uid);

      return responseData.text;
    }

    /** Saves the last changes of this certificate in the server.
        The certificate should be opened or an exception will be thrown. */
    public update(): void {
      var dataOperation = Empiria.DataOperation.parse("updateLandCertificate", this.data.uid);

      var responseData = dataOperation.writeData(this.data);

      //Empiria.Assertion.hasValue(responseData, "There was a problem updating the certificate {0}.", this.data.uid);

      this._data = this.convertToCertificateData(responseData);
    }

    // #endregion Public methods

    // #region Private methods

    /** Helper that converts a certificate server object to CertificateData type.*/
    private convertToCertificateData(serverData: any): CertificateData {
      var certificateData = {
        uid: serverData.uid,
        certificateTypeUID: serverData.type.uid,

        transactionUID: serverData.transaction.uid,
        transactionRequestedBy: serverData.transaction.requestedBy,
        transactionTime: serverData.transaction.presentationTime,
        transactionReceipt: serverData.transaction.paymentReceipt,

        recorderOfficeId: serverData.recorderOffice.id,

        propertyUID: serverData.property.uid,
        propertyCommonName: serverData.property.commonName,
        propertyLocation: serverData.property.location,
        propertyMetesAndBounds: serverData.property.metesAndBounds,

        operation: serverData.operation,
        operationDate: serverData.operationDate,
        seekForName: serverData.seekForName,
        startingYear: serverData.startingYear,
        fromOwnerName: serverData.fromOwnerName,
        toOwnerName: serverData.toOwnerName,
        marginalNotes: serverData.marginalNotes,
        useMarginalNotesAsFullBody: serverData.useMarginalNotesAsFullBody,

        status: serverData.status.uid,
        statusCode: serverData.status.code,
        certificateTypeName: serverData.type.displayName,
      };
      return certificateData;
    }

    // #endregion Private methods

  }  // class Certificate

}  // module Empiria.Land.Certificates
