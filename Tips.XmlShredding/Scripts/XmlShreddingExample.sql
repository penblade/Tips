-- XML Shredding is the process of parsing XML into something more useful and easier to query.
-- The following examples will demonstrate some of the basic methods for shredding your XML.
-- This article assumes some basic knowledge of XML and XPATH, however, most of these
-- concepts are covered under the references at the end of the article.

------------------
-- Define your XML sample
------------------

-- First, let's define a very simple XML that contains a couple root elements, a collection,
-- nested elements, and a couple attributes.

DECLARE @x XML

SET @x = '
<Library>
  <Books>
    <Book Type="Paperback">
      <Author>Robert Jordan</Author>
      <Id>26</Id>
      <PublicationDate>01/15/1990</PublicationDate>
      <Series>
        <Name>The Wheel of Time</Name>
        <Number>1</Number>
      </Series>
      <Title Chapters="53">The Eye of the World</Title>
    </Book>
    <Book Type="Hardback">
      <Author>Robert Jordan</Author>
      <Id>87</Id>
      <PublicationDate>09/15/1992</PublicationDate>
      <Series>
        <Name>The Wheel of Time</Name>
        <Number>4</Number>
      </Series>
      <Title Chapters="58">The Shadow Rising</Title>
    </Book>
    <Book Type="eBook">
      <Author>Robert Jordan</Author>
      <Id>43</Id>
      <PublicationDate>05/15/1996</PublicationDate>
      <Series>
        <Name>The Wheel of Time</Name>
        <Number>7</Number>
      </Series>
      <Title Chapters="41">A Crown of Swords</Title>
    </Book>
  </Books>
  <Id>51</Id>
  <Name>We Have Books... Read Them or Else!</Name>
</Library>'

------------------
-- .value
------------------

-- Next, let's parse this data with the "value" method.
-- It requires an XPATH within parenthesis,
-- the [1] meaning the first element returned,
-- followed by the second parameter the value type,
-- INT, DATE, VARCHAR(MAX), etc.

SELECT @x.value('(/Library/Id)[1]', 'INT') AS LibraryId,
       @x.value('(/Library/Name)[1]', 'VARCHAR(MAX)') AS LibraryName

-- .value Results

--LibraryId   LibraryName
------------- ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--51          We Have Books... Read Them or Else!

--(1 row affected)

------------------
-- .nodes
------------------

-- .nodes allows you to access collections.
-- Again, use a XPATH string to identify the elements.
-- Follwing the method call define an alias of the
-- collection and the alias of the element.
-- In this example, that's Books(Book), but
-- it could just as easily have been t(c).
-- Use .value to access the node you want just
-- like before, however, now you'll want to
-- use ./ to start within the scope of the element.
-- Of course you may use any XPATH.
-- Notice that you access attributes by navigating
-- to the node and then use the @AttributeName
-- to access it.

SELECT Books.Book.value('(./Id)[1]', 'INT') AS Id,
       Books.Book.value('(./Title)[1]', 'VARCHAR(MAX)') AS Title,
       Books.Book.value('(./Author)[1]', 'VARCHAR(MAX)') AS Author,
       Books.Book.value('(./PublicationDate)[1]', 'DATE') AS PublicationDate,
       Books.Book.value('(./Series/Name)[1]', 'VARCHAR(MAX)') AS SeriesName,
       Books.Book.value('(./Series/Number)[1]', 'VARCHAR(MAX)') AS SeriesNumber,
       Books.Book.value('@Type', 'VARCHAR(MAX)') AS BookType,
       Books.Book.value('(./Title/@Chapters)[1]', 'INT') AS Chapters
FROM @x.nodes('(/Library/Books/Book)') AS Books(Book)

-- .nodes Results

--Id          Title                                                                                                                                                                                                                                                            Author                                                                                                                                                                                                                                                           PublicationDate SeriesName                                                                                                                                                                                                                                                       SeriesNumber
------------- ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- --------------- ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--26          The Eye of the World                                                                                                                                                                                                                                             Robert Jordan                                                                                                                                                                                                                                                    1990-01-15      The Wheel of Time                                                                                                                                                                                                                                                1
--87          The Shadow Rising                                                                                                                                                                                                                                                Robert Jordan                                                                                                                                                                                                                                                    1992-09-15      The Wheel of Time                                                                                                                                                                                                                                                4
--43          A Crown of Swords                                                                                                                                                                                                                                                Robert Jordan                                                                                                                                                                                                                                                    1996-05-15      The Wheel of Time                                                                                                                                                                                                                                                7

--(3 rows affected)

------------------
-- Define your XML sample with namespaces
------------------

