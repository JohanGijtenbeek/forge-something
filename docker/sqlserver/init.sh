#!/bin/bash

echo "Wachten op SQL Server..."
until /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -Q "SELECT 1" -C &>/dev/null
do
  echo "SQL Server nog niet klaar, opnieuw proberen over 2 seconden..."
  sleep 2
done

echo "Schema laden..."
/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -d master -i /docker-entrypoint-initdb.d/init.sql -C

# SQL seed alleen voor low - medium en high gebruiken de Bogus seeder
if [ "$SEED_PROFILE" = "low" ]; then
  echo "SQL seed laden (low)..."
  /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -d ErpDb -i /docker-entrypoint-initdb.d/seed_organizations.sql -C
  /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -d ErpDb -i /docker-entrypoint-initdb.d/seed_persons.sql -C
fi

echo "Klaar."
