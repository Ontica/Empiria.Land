/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RealEstatePartition                            Pattern  : Standard Class                      *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Contains data about a property partition or subdivision.                                      *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

using Empiria.DataTypes;
using Empiria.Json;

namespace Empiria.Land.Registration {

  public enum PropertyPartitionType {
    None = 'N',
    Partial = 'P',
    Last = 'L',
    Full = 'F'
  }

  public enum PropertyPartitionSubtype {
    None = 'N',
    Whole = 'W',
    Lot = 'L',
    Apartment = 'A',
    House = 'H',
    Partial = 'P',
    PartialUnknown = 'Q',
    Last = 'S',
    LastUnknown = 'T',
  }

  /// <summary>Contains data about a real estate partition or subdivision.</summary>
  public class RealEstatePartition {

    #region Constructors and parsers

    public RealEstatePartition(string partitionType,
                               string partitionNumber,
                               string partitionRepeatUntilNumber) {
      this.PartitionType = partitionType;
      this.PartitionNumber = partitionNumber;
      this.PartitionRepeatUntilNumber = partitionRepeatUntilNumber;
    }

    public static RealEstatePartition Empty {
      get {
        return new RealEstatePartition(String.Empty, String.Empty, String.Empty);
      }
    }


    #endregion Constructors and parsers

    #region Properties

    public string PartitionType {
      get;
      private set;
    }

    public string PartitionNumber {
      get;
      private set;
    }

    public string PartitionName {
      get {
        return this.PartitionType + " " + this.PartitionNumber;
      }
    }

    public string PartitionRepeatUntilNumber {
      get;
      private set;
    }

    #endregion Properties

    #region Public methods

    internal string[] GetPartitions() {
      if (this.PartitionRepeatUntilNumber.Length == 0) {
        return new string[] { this.PartitionName };
      }
      if (!EmpiriaString.IsInteger(this.PartitionNumber) ||
          !EmpiriaString.IsInteger(this.PartitionRepeatUntilNumber)) {
        return new string[] { this.PartitionName };
      }
      int startPartition = int.Parse(this.PartitionNumber);
      int endPartition = int.Parse(this.PartitionRepeatUntilNumber);

      if (startPartition > endPartition) {
        return new string[] { this.PartitionName };
      }

      string[] partitionNames = new string[endPartition - startPartition + 1];
      for (int i = 0; i < partitionNames.Length; i++) {
        partitionNames[i] = this.PartitionType + " " + (startPartition + i).ToString();
      }
      return partitionNames;
    }

    #endregion Public methods

  }  // class RealEstatePartition

}  // namespace Empiria.Land.Registration
