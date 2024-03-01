<div align="center">
  
# 🐎 fim 

> fim is a [FiM++](https://esolangs.org/wiki/FiM%2B%2B) interpreter written in C#.

<br>

<div>
	<a href="https://github.com/Jaezmien/fim">
		<img src="https://img.shields.io/badge/version-pre--alpha-red">
	</a>
</div>

</div>

# 📝 Notes

-   fim is just a personal hobby project, seeing as FiM++ has never been updated for quite some time now.

-   The syntax used here will follow a modified `Sparkle 1.0` syntax, unlike what [fimpp](https://github.com/KarolS/fimpp) uses. Please refer to the sample reports to see the differences.

# 🖥 Usage
## CLI

```sh
$ ./fim Reports/hello.fim
Hello World!
```

## As a library 

```csharp
using fim;
using fim.celestia;

Interpreter i = Letter.WriteLetter(
    """
    Dear Princess Celestia: Hello World!
    Today I learned how to say hello world!
        I said "Hello World!".
    That's all about how to say hello world.
    Your faithful student, Twilight Sparkle.
    """
);

interpreter.MainParagraph?.Execute(); // Outputs "Hello World!" into the console.
```

See the [reports folder](./fim.cli/Reports/), or the [tests project](./fim.tests/) for sample reports you can run on fim.

# 📚 External Resources

-   [Original Equestria Daily Post](https://www.equestriadaily.com/2012/10/editorial-fim-pony-programming-language.html)

-   [Esolangs Page](https://esolangs.org/wiki/FiM%2B%2B)

-   [Language Specification](https://docs.google.com/document/d/1gU-ZROmZu0Xitw_pfC1ktCDvJH5rM85TxxQf5pg_xmg/edit#)

-   [FiM++ Fandom](https://fimpp.fandom.com)
