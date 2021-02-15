/* Empiria ***************************************************************************************************
*																																																						 *
*	 Solution  : Empiria Web																			System   : Javascript Core Library					 *
*	 File      : /general.js																			Pattern  : JavaScript Methods Library				 *
*  Version   : 3.0                                              License  : Please read license.txt file      *
*																																																						 *
*  Summary   : Contains general methods for web browser javascript clients.																	 *
*																																																						 *
********************************** Copyright(c) 1994-2017. La Vía Óntica SC, Ontica LLC and contributors.  **/

var gbSended = false;
var gsWaitScreenMessage = "";

// region Public methods

function doCommand(commandName) {
  if (gbSended) {
    return;
  }
  var catched = false;

  gbSended = true;
  if (doPageCommand(commandName, arguments)) {
    catched = true;
  } else if (doNavigationCommand(commandName, arguments)) {
    catched = true;
  } else if (doMasterPageCommand(commandName, arguments)) {
    catched = true;
  } else {
    var message = "No puedo procesar la instrucción '" + commandName + "' debido a que ";
    message += "su manejador todavía no ha sido definido en el programa.";
    showExceptionMsg(message);
    gbSended = false;
  }
  if (gbSended && catched) {
    showWaitScreen();
    document.forms[0].submit();
  }
}

function addEvent(element, eventType, functionName) {
     if (element.addEventListener) {
       removeEvent(element, eventType, functionName);
       element.addEventListener(eventType, functionName, false);
       return true;
     } else if (element.attachEvent) {
       removeEvent(element, eventType, functionName);
       return element.attachEvent("on" + eventType, functionName);
     } else {
       return false;
     }
}

function removeEvent(element, eventType, functionName) {
   if (element.removeEventListener) {
     try {
       element.removeEventListener(eventType, functionName, false);
     } finally {
       return true;
     }
   } else if (element.detachEvent) {
     return element.detachEvent("on" + eventType, functionName);
   } else {
     return false;
   }
}

function cancelEvent(oEvent) {
  if (window.event != undefined) {
    oEvent.returnValue = false;
  } else {
    oEvent.preventDefault();
  }
}

function existsElement(elementId) {
  var element = document.getElementById(elementId);
  if (element == undefined || element == null) {
    return false;
  } else {
    return true;
  }
}

function getElement(elementId) {
  //return document.getElementById(elementId);
  var element = document.getElementById(elementId);
  if (element != undefined && element != null) {
    return element;
  } else {
    alert("The element with Id '" + elementId + "' is not defined in the document.");
    return null;
  }
}

function getElements(elementName) {
  return document.getElementsByName(elementName);
}

function getEvent(oEvent) {
  return (window.event ? window.event : oEvent);
}

function getEventSource(oEvent) {
  if (oEvent.srcElement != null) { // Internet Explorer
    return oEvent.srcElement;
  } else {												 // Firefox
    return oEvent.target;
  }
}

function getInnerText(elementId) {
  var element = getElement(elementId);

  if (element.innerText != undefined) {
    return element.innerText;
  } else {
    return element.textContent;
  }
}

function setFocus(oControl) {
  if (oControl == undefined || oControl == null) {
    return;
  }
  try {
    oControl.focus();
  } catch (e) {

  }
}

function grayImage(e) {
  var oEvent = getEvent(e);
  var sourceElement = getEventSource(oEvent);	
  var imageSource = sourceElement.src;
  var imageDirectory = imageSource.substr(0, imageSource.lastIndexOf('/'));
  var imageFileName = imageSource.substr(imageSource.lastIndexOf('/') + 1);	
  var regEx = /grayed./g;

  sourceElement.src = imageDirectory + "/" + "grayed." + imageFileName.replace(regEx, "");
  resetStatus();
}

function copyToClipboard(text) {
  var textArea = document.createElement("textarea");
  textArea.value = text;

  document.body.appendChild(textArea);

  textArea.select();

  try {
    document.execCommand("copy");
  } catch(e) {
    //  no-op
  }
  document.body.removeChild(textArea);
}

