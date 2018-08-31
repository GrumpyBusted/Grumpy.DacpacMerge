CREATE TABLE [SchemaC].[ZipCode]
(
	[Id] INT NOT NULL PRIMARY KEY,
	[ZipCode] NVARCHAR(10) NOT NULL,
	[ZipName] NVARCHAR(50) NOT NULL,
)


GO

CREATE INDEX [IX_ZipCode_Name] ON [SchemaC].[ZipCode] ([ZipCode])
