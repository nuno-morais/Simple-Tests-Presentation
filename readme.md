## Bill Tracker (Simple Tests - Presentation)

Just a simple example! [Presentation](https://docs.google.com/presentation/d/1lgTCAo_sDIf5r5b4G0d33HddvFZeBUag_S8FcgVwPIU/edit?usp=sharing)

## How to execute tests

```bash
docker run -p 1433:1433 -d db-demo | xargs docker logs -f
dotnet test
```

## How to execute the project
```bash
docker run -p 1433:1433 -d db-demo | xargs docker logs -f

dotnet run --project BillTracker/BillTracker.csproj
```

Go to: https://localhost:5001/swagger/index.html
