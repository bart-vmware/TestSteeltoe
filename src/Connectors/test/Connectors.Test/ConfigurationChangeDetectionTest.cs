// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Steeltoe.Common.TestResources;
using Steeltoe.Connectors.PostgreSql;

namespace Steeltoe.Connectors.Test;

public sealed class ConfigurationChangeDetectionTest
{
    [Fact]
    public async Task Applies_local_configuration_changes_using_WebApplicationBuilder()
    {
        WebApplicationBuilder builder = TestWebApplicationBuilderFactory.Create();

        string fileContents = """
            {
              "Steeltoe": {
                "Client": {
                  "PostgreSql": {
                    "examplePostgreSqlService": {
                      "ConnectionString": "SERVER=one.com;DB=first"
                    }
                  }
                }
              }
            }
            """;

        var fileProvider = new MemoryFileProvider();
        fileProvider.IncludeFile(MemoryFileProvider.DefaultAppSettingsFileName, fileContents);

        builder.Configuration.AddJsonFile(fileProvider, MemoryFileProvider.DefaultAppSettingsFileName, false, true);
        builder.AddPostgreSql(configureOptions => configureOptions.DetectConfigurationChanges = true, null);

        await using WebApplication app = builder.Build();

        var connectorFactory = app.Services.GetRequiredService<ConnectorFactory<PostgreSqlOptions, NpgsqlConnection>>();

        connectorFactory.ServiceBindingNames.Should().ContainSingle().Which.Should().Be("examplePostgreSqlService");

        string? connectionString = connectorFactory.Get("examplePostgreSqlService").Options.ConnectionString;
        connectionString.Should().Be("Host=one.com;Database=first");

        fileContents = """
        {
          "Steeltoe": {
            "Client": {
              "PostgreSql": {
                "examplePostgreSqlService": {
                  "ConnectionString": "SERVER=two.com;DB=second"
                }
              }
            }
          }
        }
        """;

        fileProvider.ReplaceFile(MemoryFileProvider.DefaultAppSettingsFileName, fileContents);
        fileProvider.NotifyChanged();

        connectionString = connectorFactory.Get("examplePostgreSqlService").Options.ConnectionString;
        connectionString.Should().Be("Host=two.com;Database=second");

        fileContents = """
        {
          "Steeltoe": {
            "Client": {
              "PostgreSql": {
                "examplePostgreSqlService": {
                  "ConnectionString": "SERVER=three.com;DB=third"
                }
              }
            }
          }
        }
        """;

        fileProvider.ReplaceFile(MemoryFileProvider.DefaultAppSettingsFileName, fileContents);
        fileProvider.NotifyChanged();

        connectionString = connectorFactory.Get("examplePostgreSqlService").Options.ConnectionString;
        connectionString.Should().Be("Host=three.com;Database=third");
    }

