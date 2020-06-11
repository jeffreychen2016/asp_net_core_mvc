## Create local MSSQL server instance for MAC

official docker doc: https://hub.docker.com/_/microsoft-mssql-server

1. pull docker image and run the container:

   ```bash
   docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=yourStrong(!)Password' -p 1433:1433 -d mcr.microsoft.com/mssql/server:2017-latest
   ```

2. check if the container is running and get container name:

   ```bash
   docker ps
   ```

3. connect to running container:

   ```bash
   ## docker exec -it <container_id|container_name> /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P <your_password>
   docker exec -it interesting_hoover /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'yourStrong(!)Password'
   ```

## Install sql tool

install sql-cli tool so that we can query the database from outside of sqlserver.

command docs: [https://www.npmjs.com/package/sql-cli]

```
npm install -g sql-cli
```

to login to running database:

```
mssql -u sa -p P@55w0rd
```

## DB Imgration

once the container is up running, we need to create database by using the DbContext migration:

1. install The command-line interface (CLI) tools for Entity Framework Core perform design-time development tasks [[EF Core tools reference (.NET CLI) - EF Core | Microsoft Docs](https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dotnet#dotnet-ef-migrations-add)]

```
dotnet tool install —global dotnet-ef
```

2. install the .Net Core SDK
3. Install the latest Microsoft.EntityFrameworkCore.Design package. (Better to use Nuget Manager to install this)

```
dotnet add package Microsoft.EntityFrameworkCore.Design
```

4. now you are able to create migration:

```
 dotnet ef migrations add InitialMigration
```

if everything works, you should see a `Migrations/` directory created.

If see error:

```
Unable to create an object of type 'AppDbContext'. For the different patterns supported at design time, see https://go.microsoft.com/fwlink/?linkid=851728
```

Then update all packages.