-- Adjust the original XML sample by adding
-- namespaces on the Library and Books nodes.

SET @x = '
<Library xmlns="http://www.example.com/library">
  <Books xmlns="http://www.example.com/library/books">
    <Book Type="Paperback">
      <Author>Robert Jordan</Author>
      <Id>26</Id>
      <PublicationDate>01/15/1990</PublicationDate>
      <Series>
        <Name>The Wheel of Time</Name>
        <Number>1</Number>
      </Series>
      <Title Chapters="53">The Eye of the World</Title>
    </Book>
    <Book Type="Hardback">
      <Author>Robert Jordan</Author>
      <Id>87</Id>
      <PublicationDate>09/15/1992</PublicationDate>
      <Series>
        <Name>The Wheel of Time</Name>
        <Number>4</Number>
      </Series>
      <Title Chapters="58">The Shadow Rising</Title>
    </Book>
    <Book Type="eBook">
      <Author>Robert Jordan</Author>
      <Id>43</Id>
      <PublicationDate>05/15/1996</PublicationDate>
      <Series>
        <Name>The Wheel of Time</Name>
        <Number>7</Number>
      </Series>
      <Title Chapters="41">A Crown of Swords</Title>
    </Book>
  </Books>
  <Id>51</Id>
  <Name>We Have Books... Read Them or Else!</Name>
</Library>'

------------------
-- .value with namespaces
------------------

-- .value works the same except now you declare the namespace
-- before the XPATH, separating the two with a semicolan.
-- .nodes works the same way, but adding this declaration
-- reduces the readability.

