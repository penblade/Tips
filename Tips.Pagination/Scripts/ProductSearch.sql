IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[ProductSearch]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[ProductSearch]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****************************************************************************************
REQUIREMENTS:
    The following are required to run this script to
    create a stored procedure and run the sample calls
    under the testing section.

    1.  SQL Server Management Studio (SSMS) 2017
    2.  Azure Free Account or better
        a.  https://portal.azure.com
    3.  Create an Azure SQL database with the AdventureWorksLT
        a.  Database name: AdventureWorksLT
        b.  Pricing Tier: Basic

PURPOSE:
    Search for products by
          current page (0 returns all)
        , entries per page (0 returns all entries on page 1)
        , culture (exact; example 'en' for English)
        , product name (like)
        , product number (exact match)
        , product description (like).

    Returns just the records associated with the page number
    for the number of entries per page.

    Always order by the product name (asc) and culture (asc).

NOTES:
    Page number 0 returns all records to support applications
    that implement caching or alternate pagination. If you are
    getting all records, then the techniques used here are not
    as efficient.

    The CurrentRow, CurrentPage, and CurrentRowOnPage are for
    diagnostics and may be commented out to further increase
    performance by storing and returning less information.
    The CurrentPage must still be maintained in the temp table.

    The main concept used for pagination is to use the minimal
    amount of information to determine the search results storing
    only the most unique identifier(s) in the temp table.
    Given these unique ids join on all the tables to gather the
    information you need to include in the results.

    The introduction of different product descriptions by culture
    added a challenge.  I could have assumed that the culture
    would always be 'en' for English.  However, when I was selecting
    all records I was getting odd CurrentRow results until I realized
    that I needed to store the culture in the temp table as well.

FUTURE ENHANCEMENTS:
    You could further enhance this product search to include
    parameters such as parent or child product category name.
    Perhaps include products that are less than or greater than
    a specific list price, weight, sell date, etc.

TROUBLESHOOTING:
    While testing, if the CurrentRow, CurrentPage, and CurrentRowOnPage
    aren't in numerical order as expected for the filter,
    then you should verify that all of your ORDER BY statements
    are the same for each of your queries.
    
    You could also be missing a field used in the order by or
    primary key alternative in the temp table.
    Ex. product name -> product id;  culture was the gotcha for
    me in creating this stored procedure example.    

TESTING:
    -- Test: SET FMTONLY ON returns results sets.
    -- If the SET FMTONLY OFF statement is removed
    -- and you run the following statements, then
    -- you'll get the error:
    -- Invalid object name '#ProductSearchResults'.
    SET FMTONLY ON
    EXEC [dbo].[ProductSearch]
          @CurrentPage = NULL
        , @EntriesPerPage = NULL
        , @Culture = NULL
        , @ProductName = NULL
        , @ProductNumber = NULL
        , @ProductDescription = NULL
    SET FMTONLY OFF

    -- Test: Defaults to all records
    EXEC [dbo].[ProductSearch]
          @CurrentPage = NULL
        , @EntriesPerPage = NULL
        , @Culture = NULL
        , @ProductName = NULL
        , @ProductNumber = NULL
        , @ProductDescription = NULL

    -- Test: English records only
    EXEC [dbo].[ProductSearch]
          @CurrentPage = NULL
        , @EntriesPerPage = NULL
        , @Culture = 'en'
        , @ProductName = NULL
        , @ProductNumber = NULL
        , @ProductDescription = NULL

    -- Test: CurrentPage = 1 retrieves records 1 - 10
    EXEC [dbo].[ProductSearch]
          @CurrentPage = 1
        , @EntriesPerPage = 10
        , @Culture = 'en'
        , @ProductName = NULL
        , @ProductNumber = NULL
        , @ProductDescription = NULL

    -- Test: CurrentPage = 2 retrieves records 11 - 20
    EXEC [dbo].[ProductSearch]
          @CurrentPage = 2
        , @EntriesPerPage = 10
        , @Culture = 'en'
        , @ProductName = NULL
        , @ProductNumber = NULL
        , @ProductDescription = NULL

    -- Test: CurrentPage = 30 retrieves records 291 - 294
    EXEC [dbo].[ProductSearch]
          @CurrentPage = 30
        , @EntriesPerPage = 10
        , @Culture = 'en'
        , @ProductName = NULL
        , @ProductNumber = NULL
        , @ProductDescription = NULL

    -- Test: CurrentPage = 31 retrieves no records,
    -- because the page is greater than total pages
    EXEC [dbo].[ProductSearch]
          @CurrentPage = 31
        , @EntriesPerPage = 10
        , @Culture = 'en'
        , @ProductName = NULL
        , @ProductNumber = NULL
        , @ProductDescription = NULL

    -- Test: CurrentPage 1; ProductName is like 'BIKE'
    EXEC [dbo].[ProductSearch]
          @CurrentPage = 1
        , @EntriesPerPage = 10
        , @Culture = 'en'
        , @ProductName = 'BIKE'
        , @ProductNumber = NULL
        , @ProductDescription = NULL

    -- Test: CurrentPage 5; ProductName is like 'mountain'
    EXEC [dbo].[ProductSearch]
          @CurrentPage = 5
        , @EntriesPerPage = 10
        , @Culture = 'en'
        , @ProductName = 'mountain'
        , @ProductNumber = NULL
        , @ProductDescription = NULL

    -- Test: CurrentPage 2; ProductName is like 'mountain'
    -- Product Description is like 'bike'
    EXEC [dbo].[ProductSearch]
          @CurrentPage = 2
        , @EntriesPerPage = 10
        , @Culture = 'en'
        , @ProductName = 'mountain'
        , @ProductNumber = NULL
        , @ProductDescription = 'bike'

    -- Test: CurrentPage 1; ProductNumber is exactly 'SO-B909-L'
    EXEC [dbo].[ProductSearch]
          @CurrentPage = 1
        , @EntriesPerPage = 10
        , @Culture = 'en'
        , @ProductName = NULL
        , @ProductNumber = 'SO-B909-L'
        , @ProductDescription = NULL

