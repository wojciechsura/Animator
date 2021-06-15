using Animator.Engine.Base;
using Animator.Engine.Base.Persistence.Types;
using Animator.Engine.Base.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements.Persistence
{
    internal class PathElementsSerializer : TypeSerializer
    {
        // Private constants --------------------------------------------------

        private readonly char[] whitespace = new char[] { ' ', '\t', '\r', '\n' };        

        // Private methods ----------------------------------------------------

        private void OmitWhitespace(string str, ref int index)
        {
            while (index < str.Length && whitespace.Contains(str[index]))
                index++;
        }

        private float ExpectFloat(string str, ref int index)
        {
            OmitWhitespace(str, ref index);

            int start = index;

            if (index < str.Length && str[index] == '-')
                index++;

            while (index < str.Length && str[index] >= '0' && str[index] <= '9')
                index++;

            if (index < str.Length && str[index] == CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator[0])
                index++;

            while (index < str.Length && str[index] >= '0' && str[index] <= '9')
                index++;

            if (index == start)
                throw new ParseException($"Parse error: float expected on position {index}");

            return float.Parse(str.Substring(start, index - start), CultureInfo.InvariantCulture);
        }

        private int ExpectInt(string str, ref int index)
        {
            OmitWhitespace(str, ref index);

            int start = index;

            while (index < str.Length && str[index] >= '0' && str[index] <= '9')
                index++;

            if (index == start)
                throw new ParseException($"Parse error: integer expected on position {index}");

            return int.Parse(str.Substring(start, index - start), CultureInfo.InvariantCulture);
        }

        private char ExpectLetter(string str, ref int index)
        {
            OmitWhitespace(str, ref index);

            if (index >= str.Length)
                throw new ParseException($"Parse error: letter expected on position {index}");

            if (!((str[index] >= 'a' && str[index] <= 'z') || (str[index] >= 'A' && str[index] <= 'Z')))
                throw new ParseException($"Parse error: letter expected on position {index}");

            return str[index++];
        }

        private PathElement ParseRelativeArc(string data, ref int index)
        {
            return new RelativeArcPathElement
            {
                RX = ExpectFloat(data, ref index),
                RY = ExpectFloat(data, ref index),
                Angle = ExpectFloat(data, ref index),
                LargeArcFlag = ExpectInt(data, ref index) == 1,
                SweepFlag = ExpectInt(data, ref index) == 1,
                DeltaEndPoint = new PointF(ExpectFloat(data, ref index), ExpectFloat(data, ref index))
            };
        }

        private PathElement ParseAbsoluteArc(string data, ref int index)
        {
            return new AbsoluteArcPathElement
            {
                RX = ExpectFloat(data, ref index),
                RY = ExpectFloat(data, ref index),
                Angle = ExpectFloat(data, ref index),
                LargeArcFlag = ExpectInt(data, ref index) == 1,
                SweepFlag = ExpectInt(data, ref index) == 1,
                EndPoint = new PointF(ExpectFloat(data, ref index), ExpectFloat(data, ref index))
            };
        }

        private PathElement ParseRelativeShorthandQuadraticBezier(string data, ref int index)
        {
            return new RelativeQuadraticShorthandBezierPathElement()
            {
                DeltaEndPoint = new PointF(ExpectFloat(data, ref index), ExpectFloat(data, ref index))
            };
        }

        private PathElement ParseAbsoluteShorthandQuadraticBezier(string data, ref int index)
        {
            return new AbsoluteQuadraticShorthandBezierPathElement()
            {
                EndPoint = new PointF(ExpectFloat(data, ref index), ExpectFloat(data, ref index))
            };
        }

        private PathElement ParseRelativeQuadraticBezier(string data, ref int index)
        {
            return new RelativeQuadraticBezierPathElement
            {
                DeltaControlPoint = new PointF(ExpectFloat(data, ref index), ExpectFloat(data, ref index)),
                DeltaEndPoint = new PointF(ExpectFloat(data, ref index), ExpectFloat(data, ref index))
            };
        }

        private PathElement ParseAbsoluteQuadraticBezier(string data, ref int index)
        {
            return new AbsoluteQuadraticBezierPathElement
            {
                ControlPoint = new PointF(ExpectFloat(data, ref index), ExpectFloat(data, ref index)),
                EndPoint = new PointF(ExpectFloat(data, ref index), ExpectFloat(data, ref index))
            };
        }

        private PathElement ParseRelativeShorthandBezier(string data, ref int index)
        {
            return new RelativeCubicShorthandBezierPathElement
            {
                DeltaControlPoint2 = new PointF(ExpectFloat(data, ref index), ExpectFloat(data, ref index)),
                DeltaEndPoint = new PointF(ExpectFloat(data, ref index), ExpectFloat(data, ref index))
            };
        }

        private PathElement ParseAbsoluteShorthandBezier(string data, ref int index)
        {
            return new AbsoluteCubicShorthandBezierPathElement
            {
                ControlPoint2 = new PointF(ExpectFloat(data, ref index), ExpectFloat(data, ref index)),
                EndPoint = new PointF(ExpectFloat(data, ref index), ExpectFloat(data, ref index))
            };
        }

        private PathElement ParseRelativeBezier(string data, ref int index)
        {
            return new RelativeCubicBezierPathElement
            {
                DeltaControlPoint1 = new PointF(ExpectFloat(data, ref index), ExpectFloat(data, ref index)),
                DeltaControlPoint2 = new PointF(ExpectFloat(data, ref index), ExpectFloat(data, ref index)),
                DeltaEndPoint = new PointF(ExpectFloat(data, ref index), ExpectFloat(data, ref index))
            };
        }

        private PathElement ParseAbsoluteBezier(string data, ref int index)
        {
            return new AbsoluteCubicBezierPathElement
            {
                ControlPoint1 = new PointF(ExpectFloat(data, ref index), ExpectFloat(data, ref index)),
                ControlPoint2 = new PointF(ExpectFloat(data, ref index), ExpectFloat(data, ref index)),
                EndPoint = new PointF(ExpectFloat(data, ref index), ExpectFloat(data, ref index))
            };
        }

        private PathElement ParseCloseShape(string data, ref int index)
        {
            return new CloseShapePathElement();
        }

        private PathElement ParseRelativeVerticalLine(string data, ref int index)
        {
            return new RelativeVerticalLinePathElement
            {
                DY = ExpectFloat(data, ref index)
            };
        }

        private PathElement ParseAbsoluteVerticalLine(string data, ref int index)
        {
            return new AbsoluteVerticalLinePathElement
            {
                Y = ExpectFloat(data, ref index)
            };
        }

        private PathElement ParseRelativeHorizontalLine(string data, ref int index)
        {
            return new RelativeHorizontalLinePathElement
            {
                DX = ExpectFloat(data, ref index)
            };
        }

        private PathElement ParseAbsoluteHorizontalLine(string data, ref int index)
        {
            return new AbsoluteHorizontalLinePathElement
            {
                X = ExpectFloat(data, ref index)
            };
        }

        private PathElement ParseRelativeLine(string data, ref int index)
        {
            return new RelativeLinePathElement
            {
                DeltaEndPoint = new PointF(ExpectFloat(data, ref index), ExpectFloat(data, ref index))
            };
        }

        private PathElement ParseAbsoluteLine(string data, ref int index)
        {
            return new AbsoluteLinePathElement
            {
                EndPoint = new PointF(ExpectFloat(data, ref index), ExpectFloat(data, ref index))
            };
        }

        private PathElement ParseRelativeMove(string data, ref int index)
        {
            return new RelativeMovePathElement
            {
                DeltaEndPoint = new PointF(ExpectFloat(data, ref index), ExpectFloat(data, ref index))
            };
        }

        private PathElement ParseAbsoluteMove(string data, ref int index)
        {
            return new AbsoluteMovePathElement
            {
                EndPoint = new PointF(ExpectFloat(data, ref index), ExpectFloat(data, ref index))
            };
        }

        // Public methods -----------------------------------------------------

        public override bool CanDeserialize(string data) => true;

        public override IList Deserialize(string data)
        {
            List<PathElement> result = new List<PathElement>();

            int index = 0;
            while (index < data.Length)
            {
                OmitWhitespace(data, ref index);
                char c = ExpectLetter(data, ref index);

                switch (c)
                {
                    case 'M':
                        result.Add(ParseAbsoluteMove(data, ref index));
                        break;
                    case 'm':
                        result.Add(ParseRelativeMove(data, ref index));
                        break;
                    case 'L':
                        result.Add(ParseAbsoluteLine(data, ref index));
                        break;
                    case 'l':
                        result.Add(ParseRelativeLine(data, ref index));
                        break;
                    case 'H':
                        result.Add(ParseAbsoluteHorizontalLine(data, ref index));
                        break;
                    case 'h':
                        result.Add(ParseRelativeHorizontalLine(data, ref index));
                        break;
                    case 'V':
                        result.Add(ParseAbsoluteVerticalLine(data, ref index));
                        break;
                    case 'v':
                        result.Add(ParseRelativeVerticalLine(data, ref index));
                        break;
                    case 'Z':
                    case 'z':
                        result.Add(ParseCloseShape(data, ref index));
                        break;
                    case 'C':
                        result.Add(ParseAbsoluteBezier(data, ref index));
                        break;
                    case 'c':
                        result.Add(ParseRelativeBezier(data, ref index));
                        break;
                    case 'S':
                        result.Add(ParseAbsoluteShorthandBezier(data, ref index));
                        break;
                    case 's':
                        result.Add(ParseRelativeShorthandBezier(data, ref index));
                        break;
                    case 'Q':
                        result.Add(ParseAbsoluteQuadraticBezier(data, ref index));
                        break;
                    case 'q':
                        result.Add(ParseRelativeQuadraticBezier(data, ref index));
                        break;
                    case 'T':
                        result.Add(ParseAbsoluteShorthandQuadraticBezier(data, ref index));
                        break;
                    case 't':
                        result.Add(ParseRelativeShorthandQuadraticBezier(data, ref index));
                        break;
                    case 'A':
                        result.Add(ParseAbsoluteArc(data, ref index));
                        break;
                    case 'a':
                        result.Add(ParseRelativeArc(data, ref index));
                        break;
                    default:
                        throw new ParseException($"Not recognized path command: {c}");
                }
            }

            return result;
        }

        public override bool CanSerialize(object obj)
        {
            return true;
        }

        public override string Serialize(object obj)
        {
            var elementList = obj as List<PathElement>;

            if (elementList != null)
                return string.Join(" ", elementList.Select(e => e.ToPathString()));
            else
                return string.Empty;
        }

    }
}
