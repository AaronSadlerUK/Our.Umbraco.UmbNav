using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UmbNavV8.Core.Interfaces;
using UmbNavV8.Core.Models;
using UmbNavV8.Core.PropertyEditors;
using Umbraco.Core.Logging;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;

namespace UmbNavV8.Core.ValueConverters
{
    public class UmbNavV8ValueConverter : PropertyValueConverterBase
    {
        private readonly IUmbNavMenuBuilderService _umbNavMenuBuilderService;
        private readonly ILogger _logger;

        private bool _removeNaviHideItems;
        private bool _removeNoopener;
        private bool _removeNoreferrer;

        public UmbNavV8ValueConverter(ILogger logger, IUmbNavMenuBuilderService umbNavMenuBuilderService)
        {
            _logger = logger;
            _umbNavMenuBuilderService = umbNavMenuBuilderService;
        }

        public override bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias.Equals(Constants.PropertyEditorAlias);
        }

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType) => typeof(IEnumerable<UmbNavItem>);

        public override object ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
        {
            if (inter == null)
            {
                return Enumerable.Empty<UmbNavItem>();
            }

            var configuration = propertyType.DataType.ConfigurationAs<UmbNavV8Configuration>();

            if (configuration != null)
            {
                _removeNaviHideItems = configuration.RemoveNaviHideItems;
                _removeNoopener = configuration.HideNoopener;
                _removeNoreferrer = configuration.HideNoreferrer;
            }

            try
            {
                var items = JsonConvert.DeserializeObject<IEnumerable<UmbNavItem>>(inter.ToString());

                return _umbNavMenuBuilderService.BuildMenu(items, _removeNaviHideItems, _removeNoopener, _removeNoreferrer);
            }
            catch (Exception ex)
            {
                _logger.Error<UmbNavV8ValueConverter>("Failed to convert UmbNav {ex}", ex);
            }

            return Enumerable.Empty<UmbNavItem>();
        }
    }
}