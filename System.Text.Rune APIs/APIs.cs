using System.Buffers;
using System.Text;

namespace System {
    public static class StringExtensions {
        public static bool Contains(this string @this, Rune value) {
            return @this.Contains(value, StringComparison.Ordinal);
        }
        public static bool Contains(this string @this, Rune value, StringComparison comparisonType) {
            return @this.IndexOf(value, comparisonType) >= 0;
        }

        public static bool StartsWith(this string @this, Rune value) {
            return @this.StartsWith(value, StringComparison.Ordinal);
        }
        public static bool StartsWith(this string @this, Rune value, StringComparison comparisonType) {
            if (Rune.DecodeFromUtf16(@this, out Rune result, out _) is OperationStatus.Done) {
                return result.Equals(value, comparisonType);
            }
            return false;
        }
        public static bool StartsWith(this string @this, char value, StringComparison comparisonType) {
            if (@this.Length == 0) {
                return false;
            }
            return @this[0].Equals(value, comparisonType);
        }

        public static bool EndsWith(this string @this, Rune value) {
            return @this.EndsWith(value, StringComparison.Ordinal);
        }
        public static bool EndsWith(this string @this, Rune value, StringComparison comparisonType) {
            if (Rune.DecodeLastFromUtf16(@this, out Rune result, out _) is OperationStatus.Done) {
                return result.Equals(value, comparisonType);
            }
            return false;
        }
        public static bool EndsWith(this string @this, char value, StringComparison comparisonType) {
            if (@this.Length == 0) {
                return false;
            }
            return @this[^1].Equals(value, comparisonType);
        }

        public static int IndexOf(this string @this, Rune value) {
            return @this.IndexOf(value, StringComparison.Ordinal);
        }
        public static int IndexOf(this string @this, Rune value, int startIndex) {
            return @this.IndexOf(value, startIndex, StringComparison.Ordinal);
        }
        public static int IndexOf(this string @this, Rune value, int startIndex, int count) {
            return @this.IndexOf(value, startIndex, count, StringComparison.Ordinal);
        }
        public static int IndexOf(this string @this, Rune value, StringComparison comparisonType) {
            return @this.IndexOf(value, 0, comparisonType);
        }
        public static int IndexOf(this string @this, Rune value, int startIndex, StringComparison comparisonType) {
            return @this.IndexOf(value, startIndex, @this.Length - startIndex, comparisonType);
        }
        public static int IndexOf(this string @this, Rune value, int startIndex, int count, StringComparison comparisonType) {
            ArgumentOutOfRangeException.ThrowIfLessThan(startIndex, 0);
            ArgumentOutOfRangeException.ThrowIfLessThan(count, 0);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(startIndex + count, @this.Length);

            int endIndex = startIndex + count;

            for (int index = startIndex; index <= endIndex;) {
                if (Rune.DecodeFromUtf16(@this.AsSpan(index..endIndex), out Rune rune, out int charsConsumed) is not OperationStatus.Done) {
                    return -1;
                }
                if (value.Equals(rune, comparisonType)) {
                    return index;
                }
                index += charsConsumed;
            }
            return -1;
        }

        public static int LastIndexOf(this string @this, Rune value) {
            return @this.LastIndexOf(value, StringComparison.Ordinal);
        }
        public static int LastIndexOf(this string @this, Rune value, int startIndex) {
            return @this.LastIndexOf(value, startIndex, StringComparison.Ordinal);
        }
        public static int LastIndexOf(this string @this, Rune value, int startIndex, int count) {
            return @this.LastIndexOf(value, startIndex, count, StringComparison.Ordinal);
        }
        public static int LastIndexOf(this string @this, Rune value, StringComparison comparisonType) {
            return @this.LastIndexOf(value, @this.Length - 1, comparisonType);
        }
        public static int LastIndexOf(this string @this, Rune value, int startIndex, StringComparison comparisonType) {
            return @this.LastIndexOf(value, startIndex, startIndex, comparisonType);
        }
        public static int LastIndexOf(this string @this, Rune value, int startIndex, int count, StringComparison comparisonType) {
            ArgumentOutOfRangeException.ThrowIfLessThan(startIndex, 0);
            ArgumentOutOfRangeException.ThrowIfLessThan(count, 0);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(startIndex - count, @this.Length);

            int endIndex = startIndex - count;

            for (int index = startIndex; index >= endIndex;) {
                if (Rune.DecodeLastFromUtf16(@this.AsSpan(endIndex..(index + 1)), out Rune rune, out int charsConsumed) is not OperationStatus.Done) {
                    return -1;
                }
                if (value.Equals(rune, comparisonType)) {
                    return index - (charsConsumed - 1);
                }
                index -= charsConsumed;
            }
            return -1;
        }

