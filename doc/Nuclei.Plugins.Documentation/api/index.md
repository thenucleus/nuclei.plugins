
# Nuclei.Plugins


## Discovery

In order to discover new plugins instantiate an instance of the `FileSystemListener` and enable it.

[!code-csharp[FileSystemListener.Enable](..\..\Nuclei.Plugins.Samples\FileSystemListenerSample.cs?range=45-76)]

The `FileSystemListener` relies on `IProcessPluginOriginChanges` objects to indicate which types are considered
plugin files. There are currently two implementations of the `IProcessPluginOriginChanges` interface. The first
one handles bare assembly files, the `AssemblyPluginProcessor`, and the second one handles plugins in NuGet
packages, the `NuGetPluginProcessor`.


## Composition

It is assumed that all plugins are MEF based plugins. To use the discovered plugins instantiate an instance of the
`LazyLoadCatalog`

[!code-csharp[LazyLoadCatalog.Create](..\..\Nuclei.Plugins.Samples\LazyLoadCatalogSample.cs?range=41-46)]