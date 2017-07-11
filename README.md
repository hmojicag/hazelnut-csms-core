# hazelnut-csms-core
The core of the Hazelnut Cloud Storage Management Service

Navigate to your repos folder `$YourReposFolder` and
Clone the repo: `git clone https://github.com/hmojicag/hazelnut-csms-core.git`

# Using with Visual Studio Code
Open Visual Studio Code, Menu File -> Open Folder.
Select `$YourReposFolder/hazelnut-csms-core/`

# To run the CLI App client
Go to `$YourReposFolder/hazelnut-csms-core/hazelnut-csms-core`

```javascript
dotnet restore
dotnet run
```

# To run the Integration Tests
Go to `$YourReposFolder/hazelnut-csms-core/hazelnut-csms-core.test`
```javascript
dotnet test
```

# Wanna make changes to an Entity Framework Model for the SQLite Embedded BD?
The model in `$YourReposFolder/hazelnut-csms-core/hazelnut-csms-core/src/Hazelnut/CLIApp/Model/Model.cs`

In order to apply changes to the DB you first need to delete te .db file `$HOME/HazelnutCSMS/hazelnut-csms.db`

Then go to `$YourReposFolder/hazelnut-csms-core/hazelnut-csms-core` and run:
```javascript
dotnet ef migrations add InitialCreate
dotnet ef database update
```

NOTE: It will create a folder `$YourReposFolder/hazelnut-csms-core/hazelnut-csms-core/Migrations`, each time you change the model you ned to delete the contents of this folder along with the .db file. SQLite does not support all migrations (schema changes) due to limitations in SQLite so it's better to clean up everything: https://docs.microsoft.com/en-us/ef/core/get-started/netcore/new-db-sqlite