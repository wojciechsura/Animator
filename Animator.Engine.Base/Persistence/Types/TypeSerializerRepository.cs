
/******************************************************************************
*                                                                             *
* This code was generated automatically from a template. Don't modify it,     *
* because all your changes will be overwritten. Instead, if needed, modify    *
* the template file (*.tt)                                                    *
*                                                                             *
******************************************************************************/

using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Collections.Generic;

namespace Animator.Engine.Base.Persistence.Types
{
    public class TypeSerializerRepository
	{
		// Private types ------------------------------------------------------------

        private class ByteSerializer : TypeSerializer
		{
			public override bool CanSerialize(object obj) => obj is byte;
			public override bool CanDeserialize(string str) => byte.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out _);
			public override object Deserialize(string str) => byte.Parse(str, CultureInfo.InvariantCulture);
			public override string Serialize(object obj) => ((byte)obj).ToString(CultureInfo.InvariantCulture);
		}

        private class SbyteSerializer : TypeSerializer
		{
			public override bool CanSerialize(object obj) => obj is sbyte;
			public override bool CanDeserialize(string str) => sbyte.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out _);
			public override object Deserialize(string str) => sbyte.Parse(str, CultureInfo.InvariantCulture);
			public override string Serialize(object obj) => ((sbyte)obj).ToString(CultureInfo.InvariantCulture);
		}

        private class ShortSerializer : TypeSerializer
		{
			public override bool CanSerialize(object obj) => obj is short;
			public override bool CanDeserialize(string str) => short.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out _);
			public override object Deserialize(string str) => short.Parse(str, CultureInfo.InvariantCulture);
			public override string Serialize(object obj) => ((short)obj).ToString(CultureInfo.InvariantCulture);
		}

        private class UshortSerializer : TypeSerializer
		{
			public override bool CanSerialize(object obj) => obj is ushort;
			public override bool CanDeserialize(string str) => ushort.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out _);
			public override object Deserialize(string str) => ushort.Parse(str, CultureInfo.InvariantCulture);
			public override string Serialize(object obj) => ((ushort)obj).ToString(CultureInfo.InvariantCulture);
		}

        private class IntSerializer : TypeSerializer
		{
			public override bool CanSerialize(object obj) => obj is int;
			public override bool CanDeserialize(string str) => int.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out _);
			public override object Deserialize(string str) => int.Parse(str, CultureInfo.InvariantCulture);
			public override string Serialize(object obj) => ((int)obj).ToString(CultureInfo.InvariantCulture);
		}

        private class UintSerializer : TypeSerializer
		{
			public override bool CanSerialize(object obj) => obj is uint;
			public override bool CanDeserialize(string str) => uint.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out _);
			public override object Deserialize(string str) => uint.Parse(str, CultureInfo.InvariantCulture);
			public override string Serialize(object obj) => ((uint)obj).ToString(CultureInfo.InvariantCulture);
		}

        private class LongSerializer : TypeSerializer
		{
			public override bool CanSerialize(object obj) => obj is long;
			public override bool CanDeserialize(string str) => long.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out _);
			public override object Deserialize(string str) => long.Parse(str, CultureInfo.InvariantCulture);
			public override string Serialize(object obj) => ((long)obj).ToString(CultureInfo.InvariantCulture);
		}

        private class UlongSerializer : TypeSerializer
		{
			public override bool CanSerialize(object obj) => obj is ulong;
			public override bool CanDeserialize(string str) => ulong.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out _);
			public override object Deserialize(string str) => ulong.Parse(str, CultureInfo.InvariantCulture);
			public override string Serialize(object obj) => ((ulong)obj).ToString(CultureInfo.InvariantCulture);
		}

        private class FloatSerializer : TypeSerializer
		{
			public override bool CanSerialize(object obj) => obj is float;
			public override bool CanDeserialize(string str) => float.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out _);
			public override object Deserialize(string str) => float.Parse(str, CultureInfo.InvariantCulture);
			public override string Serialize(object obj) => ((float)obj).ToString(CultureInfo.InvariantCulture);
		}

        private class DoubleSerializer : TypeSerializer
		{
			public override bool CanSerialize(object obj) => obj is double;
			public override bool CanDeserialize(string str) => double.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out _);
			public override object Deserialize(string str) => double.Parse(str, CultureInfo.InvariantCulture);
			public override string Serialize(object obj) => ((double)obj).ToString(CultureInfo.InvariantCulture);
		}

		private class StringSerializer : TypeSerializer
		{
			public override bool CanSerialize(object obj) => obj is string;
			public override bool CanDeserialize(string str) => true;
			public override object Deserialize(string str) => str;
			public override string Serialize(object obj) => (string)obj;
		}

		private class BoolSerializer : TypeSerializer
		{
			public override bool CanSerialize(object obj) => obj is bool;
			public override bool CanDeserialize(string str) => bool.TryParse(str, out _);
			public override object Deserialize(string str) => bool.Parse(str);
			public override string Serialize(object obj) => ((bool)obj).ToString();
		}

		private class ColorSerializer : TypeSerializer
		{
			private readonly Regex rgbColorFormat = new Regex("^#[a-fA-F0-9]{6}$");
			private readonly Regex argbColorFormat = new Regex("^#[a-fA-F0-9]{8}$");
			private readonly Regex numRgbColorFormat = new Regex("^[0-9]{,3}(,[0-9]{,3}){2}$");
			private readonly Regex numArgbColorFormat = new Regex("^[0-9]{,3}(,[0-9]{,3}){3}$");

			public override bool CanSerialize(object obj)
			{
				return obj is System.Drawing.Color;
			}

			public override bool CanDeserialize(string str)
			{
				if (Enum.TryParse(typeof(KnownColor), str, out object knownColor))
					return true;

				if (rgbColorFormat.IsMatch(str) ||
					argbColorFormat.IsMatch(str) ||
					numRgbColorFormat.IsMatch(str) ||
					numArgbColorFormat.IsMatch(str))
					return true;

				return false;
			}

			public override object Deserialize(string str)
            {
				if (String.IsNullOrEmpty(str))
					throw new InvalidCastException($"Cannot convert empty string to color!");

				if (Enum.TryParse(typeof(KnownColor), str, out object knownColor))
					return Color.FromKnownColor((KnownColor)knownColor);

				// Format #aabbcc
				if (rgbColorFormat.IsMatch(str))
                {
					int r = Convert.ToInt32(str[1..3], 16);
					int g = Convert.ToInt32(str[3..5], 16);
					int b = Convert.ToInt32(str[5..7], 16);

					return Color.FromArgb(255, r, g, b);
                }
				
				if (argbColorFormat.IsMatch(str))
                {
					int a = Convert.ToInt32(str[1..3], 16);
					int r = Convert.ToInt32(str[3..5], 16);
					int g = Convert.ToInt32(str[5..7], 16);
					int b = Convert.ToInt32(str[7..9], 16);

					return Color.FromArgb(a, r, g, b);
				}

				if (numRgbColorFormat.IsMatch(str))
                {
					string[] values = str.Split(',');

					int r = Convert.ToInt32(values[0]);
					int g = Convert.ToInt32(values[1]);
					int b = Convert.ToInt32(values[2]);

					if (r > 255 || g > 255 || b > 255)
						throw new InvalidCastException($"Color constant values exceeds 255: {str}");

					return Color.FromArgb(255, r, g, b);
                }

				if (numArgbColorFormat.IsMatch(str))
                {
					string[] values = str.Split(',');

					int a = Convert.ToInt32(values[0]);
					int r = Convert.ToInt32(values[1]);
					int g = Convert.ToInt32(values[2]);
					int b = Convert.ToInt32(values[3]);

					if (r > 255 || g > 255 || b > 255 || a > 255)
						throw new InvalidCastException($"Color constant values exceeds 255: {str}");

					return Color.FromArgb(255, r, g, b);

				}

				throw new InvalidCastException($"Invalid color format: {str}");
			}

            public override string Serialize(object obj)
            {
				var color = (Color)obj;

				if (color.IsKnownColor)
					return color.ToKnownColor().ToString();

				if (color.A < 255)
					return $"#{color.R:x2}{color.G:x2}{color.B:x2}";
				else
					return $"#{color.R:x2}{color.G:x2}{color.B:x2}{color.A:x2}";
		    }
        }	

        private class PointSerializer : TypeSerializer
		{
			private readonly Regex pointRegex = new Regex("^\\-?[0-9]+[,;]\\-?[0-9]+$");

			public override bool CanSerialize (object obj) => obj is Point;
			public override bool CanDeserialize(string str) => pointRegex.IsMatch(str);

			public override object Deserialize(string str)
			{
				if (pointRegex.IsMatch(str))
				{
					string[] values = str.Split(new char[] {',', ';'});

					var x = int.Parse(values[0], CultureInfo.InvariantCulture);
					var y = int.Parse(values[1], CultureInfo.InvariantCulture);

					return new Point(x, y);
				}

				throw new InvalidCastException($"Unsupported point format: {str}");
			}

			public override string Serialize(object obj)
			{
				var point = (Point)obj;

				return $"{point.X.ToString(CultureInfo.InvariantCulture)};{point.Y.ToString(CultureInfo.InvariantCulture)}";
			}
		}

        private class PointFSerializer : TypeSerializer
		{
			private readonly Regex pointRegex = new Regex("^\\-?[0-9]+(\\.[0-9]+)?[,;]\\-?[0-9]+(\\.[0-9]+)?$");

			public override bool CanSerialize (object obj) => obj is PointF;
			public override bool CanDeserialize(string str) => pointRegex.IsMatch(str);

			public override object Deserialize(string str)
			{
				if (pointRegex.IsMatch(str))
				{
					string[] values = str.Split(new char[] {',', ';'});

					var x = float.Parse(values[0], CultureInfo.InvariantCulture);
					var y = float.Parse(values[1], CultureInfo.InvariantCulture);

					return new PointF(x, y);
				}

				throw new InvalidCastException($"Unsupported point format: {str}");
			}

			public override string Serialize(object obj)
			{
				var point = (PointF)obj;

				return $"{point.X.ToString(CultureInfo.InvariantCulture)};{point.Y.ToString(CultureInfo.InvariantCulture)}";
			}
		}

        private class TimeSpanSerializer : TypeSerializer
		{
			public override bool CanSerialize(object obj) => obj is TimeSpan;
			public override bool CanDeserialize(string str) => TimeSpan.TryParse(str, CultureInfo.InvariantCulture, out _);

            public override object Deserialize(string str)
            {
				if (!TimeSpan.TryParse(str, CultureInfo.InvariantCulture, out TimeSpan result))
					throw new InvalidCastException($"Invalid TimeSpan format: {str}");

				return result;
            }

            public override string Serialize(object obj)
            {
				return ((TimeSpan)obj).ToString();
            }
        }

		// Private static fields -----------------------------------------------------

		private static readonly Dictionary<Type, TypeSerializer> serializers = new();

		// Static ctor ---------------------------------------------------------------

		static TypeSerializerRepository()
		{
			serializers[typeof(byte)] = new ByteSerializer();
			serializers[typeof(sbyte)] = new SbyteSerializer();
			serializers[typeof(short)] = new ShortSerializer();
			serializers[typeof(ushort)] = new UshortSerializer();
			serializers[typeof(int)] = new IntSerializer();
			serializers[typeof(uint)] = new UintSerializer();
			serializers[typeof(long)] = new LongSerializer();
			serializers[typeof(ulong)] = new UlongSerializer();
			serializers[typeof(float)] = new FloatSerializer();
			serializers[typeof(double)] = new DoubleSerializer();
			serializers[typeof(Point)] = new PointSerializer();
			serializers[typeof(PointF)] = new PointFSerializer();
			serializers[typeof(string)] = new StringSerializer();
			serializers[typeof(bool)] = new BoolSerializer();
			serializers[typeof(Color)] = new ColorSerializer();
			serializers[typeof(TimeSpan)] = new TimeSpanSerializer();
		}

		// Static methods -----------------------------------------------------------

		public static bool Supports(Type type) => serializers.ContainsKey(type);

		public static TypeSerializer GetSerializerFor(Type type) => serializers[type];
	}
}

