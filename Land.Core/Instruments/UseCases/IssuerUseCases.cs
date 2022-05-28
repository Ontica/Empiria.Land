/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Legal Instruments                          Component : Use cases Layer                         *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Use case interactor class               *
*  Type     : IssuerUseCases                             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to interact with legal instrument issuers: notaries, judges or authorities.     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using Empiria.Services;

using Empiria.Land.Instruments.Adapters;

namespace Empiria.Land.Instruments.UseCases {

  /// <summary>Use cases used to interact with legal instrument issuers: notaries,
  /// judges or authorities.</summary>
  public class IssuerUseCases : UseCase {

    #region Constructors and parsers

    protected IssuerUseCases() {
      // no-op
    }

    static public IssuerUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<IssuerUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public FixedList<IssuerDto> SearchIssuers(IssuersSearchCommand searchCommand) {
      Assertion.Require(searchCommand, "searchCommand");

      searchCommand.EnsureIsValid();

      var list = Issuer.GetList(searchCommand);

      return IssuerMapper.Map(list);
    }


    public void UpdateAll() {
      var command = new IssuersSearchCommand();
      command.PageSize = 100000;

      var list = Issuer.GetList(command);

      foreach (var item in list) {
        item.Save();
      }
    }

    #endregion Use cases

  }  // class IssuersUseCases

}  // namespace Empiria.Land.Instruments.UseCases
