# Emik.SourceGenerators.Tattoo

[![NuGet package](https://img.shields.io/nuget/v/Emik.SourceGenerators.Tattoo.svg?logo=NuGet)](https://www.nuget.org/packages/Emik.SourceGenerators.Tattoo)
[![License](https://img.shields.io/github/license/Emik03/Emik.SourceGenerators.Tattoo.svg?style=flat)](https://github.com/Emik03/Emik.SourceGenerators.Tattoo/blob/main/LICENSE)

Source generates a file that imports all namespaces.

This project has a dependency to [Emik.Morsels](https://github.com/Emik03/Emik.Morsels), if you are building this project, refer to its [README](https://github.com/Emik03/Emik.Morsels/blob/main/README.md) first.

Example file output:

```csharp
// <auto-generated/>
global using System;
global using System.Buffers;
global using System.Buffers.Binary;
global using System.Buffers.Text;
global using System.CodeDom;
global using System.CodeDom.Compiler;
// ... <omitted for brevity>
namespace System { }

namespace System.Buffers { }

namespace System.Buffers.Binary { }

namespace System.Buffers.Text { }

namespace System.CodeDom { }

namespace System.CodeDom.Compiler { }
// ... <omitted for brevity>
```

---

- [Contribute](#contribute)
- [License](#license)

---

## Contribute

Issues and pull requests are welcome to help this repository be the best it can be.

## License

This repository falls under the [MPL-2 license](https://www.mozilla.org/en-US/MPL/2.0/).
