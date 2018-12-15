# Blue's Generic Serializer

A general purpose automatic data serializer that performs just as fast as manual serialization, at the expense of initial load time.
It is made with `Game Networking` as the main use case, but can be used for other things as well.

The goal is to eliminate the possibility of human error & to boost productivity by cutting down on development time.

It is built using C#'s `System.Linq.Expression` & `Sytem.Reflection` libraries, is AOT and .NET Standard 2.0 compliant.

*Disclaimer: This is still work in progress, it is considered to be in a `PRE-ALPHA` state. Some core features may not yet exist or be broken.*

## Core Feature List

#### Serialization Options
- [x] Selective serialization - simple inheritance chains.
- [ ] Selective serialization - extended inheritance chains.
- [x] Instance member serialization
- [ ] Static member serialization

#### Type Serialization
- [x] Primitive type member serialization.
- [x] Class type member serialization.
- [x] Primitive type based 1-dimension array & list serialization. - (In development, release is bugged.)
- [x] User defined type based 1-dimension array & list serialization. - (In development, release is bugged.)
- [ ] Multi dimension array & list serialization.
- [ ] Dictionary serialization.
- [ ] Struct serialization.


## Getting Started

1. Clone or download.
2. Build `BlueGenericSerializer` or reference it in your project directly.
3. Add the attribute `[GenericSerializable(options)]` to your serializable class.
4. Add the following delegates to your class:
	1. `public static Action<WriterType, ClassType> serialize;` [#17](https://github.com/Reousa/BlueGenericSerializer/issues/17)
	2. `public static Action<ReaderType, ClassType> deserialize;` [#17](https://github.com/Reousa/BlueGenericSerializer/issues/17)
5. Call `GenericSerializer.Initialize();`.
6. Call `ClassType.serialize(WriterInstance, ClassTypeInstance);` to serialize.
7. Done!

*More info to be added through docs.*

### Prerequisites

* Visual Studio 2017
* .NET Standard 2.0 compliant framework.


## Contributing

You may contribute through issues & pull requests. (WIP)

## Versioning

[SemVer](http://semver.org/). (WIP)

## Authors

* **[Karim H. Rizk](https://github.com/Reousa)** - *Idea & implementation*

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details

## Acknowledgments

- I would specifically like to thank [Michael Kelly](https://github.com/Michael-Kelley) for being the amazing friend & mentor that he is.
- Thanks to Google, Stackoverflow, Brilliant.org & the various book authors who've made knowledge & self-teaching accessible in the recent years.
