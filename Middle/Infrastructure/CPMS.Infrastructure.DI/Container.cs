using System;
using Microsoft.Practices.Unity;

namespace CPMS.Infrastructure.DI
{
    /// <summary>
    /// Dependency injection container.
    /// </summary>
    public class Container : IContainer
    {
        private readonly UnityContainer container;

        /// <summary>
        /// "Static initialization" - very easy obtained thread safety at instance creation since lazy is not
        /// actually needed in this context.
        /// </summary>
        private static readonly Container instance = new Container();

        /// <summary>
        /// Prevents a default instance of the <see cref="Container"/> class from being created. 
        /// </summary>
        public Container()
        {
            container = new UnityContainer();
        }

        /// <summary>
        /// Gets the singleton instance of  <see cref="Container"/>
        /// </summary>
        public static Container Instance
        {
            get
            {
                return instance;
            }
        }


        /// <summary>
        /// Register a specific concrete implementation for a contract with no key as singlecall mapping.
        /// </summary>
        /// <typeparam name="TContract">The type of the contract.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        public void RegisterType<TContract, TImplementation>() where TImplementation : TContract
        {
            RegisterType<TContract, TImplementation>(false);
        }

        /// <summary>
        /// Register a specific concrete implementation for a contract with a specific key as singlecall mapping.
        /// </summary>
        /// <param name="key">
        /// The map key.
        /// </param>
        /// <typeparam name="TContract">
        /// The type of the contract.
        /// </typeparam>
        /// <typeparam name="TImplementation">
        /// The type of the implementation.
        /// </typeparam>
        public void RegisterType<TContract, TImplementation>(string key) where TImplementation : TContract
        {
            RegisterType<TContract, TImplementation>(key, false);
        }

        /// <summary>
        /// Register a specific concrete implementation for a contract with no key as singleton or singlecall mapping.
        /// </summary>
        /// <param name="singleton">
        /// True to register as singleton, false for singlecall
        /// </param>
        /// <typeparam name="TContract">
        /// The type of the contract.
        /// </typeparam>
        /// <typeparam name="TImplementation">
        /// The type of the implementation.
        /// </typeparam>
        public void RegisterType<TContract, TImplementation>(bool singleton) where TImplementation : TContract
        {
            if (singleton)
            {
                container.RegisterType<TContract, TImplementation>(new ContainerControlledLifetimeManager());
            }
            else
            {
                container.RegisterType<TContract, TImplementation>();
            }
        }

        /// <summary>
        /// Register a specific concrete implementation for a contract with a specific key as singleton or singlecall mapping.
        /// </summary>
        /// <param name="key">
        /// The map key.
        /// </param>
        /// <param name="singleton">
        /// True to register as singleton, false for singlecall
        /// </param>
        /// <typeparam name="TContract">
        /// The type of the contract.
        /// </typeparam>
        /// <typeparam name="TImplementation">
        /// The type of the implementation.
        /// </typeparam>
        public void RegisterType<TContract, TImplementation>(string key, bool singleton) where TImplementation : TContract
        {
            if (singleton)
            {
                container.RegisterType<TContract, TImplementation>(key, new ContainerControlledLifetimeManager());
            }
            else
            {
                container.RegisterType<TContract, TImplementation>(key);
            }
        }

        /// <summary>
        /// Register a specific concrete implementation instance for a contract with no key.
        /// </summary>
        /// <param name="impl">
        /// The implementation instance.
        /// </param>
        /// <typeparam name="TContract">
        /// The type of the contract.
        /// </typeparam>
        public void RegisterInstance<TContract>(TContract impl)
        {
            container.RegisterInstance(impl);
        }

        /// <summary>
        /// Register a specific concrete implementation instance for a contract with a specified key.
        /// </summary>
        /// <param name="key">
        /// The registration key.
        /// </param>
        /// <param name="impl">
        /// The implementation instance.
        /// </param>
        /// <typeparam name="TContract">
        /// The type of the contract.
        /// </typeparam>
        public void RegisterInstance<TContract>(string key, TContract impl)
        {
            container.RegisterInstance(key, impl);
        }

        /// <summary>
        /// Resolve an implementation for a specific contract with no key mapping.
        /// </summary>
        /// <typeparam name="TContract">The type of the contract.</typeparam>
        /// <returns>An instance of the implementation found for <see cref="TContract"/>.</returns>
        public TContract Resolve<TContract>()
        {
            return container.Resolve<TContract>();
        }

        /// <summary>
        /// Resolve a weakly typed implementation for a contract type with no key mapping.
        /// </summary>
        /// <param name="tContract">
        /// The type of the contract.
        /// </param>
        /// <returns>
        /// An instance of the implementation found.
        /// </returns>
        public object Resolve(Type tContract)
        {
            return container.Resolve(tContract);
        }

        /// <summary>
        /// Resolve an implementation for a specific contract mapped with the specified key.
        /// </summary>
        /// <param name="key">
        /// The mapping key.
        /// </param>
        /// <typeparam name="TContract">
        /// The type of the interface.
        /// </typeparam>
        /// <returns>
        /// An instance of the implementation found for <see cref="TContract"/>.
        /// </returns>
        public TContract Resolve<TContract>(string key)
        {
            return container.Resolve<TContract>(key);
        }
    }
}