SELECT @x.value('declare namespace l="http://www.example.com/library";
                 (/l:Library/l:Id)[1]', 'INT') AS LibraryId,
       @x.value('declare namespace l="http://www.example.com/library";
                 (/l:Library/l:Name)[1]', 'VARCHAR(MAX)') AS LibraryName

-- .value with namespaces Results

--LibraryId   LibraryName
------------- ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--51          We Have Books... Read Them or Else!

-- Declaring namespaces for each value is annoying.  Let's wrap it with a using statement.

------------------
-- .value and .nodes with XMLNAMESPACES
------------------

-- Add the ;WITH XMLNAMESPACES before each of your select statements
-- as if you were creating a Common Table Expression (CTE).
-- Before each of your nodes in the XPATH add the namespace prefix.
-- Note that the prefix was not in the actual XML.

;WITH XMLNAMESPACES ('http://www.example.com/library' AS l)
SELECT @x.value('(/l:Library/l:Id)[1]', 'INT') AS LibraryId,
       @x.value('(/l:Library/l:Name)[1]', 'VARCHAR(MAX)') AS LibraryName

;WITH XMLNAMESPACES ('http://www.example.com/library' AS l,
                     'http://www.example.com/library/books' AS b)
SELECT Books.Book.value('(./b:Id)[1]', 'INT') AS Id,
       Books.Book.value('(./b:Title)[1]', 'VARCHAR(MAX)') AS Title,
       Books.Book.value('(./b:Author)[1]', 'VARCHAR(MAX)') AS Author,
       Books.Book.value('(./b:PublicationDate)[1]', 'DATE') AS PublicationDate,
       Books.Book.value('(./b:Series/b:Name)[1]', 'VARCHAR(MAX)') AS SeriesName,
       Books.Book.value('(./b:Series/b:Number)[1]', 'VARCHAR(MAX)') AS SeriesNumber,
       Books.Book.value('@Type', 'VARCHAR(MAX)') AS BookType,
       Books.Book.value('(./b:Title/@Chapters)[1]', 'INT') AS Chapters
FROM @x.nodes('(/l:Library/b:Books/b:Book)') AS Books(Book)

-- .value and .nodes with namespaces Results

--(1 row affected)

--LibraryId   LibraryName
------------- ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--51          We Have Books... Read Them or Else!

--(1 row affected)

--Id          Title                                                                                                                                                                                                                                                            Author                                                                                                                                                                                                                                                           PublicationDate SeriesName                                                                                                                                                                                                                                                       SeriesNumber
------------- ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- --------------- ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--26          The Eye of the World                                                                                                                                                                                                                                             Robert Jordan                                                                                                                                                                                                                                                    1990-01-15      The Wheel of Time                                                                                                                                                                                                                                                1
--87          The Shadow Rising                                                                                                                                                                                                                                                Robert Jordan                                                                                                                                                                                                                                                    1992-09-15      The Wheel of Time                                                                                                                                                                                                                                                4
--43          A Crown of Swords                                                                                                                                                                                                                                                Robert Jordan                                                                                                                                                                                                                                                    1996-05-15      The Wheel of Time                                                                                                                                                                                                                                                7

--(3 rows affected)

------------------
-- Multiple XML
------------------

-- Of course once you've managed to shred one xml you'll want to do more.
-- You can read the XML from files and queries, let's try an example from a temp table.
-- We'll populate it with the XML data we just tested into three rows.

IF OBJECT_ID('tempdb..#XmlShreddingTable') IS NOT NULL DROP TABLE #XmlShreddingTable
CREATE TABLE #XmlShreddingTable ( XmlId INT, XmlData XML )

INSERT INTO #XmlShreddingTable
VALUES (1, @x), (2, @x), (3, @x)

SELECT * FROM #XmlShreddingTable

-- For the collection of books we'll cross apply those rows
-- against the nodes to get our results for all of the record sets.
-- Then we could insert it into other tables, files,
-- or wherever we need to store the shredded data.

;WITH XMLNAMESPACES ('http://www.example.com/library' AS l),
      Records AS
(
    SELECT xst.XmlId, xst.XmlData FROM #XmlShreddingTable xst
)
SELECT Records.XmlId,
       Records.XmlData.value('(/l:Library/l:Id)[1]', 'INT') AS LibraryId,
       Records.XmlData.value('(/l:Library/l:Name)[1]', 'VARCHAR(MAX)') AS LibraryName
FROM Records

--XmlId       LibraryId   LibraryName
------------- ----------- ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--1           51          We Have Books... Read Them or Else!
--2           51          We Have Books... Read Them or Else!
--3           51          We Have Books... Read Them or Else!

--(3 rows affected)

;WITH XMLNAMESPACES ('http://www.example.com/library' AS l,
                     'http://www.example.com/library/books' AS b),
      Records AS
(
    SELECT xst.XmlId, xst.XmlData FROM #XmlShreddingTable xst
)
SELECT Records.XmlId,
       Books.Book.value('(./b:Id)[1]', 'INT') AS Id,
       Books.Book.value('(./b:Title)[1]', 'VARCHAR(MAX)') AS Title,
       Books.Book.value('(./b:Author)[1]', 'VARCHAR(MAX)') AS Author,
       Books.Book.value('(./b:PublicationDate)[1]', 'DATE') AS PublicationDate,
       Books.Book.value('(./b:Series/b:Name)[1]', 'VARCHAR(MAX)') AS SeriesName,
       Books.Book.value('(./b:Series/b:Number)[1]', 'VARCHAR(MAX)') AS SeriesNumber,
       Books.Book.value('@Type', 'VARCHAR(MAX)') AS BookType,
       Books.Book.value('(./b:Title/@Chapters)[1]', 'INT') AS Chapters
FROM Records CROSS APPLY Records.XmlData.nodes('(/l:Library/b:Books/b:Book)') AS Books(Book)

--XmlId       Id          Title                                                                                                                                                                                                                                                            Author                                                                                                                                                                                                                                                           PublicationDate SeriesName                                                                                                                                                                                                                                                       SeriesNumber                                                                                                                                                                                                                                                     BookType                                                                                                                                                                                                                                                         Chapters
------------- ----------- ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- --------------- ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- -----------
--1           26          The Eye of the World                                                                                                                                                                                                                                             Robert Jordan                                                                                                                                                                                                                                                    1990-01-15      The Wheel of Time                                                                                                                                                                                                                                                1                                                                                                                                                                                                                                                                Paperback                                                                                                                                                                                                                                                        53
--1           87          The Shadow Rising                                                                                                                                                                                                                                                Robert Jordan                                                                                                                                                                                                                                                    1992-09-15      The Wheel of Time                                                                                                                                                                                                                                                4                                                                                                                                                                                                                                                                Hardback                                                                                                                                                                                                                                                         58
--1           43          A Crown of Swords                                                                                                                                                                                                                                                Robert Jordan                                                                                                                                                                                                                                                    1996-05-15      The Wheel of Time                                                                                                                                                                                                                                                7                                                                                                                                                                                                                                                                eBook                                                                                                                                                                                                                                                            41
--2           26          The Eye of the World                                                                                                                                                                                                                                             Robert Jordan                                                                                                                                                                                                                                                    1990-01-15      The Wheel of Time                                                                                                                                                                                                                                                1                                                                                                                                                                                                                                                                Paperback                                                                                                                                                                                                                                                        53
--2           87          The Shadow Rising                                                                                                                                                                                                                                                Robert Jordan                                                                                                                                                                                                                                                    1992-09-15      The Wheel of Time                                                                                                                                                                                                                                                4                                                                                                                                                                                                                                                                Hardback                                                                                                                                                                                                                                                         58
--2           43          A Crown of Swords                                                                                                                                                                                                                                                Robert Jordan                                                                                                                                                                                                                                                    1996-05-15      The Wheel of Time                                                                                                                                                                                                                                                7                                                                                                                                                                                                                                                                eBook                                                                                                                                                                                                                                                            41
--3           26          The Eye of the World                                                                                                                                                                                                                                             Robert Jordan                                                                                                                                                                                                                                                    1990-01-15      The Wheel of Time                                                                                                                                                                                                                                                1                                                                                                                                                                                                                                                                Paperback                                                                                                                                                                                                                                                        53
--3           87          The Shadow Rising                                                                                                                                                                                                                                                Robert Jordan                                                                                                                                                                                                                                                    1992-09-15      The Wheel of Time                                                                                                                                                                                                                                                4                                                                                                                                                                                                                                                                Hardback                                                                                                                                                                                                                                                         58
--3           43          A Crown of Swords                                                                                                                                                                                                                                                Robert Jordan                                                                                                                                                                                                                                                    1996-05-15      The Wheel of Time                                                                                                                                                                                                                                                7                                                                                                                                                                                                                                                                eBook                                                                                                                                                                                                                                                            41

--(9 rows affected)

-- Instead of using a Common Table Expression (CTE) you could
-- move the select statement into the FROM statement as an inline
-- query as follows.

;WITH XMLNAMESPACES ('http://www.example.com/library' AS l)
SELECT Records.XmlId,
       Records.XmlData.value('(/l:Library/l:Id)[1]', 'INT') AS LibraryId,
       Records.XmlData.value('(/l:Library/l:Name)[1]', 'VARCHAR(MAX)') AS LibraryName
FROM (SELECT xst.XmlId, xst.XmlData FROM #XmlShreddingTable xst) AS Records

--XmlId       LibraryId   LibraryName
------------- ----------- ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--1           51          We Have Books... Read Them or Else!
--2           51          We Have Books... Read Them or Else!
--3           51          We Have Books... Read Them or Else!

--(3 rows affected)

;WITH XMLNAMESPACES ('http://www.example.com/library' AS l,
                     'http://www.example.com/library/books' AS b)
SELECT Records.XmlId,
       Books.Book.value('(./b:Id)[1]', 'INT') AS Id,
       Books.Book.value('(./b:Title)[1]', 'VARCHAR(MAX)') AS Title,
       Books.Book.value('(./b:Author)[1]', 'VARCHAR(MAX)') AS Author,
       Books.Book.value('(./b:PublicationDate)[1]', 'DATE') AS PublicationDate,
       Books.Book.value('(./b:Series/b:Name)[1]', 'VARCHAR(MAX)') AS SeriesName,
       Books.Book.value('(./b:Series/b:Number)[1]', 'VARCHAR(MAX)') AS SeriesNumber,
       Books.Book.value('@Type', 'VARCHAR(MAX)') AS BookType,
       Books.Book.value('(./b:Title/@Chapters)[1]', 'INT') AS Chapters
FROM (SELECT xst.XmlId, xst.XmlData FROM #XmlShreddingTable xst) AS Records
      CROSS APPLY Records.XmlData.nodes('(/l:Library/b:Books/b:Book)') AS Books(Book)

--XmlId       Id          Title                                                                                                                                                                                                                                                            Author                                                                                                                                                                                                                                                           PublicationDate SeriesName                                                                                                                                                                                                                                                       SeriesNumber                                                                                                                                                                                                                                                     BookType                                                                                                                                                                                                                                                         Chapters
------------- ----------- ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- --------------- ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- -----------
--1           26          The Eye of the World                                                                                                                                                                                                                                             Robert Jordan                                                                                                                                                                                                                                                    1990-01-15      The Wheel of Time                                                                                                                                                                                                                                                1                                                                                                                                                                                                                                                                Paperback                                                                                                                                                                                                                                                        53
--1           87          The Shadow Rising                                                                                                                                                                                                                                                Robert Jordan                                                                                                                                                                                                                                                    1992-09-15      The Wheel of Time                                                                                                                                                                                                                                                4                                                                                                                                                                                                                                                                Hardback                                                                                                                                                                                                                                                         58
--1           43          A Crown of Swords                                                                                                                                                                                                                                                Robert Jordan                                                                                                                                                                                                                                                    1996-05-15      The Wheel of Time                                                                                                                                                                                                                                                7                                                                                                                                                                                                                                                                eBook                                                                                                                                                                                                                                                            41
--2           26          The Eye of the World                                                                                                                                                                                                                                             Robert Jordan                                                                                                                                                                                                                                                    1990-01-15      The Wheel of Time                                                                                                                                                                                                                                                1                                                                                                                                                                                                                                                                Paperback                                                                                                                                                                                                                                                        53
--2           87          The Shadow Rising                                                                                                                                                                                                                                                Robert Jordan                                                                                                                                                                                                                                                    1992-09-15      The Wheel of Time                                                                                                                                                                                                                                                4                                                                                                                                                                                                                                                                Hardback                                                                                                                                                                                                                                                         58
--2           43          A Crown of Swords                                                                                                                                                                                                                                                Robert Jordan                                                                                                                                                                                                                                                    1996-05-15      The Wheel of Time                                                                                                                                                                                                                                                7                                                                                                                                                                                                                                                                eBook                                                                                                                                                                                                                                                            41
--3           26          The Eye of the World                                                                                                                                                                                                                                             Robert Jordan                                                                                                                                                                                                                                                    1990-01-15      The Wheel of Time                                                                                                                                                                                                                                                1                                                                                                                                                                                                                                                                Paperback                                                                                                                                                                                                                                                        53
--3           87          The Shadow Rising                                                                                                                                                                                                                                                Robert Jordan                                                                                                                                                                                                                                                    1992-09-15      The Wheel of Time                                                                                                                                                                                                                                                4                                                                                                                                                                                                                                                                Hardback                                                                                                                                                                                                                                                         58
--3           43          A Crown of Swords                                                                                                                                                                                                                                                Robert Jordan                                                                                                                                                                                                                                                    1996-05-15      The Wheel of Time                                                                                                                                                                                                                                                7                                                                                                                                                                                                                                                                eBook                                                                                                                                                                                                                                                            41

--(9 rows affected)

------------------
-- Other Considerations
------------------

-- There are plenty of ways to perform these steps
-- some more efficient than others.  Other options
-- could be to create a while loop or create cursors
-- to call stored procedures that perform xml shredding
-- and inserts each to a single table.  This would
-- allow you separate the concerns and follow a
-- command pattern with a master stored procedure
-- that calls all of the others.

-- If you are creating this data for reports, you
-- may want to consider performing a bulk load of
-- all the data from the xml shredding.
-- After that you may want to have the stored procedure
-- performing this task accept a date, date range, or flag
-- so you only shred the xmls for those records not
-- yet processed.

------------------
-- References
------------------

-- https://docs.microsoft.com/en-us/sql/xquery/xquery-language-reference-sql-server?view=sql-server-2017
-- XQuery Language Reference.  There are many ways to shred XML.

-- https://www.w3schools.com/xml/xpath_syntax.asp
-- XPath Tutorial - Syntax

-- https://docs.microsoft.com/en-us/sql/t-sql/xml/with-xmlnamespaces?view=sql-server-2017
-- Taught me how to use ;WITH XMLNAMESPACES.

-- https://docs.microsoft.com/en-us/sql/t-sql/xml/value-method-xml-data-type?view=sql-server-2017
-- https://stackoverflow.com/questions/1302064/xml-query-works-value-requires-singleton-found-xdtuntypedatomic
-- Taught me how to shred a single value.

-- https://stackoverflow.com/questions/1890923/xpath-to-fetch-sql-xml-value
-- Taught me how to shred collections of nodes.

-- https://docs.microsoft.com/en-us/sql/t-sql/xml/value-method-xml-data-type?view=sql-server-2017
-- Taught me how to shred attributes.

-- https://www.mssqltips.com/sqlservertip/1958/sql-server-cross-apply-and-outer-apply/
-- Taught me how to perform a cross apply.

-- https://docs.microsoft.com/en-us/sql/t-sql/queries/with-common-table-expression-transact-sql?view=sql-server-2017
-- Common Table Expression (CTE) overview.

-- https://stackoverflow.com/questions/61233/the-best-way-to-shred-xml-data-into-sql-server-database-columns
-- Performant XML Shredding - takes the basics of what was taught here and made it efficent when you need
-- to perform joins using CROSS APPLY and OUTER APPLY.

-- https://en.wikipedia.org/wiki/The_Wheel_of_Time
-- These books were awesome!

------------------
-- Conclusion
------------------

-- Now that you've extracted the data from the XML,
-- you can save the data to tables in your database,
-- maybe store them in temp tables to generate a
-- report.  There are plenty of options, this is
-- just the beginning.  How do you shred your XML?
-- Is there an easier way?  Do you know of a way
-- to make this perform faster and more efficiently?