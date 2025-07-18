// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.Configuration;

namespace Steeltoe.Configuration.CloudFoundry.Test;

public sealed class JsonStreamConfigurationSourceTest
{
    [Fact]
    public void Build_WithStreamSource_ReturnsExpected()
    {
        const string environment = """
            {
              "application_id": "fa05c1a9-0fc1-4fbd-bae1-139850dec7a3",
              "application_name": "my-app",
              "application_uris": ["my-app.10.244.0.34.xip.io"],
              "application_version": "fb8fbcc6-8d58-479e-bcc7-3b4ce5a7f0ca",
              "limits": {
                "disk": 1024,
                "fds": 16384,
                "mem": 256
              },
              "name": "my-app",
              "space_id": "06450c72-4669-4dc6-8096-45f9777db68a",
              "space_name": "my-space",
              "uris": [
                "my-app.10.244.0.34.xip.io",
                "my-app2.10.244.0.34.xip.io"
              ],
              "users": null,
              "version": "fb8fbcc6-8d58-479e-bcc7-3b4ce5a7f0ca"
            }
            """;

        using Stream stream = CloudFoundryConfigurationProvider.GetStream(environment);
        var source = new JsonStreamConfigurationSource(stream);
        var builder = new ConfigurationBuilder();
        builder.Add(source);
        IConfigurationRoot root = builder.Build();

        root["application_id"].Should().Be("fa05c1a9-0fc1-4fbd-bae1-139850dec7a3");
        root["limits:disk"].Should().Be("1024");
        root["uris:0"].Should().Be("my-app.10.244.0.34.xip.io");
        root["uris:1"].Should().Be("my-app2.10.244.0.34.xip.io");
    }
}
