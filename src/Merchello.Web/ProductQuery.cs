﻿using System;
using System.Collections.Generic;
using System.Linq;
using Examine;
using Examine.LuceneEngine.SearchCriteria;
using Examine.SearchCriteria;
using Merchello.Core;
using Merchello.Core.Cache;
using Merchello.Core.Models;
using Merchello.Core.Persistence.UnitOfWork;
using Merchello.Core.Services;
using Merchello.Examine;
using Merchello.Web.Models.ContentEditing;
using Umbraco.Core;
using Umbraco.Core.Logging;

namespace Merchello.Web
{
    internal class ProductQuery : QueryBase
    {
        private const string IndexName = "MerchelloProductIndexer";
        private const string SearcherName = "MerchelloProductSearcher";

        /// <summary>
        /// Retrieves a <see cref="ProductDisplay"/> given it's 'unique' Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns><see cref="ProductDisplay"/></returns>
        public static ProductDisplay GetByKey(Guid key)
        {
            return GetByKey(key.ToString());
        }

        /// <summary>
        /// Retrieves a <see cref="ProductDisplay"/> given it's 'unique' Key (string representation of the Guid)
        /// </summary>
        /// <param name="key"></param>
        /// <returns><see cref="ProductDisplay"/></returns>
        public static ProductDisplay GetByKey(string key)
        {
            var criteria = ExamineManager.Instance.CreateSearchCriteria(BooleanOperation.And);
            criteria.Field("productKey", key).And().Field("master", "True");
            var product = ExamineManager.Instance.SearchProviderCollection[SearcherName]
                .Search(criteria).Select(result => result.ToProductDisplay()).FirstOrDefault();

            if (product != null) return product;
            var merchelloContext = GetMerchelloContext(); 

            var retrieved = merchelloContext.Services.ProductService.GetByKey(new Guid(key));

            if (retrieved == null) return null;
                
            ReindexProduct(retrieved);

            return AutoMapper.Mapper.Map<ProductDisplay>(retrieved);
        }

        public static IEnumerable<ProductDisplay> GetAllProducts()
        {
            var merchelloContext = GetMerchelloContext();

            var criteria = ExamineManager.Instance.CreateSearchCriteria(IndexTypes.ProductVariant);
            criteria.Field("master", "True");

            var results = ExamineManager.Instance.SearchProviderCollection[SearcherName]
                .Search(criteria).Select(result => result.ToProductDisplay()).ToArray();


            var count = merchelloContext.Services.ProductService.ProductsCount();

            if (results.Any() && (count == results.Count())) return results;

            if (count != results.Count())
            {
                RebuildIndex(IndexName);
            }

            var retrieved = ((ProductService) merchelloContext.Services.ProductService).GetAll();

            return retrieved.Select(AutoMapper.Mapper.Map<ProductDisplay>).ToList();
        }

        /// <summary>
        /// Retrieves a <see cref="ProductVariantDisplay"/> given it's 'unique' key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static ProductVariantDisplay GetVariantDisplayByKey(Guid key)
        {
            return GetVariantDisplayByKey(key.ToString());
        }

        /// <summary>
        /// Retrieves a <see cref="ProductVariantDisplay"/> given it's 'unique' key (string representation of the Guid)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static ProductVariantDisplay GetVariantDisplayByKey(string key)
        {
            var criteria = ExamineManager.Instance.CreateSearchCriteria();
            criteria.Field("productVariantKey", key);
            try
            {
                var variant = ExamineManager.Instance.SearchProviderCollection[SearcherName]
                .Search(criteria).Select(result => result.ToProductVariantDisplay()).FirstOrDefault();

                if (variant != null) return variant;

            }
            catch (Exception ex)
            {
                LogHelper.Error<ProductQuery>("GetVariantDisplayByKey", ex);
            }
            
            // Assists in unit testing
            var merchelloContext = GetMerchelloContext();

            var retrieved = merchelloContext.Services.ProductVariantService.GetByKey(new Guid(key));
            if (retrieved != null) ReindexProductVariant(retrieved, null);

            return retrieved != null ? retrieved.ToProductVariantDisplay() : null;
        }


        /// <summary>
        /// Searches ProductIndex by name and sku for the 'term' passed
        /// </summary>
        /// <param name="term"></param>
        /// <returns>A collection of <see cref="ProductDisplay"/></returns>
        public static IEnumerable<ProductDisplay> Search(string term)
        {
            var criteria = ExamineManager.Instance.CreateSearchCriteria();
            criteria.Field("master", "True").And().GroupedOr(new[] { "name", "sku" }, term.Fuzzy());
            return Search(criteria);
        }

        /// <summary>
        /// Searches ProductIndex using <see cref="ISearchCriteria"/> passed
        /// </summary>
        /// <returns>A collection of <see cref="ProductDisplay"/></returns>
        public static IEnumerable<ProductDisplay> Search(ISearchCriteria criteria)
        {
            return ExamineManager.Instance.SearchProviderCollection[SearcherName]
                .Search(criteria).OrderByDescending(x => x.Score)
                .Select(result => result.ToProductDisplay());
        }


        /// <summary>
        /// This is a sort of fall back, in the event that the index becomes corrupted or the product was not indexed for some reason.
        /// </summary>
        /// <param name="product"></param>
        private static void ReindexProduct(IProduct product)
        {
            foreach (var variant in product.ProductVariants)
            {
                ReindexProductVariant(variant, null);
            }
            ReindexProductVariant(((Product)product).MasterVariant, product.ProductOptions);

        }

        private static void ReindexProductVariant(IProductVariant productVariant, ProductOptionCollection productOptions)
        {
            ExamineManager.Instance.IndexProviderCollection[IndexName]
                .ReIndexNode(productVariant.SerializeToXml(productOptions).Root, IndexTypes.ProductVariant);
        }

        
    }
}