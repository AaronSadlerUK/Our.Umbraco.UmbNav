using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UmbNav.Core.Interfaces;
using UmbNav.Core.Models;
using UmbNav.Core.PropertyEditors;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Serilog;

namespace UmbNav.Core.ValueConverters
{
    public class UmbNavValueConverter : PropertyValueConverterBase
    {
        private readonly IUmbNavMenuBuilderService _umbNavMenuBuilderService;
        private readonly ILogger _logger;

        private bool _removeNaviHideItems;
        private bool _removeNoopener;
        private bool _removeNoreferrer;
        private bool _removeIncludeChildNodes;

        public UmbNavValueConverter(ILogger logger, IUmbNavMenuBuilderService umbNavMenuBuilderService)
        {
            _logger = logger;
            _umbNavMenuBuilderService = umbNavMenuBuilderService;
        }

        public override bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias.Equals(UmbNavConstants.PropertyEditorAlias);
        }

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType) => typeof(IEnumerable<UmbNavItem>);

        public override object ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
        {
            if (inter == null)
            {
                return Enumerable.Empty<UmbNavItem>();
            }

            var configuration = propertyType.DataType.ConfigurationAs<UmbNavConfiguration>();

            if (configuration != null)
            {
                _removeNaviHideItems = configuration.RemoveNaviHideItems;
                _removeNoopener = configuration.HideNoopener;
                _removeNoreferrer = configuration.HideNoreferrer;
                _removeIncludeChildNodes = configuration.HideIncludeChildren;
            }

            try
            {
                var items = JsonConvert.DeserializeObject<IEnumerable<UmbNavItem>>(inter.ToString());

                return _umbNavMenuBuilderService.BuildMenu(items, 0, _removeNaviHideItems, _removeNoopener, _removeNoreferrer, _removeIncludeChildNodes);
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to convert UmbNav {ex}", ex);
            }

            return Enumerable.Empty<UmbNavItem>();
        }
    }
}