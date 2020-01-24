﻿using LetPortal.Core.Persistences;
using LetPortal.Core.Versions;
using LetPortal.Portal.Entities.Databases;
using System;

namespace LetPortal.Versions.Databases
{
    public class Database_0_0_2 : IPortalVersion
    {
        public string VersionNumber => "0.0.2";

        public void Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<DatabaseConnection>(Constants.ServiceManagementDatabaseId);
            versionContext.DeleteData<DatabaseConnection>(Constants.IdentityDatabaseId);
        }

        public void Upgrade(IVersionContext versionContext)
        {
            DatabaseOptions databaseSMOptions = versionContext.ServiceManagementOptions as DatabaseOptions;
            DatabaseOptions databseIdOptions = versionContext.IdentityDbOptions as DatabaseOptions;
            var smDatabase = new DatabaseConnection
            {
                Id = Constants.ServiceManagementDatabaseId,
                ConnectionString = databaseSMOptions.ConnectionString,
                DatabaseConnectionType = Enum.GetName(typeof(ConnectionType), databaseSMOptions.ConnectionType).ToLower(),
                DataSource = databaseSMOptions.Datasource,
                Name = "Service Management"
            };

            var identityDatabase = new DatabaseConnection
            {
                Id = Constants.IdentityDatabaseId,
                ConnectionString = databseIdOptions.ConnectionString,
                DatabaseConnectionType = Enum.GetName(typeof(ConnectionType), databseIdOptions.ConnectionType).ToLower(),
                DataSource = databseIdOptions.Datasource,
                Name = "Identity Database"
            };

            versionContext.InsertData(smDatabase);
            versionContext.InsertData(identityDatabase);
        }
    }
}
