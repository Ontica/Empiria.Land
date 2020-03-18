/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Services                       Component : Filing Forms                            *
*  Assembly : Empiria.Land.Registration.dll              Pattern   : Loose coupling interface                *
*  Type     : INotaryForm                                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes a Land System data form emitted by a notary.                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Contacts;


/// <summary>Describes a Land System data form emitted by a notary.</summary>
namespace Empiria.Land.Registration.Forms {


  public interface INotaryForm {

    RealPropertyDescription RealPropertyDescription { get; }


    Contact NotaryOffice { get; }


    Contact Notary { get; }


    string ESign { get; }


    DateTime AuthorizationTime { get; }


  }  // interface INotaryForm

}  // namespace Empiria.Land.Registration.Forms
