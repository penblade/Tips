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
-- Is there an easier way?