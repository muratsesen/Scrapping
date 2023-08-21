using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Infrastructure.Services;

namespace Infrastructure.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IWebDriver, FirefoxDriver>();
        services.AddSingleton<IMadridService, MadridService>();
        services.AddSingleton<ITurkPatentService, TurkPatentService>();

        return services;
    }
}


