using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;

namespace UmbNav.Core.PropertyEditors
{
    [DataEditor(UmbNavConstants.PropertyEditorAlias, UmbNavConstants.PackageName, UmbNavConstants.PackageFilesPath + "views/editor.html",
        ValueType = "JSON", Group = "pickers", Icon = "icon-sitemap")]
    public class UmbNavV8PropertyEditor : DataEditor, IDataEditor
    {
        private readonly IIOHelper _ioHelper;
        public UmbNavV8PropertyEditor(IDataValueEditorFactory dataValueEditorFactory, IIOHelper ioHelper)
            : base(dataValueEditorFactory)
        {
            _ioHelper = ioHelper;
        }

        public override IDataValueEditor GetValueEditor(object configuration)
        {

            var editor = base.GetValueEditor(configuration);

            if (editor is DataValueEditor valueEditor && configuration is UmbNavV8Configuration config)
            {
                valueEditor.HideLabel = config.HideLabel;
            }

            return editor;

        }

        protected override IConfigurationEditor CreateConfigurationEditor() => new UmbNavV8ConfigurationEditor(_ioHelper,_editorConfigurationParser);
        protected override IConfigurationEditor CreateConfigurationEditor() => new UmbNavV8ConfigurationEditor(_ioHelper);

        public class UmbNavV8ConfigurationEditor : ConfigurationEditor<UmbNavV8Configuration>
        {
            public UmbNavV8ConfigurationEditor(IIOHelper ioHelper, IEditorConfigurationParser editorConfigurationParser) : base(ioHelper, editorConfigurationParser)
            public UmbNavV8ConfigurationEditor(IIOHelper ioHelper) : base(ioHelper)
            {
            }
        }
    }
}