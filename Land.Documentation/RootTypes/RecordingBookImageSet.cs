/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Documentation                     Assembly : Empiria.Land.Documentation          *
*  Type      : RecordingBookImageSet                          Pattern  : Empiria Object Type                 *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Image set of a former physical land recording book.                                           *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

using Empiria.Documents;

using Empiria.Land.Registration;

namespace Empiria.Land.Documentation {

  /// <summary>Image set of a former physical land recording book.</summary>
  public class RecordingBookImageSet : ImageSet {

    #region Constructors and parsers

    private RecordingBookImageSet() {
      // Required by Empiria Framework
    }

    static public new RecordingBookImageSet Parse(int id) {
      return BaseObject.ParseId<RecordingBookImageSet>(id);
    }

    #endregion Constructors and parsers

    #region Public properties

    [DataField("PhysicalBookId")]
    public RecordingBook RecordingBook {
      get;
      private set;
    }

    #endregion Public properties

    #region Public methods

    protected override void OnSave() {
      throw new NotImplementedException();
    }

    #endregion Public methods

  }  // class RecordingBookImageSet

}  // namespace Empiria.Land.Documentation
