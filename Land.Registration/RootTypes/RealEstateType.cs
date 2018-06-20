/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RealEstateType                                 Pattern  : Storage Item                        *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Describes the type of a real estate.                                                          *
*                                                                                                            *
********************************* Copyright (c) 2009-2017. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Describes the type of a real estate.</summary>
  public class RealEstateType : GeneralObject {

    #region Constructors and parsers

    private RealEstateType() {
      // Required by Empiria Framework.
    }

    static public RealEstateType Parse(int id) {
      return BaseObject.ParseId<RealEstateType>(id);
    }

    static public RealEstateType Empty {
      get { return BaseObject.ParseEmpty<RealEstateType>(); }
    }

    static public FixedList<RealEstateType> GetList() {
      var list = GeneralObject.GetList<RealEstateType>();

      list.Sort((x, y) => x.Name.CompareTo(y.Name));

      return list;
    }

    #endregion Constructors and parsers

  } // class RealEstateType

} // namespace Empiria.Land.Registration
