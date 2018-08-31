CREATE TABLE [SchemaB].[Address]
(
	[Id] INT NOT NULL PRIMARY KEY,
	[Street] NVARCHAR(50) NOT NULL,
)


GO

CREATE INDEX [IX_Address_Name] ON [SchemaB].[Address] ([Street])
