using Autofac;

namespace VMLab.IOC
{
    public class Bootstrap
    {
        private readonly IContainer _container;

        public Bootstrap()
        {
            var builder = new ContainerBuilder();
            //builder.RegisterAssemblyModules<Module>();

            _container = builder.Build();
        }

        public T Start<T>() => _container.Resolve<T>();
    }
}
