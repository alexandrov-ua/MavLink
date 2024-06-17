using System.Text;
using MavLink.Serialize.Messages;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Scriban;
using Scriban.Runtime;

namespace MavLink.Serialize.Generator;

[Generator]
public class DialectGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
    }
    
    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxContextReceiver is SyntaxReceiver receiver)
        {
            foreach (var root in receiver.Roots)
            {
                var definition = DialectXmlParser.Parse(root.FilePath);

                var model = RootRenderModel.CreateFromDefinition(definition, root);

                var source = RenderTemplate(model);
                context.AddSource($"{root.DisplayName}.g.cs", source);
            }
        }
    }

    private static string RenderTemplate(object model)
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
        using (Stream stream = typeof(IPayload).Assembly.GetManifestResourceStream(name))
        using (StreamReader reader = new StreamReader(stream))
        {
            return reader.ReadToEnd();
        }
    }
}

public class ScribanHelper : ScriptObject
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