function hasValue(controlName, exceptionMsg) {
  var control = getElement(controlName);
  if (control.value == "") {
    showExceptionMsg(exceptionMsg);
    control.focus();
    return false;
  }
  return true;
}

function parseCommandArguments(commandArguments) {
  var args = "";

  for (var i = 1; i < commandArguments.length; i++) {
    args += ((args == "") ? "" : "|") + commandArguments[i];
  }
  return args;
}

function resetStatus() {
  window.status = window.defaultStatus;
  return true;
}

function setInnerText(element, contents) {
  if (element.innerText) {						// Internet Explorer
    element.innerText = contents;
  } else if (element.textContent) {		// Firefox
    element.textContent = contents;
  }
}

function setStatus(e) {
  var oEvent = getEvent(e);
  var sourceElement = getEventSource(oEvent);

  window.status = sourceElement.title != "" ? sourceElement.title : "No existe ayuda para este elemento";
  cancelEvent(oEvent);
  return true;
}

function showNotAllowedMessage() {
  alert("Lo siento, el usuario registrado no cuenta con los permisos\nnecesarios para efectuar esta operación.");
}

function showExceptionMsg(message) {
  alert(message);
}

function showMsg(message) {
  alert(message);
}

function showMessage(title, message) {
  var temp = title + "\n\n";
  temp += message;

  alert(temp);
}

function selectAllCheckboxes(checkBoxName) {
  var oCheckboxes = getElements(checkBoxName);
  if (oCheckboxes == undefined || oCheckboxes == null) {
    return;
  }
  var sourceElement = getEventSource(window.event);

  for (var i = 0; i < oCheckboxes.length; i++) {
    oCheckboxes[i].checked = sourceElement.checked;
  }
  return;
}

function getAllSelectedCheckboxesValues(checkboxName) {
  var oCheckboxes = getElements(checkboxName)
  if (oCheckboxes == undefined || oCheckboxes == null) {
    return "";
  }
  var elements = "";
  for (var i = 0; i < oCheckboxes.length; i++) {
    if (oCheckboxes[i].checked) {
      elements += ((elements.length == 0) ? "" : "|") + oCheckboxes[i].value;
    }
  }
  return elements;
}

function disableControls(oContainer, disabledFlag) {
  disableControlsByTag(oContainer, 'input', disabledFlag);
  disableControlsByTag(oContainer, 'textarea', disabledFlag);
  disableControlsByTag(oContainer, 'select', disabledFlag);
}

function disableControlsByTag(oContainer, tagName, disabledFlag) {
  var items = oContainer.getElementsByTagName(tagName);
  for (var i = 0; i < items.length; i++) {
    if (tagName == "input" && items[i].type == "text") {
        items[i].readOnly = disabledFlag;
    } else {
        items[i].disabled = disabledFlag;
    }
  }
}

