# Empiria Land

[![Codacy Badge](https://app.codacy.com/project/badge/Grade/b6e08c24856f4b22a347d2e2b423e70d)](https://www.codacy.com/gh/Ontica/Empiria.Land/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=Ontica/Empiria.Land&amp;utm_campaign=Badge_Grade)
&nbsp; &nbsp;
[![Maintainability](https://api.codeclimate.com/v1/badges/885eb979ff25548307c4/maintainability)](https://codeclimate.com/github/Ontica/Empiria.Land/maintainability)

[Empiria Land](http://www.ontica.org/land/) is a suite of powerful solutions that provide information services
for land recording offices, cadastral departments and other land management governmental organizations.

This repository corresponds to the system's backend for the Land Recording System.

The project is based on C# 7.0 and .NET Framework 4.8, and the web API is tailored on ASP .NET Web Api.

As other Empiria products, this backend runs over [Empiria Framework](https://github.com/Ontica/Empiria.Core)
components and, as usual, needs some of the [Empiria Extensions](https://github.com/Ontica/Empiria.Extensions).

The project can be compiled using Visual Studio 2022 Community Edition.


## Contents

Empiria Land includes the following software components:

1.  **Core Services**  
    Classes and other types used for instruments and recording acts registration.

2.  **Registration Services**  
    Classes and other types used for document and recording acts registration.

3.  **Certificate Issuing Services**  
    Services used to manage and emit official certificates related to land ownership.

4.  **Transaction Services**  
    Services to manage customer's front-desk and external system transactions.

5.  **Form Filing Services**  
    Services to validate, manage and process data forms realted with procedures in Land recording offices.

6.  **Documentation Services**  
    Code to control imaging processing for recording books and scanned documents.

7.  **User Interface Services**  
    Provides Html parsing services especially tailored to display and reuse Empiria Land user interfaces.

8.  **Pages**  
    Server generated forms, slips, documents and reports.

9.  **Providers**  
    Separated providers implementation layer used by Empiria Land components.

10.  **Web API (HTTP/REST-based)**  
    Http/Json RESTful type web services used to communicate with third party systems, to emit certificates and perform data searching operations.

11. **Tests**
    Unit and integration test cases for Empiria Land.

## Documentation

Folder [**docs**](https://github.com/Ontica/Empiria.Land/tree/master/docs) contains a web site with the full code documentation. It can be downloaded and installed in the web server of your preference.

There, `database.scripts.sql` file contains the full database script for SQL Server DBMS, and it includes the full set of tables, views, functions and stored procedures. The file `database.structure.pdf` contains a general view map of the database, and `components.pdf` presents a general view map of the system.

## Licencia

Este producto y sus partes se distribuyen mediante una licencia GNU AFFERO
GENERAL PUBLIC LICENSE, para uso exclusivo de los Gobiernos de los Estados
de Tlaxcala y Zacatecas y de su personal, y para su uso por cualquier otro
organismo en México perteneciente  a la Administración Pública Federal o a
las administraciones estatales o municipales.

Para cualquier otro uso (con excepción  a lo estipulado en los Términos de
Servicio de GitHub), es indispensable obtener con nuestra organización una
licencia distinta a esta.

Lo anterior restringe la distribución, copia, modificación, almacenamiento,
instalación, compilación o cualquier otro uso del producto o de sus partes,
a terceros, empresas privadas o a su personal, sean o no proveedores de
servicios de las entidades públicas mencionadas.

El desarrollo de este producto fue pagado en su totalidad con recursos
públicos estatales y federales, y está protegido por las leyes nacionales
e internacionales de derechos de autor.

## Copyright

Copyright © 2009-2022. La Vía Óntica SC, Ontica LLC y autores.
Todos los derechos reservados.
