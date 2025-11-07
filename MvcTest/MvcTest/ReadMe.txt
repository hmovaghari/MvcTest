dotnet add package Microsoft.EntityFrameworkCore --version 8.*
dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 8.*
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.*

dotnet tool install --global dotnet-ef --version 8.*
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.*

dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet ef migrations script --output Script.sql