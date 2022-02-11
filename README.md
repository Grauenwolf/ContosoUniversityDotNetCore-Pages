# Cleaning Clean Architecture - Jimmy Bogard’s Vertical Slice Edition

The purpose of this repository is to investigate "Vertical Slice Architecture" and see if it can be improved upon. The goal is to remove complexity without reducing functionality.

To order to see the transformation incrementally, a branch has been created for each step. Simply compare the branch with the one before it to see the progression.


## Round 0 - Base State Validation

Before trying to run the setup scripts, it is helpful to disable PowerShell's security. This is generally considered safe because an unsigned PowerShell script is no more dangerous than a CMD style batch file, and the latter is not blocked by default.

```
Set-ExecutionPolicy -ExecutionPolicy Unrestricted
```

Once that is done, you can run the setup instructions at the bottom of this page to install the databases. There is a separate one for the website and the integration tests.

All tests are passing and the website appears to be functioning normally.

## Round 1 - Remove MediatR

Throughout the Cleaning Clean Architecture series, a common theme is the unnecessary use of an internal dispatcher. Some use MediatR, others roll their own. 

In our first Cleaning Clean Architecture repository, MediatR was used to actually add functionality. That functionality was better expressed as ASP.NET Core Middleware, but at least it existed.

In our second outing, the internal dispatcher had the illusion of functionality by doing some light logging and DI. Far less impressive, but at least it hinted at what was possible.

This time the use of MediatR is 100% gratuitous. Not only is there no additional functionality added, the handler for the MediatR message is in the same class as the sender.

For example, if you are in an Index page (`Index.cshtml.cs`), then sender is the `OnGetAsync` method of the `Index` class. The handler is the `Index.QueryHandler`, which is an inner class of `Index`.






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