    [Fact]
    public void Applies_local_configuration_changes_using_WebHostBuilder()
    {
        WebHostBuilder builder = TestWebHostBuilderFactory.Create();

        string fileContents = """
            {
              "Steeltoe": {
                "Client": {
                  "PostgreSql": {
                    "examplePostgreSqlService": {
                      "ConnectionString": "SERVER=one.com;DB=first"
                    }
                  }
                }
              }
            }
            """;

        var fileProvider = new MemoryFileProvider();
        fileProvider.IncludeFile(MemoryFileProvider.DefaultAppSettingsFileName, fileContents);

        builder.ConfigureAppConfiguration(configurationBuilder =>
        {
            configurationBuilder.AddJsonFile(fileProvider, MemoryFileProvider.DefaultAppSettingsFileName, false, true);
            configurationBuilder.ConfigurePostgreSql(options => options.DetectConfigurationChanges = true);
        });

        builder.ConfigureServices((context, services) => services.AddPostgreSql(context.Configuration));
        using IWebHost app = builder.Build();

        var connectorFactory = app.Services.GetRequiredService<ConnectorFactory<PostgreSqlOptions, NpgsqlConnection>>();

        connectorFactory.ServiceBindingNames.Should().ContainSingle().Which.Should().Be("examplePostgreSqlService");

        string? connectionString = connectorFactory.Get("examplePostgreSqlService").Options.ConnectionString;
        connectionString.Should().Be("Host=one.com;Database=first");

        fileContents = """
        {
          "Steeltoe": {
            "Client": {
              "PostgreSql": {
                "examplePostgreSqlService": {
                  "ConnectionString": "SERVER=two.com;DB=second"
                }
              }
            }
          }
        }
        """;

        fileProvider.ReplaceFile(MemoryFileProvider.DefaultAppSettingsFileName, fileContents);
        fileProvider.NotifyChanged();

        connectionString = connectorFactory.Get("examplePostgreSqlService").Options.ConnectionString;
        connectionString.Should().Be("Host=two.com;Database=second");

        fileContents = """
        {
          "Steeltoe": {
            "Client": {
              "PostgreSql": {
                "examplePostgreSqlService": {
                  "ConnectionString": "SERVER=three.com;DB=third"
                }
              }
            }
          }
        }
        """;

        fileProvider.ReplaceFile(MemoryFileProvider.DefaultAppSettingsFileName, fileContents);
        fileProvider.NotifyChanged();

        connectionString = connectorFactory.Get("examplePostgreSqlService").Options.ConnectionString;
        connectionString.Should().Be("Host=three.com;Database=third");
    }

    [Fact]
    public void Applies_local_configuration_changes_using_HostBuilder()
    {
        HostBuilder builder = TestHostBuilderFactory.Create();

        string fileContents = """
            {
              "Steeltoe": {
                "Client": {
                  "PostgreSql": {
                    "examplePostgreSqlService": {
                      "ConnectionString": "SERVER=one.com;DB=first"
                    }
                  }
                }
              }
            }
            """;

        var fileProvider = new MemoryFileProvider();
        fileProvider.IncludeFile(MemoryFileProvider.DefaultAppSettingsFileName, fileContents);

        builder.ConfigureAppConfiguration(configurationBuilder =>
        {
            configurationBuilder.AddJsonFile(fileProvider, MemoryFileProvider.DefaultAppSettingsFileName, false, true);
            configurationBuilder.ConfigurePostgreSql(options => options.DetectConfigurationChanges = true);
        });

        builder.ConfigureServices((context, services) => services.AddPostgreSql(context.Configuration));
        using IHost app = builder.Build();

        var connectorFactory = app.Services.GetRequiredService<ConnectorFactory<PostgreSqlOptions, NpgsqlConnection>>();

        connectorFactory.ServiceBindingNames.Should().ContainSingle().Which.Should().Be("examplePostgreSqlService");

        string? connectionString = connectorFactory.Get("examplePostgreSqlService").Options.ConnectionString;
        connectionString.Should().Be("Host=one.com;Database=first");

        fileContents = """
        {
          "Steeltoe": {
            "Client": {
              "PostgreSql": {
                "examplePostgreSqlService": {
                  "ConnectionString": "SERVER=two.com;DB=second"
                }
              }
            }
          }
        }
        """;

        fileProvider.ReplaceFile(MemoryFileProvider.DefaultAppSettingsFileName, fileContents);
        fileProvider.NotifyChanged();

        connectionString = connectorFactory.Get("examplePostgreSqlService").Options.ConnectionString;
        connectionString.Should().Be("Host=two.com;Database=second");

        fileContents = """
        {
          "Steeltoe": {
            "Client": {
              "PostgreSql": {
                "examplePostgreSqlService": {
                  "ConnectionString": "SERVER=three.com;DB=third"
                }
              }
            }
          }
        }
        """;

        fileProvider.ReplaceFile(MemoryFileProvider.DefaultAppSettingsFileName, fileContents);
        fileProvider.NotifyChanged();

        connectionString = connectorFactory.Get("examplePostgreSqlService").Options.ConnectionString;
        connectionString.Should().Be("Host=three.com;Database=third");
    }
}
