﻿using System;
using System.Collections.Generic;

namespace Merchello.Web.Models.ContentEditing
{
    public class TaxMethodDisplay : DialogEditorDisplayBase
    {
        public Guid Key { get; set; }
        public Guid ProviderKey { get; set; }
        public string Name { get; set; }
        public string CountryCode { get; set; }
        public decimal PercentageTaxRate { get; set; }
        public IEnumerable<TaxProvinceDisplay> Provinces { get; set; }
    }
}