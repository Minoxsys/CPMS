using System;

namespace CPMS.Infrastructure.DI
{
    public interface IContainer
    {
        /// <summary>
        /// Register a specific concrete implementation for a contract with no key as singlecall mapping.
        /// </summary>
        /// <typeparam name="TContract">The type of the contract.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        void RegisterType<TContract, TImplementation>() where TImplementation : TContract;

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
        void RegisterType<TContract, TImplementation>(string key) where TImplementation : TContract;

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
        void RegisterType<TContract, TImplementation>(bool singleton) where TImplementation : TContract;

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
        void RegisterType<TContract, TImplementation>(string key, bool singleton) where TImplementation : TContract;

        /// <summary>
        /// Register a specific concrete implementation instance for a contract with no key.
        /// </summary>
        /// <param name="impl">
        /// The implementation instance.
        /// </param>
        /// <typeparam name="TContract">
        /// The type of the contract.
        /// </typeparam>
        void RegisterInstance<TContract>(TContract impl);

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
        void RegisterInstance<TContract>(string key, TContract impl);

        /// <summary>
        /// Resolve an implementation for a specific contract with no key mapping.
        /// </summary>
        /// <typeparam name="TContract">The type of the contract.</typeparam>
        /// <returns>An instance of the implementation found for <see cref="TContract"/>.</returns>
        TContract Resolve<TContract>();

        /// <summary>
        /// Resolve a weakly typed implementation for a contract type with no key mapping.
        /// </summary>
        /// <param name="tContract">
        /// The type of the contract.
        /// </param>
        /// <returns>
        /// An instance of the implementation found.
        /// </returns>
        object Resolve(Type tContract);

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
        TContract Resolve<TContract>(string key);
    }
}
