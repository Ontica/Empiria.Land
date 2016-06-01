# About Empiria Land

[Empiria Land](http://www.ontica.org/land/) is a suite of powerful solutions that provide information services
for land recording offices, cadastral departments and other land management government organizations.

This repository corresponds to the system's backend just for the Land Recording System.

The project is based on C# 6.0 and .NET Framework 4.6 and the web API is tailored on ASP .NET Web API 2.1.

As other Empiria products, this backend runs over [Empiria Framework](https://github.com/Ontica/Empiria.Core)
components and, as usual, needs some of the [Empiria Extensions](https://github.com/Ontica/Empiria.Extensions).

The project can be compiled using Visual Studio 2015 starting with the Community Edition.


# Contents

Empiria Land includes the following software components:


1. **Registration Services**  
   Classes and other types used for document and recording acts registration.

2. **Certification Services**  
   Contains types to manage and emit official certificates related to land ownership.

3. **Transaction Services**  
   Services to manage customer's front-desk and external system transactions.

4. **Documentation Services**  
   Code to control imaging processing for recording books and scanned documents.

5. **User Interface Services**  
   Provides Html parsing services especially tailored to display and reuse Empiria Land user interfaces.

6. **Web API (HTTP/REST-based)**  
   Http/Json RESTful type web services used to communicate with third party systems, to emit certificates and perform data searching operations.

7. **XML Web Services (to be deprecated)**  
   Old fashioned XML/SOAP web services that are still in use by some external systems (government treasury).

# Documentation

Folder [**docs**](https://github.com/Ontica/Empiria.Land/tree/master/docs) contains a web site with the full code documentation. It can be downloaded and installed in the web server of your preference.

There, **database.scripts.sql** file contains the full database script for SQL Server 2014, and it includes the full set of tables, views, functions and stored procedures.

**database.structure.pdf** contains a general view map of the database.

**components.pdf** file presents a general view map of the system.

# License

This system is distributed by the GNU AFFERO GENERAL PUBLIC LICENSE.

Óntica always delivers **open source** information systems, and we consider that it is specially
important in the case of public utility or government systems.

# Version

Most updated version of this product that can run in production is Empiria Land 3.0 Beta 1.

# Copyright

Copyright © 2009-2016. La Vía Óntica SC, Ontica LLC and colaborators.