function ungrayImage(e) {
  var oEvent = getEvent(e);
  var sourceElement  = getEventSource(oEvent);
  var imageSource    = sourceElement.src;
  var imageDirectory = imageSource.substr(0, imageSource.lastIndexOf('/'));
  var imageFileName  = imageSource.substr(imageSource.lastIndexOf('/') + 1);
  var regEx = /grayed./g;

  sourceElement.src = imageDirectory + "/" + imageFileName.replace(regEx, "");
  setStatus(e);
}

  var gLastWindowHeight = Number(0);

  function windowHeightChanged() {
    return (window.document.documentElement.clientHeight != gLastWindowHeight);
  }

  function markWindowHeightFixed() {
    gLastWindowHeight = document.documentElement.clientHeight;
  }

  function fixDataTableItems(oDivDataTable) {
    try {
      var oTableHeader = oDivDataTable.getElementsByTagName('tr')[0];
      oTableHeader.style.top = oDivDataTable.scrollTop - 1 + "px";
    } catch (e) {
      // no-op
    }
  }

  function resizeMultiViewDashboard() {
    try {
      var windowHeight = window.document.body.clientHeight;
      var menuHeight = getElement('tblDashboardMenu').clientHeight;
      var pageOptionsHeight = getElement('tblDashboardOptions').clientHeight;
      var dataTableControlsHeight = 0;
      if (existsElement('grdNavBarItemsCount')) {
        dataTableControlsHeight = 30;
      } else {
        dataTableControlsHeight = 0;
      }
      var newHeight = windowHeight - menuHeight - pageOptionsHeight - dataTableControlsHeight;

      getElement('divObjectExplorer').style.height = newHeight + "px";
      fixDataTableItems(getElement('divObjectExplorer'));
    } catch (e) {
      // no-op;
    } // try
  }

  function dataGridRowSelect(selectedRow, select) {
    if (select) {
      selectedRow.abbr = selectedRow.className;
      selectedRow.className = 'selectedDataRow';
    } else {
      selectedRow.className = selectedRow.abbr;
      selectedRow.abbr = '';
    }
  }

  function sendPageCommand(pageCommand) {
    if (gbSended) {
      alert("Favor de intentar la operación deseada más tarde ya que\nexiste otro proceso en ejecución que primero debe concluir.");
      return;
    }
    getElement("hdnEmpiriaPageCommandName").value = pageCommand;
    if (arguments.length == 2) {
      getElement("hdnEmpiriaPageCommandArguments").value = arguments[1];
    } else {
      getElement("hdnEmpiriaPageCommandArguments").value = '';
    }
    document.body.style.cursor = 'wait';
    gbSended = true;
    document.forms[0].submit();
  }

// endregion Public methods

// region Private command handler methods

function doNavigationCommand(commandName, commandArguments) {
  switch (commandName) {
    case 'loadViewCmd':
      gsWaitScreenMessage = "Recuperando la página solicitada";
      loadViewCommandHandler(commandName, commandArguments[1], commandArguments[2]);
      gbSended = false;
      return true;
    case 'createViewCmd':
      createViewCommandHandler(commandName, commandArguments[1], commandArguments[2]);
      gbSended = false;
      return true;
    case 'showContextualHelp':
      showContextualHelpCommandHandler();
      gbSended = false;
      return true;
    case 'uploadFile':
      showFileUploaderCommandHandler();
      gbSended = false;
      return true;		
    case 'createReportViewCmd':
      createReportViewCommandHandler(commandArguments[1]);
      gbSended = false;
      return true;
    case 'createFileDownloadCommandHandler':
      createFileDownloadCommandHandler(commandArguments[1]);
      gbSended = false;
      return true;
    case 'refreshViewCmd':
      gsWaitScreenMessage = "Actualizando el contenido de la página";
      workplaceCommandHandler(commandName);
      gbSended = false;
      return true;
    case 'backHistoryCmd':
      gsWaitScreenMessage = "Recuperando la página anterior";
      workplaceCommandHandler(commandName);
      gbSended = false;
      return true;
    case 'forwardHistoryCmd':
      gsWaitScreenMessage = "Recuperando la página siguiente";
      workplaceCommandHandler(commandName);
      gbSended = false;
      return true;
    case 'gotoFirstPageCmd':
      gsWaitScreenMessage = "Recuperando la información de la primera página";
      return true;
    case 'gotoPreviousPageCmd':
      gsWaitScreenMessage = "Recuperando la información de la página anterior";
      return true;		
    case 'gotoNextPageCmd':
      gsWaitScreenMessage = "Recuperando la información de la siguiente página";
      return true;
    case 'gotoLastPageCmd':
      gsWaitScreenMessage = "Recuperando la información de la última página";
      return true;
    case 'gotoPageCmd':
      gsWaitScreenMessage = "Recuperando la información de la página " + commandArguments[1];
      return true;
    case 'onClickTabStripCmd':
      gbSended = onClickTabStripCommandHandler(commandArguments[1], commandArguments[2]);
      return true;
    case 'onMouseOverTabStripCmd':
      gbSended = onMouseOverTabStripCommandHandler(commandArguments[1]);
      return true;		
    case 'onMouseOutTabStripCmd':
      gbSended = onMouseOutTabStripCommandHandler(commandArguments[1]);
      return true;
    case 'onClickMenuCmd':
      gbSended = onClickMenuCommandHandler(commandArguments[1]);
      return true;		
    default:
      return false;
  }
}

