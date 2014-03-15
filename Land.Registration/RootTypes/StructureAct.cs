/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land                                   Assembly : Empiria.Land                        *
*  Type      : StructureAct                                   Pattern  : Empiria Object Type                 *
*  Version   : 1.5        Date: 28/Mar/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Recording act that represents the changes on measures and limits of real estates,             *
*              as well as the creation of new properties through fusions and divisons.                       *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;

namespace Empiria.Land.Registration {

  /// <summary>Recording act that represents the changes on measures and limits of real estates, 
  ///as well as the creation of new properties through fusions and divisons.</summary>
  public class StructureAct : RecordingAct {

    #region Fields

    private const string thisTypeName = "ObjectType.RecordingAct.StructureAct";

    #endregion Fields

    #region Constructors and parsers

    private StructureAct()
      : base(thisTypeName) {
      // For create instances use Create static method instead
    }

    protected StructureAct(string typeName)
      : base(typeName) {
      // Required by Empiria Framework. Do not delete. Protected in not sealed classes, private otherwise
    }

    static public new StructureAct Parse(int id) {
      return BaseObject.Parse<StructureAct>(thisTypeName, id);
    }

    #endregion Constructors and parsers

    #region Public properties

    #endregion Public properties

    #region Public methods

    protected override void ImplementsLoadObjectData(DataRow row) {
      base.ImplementsLoadObjectData(row);
    }

    protected override void ImplementsSave() {
      base.ImplementsSave();
    }

    #endregion Public methods

  } // class StructureAct

} // namespace Empiria.Land.Registration
