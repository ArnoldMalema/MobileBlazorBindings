﻿using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.MobileBlazorBindings.HostingNew
{
    /// <summary>
    /// Represents the resources, services, and app lifecycle that's scoped to a single host HTML
    /// page load. If you reload the host page, you'll get a new one of these.
    /// </summary>
    internal class WebViewPageContext : IDisposable
    {
        private readonly IServiceScope _serviceScope;
        private readonly IServiceProvider _services;

        public WebViewRenderer Renderer { get; }
        public BlazorWebViewJSRuntime JSRuntime { get; }

        public WebViewPageContext(IServiceProvider serviceProvider, Dispatcher dispatcher, BlazorWebViewIPC ipc, Action<Exception> onUnhandledException)
        {
            _serviceScope = serviceProvider.CreateScope();
            _services = _serviceScope.ServiceProvider;
            JSRuntime = (BlazorWebViewJSRuntime)_services.GetRequiredService<IJSRuntime>();
            JSRuntime.AttachToIpcChannel(ipc);

            var loggerFactory = _services.GetRequiredService<ILoggerFactory>();
            Renderer = new WebViewRenderer(serviceProvider, loggerFactory, onUnhandledException, dispatcher, JSRuntime, ipc);
        }

        public async Task AddRootComponents(List<(Type type, string selector, ParameterView parameters)> rootComponents)
        {
            foreach (var rootComponent in rootComponents)
            {
                await Renderer.AddRootComponentAsync(rootComponent.type, rootComponent.selector, rootComponent.parameters).ConfigureAwait(true);
            }
        }

        public void Dispose()
        {
            _serviceScope.Dispose();
        }
    }
}