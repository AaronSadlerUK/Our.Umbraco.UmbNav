using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;

namespace UmbNavV8.Core.PropertyEditors
{
    [DataEditor(Constants.PropertyEditorAlias, Constants.PackageName, Constants.PackageFilesPath + "views/editor.html", ValueType = "JSON", Group = "pickers", Icon = "icon-sitemap")]
    public class UmbNavV8PropertyEditor : DataEditor
    {
        public UmbNavV8PropertyEditor(ILogger logger)
            : base(logger)
        {

        }

        protected override IConfigurationEditor CreateConfigurationEditor() => new UmbNavV8ConfigurationEditor();

        public class UmbNavV8ConfigurationEditor : ConfigurationEditor<UmbNavV8Configuration>
        {
        }
    }
}
