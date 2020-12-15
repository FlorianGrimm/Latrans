# Latrans
Experimental WebApp

## Projects
- Brimborium.Latrans.Contracts
- Brimborium.Latrans.Medaitor
- DemoWebApp
- DemoWebApp.Contracts

#setup

```
dotnet new classlib -n Brimborium.Latrans.Medaitor -o src\Brimborium.Latrans.Medaitor
dotnet sln add src\Brimborium.Latrans.Medaitor

dotnet new xunit -n Brimborium.Latrans.Medaitor.Test -o src\Brimborium.Latrans.Medaitor.Test
dotnet sln add src\Brimborium.Latrans.Medaitor.Test

dotnet new classlib -n Brimborium.Latrans.Medaitor.AspNetCore -o src\Brimborium.Latrans.Medaitor.AspNetCore
dotnet sln add src\Brimborium.Latrans.Medaitor.AspNetCore

dotnet new xunit -n Brimborium.Latrans.Medaitor.AspNetCore.Test -o src\Brimborium.Latrans.Medaitor.AspNetCore.Test
dotnet sln add src\Brimborium.Latrans.Medaitor.AspNetCore.Test

dotnet new classlib -n Brimborium.Latrans.Contracts -o src\Brimborium.Latrans.Contracts
dotnet sln add src\Brimborium.Latrans.Contracts

dotnet new classlib -n DemoWebApp.Contracts -o src\DemoWebApp.Contracts
dotnet sln add src\DemoWebApp.Contracts

dotnet new classlib -n DemoWebApp.Logic -o src\DemoWebApp.Logic
dotnet sln add src\DemoWebApp.Logic

dotnet new xunit -n DemoWebApp.Logic.Test -o src\DemoWebApp.Logic.Test
dotnet sln add src\DemoWebApp.Logic.Test

dotnet new webapp -n DemoWebApp -o src\DemoWebApp
dotnet sln add src\DemoWebApp

dotnet new tool-manifest
dotnet tool install dotnet-stryker
```

# build

```
dotnet tool restore

dotnet stryker --project-file G:\github\FlorianGrimm\Latrans\src\Brimborium.Latrans.Medaitor.Test\Brimborium.Latrans.Medaitor.
Test.csproj

dotnet stryker --project-file "src\Brimborium.Latrans.Medaitor.Test\Brimborium.Latrans.Medaitor.Test.csproj"
dotnet stryker
```


 


