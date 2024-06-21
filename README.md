# Applying Migrations in Ef core
## Create Migrations
    1. Windows command prompt: "Add-Migration MigrationName [options]"
    2. dotnet CLI: dotnet ef migrations add MigrationName [options]"
    ==> EX-PMC: "Add-Migration InitialMigration"
    ==> EX-CLI: "dotnet ef migrations add  InitialMigration --output-dir Your/Directory"

## Applying Created Migration
    1. Windows command prompt: "Update-Database [options]"
    2. dotnet CLI: dotnet ef database update [options]"
    ==> EX-PMC: "Update-Database"

## Removing a Migration
    1. Windows command prompt: "Remove-Migration [options]"
    2. dotnet CLI: dotnet ef migrations remove [options]"
    ==> EX-PMC: "Remove-Migration"