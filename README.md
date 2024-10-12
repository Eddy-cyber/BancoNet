#Dependencias necesarias
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Pomelo.EntityFrameworkCore.MySql
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Swashbuckle.AspNetCore
dotnet add package Microsoft.AspNetCore.Cors
dotnet add package Microsoft.AspNetCore.Mvc.NewtonsoftJson
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design
dotnet tool install -g dotnet-aspnet-codegenerator



#Agregar nuevos controladores
dotnet aspnet-codegenerator controller
 -name ClientesController -async -api -m Clientes -dc AppDbContext --relativeFolderPath Controllers

#Para exportar a excell
dotnet add package EPPlus