function onClickMenuCommandHandler(tabStrip) {
  if (tabStrip.className == 'tabDisabled') {
    return false;
  }
  var tabPrefix = tabStrip.id.substr(0, tabStrip.id.length - 1);
  var tabIndex = new Number(tabStrip.id.substr(tabStrip.id.length - 1));

  for (var i = 0; i < 10; i++) {
    var tabStripName = tabPrefix + i.toString();
    if (existsElement(tabStripName)) {
      if (getElement(tabStripName).className != 'tabDisabled') {
        getElement(tabStripName).className = "tabOff";
      }
    } else {
      break;
    }
    var tabStripItemViewName = 'mnu_' + i.toString();
    if (existsElement(tabStripItemViewName)) {
      getElement(tabStripItemViewName).style.display = 'none';
    }
  }
  tabStrip.className = "tabOn";
  if (existsElement('mnu_' + tabIndex.toString())) {
    getElement('mnu_' + tabIndex.toString()).style.display = 'inline';
  }
  return false;
}
function showTabStrip(tabIndex) {
  var tabPrefix = "tabStrip_";
  var tabStrip = getElement(tabPrefix + tabIndex);

  getElement('currentTabStrip').value = tabIndex;

  for (var i = 0; i < 10; i++) {
    var tabStripName = tabPrefix + i.toString();
    if (existsElement(tabStripName)) {
      if (getElement(tabStripName).className != 'tabDisabled') {
        getElement(tabStripName).className = "tabOff";
      }
    } else {
      break;
    }
    var tabStripItemViewName = 'tabStripItemView_' + i.toString();
    if (existsElement(tabStripItemViewName)) {
      getElement(tabStripItemViewName).style.display = 'none';
    }
  }
  tabStrip.className = "tabOn";
  if (existsElement('tabStripItemView_' + tabIndex.toString())) {
    getElement('tabStripItemView_' + tabIndex.toString()).style.display = 'inline';
  }
}

function showTabStripItem(tabIndex) {
  var tabPrefix = "tabStripItem_";
  var tabStrip = getElement(tabPrefix + tabIndex);

  getElement('currentTabStripItem').value = tabIndex;
  for (var i = 0; i < 10; i++) {
    var tabStripName = tabPrefix + i.toString();
    if (existsElement(tabStripName)) {
      if (getElement(tabStripName).className != 'tabDisabled') {
        getElement(tabStripName).className = "tabOff";
      }
    } else {
      break;
    }
    var tabStripItemViewName = 'tabStripItemView_' + i.toString();
    if (existsElement(tabStripItemViewName)) {
      getElement(tabStripItemViewName).style.display = 'none';
    }
  }
  tabStrip.className = "tabOn";
  if (existsElement('tabStripItemView_' + tabIndex.toString())) {
    getElement('tabStripItemView_' + tabIndex.toString()).style.display = 'inline';
  }
  setPageTitleWithTabStrip(tabStrip);
}

function setPageTitleWithTabStrip(oTabStrip) {
  var pageTitle = '';

  if (oTabStrip.abbr.length != 0) {
    pageTitle = oTabStrip.abbr;
  } else {
    pageTitle = oTabStrip.innerText;
  }
  var oMainPage = window.parent;
  var script = "setPageTitle('" + pageTitle + "');";
  oMainPage.execScript(script);
}

