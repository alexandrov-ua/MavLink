using System.IO;
using System.Linq;
using Scriban;
using Scriban.Runtime;

namespace MavLink.Serialize.Generator;

internal static class TemplateHelper
{
    public static string RenderTemplate(RootRenderModel model)
    {
        var result = GetResource("MavLink.Serialize.Generator.Templates.RootTemplate.scriban");
        var template = Template.Parse(result);

        var templateContext = new TemplateContext {MemberRenamer = member => member.Name};

        var scriptObject = new ScribanHelper();
        templateContext.PushGlobal(scriptObject);

        var entityScriptObject = new ScriptObject();
        entityScriptObject.Import(model, renamer: member => member.Name);
        templateContext.PushGlobal(entityScriptObject);

        return template.Render(templateContext);
    }

    private static string GetResource(string name)
    {
        using (Stream stream = typeof(TemplateHelper).Assembly.GetManifestResourceStream(name)!)
        using (StreamReader reader = new StreamReader(stream))
        {
            return reader.ReadToEnd();
        }
    }
}

internal class ScribanHelper : ScriptObject
{
    public static string ToPascalCase(string word)
    {
        if (string.IsNullOrEmpty(word))
            return string.Empty;

        return string.Join("", word.Split('_')
            .Select(w => w.Trim())
            .Where(w => w.Length > 0)
            .Select(w => w.Substring(0, 1).ToUpper() + w.Substring(1).ToLower()));
    }
}