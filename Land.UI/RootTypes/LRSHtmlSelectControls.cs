/* Empiria Land 2014 ******************************************************************************************
*                                                                                                             *
*  Solution  : Empiria Land                                    System   : Land Registration System            *
*  Namespace : Empiria.Land.UI                                 Assembly : Empiria.Land                        *
*  Type      : LRSHtmlSelectControls                           Pattern  : Presentation Services Static Class  *
*  Version   : 1.5        Date: 28/Mar/2014                    License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                             *
*  Summary   : Static class that generates predefined HtmlSelect controls content for Empiria Government      *
*              Land Registration System.                                                                      *
*                                                                                                             *
********************************** Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Empiria.Contacts;
using Empiria.Geography;
using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;
using Empiria.Presentation;
using Empiria.Presentation.Web;
using Empiria.Presentation.Web.Content;

namespace Empiria.Land.UI {

  /// <summary>Static class that generates predefined HtmlSelect controls content for Empiria Government Land
  /// Registration System.</summary>
  static public class LRSHtmlSelectControls {

    #region Public methods

    static public void LoadLegacyAnnotationActTypesCategoriesCombo(HtmlSelect comboControl) {
      GeneralList listType = GeneralList.Parse("LegacyAnnotationActTypesCategories.List");
      ObjectList<RecordingActTypeCategory> list = listType.GetItems<RecordingActTypeCategory>();

      HtmlSelectContent.LoadCombo(comboControl, list, "Id", "Name", "( Tipo de limitación )");
    }

    static public void LoadLegacyTraslativeActTypesCategoriesCombo(HtmlSelect comboControl) {
      GeneralList listType = GeneralList.Parse("LegacyTraslativeActTypesCategories.List");
      ObjectList<RecordingActTypeCategory> list = listType.GetItems<RecordingActTypeCategory>();

      HtmlSelectContent.LoadCombo(comboControl, list, "Id", "Name", "( Tipo de acto jurídico )");
    }

    static public void LoadRecordingActTypesCategoriesCombo(HtmlSelect comboControl) {
      GeneralList listType = GeneralList.Parse("RecordingActTypesCategories.List");
      ObjectList<RecordingActTypeCategory> list = listType.GetItems<RecordingActTypeCategory>();

      HtmlSelectContent.LoadCombo(comboControl, list, "Id", "Name", "( Tipo de acto jurídico )");
    }

    static public void LoadTransactionActTypesCategoriesCombo(HtmlSelect comboControl) {
      GeneralList listType = GeneralList.Parse("TransactionActTypesCategories.List");
      ObjectList<RecordingActTypeCategory> list = listType.GetItems<RecordingActTypeCategory>();

      HtmlSelectContent.LoadCombo(comboControl, list, "Id", "Name");
    }

    static public void LoadRecordingBookClassesCombo(HtmlSelect comboControl, string headerItemText,
                                                     RecordingActTypeCategory defaultItem) {
      GeneralList listType = GeneralList.Parse("RecordingBookClasses.List");
      ObjectList<RecordingActTypeCategory> list = listType.GetItems<RecordingActTypeCategory>();

      HtmlSelectContent.LoadCombo(comboControl, list, "Id", "Name", headerItemText);
      if (defaultItem != null && !defaultItem.IsEmptyInstance) {
        comboControl.Value = defaultItem.Id.ToString();
      }
    }

    static public string GetTransactionNewStatusComboItems(int typeId, int docTypeId, TransactionStatus status) {
      List<TransactionStatus> list = LRSTransaction.NextStatusList(LRSTransactionType.Parse(typeId),
                                                                   LRSDocumentType.Parse(docTypeId), status);
      string html = String.Empty;
      for (int i = 0; i < list.Count; i++) {
        html += HtmlSelectContent.GetComboHtmlItem(Convert.ToString((char) list[i]), 
                                                   LRSTransaction.StatusName(list[i]));
      }
      return html;
    }

    static public string GetRecordingBookClassesComboItems(string headerItemText) {
      GeneralList listType = GeneralList.Parse("RecordingBookClasses.List");
      ObjectList<RecordingActTypeCategory> list = listType.GetItems<RecordingActTypeCategory>();

      return HtmlSelectContent.GetComboHtml(list, "Id", "Name", headerItemText);
    }

    static public string GetBookImageClippersComboItems(RecorderOffice recorderOffice,
                                                        ComboControlUseMode comboControlUseMode,
                                                        Contact defaultBookImageClipper) {
      ObjectList<Contact> contacts = recorderOffice.GetContactsInRole<Contact>("RecorderOffice_ImageClippers");
      return GetContactsInRoleComboItems(contacts, defaultBookImageClipper, comboControlUseMode,
                                         "( Seleccionar al cortador del libro )",
                                         "( Todos los cortadores de libros )",
                                         "No hay cortadores definidos");
    }

    static public string GetBookImageDigitalizersComboItems(RecorderOffice recorderOffice,
                                                            ComboControlUseMode comboControlUseMode,
                                                            Contact defaultBookImageDigitalizer) {
      ObjectList<Contact> contacts = recorderOffice.GetContactsInRole<Contact>("RecorderOffice_ImageDigitalizers");

      return GetContactsInRoleComboItems(contacts, defaultBookImageDigitalizer, comboControlUseMode,
                                         "( Seleccionar al digitalizador del libro )",
                                         "( Todos los digitalizadores )",
                                         "No hay digitalizadores definidos");
    }

    static public string GetRecordingsBatchAnalystComboItems(ComboControlUseMode comboControlUseMode,
                                                             Contact defaultBookBatchRecorderUser) {
      ObjectList<Contact> contacts =
                RecorderOffice.MainRecorderOffice.GetContactsInRole<Contact>("MainRecorderOffice_RecordingsBatchAnalysts");
      contacts.Sort((x, y) => x.Alias.CompareTo(y.Alias));

      return GetContactsInRoleComboItems(contacts, defaultBookBatchRecorderUser, comboControlUseMode,
                                         "( Seleccionar al analista del libro )",
                                         "( Todos los analistas de libros )",
                                         "No hay analistas definidos");
    }

    static public void LoadPropertyTypesCombo(HtmlSelect comboControl, ComboControlUseMode comboControlUseMode,
                                              PropertyType defaultItem) {
      ObjectList<PropertyType> list = PropertyType.GetList();

      string header = comboControlUseMode == ComboControlUseMode.ObjectCreation
                                ? "( Seleccionar tipo de predio )" : "( Todos los tipos )";
      HtmlSelectContent.LoadCombo(comboControl, list, "Id", "Name", header, String.Empty, PropertyType.Unknown.Name);

      if (defaultItem != null && !defaultItem.IsEmptyInstance) {
        comboControl.Value = defaultItem.Id.ToString();
      }
    }

    static public void LoadRecordingActTypesCombo(RecordingActTypeCategory recordingActTypeCategory,
                                                  HtmlSelect comboControl, ComboControlUseMode comboControlUseMode,
                                                  RecordingActType defaultRecordingActType) {
      ObjectList<RecordingActType> recordingActTypeList = recordingActTypeCategory.GetItems();

      string header = (comboControlUseMode == ComboControlUseMode.ObjectCreation)
                              ? "( Primero seleccionar la categoría de la inscripción )" : "( Todos  los actos jurídicos )";

      HtmlSelectContent.LoadCombo(comboControl, recordingActTypeList, "Id", "DisplayName", header);
      if (defaultRecordingActType != null && defaultRecordingActType != RecordingActType.Empty) {
        comboControl.Value = defaultRecordingActType.Id.ToString();
      }
    }

    static public void LoadRecorderOfficeCombo(HtmlSelect comboControl, ComboControlUseMode comboControlUseMode,
                                               RecorderOffice defaultOffice) {
      ObjectList<RecorderOffice> officeList = RecorderOffice.GetList();


      string header = (comboControlUseMode == ComboControlUseMode.ObjectCreation)
                              ? "( Seleccionar un Distrito )" : "( Todos los Distritos )";

      HtmlSelectContent.LoadCombo(comboControl, officeList, "Id", "Alias", header);
      if (defaultOffice != null && !defaultOffice.IsEmptyInstance) {
        comboControl.Value = defaultOffice.Id.ToString();
      }
    }

    static public void LoadDomainRecordingSections(HtmlSelect comboControl, ComboControlUseMode comboControlUseMode,
                                                   string defaultValue = "") {
      ObjectList<KeyValuePair> list = KeyValuePair.GetList("LRSDomainTraslativeSection.Combo.List");

      string header = (comboControlUseMode == ComboControlUseMode.ObjectCreation)
                              ? "( Distrito / Sección )" : "( Todas los Distritos )";
      HtmlSelectContent.LoadCombo(comboControl, list, "Value", "Name", header);
      if (defaultValue != null) {
        comboControl.Value = defaultValue;
      }
    }

    static public void LoadRecorderOfficeMunicipalitiesCombo(HtmlSelect comboControl, ComboControlUseMode comboControlUseMode,
                                                             RecorderOffice recorderOffice, GeographicRegionItem defaultItem) {
      ObjectList<GeographicRegionItem> list = recorderOffice.GetMunicipalities();

      string header = (comboControlUseMode == ComboControlUseMode.ObjectCreation)
                              ? "( Seleccionar un municipio )" : "( Todos los municipios )";

      HtmlSelectContent.LoadCombo(comboControl, list, "Id", "Name", header);
      if (defaultItem != null && !defaultItem.IsEmptyInstance) {
        comboControl.Value = defaultItem.Id.ToString();
      }
    }

    static public void LoadRecorderOfficersCombo(HtmlSelect comboControl, ComboControlUseMode comboControlUseMode,
                                                 RecordingBook recordingBook, Contact defaultRecorderOfficer) {
      RecorderOffice office = recordingBook.RecorderOffice;
      ObjectList<Person> officers = office.GetRecorderOfficials(recordingBook.RecordingsControlTimePeriod);

      string header = (comboControlUseMode == ComboControlUseMode.ObjectCreation)
                        ? "( Seleccionar al C. Oficial Registrador )" : "( Todos los C. Oficiales Registradores )";

      HtmlSelectContent.LoadCombo(comboControl, officers, "Id", "FamilyFullName", header,
                                  "No se puede determinar o sólo aparece la firma", String.Empty);
      if (defaultRecorderOfficer != null && defaultRecorderOfficer != RecorderOffice.Empty) {
        comboControl.Value = defaultRecorderOfficer.Id.ToString();
      }
    }

    static public RecorderOffice ParseRecorderOffice(WebPage webPage, string controlUniqueID) {
      string selectedValue = webPage.GetControlState(controlUniqueID);
      if (!String.IsNullOrEmpty(selectedValue)) {
        return RecorderOffice.Parse(int.Parse(selectedValue));
      } else {
        return RecorderOffice.Empty;
      }
    }

    static public RecordingActTypeCategory ParseRecordingActTypeCategory(WebPage webPage,
                                                                         string controlUniqueID) {
      string selectedValue = webPage.GetControlState(controlUniqueID);
      if (!String.IsNullOrEmpty(selectedValue)) {
        return RecordingActTypeCategory.Parse(int.Parse(selectedValue));
      } else {
        return RecordingActTypeCategory.Empty;
      }
    }

    #endregion Public methods

    #region Private methods

    static private string GetContactsInRoleComboItems(ObjectList<Contact> contacts,
                                                      Contact selectedContact,
                                                      ComboControlUseMode comboControlUseMode,
                                                      string objectCreationFirstItem,
                                                      string objectSearchFirstItem,
                                                      string noItemsFirstItem) {
      if (contacts.Count == 0) {
        return HtmlSelectContent.GetComboHtmlItem(String.Empty, noItemsFirstItem);
      }

      string xhtml = String.Empty;
      if (contacts.Count != 1) {
        if (comboControlUseMode == ComboControlUseMode.ObjectCreation) {
          xhtml = HtmlSelectContent.GetComboHtmlItem(String.Empty, objectCreationFirstItem);
        } else {
          xhtml = HtmlSelectContent.GetComboHtmlItem(String.Empty, objectSearchFirstItem);
        }
      }
      xhtml += HtmlSelectContent.GetComboHtml(contacts, "Id", "Alias", String.Empty);
      if (!selectedContact.IsEmptyInstance) {
        xhtml.Replace("value='" + selectedContact.Id.ToString() + "'>", "value='" + selectedContact.Id.ToString() + " selected'>");
      }
      return xhtml;
    }

    #endregion Private methods

  } // class LRSHtmlSelectControls

} // namespace Empiria.Land.UI
