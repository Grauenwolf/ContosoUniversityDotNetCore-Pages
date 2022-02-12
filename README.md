# Cleaning Clean Architecture - Vertical Slice Edition

The purpose of this repository is to investigate "Vertical Slice Architecture" and see if it can be improved upon. The goal is to remove complexity without reducing functionality.

To order to see the transformation incrementally, a branch has been created for each step. Simply compare the branch with the one before it to see the progression.


## Round 0 - Base State Validation

Use SSDT to deploy the "local" and "local test" databases. (These instructions supersede the PowerShell scripts.)

All tests are passing and the website appears to be functioning normally.

## Round 1 - Remove MediatR

Throughout the Cleaning Clean Architecture series, a common theme is the unnecessary use of an internal dispatcher. Some use MediatR, others roll their own. 

In our first Cleaning Clean Architecture repository, MediatR was used to actually add functionality. That functionality was better expressed as ASP.NET Core Middleware, but at least it existed.

In our second outing, the internal dispatcher had the illusion of functionality by doing some light logging and DI. Far less impressive, but at least it hinted at what was possible.

This time the use of MediatR is 100% gratuitous. Not only is there no additional functionality added, the handler for the MediatR message is in the same class as the sender.

For example, if you are in an Index page (`Index.cshtml.cs`), then sender is the `OnGetAsync` method of the `Index` class. The handler is the `Index.QueryHandler`, which is an inner class of `Index`.


### Fix

The first part of the fix is easy, 

1. Move the necessary DI components (e.g. the database) into the constructor that formally accepted a MediatR object.
2. Remove the Handler inner classes, leaving the `Handle` functions in outer class. 
3. Change all calls to `_mediate.Send` to `Handle`.
4. Ensure the `Handle` method actually call `SaveChanges()`. (Some of them didn't, and that was causing the updated tests to fail.)

## Round 2 - Fix Page Sizes
 
Currently the page size for Student is hard-coded inside the `Handle` method. This is the wrong place. Page size is a UI concern and the UI may wish to vary it depending on things such as user input or the size of the screen.

So we're going to move it from the data access method, `Handle`, to the UI method, `OnGetAsync`. 

At the same time, we're going to change the default page size to 10. While there is no perfect number, certain values such as 5, 10, 25, 50, and 100 are common. Conversely, numbers such as 3, 4, 8, and 9 are not expected by the user. So upon seeing such counts, will think they are at the end of the list.


## Round 3 - Managing the SQL 

The database schema is stored in an unusual place. It consists of two SQL scripts in the website's App_Data folder. That's not really an ergonomic way of handling database schema. Developers aren't likely to think to look there, assuming instead that the database schema is managed by EF Core. 

In the short term, dropping all of the tables into one script is like dropping all of your classes into one file. While technically allowed, it is considered bad form and makes it harder to find code of interest.

Moreover, over time the number of scripts will increase. Each time a table or column is altered, a new script needs to be created. Numbering becomes a concern when branching-and-merging come into play. And you have to read all of the scripts in order to see what the end-state looks like.

Fortunately, there is an elegant alternative called SQL Server Data Tools or SSDT.

Note: In order to avoid breaking the existing PowerShell scripts, the `App_Data\runAfterCreateDatabase\` folder will not be removed at this time.

# ContosoUniversity on ASP.NET Core 6.0 on .NET 6 and Razor Pages

Contoso University, the way I would write it.

This example requires some tools and PowerShell modules, you should run `setup.cmd` or `setup.ps1` to install them.

To prepare the database, execute the build script using [PSake](https://psake.readthedocs.io/): `psake migrate`. Open the solution and run!

## Things demonstrated

- CQRS and MediatR
- AutoMapper
- Vertical slice architecture
- Razor Pages
- Fluent Validation
- HtmlTags
- Entity Framework Core

## Migrating the Database

Grate will automatically create or upgrade (migrate) the database to the latest schema version when you run it:

From PowerShell:
```
invoke-psake migrate
```

From CMD:
```
psake migrate
```

When running unit tests, you can recreate the unit test database using:

```
invoke-psake migratetest
```

## Versioning

Version numbers can be passed on the build script command line:

From PowerShell:
```
invoke-psake CI -properties ${'version':'1.2.3-dev.5'}
```

Because we're passing a PowerShell dictionary on the command line, the cmd script doesn't handle this very nicely.

Or generate a version using [GitVersion](https://gitversion.net/docs/) locally:
```
psake localversion
```
will generate a semantic version and output it.


