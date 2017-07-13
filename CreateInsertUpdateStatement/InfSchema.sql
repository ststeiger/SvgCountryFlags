
-- SELECT OBJECT_DEFINITION(object_id('information_schema.schemata'))
-- SELECT * FROM sys.columns WHERE is_computed = 1


-- CREATE VIEW information_schema.routine_columns AS 
SELECT 
	 DB_NAME() AS table_catalog
	,SCHEMA_NAME(o.schema_id) AS table_schema
	,o.NAME AS table_name
	,c.NAME AS column_name
	,c.column_id AS ordinal_position
	,OBJECT_DEFINITION(c.default_object_id) AS column_default
	,CONVERT(varchar(3), CASE WHEN c.is_nullable = 1 THEN 'YES' ELSE 'NO' END) AS is_nullable
	,ISNULL(TYPE_NAME(c.system_type_id), t.NAME) AS data_type
	,COLUMNPROPERTY(c.object_id, c.NAME, 'charmaxlen') AS character_maximum_length
	,COLUMNPROPERTY(c.object_id, c.NAME, 'octetmaxlen') AS character_octet_length

	,CONVERT
	(
		 tinyint
		,CASE -- int/decimal/numeric/real/float/money    
			WHEN c.system_type_id IN (48, 52, 56, 59, 60, 62, 106, 108, 122, 127) THEN c.precision    
		 END
	) AS NUMERIC_PRECISION
	 
	,CONVERT
	(
		 smallint
		,CASE -- int/money/decimal/numeric    
			WHEN c.system_type_id IN (48, 52, 56, 60, 106, 108, 122, 127) THEN 10    
			WHEN c.system_type_id IN (59, 62) THEN 2 
		 END
	) AS NUMERIC_PRECISION_RADIX 

	, -- real/float   
	CONVERT
	(
		 int
		,CASE -- datetime/smalldatetime    
			WHEN c.system_type_id IN (40, 41, 42, 43, 58, 61) THEN NULL    
			ELSE ODBCSCALE(c.system_type_id, c.scale) 
		END
	) AS NUMERIC_SCALE 

	,CONVERT
	(
		 smallint
		,CASE -- datetime/smalldatetime    
			WHEN c.system_type_id IN (40, 41, 42, 43, 58, 61) THEN ODBCSCALE(c.system_type_id, c.scale) 
		 END
	) AS DATETIME_PRECISION

	,CONVERT(sysname, null) AS CHARACTER_SET_CATALOG
	,CONVERT(sysname, null) COLLATE catalog_default AS CHARACTER_SET_SCHEMA

	,CONVERT
	( 
		 sysname
		,CASE 
			WHEN c.system_type_id IN (35, 167, 175) -- char/varchar/text    
				THEN COLLATIONPROPERTY(c.collation_name, 'sqlcharsetname') 
			WHEN c.system_type_id IN (99, 231, 239) -- nchar/nvarchar/ntext    
				THEN N'UNICODE' 
		 END
	) AS CHARACTER_SET_NAME 
	
	,CONVERT(sysname, null) AS COLLATION_CATALOG 
	,CONVERT(sysname, null) COLLATE catalog_default AS COLLATION_SCHEMA 
	,c.collation_name AS COLLATION_NAME 
	,CONVERT(sysname, CASE WHEN c.user_type_id > 256 THEN DB_NAME() END) AS DOMAIN_CATALOG 
	,CONVERT(sysname, CASE WHEN c.user_type_id > 256 THEN SCHEMA_NAME(t.schema_id) END) AS DOMAIN_SCHEMA 
	,CONVERT(sysname, CASE WHEN c.user_type_id > 256 THEN TYPE_NAME(c.user_type_id) END) AS DOMAIN_NAME 
FROM sys.objects AS o 

INNER JOIN sys.columns AS c 
	ON c.object_id = o.object_id 
	
LEFT JOIN sys.types t 
	ON c.user_type_id = t.user_type_id 
	
WHERE o.type IN ('TF','IF', 'FT')



