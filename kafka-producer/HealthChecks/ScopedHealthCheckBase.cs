using App.Metrics.Health;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Kafka.Producer.HealthChecks
{
    public abstract class ScopedHealthCheckBase : HealthCheck
    {
        protected IServiceScopeFactory ServiceScopeFactory { get; }
        public ScopedHealthCheckBase(IServiceScopeFactory serviceScopeFactory, string name) : base(name)
        {
            ServiceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        }
    }
}
