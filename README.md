# Latrans
Experimental WebApp

## Projects
- Brimborium.Latrans.Contracts
- Brimborium.Latrans.Medaitor
- DemoWebApp
- DemoWebApp.Contracts
benchmark

#setup

```
dotnet new "Benchmark Project" -n Brimborium.Latrans.Benchmark  -o src\Brimborium.Latrans.Benchmark 
dotnet sln add src\Brimborium.Latrans.Benchmark 

dotnet new classlib -n Brimborium.Latrans.JSON -o src\Brimborium.Latrans.JSON
dotnet sln add src\Brimborium.Latrans.JSON

dotnet new classlib -n Brimborium.Latrans.JSON.AspNetCoreMvcFormatter -o src\Brimborium.Latrans.JSON.AspNetCoreMvcFormatter
dotnet sln add src\Brimborium.Latrans.JSON.AspNetCoreMvcFormatter

dotnet new xunit -n Brimborium.Latrans.JSON.Test -o src\Brimborium.Latrans.JSON.Test
dotnet sln add src\Brimborium.Latrans.JSON.Test,

dotnet new classlib -n Brimborium.Latrans.Utility  -o src\Brimborium.Latrans.Utility 
dotnet sln add src\Brimborium.Latrans.Utility 

dotnet new xunit -n Brimborium.Latrans.Utility.Test -o src\Brimborium.Latrans.Utility.Test
dotnet sln add src\Brimborium.Latrans.Utility.Test

dotnet new classlib -n Brimborium.Latrans.Medaitor -o src\Brimborium.Latrans.Medaitor
dotnet sln add src\Brimborium.Latrans.Medaitor

dotnet new xunit -n Brimborium.Latrans.Medaitor.Test -o src\Brimborium.Latrans.Medaitor.Test
dotnet sln add src\Brimborium.Latrans.Medaitor.Test

dotnet new classlib -n Brimborium.Latrans.Medaitor.AspNetCore -o src\Brimborium.Latrans.Medaitor.AspNetCore
dotnet sln add src\Brimborium.Latrans.Medaitor.AspNetCore

dotnet new xunit -n Brimborium.Latrans.Medaitor.AspNetCore.Test -o src\Brimborium.Latrans.Medaitor.AspNetCore.Test
dotnet sln add src\Brimborium.Latrans.Medaitor.AspNetCore.Test

dotnet new classlib -n Brimborium.Latrans.StoreageReadable -o src\Brimborium.Latrans.StoreageReadable
dotnet sln add src\Brimborium.Latrans.StoreageReadable

dotnet new xunit -n Brimborium.Latrans.StoreageReadable.Test -o src\Brimborium.Latrans.StoreageReadable.Test
dotnet sln add src\Brimborium.Latrans.StoreageReadable.Test

dotnet new classlib -n Brimborium.Latrans.StoreageFASTER -o src\Brimborium.Latrans.StoreageFASTER
dotnet sln add src\Brimborium.Latrans.StoreageFASTER

dotnet new xunit -n Brimborium.Latrans.StoreageFASTER.Test -o src\Brimborium.Latrans.StoreageFASTER.Test
dotnet sln add src\Brimborium.Latrans.StoreageFASTER.Test

dotnet new classlib -n Brimborium.Latrans.StoreageFASTERAzure -o src\Brimborium.Latrans.StoreageFASTERAzure
dotnet sln add src\Brimborium.Latrans.StoreageFASTERAzure

dotnet new xunit -n Brimborium.Latrans.StoreageFASTERAzure.Test -o src\Brimborium.Latrans.StoreageFASTERAzure.Test
dotnet sln add src\Brimborium.Latrans.StoreageFASTERAzure.Test

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

# setup computer

```
dotnet tool install -g BenchmarkDotNet.Tool
```

dotnet run -c Release --project src\Brimborium.Latrans.Benchmark --runtimes netcoreapp31 --list Tree
dotnet run -c Release -- --job short --runtimes clr core --filter *BenchmarkClass1*

```
dotnet build -c Release src\Brimborium.Latrans.Benchmark
dotnet benchmark output\Brimborium.Latrans.Benchmark\bin\Release\netcoreapp3.1\Brimborium.Latrans.Benchmark.dll --filter *

G:\github\FlorianGrimm\Latrans\output\Brimborium.Latrans.Benchmark\bin\Release\netcoreapp3.1\Brimborium.Latrans.Benchmark.dll
G:\github\FlorianGrimm\Latrans\output\Brimborium.Latrans.Benchmark\bin\Debug\netcoreapp3.1\Brimborium.Latrans.Benchmark.dll
```

# build


```
dotnet build /restore
```


