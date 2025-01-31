﻿// Copyright (c) Microsoft. All rights reserved.

using Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Models;

const string DefaultSemanticFunctionsFolder = "Prompts";
string semanticFunctionsFolder = Environment.GetEnvironmentVariable("SEMANTIC_PLUGINS_FOLDER") ?? DefaultSemanticFunctionsFolder;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services
            .AddScoped<IKernel>((providers) =>
            {
                // This will be called each time a new Kernel is needed

                // Get a logger instance
                ILoggerFactory loggerFactory = providers
                    .GetRequiredService<ILoggerFactory>();

                // Register your AI Providers...
                var appSettings = AppSettings.LoadSettings();
                IKernel kernel = new KernelBuilder()
                    .WithChatCompletionService(appSettings.Kernel)
                    .WithLoggerFactory(loggerFactory)
                    .Build();

                // Load your semantic functions...
                kernel.ImportPromptsFromDirectory(appSettings.AIPlugin.NameForModel, semanticFunctionsFolder);

                return kernel;
            })
            .AddScoped<IAIPluginRunner, AIPluginRunner>();
    })
    .Build();

host.Run();
