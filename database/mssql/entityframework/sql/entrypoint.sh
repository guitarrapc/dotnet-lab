#!/bin/bash
database=$SQLCMDDBNAME

# wait for SQL Server to come up
NEXT_WAIT_TIME=1
echo monitoring db server is online.
until /opt/mssql-tools/bin/sqlcmd -P $SA_PASSWORD -d master -Q 'select 0'; do
  echo sleep ${NEXT_WAIT_TIME} sec for next interval
  sleep ${NEXT_WAIT_TIME}
  # MEMO: should timeout on 1min or above? if so, should pass crash log to the host for investigation.
  if [ ${NEXT_WAIT_TIME} -ne 4 ]; then
    NEXT_WAIT_TIME=$(( $NEXT_WAIT_TIME + 1 ))
  fi
done

echo confirm db server is online.
echo importing data...

# run the init script to create the DB and the tables in /table
/opt/mssql-tools/bin/sqlcmd -P $SA_PASSWORD -d master -i ./init.sql

# create table
for entry in "table/*.sql"
do
  echo executing $entry
  /opt/mssql-tools/bin/sqlcmd -P $SA_PASSWORD -i $entry
done

# import the data from the csv files
mkdir -p ./data_conv
for entry in $(find data -name "*.csv")
do
  # i.e: transform /data/MyTable.csv to MyTable
  shortname=$(echo $entry | cut -f 1 -d '.' | cut -f 2 -d '/')
  tableName=[$database].[dbo].[$shortname]
  filename="$shortname.csv"

  # convert utf8 to utf16le
  echo converting $filename from utf8 to utf16
  cat $(pwd)/data/$shortname.csv
  iconv -f UTF8 -t UTF16 $(pwd)/data/$filename -o $(pwd)/data_conv/$filename

  # csv can be both crlf and lf with ROWTERMINATOR 0x0A. dont't use ROWTERMINATOR \n as it force you use crlf for csv.
  # csv must be utf16 encoding. both UTF-16BE and UTF-16LE is available in Bulk Insert. (you don't need to think about bom. )
  # do not add (DATAFILETYPE = 'widechar') as it fail....
  echo importing $tableName from $filename
  /opt/mssql-tools/bin/sqlcmd -P $SA_PASSWORD -Q "BULK INSERT $tableName FROM '$(pwd)/data_conv/$filename' WITH ( FIELDTERMINATOR = ',', ROWTERMINATOR = '0x0A');"
done