-- CREATE VIEW information_schema.routines AS 
SELECT 
	 DB_NAME() AS specific_catalog 
	,SCHEMA_NAME(o.schema_id) AS specific_schema
	,o.NAME AS specific_name
	,DB_NAME() AS routine_catalog
	,SCHEMA_NAME(o.schema_id) AS routine_schema
	,o.NAME AS routine_name
	
	, 
	CONVERT
	(
		nvarchar(20)
		, 
		CASE 
			WHEN o.type IN ('P', 'PC') THEN 'PROCEDURE' 
			ELSE 'FUNCTION' 
		END
	) AS routine_type
	
	,CONVERT(sysname, NULL) AS module_catalog
	,CONVERT(sysname, NULL) COLLATE catalog_default AS module_schema
	,CONVERT(sysname, NULL) COLLATE catalog_default AS module_name
	,CONVERT(sysname, NULL) AS udt_catalog
	,CONVERT(sysname, NULL) COLLATE catalog_default AS udt_schema
	,CONVERT(sysname, NULL) COLLATE catalog_default AS udt_name
	,CONVERT
	(
		sysname
		, 
		CASE 
			WHEN o.type IN ('TF', 'IF', 'FT') THEN N'TABLE' 
			ELSE ISNULL(TYPE_NAME(c.system_type_id), TYPE_NAME(c.user_type_id)) 
		END
	) AS data_type
	
	,COLUMNPROPERTY(c.object_id, c.NAME, 'charmaxlen') AS character_maximum_length 
	,COLUMNPROPERTY(c.object_id, c.NAME, 'octetmaxlen') AS character_octet_length 
	,CONVERT(sysname, NULL) AS collation_catalog 
	,CONVERT(sysname, NULL) COLLATE catalog_default AS collation_schema 
	
	,CONVERT
	(
		sysname
		, 
		CASE 
			WHEN c.system_type_id IN (35, 99, 167, 175, 231, 239) -- [n]char/[n]varchar/[n]text 
			THEN DATABASEPROPERTYEX(DB_NAME(), 'collation') 
		END
	) AS COLLATION_NAME 
	 
	,CONVERT(sysname, null) AS CHARACTER_SET_CATALOG 
	,CONVERT(sysname, null) COLLATE catalog_default AS CHARACTER_SET_SCHEMA 
	
	,CONVERT
	(
		sysname
		,
		CASE    
			WHEN c.system_type_id IN (35, 167, 175) THEN SERVERPROPERTY('sqlcharsetname') -- char/varchar/text 
			WHEN c.system_type_id IN (99, 231, 239) THEN N'UNICODE' -- nchar/nvarchar/ntext 
		END 
	) AS CHARACTER_SET_NAME 
	
	,
	CONVERT
	(
		tinyint
		,
		CASE -- int/decimal/numeric/real/float/money    
			WHEN c.system_type_id IN (48, 52, 56, 59, 60, 62, 106, 108, 122, 127) THEN c.precision    
		END 
	) AS NUMERIC_PRECISION
	
	,CONVERT(smallint, CASE -- int/money/decimal/numeric    
	WHEN c.system_type_id IN (48, 52, 56, 60, 106, 108, 122, 127) THEN 10    
	WHEN c.system_type_id IN (59, 62) THEN 2 END
	) AS NUMERIC_PRECISION_RADIX
	
	, -- real/float   
	CONVERT
	(
		int
		, 
		CASE -- datetime/smalldatetime    
			WHEN c.system_type_id IN (40, 41, 42, 43, 58, 61) THEN NULL    
			ELSE ODBCSCALE(c.system_type_id, c.scale) 
		END
	) AS NUMERIC_SCALE
	
	,CONVERT
	(
		smallint
		,CASE -- datetime/smalldatetime    
			WHEN c.system_type_id IN (40, 41, 42, 43, 58, 61) THEN ODBCSCALE(c.system_type_id, c.scale) 
		 END
	) AS DATETIME_PRECISION
	
	,CONVERT(nvarchar(30), null) AS INTERVAL_TYPE
	,CONVERT(smallint, null) AS INTERVAL_PRECISION
	,CONVERT(sysname, null) AS TYPE_UDT_CATALOG
	,CONVERT(sysname, null) COLLATE catalog_default AS TYPE_UDT_SCHEMA
	,CONVERT(sysname, null) COLLATE catalog_default AS TYPE_UDT_NAME
	,CONVERT(sysname, null) AS SCOPE_CATALOG
	,CONVERT(sysname, null) COLLATE catalog_default AS SCOPE_SCHEMA
	,CONVERT(sysname, null) COLLATE catalog_default AS SCOPE_NAME
	,CONVERT(bigint, null) AS MAXIMUM_CARDINALITY
	,CONVERT(sysname, null) AS DTD_IDENTIFIER
	
	,CONVERT(nvarchar(30), CASE WHEN o.type IN ('P ', 'FN', 'TF', 'IF') THEN 'SQL' ELSE 'EXTERNAL' END) AS ROUTINE_BODY
	
	,OBJECT_DEFINITION(o.object_id) AS ROUTINE_DEFINITION 
	
	,CONVERT(sysname, null) AS EXTERNAL_NAME
	,CONVERT(nvarchar(30), null) AS EXTERNAL_LANGUAGE
	,CONVERT(nvarchar(30), null) AS PARAMETER_STYLE
	,CONVERT(nvarchar(10), CASE WHEN ObjectProperty(o.object_id, 'IsDeterministic') = 1 THEN 'YES' ELSE 'NO' END) AS IS_DETERMINISTIC
	,CONVERT(nvarchar(30), CASE WHEN o.type IN ('P', 'PC') THEN 'MODIFIES' ELSE 'READS' END) AS SQL_DATA_ACCESS
	
	--,CONVERT(nvarchar(10), CASE WHEN o.type in ('P', 'PC') THEN null WHEN o.null_on_null_input = 1 THEN 'YES' ELSE 'NO' END) AS IS_NULL_CALL
	
	,CONVERT(sysname, null) AS SQL_PATH
	,CONVERT(nvarchar(10), 'YES') AS SCHEMA_LEVEL_ROUTINE
	,CONVERT(smallint, CASE WHEN o.type IN ('P ', 'PC') THEN -1 ELSE 0 END) AS MAX_DYNAMIC_RESULT_SETS
	,CONVERT(nvarchar(10), 'NO') AS IS_USER_DEFINED_CAST
	,CONVERT(nvarchar(10), 'NO') AS IS_IMPLICITLY_INVOCABLE
	,o.create_date AS CREATED
	,o.modify_date AS LAST_ALTERED 
