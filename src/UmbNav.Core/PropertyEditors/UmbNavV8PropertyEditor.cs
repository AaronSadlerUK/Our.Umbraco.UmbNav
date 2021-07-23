#if NETCOREAPP
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
#else
using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;
#endif

namespace UmbNav.Core.PropertyEditors
{
    [DataEditor(Constants.PropertyEditorAlias, Constants.PackageName, Constants.PackageFilesPath + "views/editor.html",
        ValueType = "JSON", Group = "pickers", Icon = "icon-sitemap")]
    public class UmbNavV8PropertyEditor : DataEditor, IDataEditor
    {
#if NETCOREAPP
        private readonly IIOHelper _ioHelper;
        public UmbNavV8PropertyEditor(IDataValueEditorFactory dataValueEditorFactory, IIOHelper ioHelper)
            : base(dataValueEditorFactory)
        {
            _ioHelper = ioHelper;
        }
#else
        public UmbNavV8PropertyEditor(ILogger logger)
            : base(logger)
        {
        }
#endif

        public override IDataValueEditor GetValueEditor(object configuration)
        {

            var editor = base.GetValueEditor(configuration);

            if (editor is DataValueEditor valueEditor && configuration is UmbNavV8Configuration config)
            {
                valueEditor.HideLabel = config.HideLabel;
            }

            return editor;

        }

#if NETCOREAPP
        protected override IConfigurationEditor CreateConfigurationEditor() => new UmbNavV8ConfigurationEditor(_ioHelper);
#else
        protected override IConfigurationEditor CreateConfigurationEditor() => new UmbNavV8ConfigurationEditor();
#endif

        public class UmbNavV8ConfigurationEditor : ConfigurationEditor<UmbNavV8Configuration>
        {
#if NETCOREAPP
            public UmbNavV8ConfigurationEditor(IIOHelper ioHelper) : base(ioHelper)
            {
            }
#else
            public UmbNavV8ConfigurationEditor()
            {
            }
#endif
        }
    }
}