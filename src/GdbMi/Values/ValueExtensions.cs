namespace GdbMi.Values
{
    /// <summary>
    /// <c>ValueExtensions</c> extends <c>Value</c> objects with additional utilties.
    /// </summary>
    public static class ValueExtensions
    {
        /// <summary>
        /// Converts <c>Value</c> to <c>string</c>.
        /// </summary>
        /// <param name="value">Underlying value on which to perform a type conversion.</param>
        /// <param name="result">When the method returns contains a <c>string</c> representation of the underlying value, if the conversion succeeded, or <c>default(string)</c> if the conversion failed.</param>
        /// <returns>true if <c>value</c> was converted successfully; otherwise, false.</returns>
        public static bool ConvertValue(this Value value, out string result)
        {
            switch (value)
            {
                case var v when v is ConstValue constValue:
                    result = constValue.Value;
                    return true;

                case var v when v is ResultValue resultValue && resultValue.Value is ConstValue constValue:
                    result = constValue.Value;
                    return true;

                default:
                    result = default;
                    return false;
            }
        }

        /// <summary>
        /// Converts <c>Value</c> to <c>int</c>.
        /// </summary>
        /// <param name="value">Underlying value on which to perform a type conversion.</param>
        /// <param name="result">When the method returns contains a <c>int</c> representation of the underlying value, if the conversion succeeded, or <c>default(int)</c> if the conversion failed.</param>
        /// <returns>true if <c>value</c> was converted successfully; otherwise, false.</returns>
        public static bool ConvertValue(this Value value, out int result)
        {
            switch (value)
            {
                case var v when v is ConstValue constValue:
                    return int.TryParse(constValue.Value, out result);

                case var v when v is ResultValue resultValue && resultValue.Value is ConstValue constValue:
                    return int.TryParse(constValue.Value, out result);

                default:
                    result = default;
                    return false;
            }
        }

        /// <summary>
        /// Converts <c>Value</c> to <c>long</c>.
        /// </summary>
        /// <param name="value">Underlying value on which to perform a type conversion.</param>
        /// <param name="result">When the method returns contains a <c>long</c> representation of the underlying value, if the conversion succeeded, or <c>default(long)</c> if the conversion failed.</param>
        /// <returns>true if <c>value</c> was converted successfully; otherwise, false.</returns>
        public static bool ConvertValue(this Value value, out long result)
        {
            switch (value)
            {
                case var v when v is ConstValue constValue:
                    return long.TryParse(constValue.Value, out result);

                case var v when v is ResultValue resultValue && resultValue.Value is ConstValue constValue:
                    return long.TryParse(constValue.Value, out result);

                default:
                    result = default;
                    return false;
            }
        }
    }
}
