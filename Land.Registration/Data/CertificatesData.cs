/* Empiria Land 2015 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                 System   : Land Registration System              *
*  Namespace : Empiria.Land.Data                            Assembly : Empiria.Land.Registration             *
*  Type      : CertificatesData                             Pattern  : Data Services                         *
*  Version   : 2.0        Date: 23/Oct/2015                 License  : Please read license.txt file          *
*                                                                                                            *
*  Summary   : Provides database read and write methods for land certificates.                               *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;

using Empiria.Data;

using Empiria.Land.Registration;
using Empiria.Land.Certification;

namespace Empiria.Land.Data {

  /// <summary>Provides database read and write methods for land certificates.</summary>
  static internal class CertificatesData {

    #region Public methods

    static internal string BuildCertificateUID() {
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
      temp = "CE" + temp.Substring(0, 4) + "-" + temp.Substring(4, 6) + "-" + temp.Substring(10, 4);

      temp += "ABCDEFHJKMNPRTWXYZ".Substring((hashCode * Convert.ToInt32(prefix[0])) % 17, 1);
      temp += "9A8B7CD5E4F2".Substring((hashCode * Convert.ToInt32(prefix[1])) % 11, 1);

      return temp;
    }

    static internal FixedList<Certificate> ResourceEmittedCertificates(Resource resource) {
      if (resource.IsEmptyInstance) {
        return new FixedList<Certificate>();
      }

      var operation = DataOperation.Parse("qryLRSResourceEmittedCertificates", resource.Id);

      return DataReader.GetList<Certificate>(operation,
                                             (x) => BaseObject.ParseList<Certificate>(x)).ToFixedList();
    }

    internal static void WriteCertificate(Certificate o) {
      var op = DataOperation.Parse("writeLRSCertificate",
                          o.Id, o.CertificateType.Id, o.UID,
                          o.Transaction.Id, o.RecorderOffice.Id, o.Property.Id, o.OwnerName,
                          o.UserNotes, o.ExtensionData.ToJson(), o.AsText, o.Keywords,
                          o.IssueTime, o.IssuedBy.Id, o.SignedBy.Id, (char) o.IssueMode,
                          o.PostedBy.Id, o.PostingTime, (char) o.Status, o.Integrity.GetUpdatedHashCode());

      DataWriter.Execute(op);
    }

    #endregion Public methods

  } // class CertificatesData

} // namespace Empiria.Land.Data