****************************************************************************************/
CREATE PROCEDURE [dbo].[ProductSearch] (
      @CurrentPage INT
    , @EntriesPerPage INT
    , @Culture VARCHAR(6)
    , @ProductName NVARCHAR(50)
    , @ProductNumber NVARCHAR(25)
    , @ProductDescription NVARCHAR(400)
)
AS

SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

-- When "SET FMTONLY ON" is used by extensions like "Entity Framework Reverse POCO Generator"
-- Temp tables cause the error: "Invalid object name '#{temp table name}'"
-- To avoid the error and still return the result type formats, just turn the feature off.
-- The extension then creates all of the appropriate result models, even for multiple result sets.
SET FMTONLY OFF

-- Set parameters to local variables forcing the SQL query plan to
-- cache a generically optimized query plan instead of caching by value.
-- On average, this will perform better.
DECLARE
      @localCurrentPage INT = @CurrentPage
    , @localEntriesPerPage INT = @EntriesPerPage

DECLARE
      @localCulture NVARCHAR(6) = @Culture
    , @localProductName NVARCHAR(50) = @ProductName
    , @localProductNumber NVARCHAR(25) = @ProductNumber
    , @localProductDescription NVARCHAR(400) = @ProductDescription

-- Pagination Defaults
IF (@localCurrentPage IS NULL OR @localCurrentPage = '') SET @localCurrentPage = 0
IF (@localEntriesPerPage IS NULL OR @localEntriesPerPage = '') SET @localEntriesPerPage = 0

-- Create a temp table to store the current page and product id of the search results.
IF OBJECT_ID('tempdb..#ProductSearchResults') IS NOT NULL DROP TABLE #ProductSearchResults
CREATE TABLE dbo.#ProductSearchResults (CurrentRow INT, CurrentPage INT, CurrentRowOnPage INT, ProductID INT, Culture NVARCHAR(6))

-- Get the page number and product id for the search results.
-- Do not gather all of the fields that need returned at this time
-- as that would get way too much information that is never returned.
INSERT INTO #ProductSearchResults
SELECT
    -- Get the row number and subtract 1 to make it 0-based to group 1-10 together
    -- Divide by @localEntriesPerPage to get the 0-based page number
    -- Add 1 to make the page number 1-based.
    -- ex. 1 + ( 1 - 1) / 10 = 1 -- Row  1
    -- ex. 1 + (10 - 1) / 10 = 1 -- Row 10
    -- ex. 1 + (11 - 1) / 10 = 2 -- Row 11

    -- Handle the division by 0 case.

    -- The current row.
    -- *1* The ORDER BY must match for all queries.
      1 + (ROW_NUMBER() OVER (ORDER BY p.Name, pmpd.Culture) - 1) AS CurrentRow

    -- The current page.
    , CASE WHEN @localEntriesPerPage > 0
        -- *1* The ORDER BY must match for all queries.
        THEN 1 + ((ROW_NUMBER() OVER (ORDER BY p.Name, pmpd.Culture) - 1) / @localEntriesPerPage)
        ELSE 0
      END AS CurrentPage

      -- The current row on the page.
    , CASE WHEN @localEntriesPerPage > 0
        -- *1* The ORDER BY must match for all queries.
        THEN 1 + ((ROW_NUMBER() OVER (ORDER BY p.Name, pmpd.Culture) - 1) % @localEntriesPerPage)
        ELSE 0
      END AS CurrentRowOnPage

      -- While you could include the product name instead of the product id,
      -- the NVARCHAR is not as performant as using an INT and a primary key
      -- is preferred over a column that may not have been indexed.
    , p.ProductID

      -- The culture is required to get the distinct description associated
      -- with the current row.

      -- You could instead enforce that only a single culture may be used at a time,
      -- which makes perfect since in this product search.  This would allow you
      -- to remove it from the temp table and use the local culture variable in
      -- the queries.  You would need to return an exception at the beginning
      -- of the stored procedure to enforce that requirement.
    , pmpd.Culture
