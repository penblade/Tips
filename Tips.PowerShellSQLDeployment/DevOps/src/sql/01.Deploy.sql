SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Drop the stored procedure if it exists.
IF EXISTS (
          SELECT  *
          FROM    sys.objects
          WHERE   object_id = OBJECT_ID(N'[dbo].[GetTimeOfQuery]')
                    AND type in (N'P', N'PC') )
          DROP PROCEDURE [dbo].[GetTimeOfQuery]
GO
CREATE PROCEDURE [dbo].[GetTimeOfQuery]
AS
    SET NOCOUNT ON
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

    SELECT GETDATE() AS TimeOfQuery
GO
--GRANT EXECUTE ON [dbo].[GetTimeOfQuery] TO [{DatabaseRole}]
--GO
-- Verify stored procedure was created by displaying its definition
-- sp_helptext GetTimeOfQuery
