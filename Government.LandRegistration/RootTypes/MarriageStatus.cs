/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land                                   Assembly : Empiria.Land                        *
*  Type      : MarriageStatus                                 Pattern  : Storage Item                        *
*  Version   : 5.5        Date: 28/Mar/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Describes a person marriage status.                                                           *
*                                                                                                            *
********************************* Copyright (c) 1999-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Describes a person marriage status.</summary>
  public class MarriageStatus : GeneralObject {

    #region Fields

    private const string thisTypeName = "ObjectType.GeneralObject.MarriageStatus";

    #endregion Fields

    #region Constructors and parsers

    public MarriageStatus()
      : base(thisTypeName) {

    }

    protected MarriageStatus(string typeName)
      : base(typeName) {
      // Required by Empiria Framework. Do not delete. Protected in not sealed classes, private otherwise
    }

    static public MarriageStatus Empty {
      get { return BaseObject.ParseEmpty<MarriageStatus>(thisTypeName); }
    }

    static public MarriageStatus Unknown {
      get { return BaseObject.ParseUnknown<MarriageStatus>(thisTypeName); }
    }

    static public MarriageStatus Parse(int id) {
      return BaseObject.Parse<MarriageStatus>(thisTypeName, id);
    }

    static public ObjectList<MarriageStatus> GetList() {
      ObjectList<MarriageStatus> list = GeneralObject.ParseList<MarriageStatus>(thisTypeName);

      list.Sort((x, y) => x.Name.CompareTo(y.Name));

      return list;
    }

    #endregion Constructors and parsers

  } // class MarriageStatus

} // namespace Empiria.Land.Registration
