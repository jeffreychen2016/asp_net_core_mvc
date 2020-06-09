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
   docker exec -it flamboyant_raman /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'yourStrong(!)Password'
   ```
