[![Build status](https://ci.appveyor.com/api/projects/status/yb22aa3i133kj54e?svg=true)](https://ci.appveyor.com/project/GrumpyBusted/grumpy-dacpacmerge)
[![codecov](https://codecov.io/gh/GrumpyBusted/Grumpy.DacpacMerge/branch/master/graph/badge.svg)](https://codecov.io/gh/GrumpyBusted/Grumpy.DacpacMerge)
[![nuget](https://img.shields.io/nuget/v/Grumpy.DacpacMerge.svg)](https://www.nuget.org/packages/Grumpy.DacpacMerge/)
[![downloads](https://img.shields.io/nuget/dt/Grumpy.DacpacMerge.svg)](https://www.nuget.org/packages/Grumpy.DacpacMerge/)

# Grumpy.DacpacMerge
This library includes a tool for helping to deploy single SQL Schemas to existing MS-SQL Databases. The standard Data-tier Application Package file (DacPac)
deploy using SQL-Package and SQL Server Data Tools (SSDT) only support deploying whole databases. In some enterprises multiple logical
databases are created in the same MS-SQL Databases with different schemas, for different reasons, some good and some
not so good.

Using this library to build your own tool to deploy single schemas, should be simple, I will add then sample code latter.

This tool load the database model from the new DacPac-File and merges this with the existing database
model from a specified MS-SQL Database. The element that exist in the new Dacpac-File will override the element already in
the database with the same name.