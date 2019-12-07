# Imagegram
- The visual studio code structure is made using Domain Driven Design
- Documentation is generated using,
  1. Swagger: For Visual Studio
  2. Redoc: For end user

# Tools used!
  - .NET Core and SQL Server 2012
  - Dapper ORM
  - Fluent Validation for parameter validations
  - HTML Sanitizer
  - SixLabors/ImageSharp for Image cropping and conversion

# Assumptions:
- UUID is not validated against DB.
- UUID is generated using GUID.

# How to run:
- Default URL running using Visual Studio is Swagger endpoint.
- **Database scripts** available under DatabaseScript folder.
- OpenAPI specification available under Specifications folder.
- **Documentation** is available uner Documentation folder.
  This is generated using **Redoc**


# To Dos:
- Moving image format vaidation to Fulent (to write a custom validation)
  Currently, this is implemented as normal validation outside of Fulent.
- Get all posts endpoint API pagination support.
