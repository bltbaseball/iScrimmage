DECLARE @TableName VARCHAR(MAX) = 'Member'
DECLARE @TableSchema VARCHAR(MAX) = 'blt'
DECLARE @result varchar(max) = ''
DECLARE @namespace varchar(500) = 'iScrimmage.Core.Models'

SET @result = ''

SET @result = @result + 'public class ' + @TableName + ': BaseModel' + CHAR(13) + '{'

SELECT @result = @result + CHAR(13) 
    + ' public virtual ' + ColumnType + ' ' + ColumnName + ' { get; set; } ' + CHAR(13) 
FROM
(
    SELECT  c.COLUMN_NAME   AS ColumnName 
        , CASE c.DATA_TYPE   
            WHEN 'bigint' THEN
                CASE C.IS_NULLABLE
                    WHEN 'YES' THEN 'Int64?' ELSE 'Int64' END
            WHEN 'binary' THEN 'Byte[]'
            WHEN 'bit' THEN 
                CASE C.IS_NULLABLE
                    WHEN 'YES' THEN 'Boolean?' ELSE 'Boolean' END            
            WHEN 'char' THEN 'String'
            WHEN 'date' THEN
                CASE C.IS_NULLABLE
                    WHEN 'YES' THEN 'DateTime?' ELSE 'DateTime' END                        
            WHEN 'datetime' THEN
                CASE C.IS_NULLABLE
                    WHEN 'YES' THEN 'DateTime?' ELSE 'DateTime' END                        
            WHEN 'datetime2' THEN  
                CASE C.IS_NULLABLE
                    WHEN 'YES' THEN 'DateTime?' ELSE 'DateTime' END                        
            WHEN 'datetimeoffset' THEN 
                CASE C.IS_NULLABLE
                    WHEN 'YES' THEN 'DateTimeOffset?' ELSE 'DateTimeOffset' END                                    
            WHEN 'decimal' THEN  
                CASE C.IS_NULLABLE
                    WHEN 'YES' THEN 'Decimal?' ELSE 'Decimal' END                                    
            WHEN 'float' THEN 
                CASE C.IS_NULLABLE
                    WHEN 'YES' THEN 'Single?' ELSE 'Single' END                                    
            WHEN 'image' THEN 'Byte[]'
            WHEN 'int' THEN  
                CASE C.IS_NULLABLE
                    WHEN 'YES' THEN 'Int32?' ELSE 'Int32' END
            WHEN 'money' THEN
                CASE C.IS_NULLABLE
                    WHEN 'YES' THEN 'Decimal?' ELSE 'Decimal' END                                                
            WHEN 'nchar' THEN 'String'
            WHEN 'ntext' THEN 'String'
            WHEN 'numeric' THEN
                CASE C.IS_NULLABLE
                    WHEN 'YES' THEN 'Decimal?' ELSE 'Decimal' END                                                            
            WHEN 'nvarchar' THEN 'String'
            WHEN 'real' THEN 
                CASE C.IS_NULLABLE
                    WHEN 'YES' THEN 'Double?' ELSE 'Double' END                                                                        
            WHEN 'smalldatetime' THEN 
                CASE C.IS_NULLABLE
                    WHEN 'YES' THEN 'DateTime?' ELSE 'DateTime' END                                    
            WHEN 'smallint' THEN 
                CASE C.IS_NULLABLE
                    WHEN 'YES' THEN 'Int16?' ELSE 'Int16'END            
            WHEN 'smallmoney' THEN  
                CASE C.IS_NULLABLE
                    WHEN 'YES' THEN 'Decimal?' ELSE 'Decimal' END                                                                        
            WHEN 'text' THEN 'String'
            WHEN 'time' THEN 
                CASE C.IS_NULLABLE
                    WHEN 'YES' THEN 'TimeSpan?' ELSE 'TimeSpan' END                                                                                    
            WHEN 'timestamp' THEN 
                CASE C.IS_NULLABLE
                    WHEN 'YES' THEN 'DateTime?' ELSE 'DateTime' END                                    
            WHEN 'tinyint' THEN 
                CASE C.IS_NULLABLE
                    WHEN 'YES' THEN 'Byte?' ELSE 'Byte' END                                                
            WHEN 'uniqueidentifier' THEN 'Guid'
            WHEN 'varbinary' THEN 'Byte[]'
            WHEN 'varchar' THEN 'String'
            ELSE 'Object'
        END AS ColumnType
        , c.ORDINAL_POSITION 
FROM    INFORMATION_SCHEMA.COLUMNS c
WHERE   c.TABLE_NAME = @TableName and ISNULL(@TableSchema, c.TABLE_SCHEMA) = c.TABLE_SCHEMA  
AND c.COLUMN_NAME not in ('Id','CreatedOn', 'CreatedBy','ModifiedOn', 'ModifiedBy','Archived')
) t
ORDER BY t.ORDINAL_POSITION

SET @result = @result  + '}' + CHAR(13)

PRINT @result