        public static string Replace(this string @this, Rune oldRune, Rune newRune) {
            return @this.Replace(oldRune, newRune, StringComparison.Ordinal);
        }
        public static string Replace(this string @this, Rune oldRune, Rune newRune, StringComparison stringComparison) {
            if (@this.Length == 0) {
                return @this;
            }

            // IMPORTANT / TODO:
            // We need an overload for string.Replace(ReadOnlySpan<char> oldValue, ReadOnlySpan<char> newValue, int startIndex, int count) to avoid using a StringBuilder.
            // The overload is already available internally through ReplaceCor but not exposed publicly.
            // When added, this method can look like the StringBuilder Replace method below.
            // For now we'll just build a string with a new StringBuilder.

            StringBuilder stringBuilder = new(@this.Length);

            foreach (Rune rune in @this.EnumerateRunes()) {
                if (oldRune.Equals(rune, stringComparison)) {
                    stringBuilder.Append(newRune);
                }
                else {
                    stringBuilder.Append(rune);
                }
            }

            return stringBuilder.ToString();
        }

        public static string[] Split(this string @this, Rune separator, StringSplitOptions options = StringSplitOptions.None) {
            return @this.Split(separator, int.MaxValue, options);
        }
        public static string[] Split(this string @this, Rune separator, int count, StringSplitOptions options = StringSplitOptions.None) {
            if (@this.Length == 0) {
                return [];
            }

            Span<char> separatorChars = stackalloc char[2];
            int separatorCharsWritten = separator.EncodeToUtf16(separatorChars);
            ReadOnlySpan<char> separatorCharsSlice = separatorChars[..separatorCharsWritten];

            // IMPORTANT / TODO:
            // We need an overload for string.Split(ReadOnlySpan<char>, int, StringSplitOptions) to avoid copying the separator to a string.
            // The overload is already available internally through SplitInternal but not exposed publicly.
            // For now we'll just convert the separator to a string.
            return @this.Split(separator.ToString(), count, options); // return @this.Split(separatorCharsSlice, count, options);
        }

        public static string Trim(this string @this, Rune trimRune) {
            if (@this.Length == 0) {
                return @this;
            }

            // Convert trimRune to span
            Span<char> trimChars = stackalloc char[2];
            int trimCharsWritten = trimRune.EncodeToUtf16(trimChars);
            ReadOnlySpan<char> trimCharsSlice = trimChars[..trimCharsWritten];

            // Trim start
            int index = 0;
            while (true) {
                if (index > @this.Length) {
                    return string.Empty;
                }
                if (!@this.AsSpan(index).StartsWith(trimCharsSlice)) {
                    break;
                }
                index += trimCharsSlice.Length;
            }

            // Trim end
            int endIndex = @this.Length - 1;
            while (true) {
                if (endIndex < index) {
                    return string.Empty;
                }
                if (!@this.AsSpan(..endIndex).EndsWith(trimCharsSlice)) {
                    break;
                }
                endIndex -= trimCharsSlice.Length;
            }

            return @this[index..endIndex];
        }
        public static string TrimStart(this string @this, Rune trimRune) {
            if (@this.Length == 0) {
                return @this;
            }

            // Convert trimRune to span
            Span<char> trimChars = stackalloc char[2];
            int trimCharsWritten = trimRune.EncodeToUtf16(trimChars);
            ReadOnlySpan<char> trimCharsSlice = trimChars[..trimCharsWritten];

            // Trim start
            int index = 0;
            while (true) {
                if (index > @this.Length) {
                    return string.Empty;
                }
                if (!@this.AsSpan(index).StartsWith(trimCharsSlice)) {
                    break;
                }
                index += trimCharsSlice.Length;
            }
            return @this[index..];
        }
        public static string TrimEnd(this string @this, Rune trimRune) {
            if (@this.Length == 0) {
                return @this;
            }

            // Convert trimRune to span
            Span<char> trimChars = stackalloc char[2];
            int trimCharsWritten = trimRune.EncodeToUtf16(trimChars);
            ReadOnlySpan<char> trimCharsSlice = trimChars[..trimCharsWritten];

            // Trim end
            int endIndex = @this.Length - 1;
            while (true) {
                if (endIndex < 0) {
                    return string.Empty;
                }
                if (!@this.AsSpan(..endIndex).EndsWith(trimCharsSlice)) {
                    break;
                }
                endIndex -= trimCharsSlice.Length;
            }
            return @this[..endIndex];
        }
    }

    public static class CharExtensions {
        public static bool Equals(this char @this, char value, StringComparison comparisonType) {
            ReadOnlySpan<char> leftCharsSlice = [@this];
            ReadOnlySpan<char> rightCharsSlice = [value];
            return leftCharsSlice.Equals(rightCharsSlice, comparisonType);
        }
    }
}

namespace System.Text {
    public static class RuneExtensions {
        public static bool Equals(this Rune left, Rune right) {
            return left == right;
        }
        public static bool Equals(this Rune left, Rune right, StringComparison comparisonType) {
            if (comparisonType is StringComparison.Ordinal) {
                return left == right;
            }

            // Convert left to span
            Span<char> leftChars = stackalloc char[2];
            int leftCharsWritten = left.EncodeToUtf16(leftChars);
            ReadOnlySpan<char> leftCharsSlice = leftChars[..leftCharsWritten];

            // Convert right to span
            Span<char> rightChars = stackalloc char[2];
            int rightCharsWritten = right.EncodeToUtf16(rightChars);
            ReadOnlySpan<char> rightCharsSlice = rightChars[..rightCharsWritten];

            // Compare span equality
            return leftCharsSlice.Equals(rightCharsSlice, comparisonType);
        }
    }

    public static class StringBuilderExtensions {
        public static StringBuilder Append(this StringBuilder @this, Rune value) {
            // Convert value to span
            Span<char> chars = stackalloc char[2];
            int charsWritten = value.EncodeToUtf16(chars);
            ReadOnlySpan<char> charsSlice = chars[..charsWritten];

            // Append span
            @this.Append(charsSlice);
            return @this;
        }

        public static Rune GetRuneAt(this StringBuilder @this, int index) {
            if (TryGetRuneAt(@this, index, out Rune value)) {
                return value;
            }
            throw new Exception($"Unable to get rune at {index}.");
        }

        public static bool TryGetRuneAt(this StringBuilder @this, int index, out Rune value) {
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, @this.Length);

            // Get span at StringBuilder index
            bool twoCharsAvailable = index + 1 < @this.Length;
            Span<char> chars = twoCharsAvailable
                ? [@this[index], @this[index + 1]]
                : [@this[index]];

            OperationStatus status = Rune.DecodeFromUtf16(chars, out Rune result, out _);
            if (status is OperationStatus.Done) {
                value = result;
                return true;
            }
            else {
                value = default;
                return false;
            }
        }

        public static StringBuilder Insert(this StringBuilder @this, int index, Rune value) {
            Span<char> chars = stackalloc char[2];
            int charsWritten = value.EncodeToUtf16(chars);
            @this.Insert(index, chars[..charsWritten]);
            return @this;
        }

