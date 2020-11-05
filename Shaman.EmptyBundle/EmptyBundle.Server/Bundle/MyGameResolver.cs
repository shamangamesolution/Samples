using System;
using Microsoft.Extensions.DependencyInjection;
using Shaman.Common.Utils.TaskScheduling;
using Shaman.Contract.Bundle;
using Shaman.Contract.Bundle.DI;
using Shaman.Contract.Routing.Meta;
using Shaman.Serialization;

namespace Bundle
{
    public class MyGameResolver : GameBundleBase
    {
        protected override void OnConfigureServices(IServiceCollection serviceCollection)
        {
            try
            {
                //singletons
                serviceCollection.AddSingleton<ITaskSchedulerFactory, TaskSchedulerFactory>();
                serviceCollection.AddSingleton<ISerializer, BinarySerializer>();
                //transients
                serviceCollection.AddTransient<IRoomControllerFactory, GameModeControllerFactory>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        protected override void OnStart(IServiceProvider serviceProvider)
        {
            PendingTask.DurationMonitoringTime(TimeSpan.FromHours(24));
        }
    }
}