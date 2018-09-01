[![Build status](https://ci.appveyor.com/api/projects/status/yb22aa3i133kj54e?svg=true)](https://ci.appveyor.com/project/GrumpyBusted/grumpy-dacpacmerge)
[![codecov](https://codecov.io/gh/GrumpyBusted/Grumpy.DacpacMerge/branch/master/graph/badge.svg)](https://codecov.io/gh/GrumpyBusted/Grumpy.DacpacMerge)
[![nuget](https://img.shields.io/nuget/v/Grumpy.DacpacMerge.svg)](https://www.nuget.org/packages/Grumpy.DacpacMerge/)
[![downloads](https://img.shields.io/nuget/dt/Grumpy.DacpacMerge.svg)](https://www.nuget.org/packages/Grumpy.DacpacMerge/)

# Grumpy.DacpacMerge
This library includes a tool for helping to deploy single SQL Schemas to existing 
MS-SQL Databases. The standard Data-tier Application Package file (DacPac), 
deploy using SQL-Package and SQL Server Data Tools (SSDT) only support deploying 
whole databases. In some enterprises multiple logical databases are created in 
the same MS-SQL Databases with different schemas, for different reasons, some 
good and some not so good.

Using this library to build your own tool to deploy single schemas, should be simple, 
I will add then sample code latter.

This tool load the database model from the new DacPac-File and merges this with the 
existing database model from a specified MS-SQL Database. The element that exists in 
the new DacPac-File will override the element already in the database with the same name.

The database to merge with must exists before hand. You can have multiple schemas in the 
same DacPac-File, if needed.

## Code sample
The following sample will read the definition for the database "MyDatabase" on Local DB, 
Remove all element from that in "MySchema". Find all the non-schema related elements from 
the DacPac-file "MyInput.dacpac" and remove these from the elements from the database model. 
Then add all the elements from the "MyInput.dacpac" to the database model, and save this 
in the new file "MyOutput.dacpac". The owner of the schema "MySchema" will be set to 
"MySchemaUser".

```C#
var logger = NullLogger.Instance;
const string inputDacpacFileName = "MyInput.dacpac";
const string databaseSource = @"(localdb)\MSSQLLocalDB";
const string databaseName = "MyDatabase";
var databaseSchemas = new[] {"MySchema"};
const string schemaOwnerUser = "MySchemaUser";
const string outputDacpacFileName = "MyOutput.dacpac";

var service = new DacpacMergeServiceBuilder(logger).Build();

service.Merge(inputDacpacFileName, 
              databaseSource, 
              databaseName, 
              databaseSchemas, 
              schemaOwnerUser, 
              outputDacpacFileName);
```
The intention is to have the argument to be set from command line arguments, but use as you need.

The arguments [databaseSchema], [schemaOwnerUser] and [outputDacpacFileName] are optional. 
Default for [databaseSchema] is all schemas defined in the input Data-tier Application 
Package file (DacPac). If [schemaOwnerUser] are omitted the schema user is not changed. 
If the [outputDacpacFileName] are omitted the result will be saved back into the input 
file.