FROM sys.objects AS o 

LEFT JOIN sys.parameters c    
	ON (c.object_id = o.object_id AND c.parameter_id = 0) 
	
WHERE o.type IN ('P', 'FN', 'TF', 'IF', 'AF', 'FT', 'IS', 'PC', 'FS') 




-- CREATE VIEW INFORMATION_SCHEMA.TABLES AS 
SELECT
	 DB_NAME() AS TABLE_CATALOG
	,s.name AS TABLE_SCHEMA
	,o.name AS TABLE_NAME
	,
	CASE o.type
		WHEN 'U' THEN 'BASE TABLE'
		WHEN 'V' THEN 'VIEW'
	END AS TABLE_TYPE 
FROM sys.objects AS o 

LEFT JOIN sys.schemas AS s 
  ON s.schema_id = o.schema_id 
  
WHERE o.type IN ('U', 'V') 





-- CREATE VIEW INFORMATION_SCHEMA.VIEWS AS 
SELECT
	 DB_NAME() AS TABLE_CATALOG
	,SCHEMA_NAME(schema_id) AS TABLE_SCHEMA
	,name AS TABLE_NAME
	,OBJECT_DEFINITION(object_id) AS VIEW_DEFINITION 
	
	,CONVERT
	(
		varchar(7)
		,
		CASE with_check_option
			WHEN 1 THEN 'CASCADE'
			ELSE 'NONE'
		END 
	) AS CHECK_OPTION 
	
	,'NO' AS IS_UPDATABLE
FROM sys.views





