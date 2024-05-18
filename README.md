# Empiria Land Backend

[![Codacy Badge](https://app.codacy.com/project/badge/Grade/b6e08c24856f4b22a347d2e2b423e70d)](https://www.codacy.com/gh/Ontica/Empiria.Land/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=Ontica/Empiria.Land&amp;utm_campaign=Badge_Grade)
&nbsp; &nbsp;
[![Maintainability](https://api.codeclimate.com/v1/badges/885eb979ff25548307c4/maintainability)](https://codeclimate.com/github/Ontica/Empiria.Land/maintainability)

[Empiria Land](http://www.ontica.org/land/) es una suite de soluciones de software, altamente segura, para la administración
y modernización de oficinas del registro público de la propiedad o de otras organizaciones gubernamentales encargadas de
la administración y control de la propiedad territorial.

Este repositorio contiene los módulos de propósito general para la capa de *backend* para *Sistemas del Registro Público de la Propiedad*.

Todos los módulos están escritos en C# 7.0 y utilizan .NET Framework versión 4.8.
Los módulos pueden ser compilados utilizando [Visual Studio 2022 Community Edition](https://visualstudio.microsoft.com/vs/community/).

El acceso a los servicios que ofrece el *backend* se realiza mediante llamadas a servicios web de tipo RESTful,
mismos que están basados en ASP .NET.

Al igual que otros productos Empiria, este *backend* se apoya en [Empiria Framework](https://github.com/Ontica/Empiria.Core),
y también en algunos módulos de [Empiria Extensions](https://github.com/Ontica/Empiria.Extensions).

## Contenido

El *backend* de Empiria Land se conforma de los siguientes componentes de software:

1.  **Core**  
    Casos de uso, servicios y tipos generales para el registro de instrumentos jurídicos y sus actos registrales.

2.  **Certification**  
    Genera certificados sobre predios, propietarios, actos jurídicos, de inscripción de instrumentos o de personas morales.

2.  **Digitalization**  
    Componentes para administrar la digitalización de libros, escrituras, oficios y otros documentos que se reciben en papel.

3.  **ESign**  
    Proporciona servicios de firma electrónica para el firmado seguro e inalterable de sellos registrales y certificados.

4.  **Integration**  
    Contiene interfaces para la integración del Sistema con otros proveedores de servicios o información, como es el caso
    de la integración con sistemas de catastro y sistemas de tesorería o cajas.

5.  **Pages**  
    Genera de forma impresa sellos registrales y certificados, así como comprobantes de pago y de recepción de trámites.

6.  **Providers**  
    Capa separada de proveedores de servicios utilizados por el Sistema.

7.  **Search Services**  
    Servicios de búsqueda de información sobre predios, inscripciones y personas físicas y morales.

8.  **Tests**  
    Pruebas de integración de los módulos del sistema.

9.  **Transactions**  
    Casos de uso y reglas para el manejo del flujo de trabajo en oficinas del registro público de la propiedad y catastros.

10. **Web API (HTTP/REST-based)**  
    Capa de servicios web de tipo Http/Json RESTful type que se emplean para comunicar otras aplicaciones con este *backend*.


## Licencia

Este producto y sus partes se distribuyen mediante una licencia GNU AFFERO
GENERAL PUBLIC LICENSE, para uso exclusivo de los Gobiernos de los Estados
de Tlaxcala y Zacatecas y de su personal, y también para su uso por cualquier
otro organismo en México perteneciente a la Administración Pública Federal o
a las administraciones estatales o municipales.

Para cualquier otro uso (con excepción a lo estipulado en los Términos de
Servicio de GitHub), es indispensable obtener con nuestra organización una
licencia distinta a esta.

Lo anterior restringe la distribución, copia, modificación, almacenamiento,
instalación, compilación o cualquier otro uso del producto o de sus partes,
a terceros, empresas privadas o a su personal, sean o no proveedores de
servicios de las entidades públicas mencionadas.

El desarrollo, evolución y mantenimiento de este producto está siendo pagado
en su totalidad con recursos públicos, y está protegido por las leyes nacionales
e internacionales de derechos de autor.

## Copyright

Copyright © 2009-2024. La Vía Óntica SC, Ontica LLC y autores.
Todos los derechos reservados.
