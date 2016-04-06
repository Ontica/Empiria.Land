/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                 System   : Land Registration System              *
*  Namespace : Empiria.Land.Registration.Data               Assembly : Empiria.Land.Registration             *
*  Type      : DocumentsData                                Pattern  : Data Services                         *
*  Version   : 2.1                                          License  : Please read license.txt file          *
*                                                                                                            *
*  Summary   : Provides database read and write methods for recording documents.                             *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;

using Empiria.Data;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Registration.Data {

  /// <summary>Provides database read and write methods for recording documents.</summary>
  static internal class DocumentsData {

    #region Public methods

    static internal string GenerateDocumentUID() {
      string temp = String.Empty;
      int hashCode = 0;
      bool useLetters = false;
      for (int i = 0; i < 7; i++) {
        if (useLetters) {
          temp += EmpiriaMath.GetRandomCharacter(temp);
          temp += EmpiriaMath.GetRandomCharacter(temp);
        } else {
          temp += EmpiriaMath.GetRandomDigit(temp);
          temp += EmpiriaMath.GetRandomDigit(temp);
        }
        hashCode += ((Convert.ToInt32(temp[temp.Length - 2]) +
                      Convert.ToInt32(temp[temp.Length - 1])) % ((int) Math.Pow(i + 1, 2)));
        useLetters = !useLetters;
      }
      string prefix = ExecutionServer.LicenseName == "Zacatecas" ? "ZS" : "TL";
      temp = "RP" + temp.Substring(0, 4) + "-" + temp.Substring(4, 6) + "-" + temp.Substring(10, 4);

      hashCode = (hashCode * Convert.ToInt32(prefix[0])) % 49;
      hashCode = (hashCode * Convert.ToInt32(prefix[1])) % 53;

      temp += "ABCDEFGHJKMLNPQRSTUVWXYZ".Substring(hashCode % 24, 1);
      temp += "9A8B7C6D5E4F3G2H1JKR".Substring(hashCode % 20, 1);

      return temp;
    }

    static internal LRSTransaction GetDocumentTransaction(RecordingDocument document) {
      if (document.IsEmptyInstance) {
        return LRSTransaction.Empty;
      }

      var sql = String.Format("SELECT * FROM LRSTransactions WHERE DocumentId = {0}", document.Id);

      var dataRow = DataReader.GetDataRow(DataOperation.Parse(sql));
      if (dataRow != null) {
        return BaseObject.ParseDataRow<LRSTransaction>(dataRow);
      } else {
        return LRSTransaction.Empty;
      }
    }

    internal static string GetNextImagingControlID(RecordingDocument document) {
      string prefix = document.AuthorizationTime.ToString("yyyy-MM");

      var sql = String.Format("SELECT MAX(ImagingControlID) " +
                              "FROM LRSDocuments WHERE ImagingControlID LIKE '{0}-%'", prefix);

      var imagingControlID = DataReader.GetScalar<String>(DataOperation.Parse(sql), String.Empty);

      if (imagingControlID != String.Empty) {
        var counter = int.Parse(imagingControlID.Split('-')[2]);
        counter++;
        return prefix + "-" + counter.ToString("00000");
      } else {
        return prefix + "-" + 1.ToString("00000");
      }
    }

    static internal DataRow GetRecordingMainDocument(Recording recording) {
      return DataReader.GetDataRow(DataOperation.Parse("getLRSRecordingMainDocument", recording.Id));
    }

    internal static int SaveImagingControlID(RecordingDocument document) {
      var op = DataOperation.Parse("setLRSDocumentImagingControlID",
                                   document.Id, document.ImagingControlID);
      return DataWriter.Execute(op);
    }

    #endregion Public methods

  } // class DocumentsData

} // namespace Empiria.Land.Registration.Data