function onClickTabStripCommandHandler(tabStrip, processOnServer) {
  if (tabStrip.className == 'tabDisabled') {
    return false;
  }
  var tabPrefix = tabStrip.id.substr(0, tabStrip.id.length - 1);
  var tabIndex = new Number(tabStrip.id.substr(tabStrip.id.length - 1));
  if (processOnServer != undefined && processOnServer != null) {
    getElement('currentTabStripItem').value = tabIndex;
    return true;
  }

  for (var i = 0; i < 10; i++) {
    var tabStripName = tabPrefix + i.toString();
    if (existsElement(tabStripName)) {
      if (getElement(tabStripName).className != 'tabDisabled') {
        getElement(tabStripName).className = "tabOff";
      }
    } else {
      break;
    }
    var tabStripItemViewName = 'tabStripItemView_' + i.toString();
    if (existsElement(tabStripItemViewName)) {
      getElement(tabStripItemViewName).style.display = 'none';
    }
  }
  tabStrip.className = "tabOn";
  if (existsElement('tabStripItemView_' + tabIndex.toString())) {
    getElement('tabStripItemView_' + tabIndex.toString()).style.display = 'inline';
  }
  return false;
}

function onMouseOverTabStripCommandHandler(tabStrip) {
  if (tabStrip.className != 'tabOff') {
    return false;
  }
  tabStrip.className = "tabHover";
}

function onMouseOutTabStripCommandHandler(tabStrip) {
  if (tabStrip.className != 'tabHover') {
    return false;
  }
  tabStrip.className = "tabOff";
}

function openInWindow(oWindow, url, fullWindow) {
  var options = "";

  if (fullWindow != null && fullWindow === true) {
    options = "status=yes,scrollbars=yes,fullscreen=yes,location=no,menubar=no,resizable=yes";
  } else {
    options = "status=yes,scrollbars=yes,fullscreen=no,location=no,menubar=no,resizable=yes," +
              "height=780px,width=900px";
  }


  if (oWindow == null || oWindow.closed) {
    oWindow = window.open(url, "_blank", options);
  } else {
    oWindow.focus();
    oWindow.document.location = url;
    //oWindow.navigate(url);
  }
  return oWindow;
}

function closeWindow(oWindow) {
  if (oWindow == null || oWindow.closed) {
    return;
  }
  oWindow.close();
}

function createNewWindow(url) {
  var oViewer = null;
  var openExclusive = false;
  //var options = getWindowFeatures(viewName);

  var options = "status=yes,scrollbars=yes,fullscreen=no,location=no,menubar=no,resizable=yes,height=800px,width=1020px";

//  if ((viewParameters != null) && (viewParameters != "")) {
//    commandPage += "&" + viewParameters;
//  }
//	if (openExclusive) {
//	  oViewer = getExclusiveWindow(pageName);
//  }
  if (oViewer == null || oViewer.closed) {
    oViewer = window.open(url, "_blank", options);
  } else {
    oViewer.focus();
    oViewer.navigate(url);
  }
}

function createViewCommandHandler(commandName, viewName, viewParameters) {
  var oViewer = null;
  var openExclusive = false;
  var options = getWindowFeatures(viewName);

  var commandPage = "../workplace/command.processor.aspx";
  commandPage += "?commandName=" + commandName;
  commandPage += "&viewName=" + viewName;
  if ((viewParameters != null) && (viewParameters != "")) {
    commandPage += "&" + viewParameters;
  }
  if (openExclusive) {
    oViewer = getExclusiveWindow(pageName);
  }
  if (oViewer == null || oViewer.closed) {
    oViewer = window.open(commandPage, "_blank", options);
  } else {
    oViewer.focus();
    oViewer.navigate(commandPage);
  }
}

function showFileUploaderCommandHandler() {
  var display = getElement('divFileUploader').style.display;	

  if (display == "none") {
    getElement('divFileUploader').style.display = "inline";
    getElement('btnUploadFile').value = "Cancelar la operación";
  } else {
    getElement('divFileUploader').style.display = "none";
    getElement('btnUploadFile').value = "Agregar documento ...";
  }
}

