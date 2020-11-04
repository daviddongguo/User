using Autofac;
using Microsoft.Extensions.Configuration;
using user.Services;

namespace user
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AuthService>()
                .As<IAuthService>()
                .InstancePerLifetimeScope();
        }
    }
}
