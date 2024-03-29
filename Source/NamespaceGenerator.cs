// SPDX-License-Identifier: MPL-2.0
namespace Emik.SourceGenerators.Tattoo;

/// <summary>Creates global imports of every namespace of the current project.</summary>
[Generator]
public sealed class NamespaceGenerator : ISourceGenerator
{
    const string
        FileName = "GlobalUsings.g.cs",
        Global = "global::",
        ProjectExtension = ".csproj";

    static readonly AssemblyName s_name = typeof(NamespaceGenerator).Assembly.GetName();

    static readonly string s_header =
        $"// <auto-generated/>\n// {s_name.Name}, {s_name.Version.ToShortString()}\n#pragma warning disable\n";

    /// <inheritdoc />
    void ISourceGenerator.Initialize(GeneratorInitializationContext context) { }

    /// <inheritdoc />
    void ISourceGenerator.Execute(GeneratorExecutionContext context) => MakeFile(context.Compilation);

    static void MakeFile(Compilation compilation)
    {
        if (ProjectPath(compilation) is { } project &&
            Imports(compilation) is var contents &&
            Path.Combine(project, FileName) is var path)
            File.WriteAllText(path, contents);
    }

    [Pure]
    static bool IsPathGeneratedGlobalUsing(scoped in ReadOnlySpan<char> filePath) =>
        (filePath.LastIndexOfAny('/', '\\') is var index and not -1 ? filePath[++index..] : filePath) is FileName;

    [MustUseReturnValue]
    static string Imports(in Compilation compilation)
    {
        StringBuilder sb = new(s_header);

        compilation
           .References
           .Select(compilation.GetAssemblyOrModuleSymbol)
           .Select(x => (x as IModuleSymbol)?.ContainingAssembly ?? x)
           .OfType<IAssemblySymbol>()
           .Filter()
           .SelectMany(x => x.GetAllMembers())
           .OfType<INamespaceSymbol>()
           .Concat(compilation.GetSymbolsWithName(_ => true, SymbolFilter.Namespace))
           .Select(x => x.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
           .Distinct(StringComparer.Ordinal)
           .Omit(string.IsNullOrWhiteSpace)
           .Omit(x => x.Contains('<') || x.Contains('>'))
           .OrderByDescending(x => x.StartsWith($"{Global}{nameof(System)}"))
           .ThenBy(x => x, StringComparer.Ordinal)
           .For(x => sb.Append("global using ").Append(x).Append(";\n"))
           .Peek(_ => sb.Append("\n// Polyfills of namespaces in case dependencies are conditional.\n"))
           .For(x => sb.Append("namespace ").Append(x.StartsWith(Global) ? x[Global.Length..] : x).Append(" { }\n\n"));

        return $"{sb.Remove(^1..)}";
    }

    [MustUseReturnValue]
    static string? ProjectPath(Compilation compilation) =>
        compilation.SyntaxTrees.Any(x => IsPathGeneratedGlobalUsing(x.FilePath.AsSpan()))
            ? null
            : compilation
               .SyntaxTrees
               .SelectMany(x => Path.GetDirectoryName(x.FilePath).FindSmallPathToNull(Path.GetDirectoryName))
               .Distinct(StringComparer.Ordinal)
               .FirstOrDefault(x => Directory.EnumerateFiles(x).Any(x => x.EndsWith(ProjectExtension)));
}
