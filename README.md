# Library Inventory Management System - CS4430 - Final Group Project

### Description
This is the Library Inventory Management System (LIMS) made as part of our
WMU Database Management Class Final in 2020. The LIMS replicates a standard 
library inventory tracking system using C# as the base language and MySQL 
as the DBMS. The intended users of the application are employees at a library 
as well as library customers. 

### User Interface Demo  
![UI Demo](imgs/UI%20-%20Demo.gif)

### Dependencies
+ Visual Studio 2019
    - ASP.NET and web development
    - .NET desktop development
    - .NET Core cross-platform development
+ MySQL 8.0.19
    - MySQL Connector/NET 8.0.19
+ Bootstrap 4.4.1
+ jQuery 3.3.1

### Installation
1. Install [Visual Studio 2019](https://visualstudio.microsoft.com/vs/) using the Visual Studio installer  
    a. within the Visual Studio installer, add the "ASP.NET and web development" package  
    b. within the Visual Studio installer, add the ".NET desktop development" package  
    c. within the Visual Studio installer, add the ".NET Core cross-platform development" package  
2. Install [MySQL 8.0.19](https://dev.mysql.com/downloads/mysql/)  
    a. install the [MySQL .NET connector](https://dev.mysql.com/downloads/connector/net/)  
3. Build the database  
    a. Connect to your MySQL Server  
    b. Run the database building script */scripts/create_lims_db.sql*  
4. Configure the database connection in */LIMS/DB_Config.json*  
5. Build the project using the Visual Studio build system

### Features Include: ####
*Guest:*
+ Book search results w/filtering by:
    + Title (default) :heavy_check_mark:
    + Author :heavy_check_mark:
    + Date published
    + Genre :heavy_check_mark:
    + Type (if differents products)

*User:*
+ Placing book reservations :heavy_check_mark:
+ Placing book requests :heavy_check_mark:
+ Reviewing Books :heavy_check_mark:
+ Checking book availability (not reserved/checked out) :heavy_check_mark:

*Employee:*
+ Check out books for users :heavy_check_mark:
+ Seeing when checked out books are available 
+ Seeing what books are in stock :heavy_check_mark:
+ Adding new books and new copies of books to db :heavy_check_mark:

*Manager:*
+ Placing orders for new books :heavy_check_mark:
+ Page for displaying book orders :heavy_check_mark:
+ Page for checking book requests :heavy_check_mark:
