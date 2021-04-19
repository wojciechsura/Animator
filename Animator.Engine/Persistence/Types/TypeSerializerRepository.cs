
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

namespace Animator.Engine.Persistence.Types
{
    public class TypeSerializerRepository
	{
		// Private types ------------------------------------------------------------

        private class ByteSerializer : TypeSerializer
		{
			public override object Deserialize(string str) => byte.Parse(str);
			public override string Serialize(object obj) => ((byte)obj).ToString(CultureInfo.InvariantCulture);
		}

        private class SbyteSerializer : TypeSerializer
		{
			public override object Deserialize(string str) => sbyte.Parse(str);
			public override string Serialize(object obj) => ((sbyte)obj).ToString(CultureInfo.InvariantCulture);
		}

        private class ShortSerializer : TypeSerializer
		{
			public override object Deserialize(string str) => short.Parse(str);
			public override string Serialize(object obj) => ((short)obj).ToString(CultureInfo.InvariantCulture);
		}

        private class UshortSerializer : TypeSerializer
		{
			public override object Deserialize(string str) => ushort.Parse(str);
			public override string Serialize(object obj) => ((ushort)obj).ToString(CultureInfo.InvariantCulture);
		}

        private class IntSerializer : TypeSerializer
		{
			public override object Deserialize(string str) => int.Parse(str);
			public override string Serialize(object obj) => ((int)obj).ToString(CultureInfo.InvariantCulture);
		}

        private class UintSerializer : TypeSerializer
		{
			public override object Deserialize(string str) => uint.Parse(str);
			public override string Serialize(object obj) => ((uint)obj).ToString(CultureInfo.InvariantCulture);
		}

        private class LongSerializer : TypeSerializer
		{
			public override object Deserialize(string str) => long.Parse(str);
			public override string Serialize(object obj) => ((long)obj).ToString(CultureInfo.InvariantCulture);
		}

        private class UlongSerializer : TypeSerializer
		{
			public override object Deserialize(string str) => ulong.Parse(str);
			public override string Serialize(object obj) => ((ulong)obj).ToString(CultureInfo.InvariantCulture);
		}

        private class FloatSerializer : TypeSerializer
		{
			public override object Deserialize(string str) => float.Parse(str);
			public override string Serialize(object obj) => ((float)obj).ToString(CultureInfo.InvariantCulture);
		}

        private class DoubleSerializer : TypeSerializer
		{
			public override object Deserialize(string str) => double.Parse(str);
			public override string Serialize(object obj) => ((double)obj).ToString(CultureInfo.InvariantCulture);
		}

		private class StringSerializer : TypeSerializer
		{
			public override object Deserialize(string str) => str;
			public override string Serialize(object obj) => (string)obj;
		}

		private class ColorSerializer : TypeSerializer
		{
			private readonly Regex rgbColorFormat = new Regex("^#[a-fA-F0-9]{6}$");
			private readonly Regex rgbaColorFormat = new Regex("^#[a-fA-F0-9]{8}$");
			private readonly Regex numRgbColorFormat = new Regex("^[0-9]{,3}(,[0-9]{,3}){2}$");
			private readonly Regex numRgbaColorFormat = new Regex("^[0-9]{,3}(,[0-9]{,3}){3}$");

			public override object Deserialize(string str)
            {
				if (String.IsNullOrEmpty(str))
					throw new InvalidCastException($"Cannot convert empty string to color!");

				if (Enum.TryParse(typeof(KnownColor), str, out object knownColor))
					return Color.FromKnownColor((KnownColor)knownColor);

				// Format #aabbcc
				if (rgbColorFormat.IsMatch(str))
                {
					int r = Convert.ToInt32(str.Substring(1, 2), 16);
					int g = Convert.ToInt32(str.Substring(3, 2), 16);
					int b = Convert.ToInt32(str.Substring(5, 2), 16);

					return Color.FromArgb(255, r, g, b);
                }
				
				if (rgbaColorFormat.IsMatch(str))
                {
					int r = Convert.ToInt32(str.Substring(1, 2), 16);
					int g = Convert.ToInt32(str.Substring(3, 2), 16);
					int b = Convert.ToInt32(str.Substring(5, 2), 16);
					int a = Convert.ToInt32(str.Substring(5, 2), 16);

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

				if (numRgbaColorFormat.IsMatch(str))
                {
					string[] values = str.Split(',');

					int r = Convert.ToInt32(values[0]);
					int g = Convert.ToInt32(values[1]);
					int b = Convert.ToInt32(values[2]);
					int a = Convert.ToInt32(values[3]);

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
			private readonly Regex pointRegex = new Regex("^[0-9]+;[0-9]+$");

			public override object Deserialize(string str)
			{
				if (pointRegex.IsMatch(str))
				{
					string[] values = str.Split(';');

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
			private readonly Regex pointRegex = new Regex("^[0-9]+(\\.[0-9]+)?;[0-9]+(\\.[0-9]+)?$");

			public override object Deserialize(string str)
			{
				if (pointRegex.IsMatch(str))
				{
					string[] values = str.Split(';');

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
			serializers[typeof(Color)] = new ColorSerializer();
		}

		// Static methods -----------------------------------------------------------

		public static bool Supports(Type type) => serializers.ContainsKey(type);

		public static TypeSerializer GetSerializerFor(Type type) => serializers[type];
	}
}

