![Icon](icons/package_icon.png)

Byte Encodings
==============

Provides the easy way to convert bytes to any string format in dotnet core. 
Many formats (Hex, Base32, Base58, Base64 and [more](src/ByteEncodings/KnownAlphabets.cs)) 
are already included out of the box. 
Backward conversion from string to bytes are supported as well.

### What you need to know
This is not predicatable length encoding like Hex or Base64. It converts bytes
to BigInteger and expresses the number in a base which matches _alphabet_ length.

What this means to you is
 * don't use it with bunch of bytes - it'll be slow,
 * converting from string you should know the expected bytes length 

### Instalation
Nothing fancy - install nuget package
```
Install-Package ByteEncodings 
```

### Usage
Look for inspiration in [Test/Inspiration.cs](src/ByteEncodings.Test/Inspirations.cs), 
but in shortcut, use class:
 * ByteEncoding - for bytes encodings to plenty string formats,
 * Alphabet - if you need own alphabets / formats,
 * BaseConverter - Integer arithmetic, number base convertion

Take a look in [Test project](src/ByteEncodings.Test/Inspirations.cs) for sample usage.

### Like it? Give me a star!
Thank you! I appreciate your contribution, bug reports, thougths, **stars** :+1:

Questions :question: - just fill an [issue](https://github.com/pnowosie/ByteEncodings/issues/new).

### Icon
[Translation](https://thenounproject.com/search/?q=translation&i=182571) by diavd from the Noun Project