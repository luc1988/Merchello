﻿using System;
using System.Collections.Generic;
using Merchello.Core.Models;
using Merchello.Core.Models.TypeFields;
using Umbraco.Core.Services;

namespace Merchello.Core.Services
{
    /// <summary>
    /// Defines the CustomerRegistryService, which provides access to operations involving <see cref="ICustomerItemCache"/>
    /// </summary>
    public interface ICustomerItemCacheService : IService
    {
        /// <summary>
        /// Creates an item cache (or retrieves an existing one) based on type and saves it to the database
        /// </summary>
        ICustomerItemCache GetCustomerItemCacheWithKey(ICustomerBase customer, ItemCacheType itemCacheType);

        /// <summary>
        /// Saves a single <see cref="ICustomerItemCache"/> object
        /// </summary>
        /// <param name="customerItemCache">The <see cref="ICustomerItemCache"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(ICustomerItemCache customerItemCache, bool raiseEvents = true);

        /// <summary>
        /// Saves a collection of <see cref="ICustomerItemCache"/> objects
        /// </summary>
        /// <param name="customerItemCaches"></param>
        /// <param name="raiseEvents"></param>
        void Save(IEnumerable<ICustomerItemCache> customerItemCaches, bool raiseEvents = true);

        /// <summary>
        /// Deletes a single <see cref="ICustomerItemCache"/> object
        /// </summary>
        /// <param name="customerItemCache"><see cref="ICustomerItemCache"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(ICustomerItemCache customerItemCache, bool raiseEvents = true);

        /// <summary>
        /// Deletes a collection of <see cref="ICustomerItemCache"/> objects
        /// </summary>
        /// <param name="customerItemCaches">Collection of <see cref="ICustomerItemCache"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IEnumerable<ICustomerItemCache> customerItemCaches, bool raiseEvents = true);

        /// <summary>
        /// Gets an <see cref="ICustomerItemCache"/> object by its Id
        /// </summary>
        /// <param name="id">int Id of the Address to retrieve</param>
        /// <returns><see cref="ICustomerItemCache"/></returns>
        ICustomerItemCache GetById(int id);

        /// <summary>
        /// Gets a collection of <see cref="ICustomerItemCache"/> objects by the <see cref="ICustomerBase"/>
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        IEnumerable<ICustomerItemCache> GetCustomerItemCacheByCustomer(ICustomerBase customer);

        /// <summary>
        /// Returns the consumer's registry of a given type
        /// </summary>
        /// <param name="customer"><see cref="ICustomerBase"/></param>
        /// <param name="itemCacheTfKey"><see cref="ITypeField"/>.TypeKey</param>
        /// <returns><see cref="ICustomerItemCache"/></returns>
        /// <remarks>
        /// Public use of this method is intended to access ItemCacheType.Custom records
        /// </remarks>
        ICustomerItemCache GetCustomerItemCacheByCustomer(ICustomerBase customer, Guid itemCacheTfKey);

        /// <summary>
        /// Gets an <see cref="ICustomerItemCache"/> object by the <see cref="ICustomerBase"/>
        /// </summary>
        /// <param name="customer">The <see cref="ICustomerBase"/> object</param>
        /// <param name="itemCacheType"></param>
        /// <returns><see cref="ICustomerItemCache"/></returns>
        ICustomerItemCache GetCustomerItemCacheByCustomer(ICustomerBase customer, ItemCacheType itemCacheType);
            
        /// <summary>
        /// Gets list of <see cref="ICustomerItemCache"/> objects given a list of Ids
        /// </summary>
        /// <param name="ids">List of int Id for customer registries to retrieve</param>
        /// <returns>List of <see cref="ICustomerItemCache"/></returns>
        IEnumerable<ICustomerItemCache> GetByIds(IEnumerable<int> ids);

    }
}