-- CREATE VIEW information_schema.columns AS 
SELECT 
	 DB_NAME() AS table_catalog 
	,SCHEMA_NAME(o.schema_id) AS table_schema 
	,o.NAME AS table_name 
	,c.NAME AS column_name 
	,c.is_computed AS is_computed 
	,COLUMNPROPERTY(c.object_id, c.NAME, 'ordinal') AS ordinal_position 
	,OBJECT_DEFINITION(c.default_object_id) AS column_default 
	,CONVERT(VARCHAR(3), CASE c.is_nullable WHEN 1 THEN 'YES' ELSE 'NO' END) AS is_nullable 
	,ISNULL(TYPE_NAME(c.system_type_id), t.NAME) AS data_type 
	,COLUMNPROPERTY(c.object_id, c.NAME, 'charmaxlen') AS character_maximum_length 
	,COLUMNPROPERTY(c.object_id, c.NAME, 'octetmaxlen') AS character_octet_length 

	,
	CONVERT
	(
		TINYINT
		, 
		CASE -- int/decimal/numeric/real/float/money    
			WHEN c.system_type_id IN (48, 52, 56, 59, 60, 62, 106, 108, 122, 127) THEN c.precision    
		END
	) AS NUMERIC_PRECISION

	,CONVERT
	(
		smallint
		, 
		CASE -- int/money/decimal/numeric    
			WHEN c.system_type_id IN (48, 52, 56, 60, 106, 108, 122, 127) THEN 10    
			WHEN c.system_type_id IN (59, 62) THEN 2 
		END
	) AS NUMERIC_PRECISION_RADIX

	, -- real/float   
	CONVERT
	(
		int
		, 
		CASE -- datetime/smalldatetime    
			WHEN c.system_type_id IN (40, 41, 42, 43, 58, 61) THEN NULL    
			ELSE ODBCSCALE(c.system_type_id, c.scale) END
	) AS NUMERIC_SCALE

	,CONVERT
	(
		smallint, 
		CASE -- datetime/smalldatetime 
			WHEN c.system_type_id IN (40, 41, 42, 43, 58, 61) THEN ODBCSCALE(c.system_type_id, c.scale) 
		END
	) AS DATETIME_PRECISION

	,CONVERT(sysname, null) AS CHARACTER_SET_CATALOG 
	,CONVERT(sysname, null) COLLATE catalog_default AS CHARACTER_SET_SCHEMA 
	
	,CONVERT
	(
		sysname
		, 
		CASE 
			WHEN c.system_type_id IN (35, 167, 175) -- char/varchar/text     
				THEN COLLATIONPROPERTY(c.collation_name, 'sqlcharsetname') 
			WHEN c.system_type_id IN (99, 231, 239) -- nchar/nvarchar/ntext     
				THEN N'UNICODE' 
		END
	) AS CHARACTER_SET_NAME

	,CONVERT(sysname, null) AS COLLATION_CATALOG
	,CONVERT(sysname, null) COLLATE catalog_default AS COLLATION_SCHEMA
	,c.collation_name AS COLLATION_NAME 
	,CONVERT(sysname, CASE WHEN c.user_type_id > 256 THEN DB_NAME() END) AS DOMAIN_CATALOG 
	,CONVERT(sysname, CASE WHEN c.user_type_id > 256 THEN SCHEMA_NAME(t.schema_id) END) AS DOMAIN_SCHEMA 
	,CONVERT(sysname, CASE WHEN c.user_type_id > 256 THEN TYPE_NAME(c.user_type_id) END) AS DOMAIN_NAME 
FROM sys.objects AS o 

INNER JOIN sys.columns AS c 
	ON c.object_id = o.object_id 
	
LEFT JOIN sys.types AS t 
	ON c.user_type_id = t.user_type_id 
	
-- https://devio.wordpress.com/2008/01/17/mssql-database-object-types/
WHERE o.type IN 
(
	'U'
	--,'V'
	--,'IF' -- SQL inline table-valued function
	--,'TF' -- SQL table-valued-function
	--,'FT' -- Assembly (CLR) table-valued function
	
	--,'FN' -- SQL scalar function -- doesn't exist
	--,'FS' -- Assembly (CLR) scalar function -- doesn't exist
	--,'P'  -- SQL stored procedure	-- doesn't exist
	--,'PC' -- Assembly (CLR) stored procedure -- doesn't exist
) 


-- SELECT * FROM sys.types