FROM
    SalesLT.Product p
    INNER JOIN SalesLT.ProductModel pm ON p.ProductModelID = pm.ProductModelID
    INNER JOIN SalesLT.ProductModelProductDescription pmpd ON pm.ProductModelID = pmpd.ProductModelID
    INNER JOIN SalesLT.ProductDescription pd ON pmpd.ProductDescriptionID = pd.ProductDescriptionID
WHERE
        -- Culture (exact)
        (@localCulture IS NULL OR @localCulture = '' OR pmpd.Culture = @localCulture)
        -- ProductName (like)
    AND (@localProductName IS NULL OR @localProductName = '' OR p.Name LIKE '%' + @localProductName + '%')
        -- ProductNumber (exact)
    AND (@localProductNumber IS NULL OR @localProductNumber = '' OR p.ProductNumber = @localProductNumber)
        -- ProductDescription (like) - This may not be culture compliant except for english.
    AND (@localProductDescription IS NULL OR @localProductDescription = '' OR pd.Description LIKE '%' + @localProductDescription + '%')
-- *1* The ORDER BY must match for all queries.
ORDER BY p.Name, pmpd.Culture

-- Diagnostics: Show what is currently in the temp table.
--SELECT * FROM #ProductSearchResults

-- Gather all of the fields to be returned for the requested subset of records.
SELECT
      r.CurrentRow
    , r.CurrentPage
    , r.CurrentRowOnPage
    , r.ProductID
    , pmpd.Culture
    , p.ProductNumber
    , p.Name AS ProductName
    , pd.Description AS ProductDescription
    -- Take advantage of the fact that there will always be a
    -- parent/child category relationship and is not further nested.
    , pc2.Name AS ParentProductCategory
    , pc1.Name AS ChildProductCategory
FROM #ProductSearchResults r
    INNER JOIN SalesLT.Product p ON r.ProductID = p.ProductID
    INNER JOIN SalesLT.ProductModel pm ON p.ProductModelID = pm.ProductModelID
    INNER JOIN SalesLT.ProductModelProductDescription pmpd ON pm.ProductModelID = pmpd.ProductModelID
    INNER JOIN SalesLT.ProductDescription pd ON pmpd.ProductDescriptionID = pd.ProductDescriptionID
    -- Join on additional tables to get information that wasn't required to perform the search.
    INNER JOIN SalesLT.ProductCategory pc1 ON p.ProductCategoryID = pc1.ProductCategoryID
    INNER JOIN SalesLT.ProductCategory pc2 ON pc1.ParentProductCategoryID = pc2.ProductCategoryID
WHERE
    -- Required if you want all records returned and you'll handle pagination in code.
    (r.CurrentPage = @localCurrentPage OR @localCurrentPage = 0)

    -- Use the culture from the temp table.
    -- It's required to get the distinct product description by current row.

    -- You could enforce the culture is provided to this stored procedure.
    -- By doing so you could change this to use the local culture variable.
    AND (r.Culture = pmpd.Culture)
-- *1* The ORDER BY must match for all queries.
ORDER BY p.Name, pmpd.Culture

-- Return pagination information.
SELECT
      @localCurrentPage AS CurrentPage
    , @localEntriesPerPage AS EntriesPerPage
    , COUNT(r.ProductId) AS TotalEntries
    , MAX(r.CurrentPage) AS TotalPages
FROM #ProductSearchResults r

IF OBJECT_ID('tempdb..#ProductSearchResults') IS NOT NULL DROP TABLE #ProductSearchResults

GO

-- Depending on how you plan to implement this stored procedure
-- you should consider restricting access to it to only those
-- in specific roles, such as a company website role.
--GRANT EXECUTE ON [dbo].[ProductSearch] TO [CompanyWebsiteRole]
--GO