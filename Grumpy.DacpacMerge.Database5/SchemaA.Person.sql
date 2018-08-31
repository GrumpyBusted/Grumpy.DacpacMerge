CREATE TABLE [SchemaA].[Person]
(
	[Id] INT NOT NULL PRIMARY KEY,
	[Name] NVARCHAR(50) NOT NULL, 
    [Age] INT NOT NULL DEFAULT 0
)
    


GO

CREATE INDEX [IX_Person_Name] ON [SchemaA].[Person] ([Name])