        public static StringBuilder Replace(this StringBuilder @this, Rune oldRune, Rune newRune) {
            return @this.Replace(oldRune, newRune, 0, @this.Length);
        }
        public static StringBuilder Replace(this StringBuilder @this, Rune oldRune, Rune newRune, int startIndex, int count) {
            // Convert oldRune to span
            Span<char> leftChars = stackalloc char[2];
            int leftCharsWritten = oldRune.EncodeToUtf16(leftChars);
            ReadOnlySpan<char> leftCharsSlice = leftChars[..leftCharsWritten];

            // Convert newRune to span
            Span<char> rightChars = stackalloc char[2];
            int rightCharsWritten = newRune.EncodeToUtf16(rightChars);
            ReadOnlySpan<char> rightCharsSlice = rightChars[..rightCharsWritten];

            // Replace span with span
            return @this.Replace(leftCharsSlice, rightCharsSlice, startIndex, count);
        }
    }
}

namespace System.IO {
    public static class TextWriterExtensions {
        public static void Write(this TextWriter @this, Rune value) {
            // Convert value to span
            Span<char> chars = stackalloc char[2];
            int charsWritten = value.EncodeToUtf16(chars);
            ReadOnlySpan<char> charsSlice = chars[..charsWritten];

            // Write span
            @this.Write(charsSlice);
        }

        public static Task WriteAsync(this TextWriter @this, Rune value) {
            // Convert value to span
            Span<char> chars = stackalloc char[2];
            int charsWritten = value.EncodeToUtf16(chars);
            ReadOnlySpan<char> charsSlice = chars[..charsWritten];

            // Write chars individually (can't use span with async)
            if (charsSlice.Length == 2) {
                async Task WriteAsyncPair(char highSurrogate, char lowSurrogate) {
                    await @this.WriteAsync(highSurrogate);
                    await @this.WriteAsync(lowSurrogate);
                }
                return WriteAsyncPair(charsSlice[0], charsSlice[1]);
            }
            else {
                return @this.WriteAsync(charsSlice[0]);
            }
        }

        public static void WriteLine(this TextWriter @this, Rune value) {
            // Convert value to span
            Span<char> chars = stackalloc char[2];
            int charsWritten = value.EncodeToUtf16(chars);
            ReadOnlySpan<char> charsSlice = chars[..charsWritten];

            // Write span
            @this.WriteLine(charsSlice);
        }

        public static Task WriteLineAsync(this TextWriter @this, Rune value) {
            // Convert value to span
            Span<char> chars = stackalloc char[2];
            int charsWritten = value.EncodeToUtf16(chars);
            ReadOnlySpan<char> charsSlice = chars[..charsWritten];

            // Write chars individually (can't use span with async)
            if (charsSlice.Length == 2) {
                async Task WriteLineAsyncPair(char highSurrogate, char lowSurrogate) {
                    await @this.WriteAsync(highSurrogate);
                    await @this.WriteLineAsync(lowSurrogate);
                }
                return WriteLineAsyncPair(charsSlice[0], charsSlice[1]);
            }
            else {
                return @this.WriteLineAsync(charsSlice[0]);
            }
        }
    }
}

namespace System.Globalization {
    public static class TextInfoExtensions {
        public static Rune ToLower(this TextInfo @this, Rune r) {
            // Convert rune to span
            Span<char> chars = stackalloc char[2];
            int charsWritten = r.EncodeToUtf16(chars);
            ReadOnlySpan<char> charsSlice = chars[..charsWritten];

            // Change span to lower and convert to rune
            if (charsSlice.Length == 2) {
                return new Rune(@this.ToLower(charsSlice[0]), @this.ToLower(charsSlice[1]));
            }
            else {
                return new Rune(@this.ToLower(charsSlice[0]));
            }
        }
        public static Rune ToUpper(this TextInfo @this, Rune r) {
            // Convert rune to span
            Span<char> chars = stackalloc char[2];
            int charsWritten = r.EncodeToUtf16(chars);
            ReadOnlySpan<char> charsSlice = chars[..charsWritten];

            // Change span to upper and convert to rune
            if (charsSlice.Length == 2) {
                return new Rune(@this.ToUpper(charsSlice[0]), @this.ToUpper(charsSlice[1]));
            }
            else {
                return new Rune(@this.ToUpper(charsSlice[0]));
            }
        }
    }
}