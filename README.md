### IMPORTANT

This project has fallen out of favor, as there are already other serializers that do the same if not better, now.
This project is hence no-longer scheduled to be supported.

# Yama Serializer

A general purpose automatic data serializer that performs just as fast as manual serialization, at the expense of initial load time.
It is made with `Game Networking` as the main use case, but can be used for other things as well.

The goal is to eliminate the possibility of human error & to boost productivity by cutting down on development time.

It is built using C#'s `System.Linq.Expression` & `Sytem.Reflection` libraries, is AOT and .NET Standard 2.0 compliant.

*Disclaimer: This is still work in progress, it is considered to be in a `PRE-ALPHA` state. Some core features may not yet exist or be broken. It's schedueled to be completely rewritten, fully breaking backwards compatibility. It's stable enough for production use, but I'd wait a while.*

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
	1. `public static Action<WriterType, ClassType> serialize;`
	2. `public static Action<ReaderType, ClassType> deserialize;`
5. Call `GenericSerializer.Initialize();` at program startup.
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

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details

## Notes

- Whilst this is fairly stable, it's not production ready if you don't know what you're doing.
