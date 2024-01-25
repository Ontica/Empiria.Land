/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RealEstatePartitionDTO                         Pattern  : Data Transfer Object                *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Data transfer object that holds data about a real estate partition or subdivision.            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Data transfer object that holds data about a real estate partition or subdivision.</summary>
  public class RealEstatePartitionDTO {

    #region Constructors and parsers

    public RealEstatePartitionDTO(string partitionType,
                               string partitionNumber,
                               string partitionRepeatUntilNumber) {
      this.PartitionType = partitionType;
      this.PartitionNumber = partitionNumber;
      this.PartitionRepeatUntilNumber = partitionRepeatUntilNumber;
    }

    public static RealEstatePartitionDTO Empty {
      get {
        return new RealEstatePartitionDTO(String.Empty, String.Empty, String.Empty);
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

    internal string[] GetPartitionNames() {
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

  }  // class RealEstatePartitionDTO

}  // namespace Empiria.Land.Registration