function showContextualHelpCommandHandler() {
  var display = getElement('divRightMenu').style.display;

  if (display == "none") {
    getElement('divRightMenu').style.display = "inline";
    getElement('divContextualHelp').style.display = "inline";
    getElement('divDocuments').style.display = "none";
  } else {
    getElement('divRightMenu').style.display = "none";
    getElement('divContextualHelp').style.display = "none";
    getElement('divDocuments').style.display = "none";
  }
}

function doWorkItemCommandHandler(commandName, workItemId) {
  var oViewer = null;
  var openExclusive = false;
  var options = getWindowFeatures(commandName);

  var commandPage = "../workflow/do.workitem.aspx";
  commandPage += "?workItemId=" + workItemId;

  if (openExclusive) {
    oViewer = getExclusiveWindow(pageName);
  }
  if (oViewer == null || oViewer.closed) {
    oViewer = window.open(commandPage, "_blank", options);
  } else {
    oViewer.focus();
    oViewer.navigate(commandPage);
  }
}

function createReportViewCommandHandler(reportName) {
  var oViewer = null;
  var openExclusive = false;
  var options = getReportWindowFeatures(reportName);

  var url = reportName;  // TODO: change this to ReportName later
  if (openExclusive) {
    oViewer = getExclusiveWindow(reportName);
  }
  if (oViewer == null || oViewer.closed) {
    oViewer = window.open(url, "_blank", options);
  } else {
    oViewer.focus();
    oViewer.navigate(url);
  }
}

function createFileDownloadCommandHandler(url) {
  var oViewer = null;
  var openExclusive = false;
  var options = "status=yes,scrollbars=no,fullscreen=no,location=no,menubar=no,resizable=no,";
  options += "height=220px,width=420px";

  if (openExclusive) {
    oViewer = getExclusiveWindow(url);
  }
  if (oViewer == null || oViewer.closed) {
    oViewer = window.open(url, "_blank", options);
  } else {
    oViewer.focus();
    oViewer.navigate(url);
  }
}

function loadViewCommandHandler(commandName, viewName, viewParameters) {
  var commandPage = "../workplace/command.processor.aspx";

  commandPage += "?commandName=" + commandName;
  commandPage += "&viewName=" + viewName;
  if ((viewParameters != null) && (viewParameters != "")) {
    commandPage += "&" + viewParameters;
  }
  if (getElement('hdnEmpiriaWorkplace').value != "") {
    commandPage += "&workplace=" + getElement('hdnEmpiriaWorkplace').value;
  }
  window.location.replace(commandPage);
}

function workplaceCommandHandler(commandName) {
  var commandPage = "../workplace/command.processor.aspx";

  commandPage += "?commandName=" + commandName;
  if (getElement('hdnEmpiriaWorkplace').value != "") {
    commandPage += "&workplace=" + getElement('hdnEmpiriaWorkplace').value;
  }
  window.location.replace(commandPage);
}

function getExclusiveWindow(url) {
  return null;
}

function getWindowFeatures(viewName) {
   var temp = "status=yes,scrollbars=yes,fullscreen=no,location=no,menubar=no,resizable=yes,";
      temp += "height=520px,width=720px";

//  var url = "../ajax/workplace.data.aspx";
//  url += "?commandName=windowFeaturesCmd";
//  url += "&viewName=" + viewName;
//
//
// return invokeAjaxMethod(false, url, null);

 return temp;
}

function getReportWindowFeatures(reportName) {
   var temp = "status=yes,scrollbars=yes,fullscreen=no,location=no,menubar=yes,resizable=yes,";
      temp += "height=620px,width=920px";

//  var url = "../ajax/workplace.data.aspx";
//  url += "?commandName=windowFeaturesCmd";
//  url += "&viewName=" + viewName;
//
//
// return invokeAjaxMethod(false, url, null);

 return temp;
}

// endregion Private command handler methods
