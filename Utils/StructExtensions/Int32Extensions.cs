using System;

namespace Commons.Utils.StructExtensions
{
    public static class Int32Extensions
    {
        public static int ParseOrDefault(string val, int defaultVal)
        {
            try
            {
                return int.Parse(val);
            }
            catch
            {
                return defaultVal;
            }
        }

        private static bool IsIntegerType(Type type)
        {
            return Type.GetTypeCode(type) switch
            {
                TypeCode.Byte or TypeCode.SByte or TypeCode.UInt16 or TypeCode.UInt32 or TypeCode.UInt64 or TypeCode.Int16 or TypeCode.Int32 or TypeCode.Int64 or TypeCode.Decimal => true,
                _ => type.IsArray && IsIntegerType(type.GetElementType()),
            };
        }

        public static bool IsNumeric(this Type type) => type == typeof(byte) ||
                type == typeof(sbyte) ||
                type == typeof(int) ||
                type == typeof(uint) ||
                type == typeof(short) ||
                type == typeof(ushort) ||
                type == typeof(long) ||
                type == typeof(ulong) ||
                type == typeof(float) ||
                type == typeof(double) ||
                type == typeof(decimal);

        public static bool IsFloatingPoint(this Type type)
        {
            return Type.GetTypeCode(type) switch
            {
                TypeCode.Decimal or TypeCode.Double or TypeCode.Single => true,
                _ => false,
            };
        }

        public static bool IsSigned(this Type type)
        {
            return Type.GetTypeCode(type) switch
            {
                TypeCode.SByte or TypeCode.Int16 or TypeCode.Int32 or TypeCode.Int64 or TypeCode.Decimal or TypeCode.Double or TypeCode.Single => true,
                _ => false,
            };
        }

        public static bool IsInteger(this Type type) => type.IsPrimitive && IsIntegerType(type);

        public static bool IsSignedInteger(this Type type) => type.IsSigned() && IsInteger(type);
    }
}
