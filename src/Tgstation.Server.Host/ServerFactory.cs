﻿using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Tgstation.Server.Host.Core;
using Tgstation.Server.Host.Extensions;
using Tgstation.Server.Host.IO;
using Tgstation.Server.Host.Setup;
using Tgstation.Server.Host.System;

namespace Tgstation.Server.Host
{
	/// <summary>
	/// Implementation of <see cref="IServerFactory"/>.
	/// </summary>
	sealed class ServerFactory : IServerFactory
	{
		/// <summary>
		/// The <see cref="IAssemblyInformationProvider"/> for the <see cref="ServerFactory"/>.
		/// </summary>
		readonly IAssemblyInformationProvider assemblyInformationProvider;

		/// <inheritdoc />
		public IIOManager IOManager { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ServerFactory"/> class.
		/// </summary>
		/// <param name="assemblyInformationProvider">The value of <see cref="assemblyInformationProvider"/>.</param>
		/// <param name="ioManager">The value of <see cref="IOManager"/>.</param>
		internal ServerFactory(IAssemblyInformationProvider assemblyInformationProvider, IIOManager ioManager)
		{
			this.assemblyInformationProvider = assemblyInformationProvider ?? throw new ArgumentNullException(nameof(assemblyInformationProvider));
			IOManager = ioManager ?? throw new ArgumentNullException(nameof(ioManager));
		}

		/// <inheritdoc />
		// TODO: Decomplexify
#pragma warning disable CA1506
		public async Task<IServer> CreateServer(string[] args, string updatePath, CancellationToken cancellationToken)
		{
			if (args == null)
				throw new ArgumentNullException(nameof(args));

			var basePath = IOManager.ResolvePath();
			IHostBuilder CreateDefaultBuilder() => Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
				.ConfigureAppConfiguration((context, builder) =>
				{
					builder.SetBasePath(basePath);

					builder.AddYamlFile("appsettings.yml", optional: true, reloadOnChange: true)
						.AddYamlFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.yml", optional: true, reloadOnChange: true);

					// reorganize the builder so our yaml configs don't override the env/cmdline configs
					// values obtained via debugger
					var environmentJsonConfig = builder.Sources[2];
					var envConfig = builder.Sources[3];
					var cmdLineConfig = builder.Sources[4];
					var baseYmlConfig = builder.Sources[5];
					var environmentYmlConfig = builder.Sources[6];

					builder.Sources[2] = baseYmlConfig;
					builder.Sources[3] = environmentJsonConfig;
					builder.Sources[4] = environmentYmlConfig;
					builder.Sources[5] = envConfig;
					builder.Sources[6] = cmdLineConfig;
				});

			var setupWizardHostBuilder = CreateDefaultBuilder()
				.UseSetupApplication();

			IPostSetupServices<ServerFactory> postSetupServices;
			using (var setupHost = setupWizardHostBuilder.Build())
			{
				postSetupServices = setupHost.Services.GetRequiredService<IPostSetupServices<ServerFactory>>();
				await setupHost.RunAsync(cancellationToken).ConfigureAwait(false);

				if (postSetupServices.GeneralConfiguration.SetupWizardMode == SetupWizardMode.Only)
				{
					postSetupServices.Logger.LogInformation("Shutting down due to only running setup wizard.");
					return null;
				}
			}

			var hostBuilder = CreateDefaultBuilder()
				.ConfigureWebHost(webHostBuilder =>
					webHostBuilder
						.UseKestrel(kestrelOptions =>
						{
							var serverAddressProvider = kestrelOptions.ApplicationServices.GetRequiredService<IServerAddressProvider>();
							kestrelOptions.Listen(
								serverAddressProvider.AddressEndPoint,
								listenOptions => listenOptions.Protocols = HttpProtocols.Http1AndHttp2);
						})
						.UseIIS()
						.UseIISIntegration()
						.UseApplication(postSetupServices)
						.SuppressStatusMessages(true)
						.UseShutdownTimeout(TimeSpan.FromMilliseconds(postSetupServices.GeneralConfiguration.RestartTimeout)));

			if (updatePath != null)
				hostBuilder.UseContentRoot(
					IOManager.ResolvePath(
						IOManager.GetDirectoryName(assemblyInformationProvider.Path)));

			return new Server(hostBuilder, updatePath);
		}
#pragma warning restore CA1506
	}
}
