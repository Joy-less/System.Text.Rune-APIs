namespace System.Text.Rune_API_Tests {
    public class UnitTest1 {
        private static Rune GrinningFaceRune => new(0x1F600); // Grinning Face 😀 - Non-BMP
        private static Rune SpanishUpperNRune => new('Ñ'); // Spanish uppercase N - BMP
        private static Rune SpanishLowerNRune => new('ñ'); // Spanish lowercase N - BMP

        [Fact]
        public void StringBuilder_Append__Rune() {
            StringBuilder stringBuilder = new("abcde");
            stringBuilder.Append(GrinningFaceRune);
            Assert.Equal($"abcde{GrinningFaceRune}", stringBuilder.ToString());
        }
        [Fact]
        public void StringBuilder_GetRuneAt__Int32() {
            StringBuilder stringBuilder = new($"ab{GrinningFaceRune}cde");
            Rune rune = stringBuilder.GetRuneAt(2);
            Assert.Equal(GrinningFaceRune, rune);
        }
        [Fact]
        public void StringBuilder_TryGetRuneAt__Int32_Rune() {
            StringBuilder stringBuilder = new($"ab{GrinningFaceRune}cde");
            Assert.True(stringBuilder.TryGetRuneAt(2, out Rune rune));
            Assert.Equal(GrinningFaceRune, rune);

            Assert.False(stringBuilder.TryGetRuneAt(3, out Rune rune2));
            Assert.Equal(default, rune2);
        }
        [Fact]
        public void StringBuilder_Insert__Int32_Rune() {
            StringBuilder stringBuilder = new("abcde");
            stringBuilder.Insert(2, GrinningFaceRune);
            Assert.Equal("ab😀cde", stringBuilder.ToString());
        }
        [Fact]
        public void Rune_Equals__Rune_Rune() {
            Assert.False(RuneExtensions.Equals(SpanishLowerNRune, SpanishUpperNRune));
        }
        [Fact]
        public void Rune_Equals__Rune_Rune_StringComparison() {
            Assert.False(RuneExtensions.Equals(SpanishLowerNRune, SpanishUpperNRune, StringComparison.Ordinal));
            Assert.True(RuneExtensions.Equals(SpanishLowerNRune, SpanishUpperNRune, StringComparison.InvariantCultureIgnoreCase));
        }
        [Fact]
        public void Char_Equals__Char_Char_StringComparison() {
            Assert.False('a'.Equals('A', StringComparison.InvariantCulture));
            Assert.True('a'.Equals('A', StringComparison.InvariantCultureIgnoreCase));
        }
        [Fact]
        public void String_Contains__Rune() {
            Assert.False($"abcde".Contains(GrinningFaceRune));
            Assert.True($"abc{GrinningFaceRune}de".Contains(GrinningFaceRune));
        }
        [Fact]
        public void String_Contains__Rune_StringComparison() {
            Assert.False($"abcde".Contains(SpanishUpperNRune));
            Assert.False($"abc{SpanishLowerNRune}de".Contains(SpanishUpperNRune, StringComparison.Ordinal));
            Assert.True($"abc{SpanishLowerNRune}de".Contains(SpanishUpperNRune, StringComparison.InvariantCultureIgnoreCase));
        }
        [Fact]
        public void String_StartsWith__Rune() {
            Assert.False($"abcde".StartsWith(SpanishUpperNRune));
            Assert.True($"{SpanishUpperNRune}abcde".StartsWith(SpanishUpperNRune));
        }
        [Fact]
        public void String_StartsWith__Rune_StringComparison() {
            Assert.False($"abcde".StartsWith(SpanishUpperNRune));
            Assert.False($"{SpanishLowerNRune}abcde".StartsWith(SpanishUpperNRune, StringComparison.Ordinal));
            Assert.True($"{SpanishLowerNRune}abcde".StartsWith(SpanishUpperNRune, StringComparison.InvariantCultureIgnoreCase));
        }
        [Fact]
        public void String_StartsWith__Char_StringComparison() {
            Assert.False($"{SpanishLowerNRune}abcde".StartsWith((char)SpanishUpperNRune.Value, StringComparison.Ordinal));
            Assert.True($"{SpanishLowerNRune}abcde".StartsWith((char)SpanishUpperNRune.Value, StringComparison.InvariantCultureIgnoreCase));
            Assert.False($"a{SpanishLowerNRune}bcdef".StartsWith((char)SpanishUpperNRune.Value, StringComparison.InvariantCultureIgnoreCase));
        }
        [Fact]
        public void String_EndsWith__Rune() {
            Assert.False($"abcde".EndsWith(SpanishUpperNRune));
            Assert.True($"abcde{SpanishUpperNRune}".EndsWith(SpanishUpperNRune));
            Assert.False($"abcd{SpanishUpperNRune}e".EndsWith(SpanishUpperNRune));
        }
        [Fact]
        public void String_EndsWith__Rune_StringComparison() {
            Assert.False($"abcde".EndsWith(SpanishUpperNRune));
            Assert.False($"abcde{SpanishLowerNRune}".EndsWith(SpanishUpperNRune, StringComparison.Ordinal));
            Assert.True($"abcde{SpanishLowerNRune}".EndsWith(SpanishUpperNRune, StringComparison.InvariantCultureIgnoreCase));
            Assert.False($"abcde{SpanishLowerNRune}f".EndsWith(SpanishUpperNRune, StringComparison.InvariantCultureIgnoreCase));
        }
        [Fact]
        public void String_EndsWith__Char_StringComparison() {
            Assert.False($"abcde{SpanishLowerNRune}".EndsWith((char)SpanishUpperNRune.Value, StringComparison.Ordinal));
            Assert.True($"abcde{SpanishLowerNRune}".EndsWith((char)SpanishUpperNRune.Value, StringComparison.InvariantCultureIgnoreCase));
            Assert.False($"abcde{SpanishLowerNRune}f".EndsWith((char)SpanishUpperNRune.Value, StringComparison.InvariantCultureIgnoreCase));
        }
        [Fact]
        public void String_IndexOf__Rune() {
            Assert.Equal(2, $"ab{SpanishLowerNRune}c".IndexOf(SpanishLowerNRune));
        }
        [Fact]
        public void String_IndexOf__Rune_Int32() {
            Assert.Equal(2, $"ab{SpanishLowerNRune}c".IndexOf(SpanishLowerNRune, 2));
            Assert.Equal(-1, $"ab{SpanishLowerNRune}c".IndexOf(SpanishLowerNRune, 3));
        }
        [Fact]
        public void String_IndexOf__Rune_Int32_Int32() {
            Assert.Equal(2, $"ab{SpanishLowerNRune}c".IndexOf(SpanishLowerNRune, 2, 1));
            Assert.Equal(-1, $"ab{SpanishLowerNRune}c".IndexOf(SpanishLowerNRune, 3, 1));
        }
        [Fact]
        public void String_IndexOf__Rune_Int32_Int32_StringComparison() {
            Assert.Equal(-1, $"ab{SpanishLowerNRune}c".IndexOf(SpanishUpperNRune, 2, 1, StringComparison.Ordinal));
            Assert.Equal(2, $"ab{SpanishLowerNRune}c".IndexOf(SpanishUpperNRune, 2, 1, StringComparison.InvariantCultureIgnoreCase));
        }
        [Fact]
        public void String_LastIndexOf__Rune_StringComparison() {
            Assert.Equal(-1, $"ab{SpanishLowerNRune}c".LastIndexOf(SpanishUpperNRune, StringComparison.Ordinal));
            Assert.Equal(2, $"ab{SpanishLowerNRune}c".LastIndexOf(SpanishUpperNRune, StringComparison.InvariantCultureIgnoreCase));
            Assert.Equal(4, $"ab{SpanishLowerNRune}c{SpanishLowerNRune}".LastIndexOf(SpanishUpperNRune, StringComparison.InvariantCultureIgnoreCase));
        }
        [Fact]
        public void String_Replace__Rune_Rune_StringComparison() {
            Assert.Equal($"ab{SpanishLowerNRune}{SpanishLowerNRune}c",
                $"ab{SpanishLowerNRune}{SpanishLowerNRune}c".Replace(SpanishUpperNRune, GrinningFaceRune, StringComparison.Ordinal));
            Assert.Equal($"ab{GrinningFaceRune}{GrinningFaceRune}c",
                $"ab{SpanishLowerNRune}{SpanishLowerNRune}c".Replace(SpanishUpperNRune, GrinningFaceRune, StringComparison.InvariantCultureIgnoreCase));
        }
        [Fact]
        public void String_Trim_Start__Rune() {
            Assert.Equal($"a{SpanishLowerNRune}b", $"{SpanishLowerNRune}{SpanishLowerNRune}a{SpanishLowerNRune}b".TrimStart(SpanishLowerNRune));
            Assert.Equal("", $"{SpanishLowerNRune}".TrimStart(SpanishLowerNRune));
        }
        [Fact]
        public void String_Trim_End__Rune() {
            Assert.Equal($"a{SpanishLowerNRune}{SpanishLowerNRune}b", $"a{SpanishLowerNRune}{SpanishLowerNRune}b{SpanishLowerNRune}".TrimEnd(SpanishLowerNRune));
            Assert.Equal("", $"{SpanishLowerNRune}".TrimEnd(SpanishLowerNRune));
            Assert.Equal($"x", $"x{SpanishLowerNRune}{SpanishLowerNRune}".TrimEnd(SpanishLowerNRune));
        }
        [Fact]
        public void String_Trim__Rune() {
            Assert.Equal($"x", $"{SpanishLowerNRune}x{SpanishLowerNRune}".Trim(SpanishLowerNRune));
            Assert.Equal("", $"{SpanishLowerNRune}".Trim(SpanishLowerNRune));
        }
        [Fact]
        public void TextWriter_Write__Rune() {
            using MemoryStream memStream = new();
            using (StreamWriter writer = new(memStream, Encoding.UTF8, leaveOpen: true)) {
                writer.Write(GrinningFaceRune);
            }
            memStream.Position = 0;
            using StreamReader reader = new(memStream, Encoding.UTF8);
            string read = reader.ReadToEnd();
            Assert.Equal(GrinningFaceRune.ToString(), read);
        }
        [Fact]
        public async Task TextWriter_WriteLineAsync__Rune() {
            using MemoryStream memStream = new();
            using (StreamWriter writer = new(memStream, Encoding.UTF8, leaveOpen: true)) {
                await writer.WriteLineAsync(GrinningFaceRune);
            }
            memStream.Position = 0;
            using StreamReader reader = new(memStream, Encoding.UTF8);
            string read = reader.ReadToEnd();
            Assert.Equal(GrinningFaceRune.ToString() + Environment.NewLine, read);
        }
    }
}
