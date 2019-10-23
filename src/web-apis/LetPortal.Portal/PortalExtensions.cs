﻿using LetPortal.Core;
using LetPortal.Core.Files;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Executions;
using LetPortal.Portal.Executions.Mongo;
using LetPortal.Portal.Executions.MySQL;
using LetPortal.Portal.Executions.PostgreSql;
using LetPortal.Portal.Executions.SqlServer;
using LetPortal.Portal.Options.Files;
using LetPortal.Portal.Persistences;
using LetPortal.Portal.Providers.Databases;
using LetPortal.Portal.Providers.EntitySchemas;
using LetPortal.Portal.Providers.Pages;
using LetPortal.Portal.Repositories;
using LetPortal.Portal.Repositories.Apps;
using LetPortal.Portal.Repositories.Components;
using LetPortal.Portal.Repositories.Databases;
using LetPortal.Portal.Repositories.Datasources;
using LetPortal.Portal.Repositories.EntitySchemas;
using LetPortal.Portal.Repositories.Files;
using LetPortal.Portal.Repositories.Pages;
using LetPortal.Portal.Services.Components;
using LetPortal.Portal.Services.Databases;
using LetPortal.Portal.Services.Datasources;
using LetPortal.Portal.Services.EntitySchemas;
using LetPortal.Portal.Services.Files;
using LetPortal.Portal.Services.Files.Validators;
using LetPortal.Portal.Services.Http;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LetPortal.Portal
{
    public static class PortalExtensions
    {
        public static ILetPortalBuilder AddPortalService(
            this ILetPortalBuilder builder,
            Action<PortalOptions> action = null)
        {
            var portalOptions = new PortalOptions();
            if(action != null)
            {
                action.Invoke(portalOptions);
            }
            if(builder.ConnectionType == ConnectionType.MongoDB)
            {
                MongoDbRegistry.RegisterEntities();

                // Register all mongo repositories
                builder.Services.AddSingleton<IDatabaseRepository, DatabaseMongoRepository>();
                builder.Services.AddSingleton<IDatasourceRepository, DatasourceMongoRepository>();
                builder.Services.AddSingleton<IEntitySchemaRepository, EntitySchemaMongoRepository>();
                builder.Services.AddSingleton<IAppRepository, AppMongoRepository>();
                builder.Services.AddSingleton<IAppVersionRepository, AppVersionMongoRepository>();
                builder.Services.AddSingleton<IPageRepository, PageMongoRepository>();
                builder.Services.AddSingleton<IDynamicListRepository, DynamicListMongoRepository>();
                builder.Services.AddSingleton<IStandardRepository, StandardMongoRepository>();
                builder.Services.AddSingleton<IFileRepository, FileMongoRepository>();
                builder.Services.AddSingleton<IChartRepository, ChartMongoRepository>();
            }

            if(builder.ConnectionType == ConnectionType.SQLServer
                || builder.ConnectionType == ConnectionType.PostgreSQL
                || builder.ConnectionType == ConnectionType.MySQL)
            {
                builder.Services.AddTransient<LetPortalDbContext>();
                // Register all EF repositories
                builder.Services.AddTransient<IDatabaseRepository, DatabaseEFRepository>();
                builder.Services.AddTransient<IDatasourceRepository, DatasourceEFRepository>();
                builder.Services.AddTransient<IEntitySchemaRepository, EntitySchemaEFRepository>();
                builder.Services.AddTransient<IAppRepository, AppEFRepository>();
                builder.Services.AddTransient<IPageRepository, PageEFRepository>();
                builder.Services.AddTransient<IDynamicListRepository, DynamicListEFRepository>();
                builder.Services.AddTransient<IStandardRepository, StandardEFRepository>();
                builder.Services.AddTransient<IFileRepository, FileEFRepository>();
                builder.Services.AddTransient<IChartRepository, ChartEFRepository>();
            }

            if(portalOptions.EnableFileServer)
            {
                builder.Services.Configure<FileOptions>(builder.Configuration.GetSection("FileOptions"));
                builder.Services.Configure<FileValidatorOptions>(builder.Configuration.GetSection("FileOptions").GetSection("FileValidatorOptions"));
                builder.Services.Configure<DiskStorageOptions>(builder.Configuration.GetSection("FileOptions").GetSection("DiskStorageOptions"));
                builder.Services.Configure<DatabaseStorageOptions>(builder.Configuration.GetSection("FileOptions").GetSection("DatabaseStorageOptions"));

                builder.Services.AddTransient<IFileConnectorExecution, DiskFileConnectorExecution>();
                builder.Services.AddTransient<IFileConnectorExecution, DatabaseFileConnectorExecution>();
                builder.Services.AddTransient<IFileValidatorRule, CheckFileExtensionRule>();
                builder.Services.AddTransient<IFileValidatorRule, CheckFileSizeRule>();

                builder.Services.AddTransient<IStoreFileDatabase, MySqlStoreFileDatabase>();
                builder.Services.AddTransient<IStoreFileDatabase, SqlServerStoreFileDatabase>();
                builder.Services.AddTransient<IStoreFileDatabase, PostgreStoreFileDatabase>();
                builder.Services.AddTransient<IStoreFileDatabase, MongoStoreFileDatabase>();
            }

            if(builder.ConnectionType == ConnectionType.MongoDB)
            {
                builder.Services.AddTransient<IExecutionDatabase, MongoExecutionDatabase>();
                builder.Services.AddTransient<IExtractionDatabase, MongoExtractionDatabase>();
                builder.Services.AddTransient<IExtractionDatasource, MongoExtractionDatasource>();
                builder.Services.AddTransient<IDynamicListQueryDatabase, MongoDynamicListQueryDatabase>();
                builder.Services.AddTransient<IAnalyzeDatabase, MongoAnalyzeDatabase>();
            }
            else if(builder.ConnectionType == ConnectionType.PostgreSQL)
            {
                builder.Services.AddTransient<IAnalyzeDatabase, PostgreAnalyzeDatabase>();
                builder.Services.AddTransient<IExecutionDatabase, PostgreExecutionDatabase>();
                builder.Services.AddTransient<IExtractionDatabase, PostgreExtractionDatabase>();
                builder.Services.AddTransient<IDynamicListQueryDatabase, PostgreDynamicListQueryDatabase>();
                builder.Services.AddTransient<IExtractionDatasource, PostgreExtractionDatasource>();
            }
            else if(builder.ConnectionType == ConnectionType.SQLServer)
            {
                builder.Services.AddTransient<IAnalyzeDatabase, SqlServerAnalyzeDatabase>();
                builder.Services.AddTransient<IExecutionDatabase, SqlServerExecutionDatabase>();
                builder.Services.AddTransient<IExtractionDatabase, SqlServerExtractionDatabase>();
                builder.Services.AddTransient<IDynamicListQueryDatabase, SqlServerDynamicListQueryDatabase>();
                builder.Services.AddTransient<IExtractionDatasource, SqlServerExtractionDatasource>();
            }
            else if(builder.ConnectionType == ConnectionType.MySQL)
            {
                builder.Services.AddTransient<IAnalyzeDatabase, MySqlAnalyzeDatabase>();
                builder.Services.AddTransient<IExecutionDatabase, MySqlExecutionDatabase>();
                builder.Services.AddTransient<IExtractionDatabase, MySqlExtractionDatabase>();
                builder.Services.AddTransient<IDynamicListQueryDatabase, MySqlDynamicListQueryDatabase>();
                builder.Services.AddTransient<IExtractionDatasource, MySqlExtractionDatasource>();
            }

            builder.Services.AddTransient<IDatabaseServiceProvider, InternalDatabaseServiceProvider>();            
            builder.Services.AddTransient<IEntitySchemaServiceProvider, InternalEntitySchemaServiceProvider>();
            builder.Services.AddTransient<IPageServiceProvider, InternalPageServiceProvider>();

            builder.Services.AddTransient<IDynamicQueryBuilder, DynamicQueryBuilder>();

            builder.Services.AddTransient<IEntitySchemaService, EntitySchemaService>();
            builder.Services.AddTransient<IDynamicListService, DynamicListService>();
            builder.Services.AddTransient<IDatasourceService, DatasourceService>();
            builder.Services.AddTransient<IDatabaseService, DatabaseService>();
            builder.Services.AddTransient<IFileService, FileService>();
            builder.Services.AddTransient<HttpService>();
            builder.Services.AddHttpClient<HttpService>();
            return builder;
        }
    }
}
