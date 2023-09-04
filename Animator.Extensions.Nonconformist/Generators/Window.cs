using Animator.Engine.Base;
using Animator.Engine.Base.Persistence.Types;
using Animator.Engine.Elements;
using Animator.Extensions.Nonconformist.Models.Window;
using Animator.Extensions.Utils.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Animator.Extensions.Nonconformist.Generators
{
    public class Window : BaseGenerator
    {
        private class AnimationTimes
        {
            public TimeSpan ShowStart { get; init; }
            public TimeSpan ShowEnd { get; init; }
            public TimeSpan CornerShowStart { get; init; }
            public TimeSpan CornerShowEnd { get; init; }
            public TimeSpan CornerMoveStart { get; init; }
            public TimeSpan CornerMoveEnd { get; init; }
            public TimeSpan EdgeAnimStart { get; init; }
            public TimeSpan EdgeAnimEnd { get; init; }
            public TimeSpan ContentStart { get; init; }
            public TimeSpan ContentEnd { get; init; }
            public TimeSpan HeaderShowStart { get; internal set; }
            public TimeSpan HeaderShowEnd { get; internal set; }
        }

        private AnimationTimes CalculateAnimationTimes(Config config)
        {
            var showStart = (TimeSpan)TypeSerialization.Deserialize(config.Animation.ShowStart, typeof(TimeSpan));
            var showEnd = (TimeSpan)TypeSerialization.Deserialize(config.Animation.ShowEnd, typeof(TimeSpan));
            var length = showEnd - showStart;
            var cornerMoveStart = showStart;
            var cornerMoveEnd = showStart + (length / 2.0f);
            var cornerShowStart = showStart + (length / 5.0f);
            var cornerShowEnd = showStart + (length / 2.0f);
            var edgeAnimStart = showStart + (length / 10.0f);
            var edgeAnimEnd = showStart + (length / 2.0f);
            var contentStart = showStart + (length / 5.0f);
            var contentEnd = showStart + (3 * length / 5.0f);
            var headerShowStart = showStart + (length / 2.0f);
            var headerShowEnd = showEnd;

            return new AnimationTimes
            {
                ShowStart = showStart,
                ShowEnd = showEnd,
                CornerMoveStart = cornerMoveStart,
                CornerMoveEnd = cornerMoveEnd,
                CornerShowStart = cornerShowStart,
                CornerShowEnd = cornerShowEnd,
                EdgeAnimStart = edgeAnimStart,
                EdgeAnimEnd = edgeAnimEnd,
                ContentStart = contentStart,
                ContentEnd = contentEnd,
                HeaderShowStart = headerShowStart,
                HeaderShowEnd = headerShowEnd,
            };
        }

        private Visual BuildCorner(Config config, AnimationTimes animationTimes, float flipX, float flipY)
        {
            float halfWidth = config.Width / 2.0f;
            float halfHeight = config.Height / 2.0f;

            var corner = new Animator.Engine.Elements.Path
            {
                Definition =
                {
                    new MoveSegment
                    {
                        EndPoint = new System.Drawing.PointF(-config.DecorSize, 0.0f)
                    },
                    new RelativeVerticalLineSegment
                    {
                        DY = -config.DecorSize
                    },
                    new RelativeHorizontalLineSegment
                    {
                        DX = config.DecorSize
                    },
                    new RelativeLineSegment
                    {
                        DeltaEndPoint = new System.Drawing.PointF(-config.CornerThickness, config.CornerThickness)
                    },
                    new RelativeHorizontalLineSegment
                    {
                        DX = -(config.DecorSize - 2 * config.CornerThickness)
                    },
                    new RelativeVerticalLineSegment
                    {
                        DY = (config.DecorSize - 2 * config.CornerThickness)  
                    },
                    new CloseShapeSegment()
                },
                Brush = new Animator.Engine.Elements.SolidBrush
                {
                    Color = System.Drawing.Color.FromArgb(0xe0, 0xff, 0x8e, 0x32)
                },
                Position = new System.Drawing.PointF(config.Width / 2.0f, config.Height / 2.0f),
                Alpha = 0.0f,
                Scale = new System.Drawing.PointF(flipX, flipY),
                Animations =
                {
                    new AnimatePoint
                    {
                        PropertyRef = nameof(Visual.Position),
                        From = new System.Drawing.PointF(config.Width / 2.0f, config.Height / 2.0f),
                        To = new System.Drawing.PointF((1.0f - flipX) * halfWidth + flipX * config.DecorSize, 
                            (1.0f - flipY) * halfHeight + flipY * config.DecorSize),
                        StartTime = animationTimes.CornerMoveStart,
                        EndTime = animationTimes.CornerMoveEnd,
                        EasingFunction = Engine.Elements.Types.EasingFunction.QuadBoth
                    },
                    new AnimateFloat
                    {
                        PropertyRef = nameof(Visual.Alpha),
                        From = 0.0f,
                        To = 1.0f,
                        StartTime = animationTimes.CornerShowStart,
                        EndTime = animationTimes.CornerShowEnd,
                        EasingFunction = Engine.Elements.Types.EasingFunction.Linear
                    }
                }
            };

            return corner;
        }

        private Visual BuildHEdge(
            Config config,
            AnimationTimes animationTimes,
            float yLine)
        {
            float halfWidth = config.Width / 2.0f;
            float halfHeight = config.Height / 2.0f;
            float halfLineWidth = halfWidth - config.DecorSize * 1.2f;
            float halfLineHeight = halfHeight - config.DecorSize * 1.2f;

            float y = (1.0f + yLine) * halfHeight - yLine * (config.CornerThickness / 2.0f);

            var result = new Animator.Engine.Elements.Line
            {
                Start = new System.Drawing.PointF(-halfLineWidth, 0.0f),
                End = new System.Drawing.PointF(halfLineWidth, 0.0f),
                Position = new System.Drawing.PointF(halfWidth, y),
                Pen = new Pen
                {
                    Color = System.Drawing.Color.FromArgb(0xe0, 0x60, 0xeb, 0xf2),
                    Width = config.CornerThickness
                },
                Animations =
                {
                    new AnimatePoint
                    {
                        PropertyRef = nameof(Visual.Position),
                        From = new System.Drawing.PointF(halfWidth, y + yLine * config.DecorSize * 0.5f),
                        To = new System.Drawing.PointF(halfWidth, y),
                        StartTime = animationTimes.EdgeAnimStart,
                        EndTime = animationTimes.EdgeAnimEnd
                    },
                    new AnimateFloat
                    {
                        PropertyRef = nameof(Visual.Alpha),
                        From = 0.0f,
                        To = 1.0f,
                        StartTime = animationTimes.EdgeAnimStart,
                        EndTime = animationTimes.EdgeAnimEnd,
                        EasingFunction = Engine.Elements.Types.EasingFunction.Linear
                    }
                }
            };

            return result;
        }

        private Visual BuildVEdge(
            Config config,
            AnimationTimes animationTimes,
            float xLine)
        {
            float halfWidth = config.Width / 2.0f;
            float halfHeight = config.Height / 2.0f;
            float halfLineWidth = halfWidth - config.DecorSize * 1.2f;
            float halfLineHeight = halfHeight - config.DecorSize * 1.2f;
            float x = (1.0f + xLine) * halfWidth - xLine * (config.CornerThickness / 2.0f);

            var result = new Animator.Engine.Elements.Line
            {
                Start = new System.Drawing.PointF(0.0f, -halfLineHeight),
                End = new System.Drawing.PointF(0.0f, halfLineHeight),
                Position = new System.Drawing.PointF(x, halfHeight),
                Pen = new Pen
                {
                    Color = System.Drawing.Color.FromArgb(0xe0, 0x60, 0xeb, 0xf2),
                    Width = config.CornerThickness
                },
                Animations =
                {
                    new AnimatePoint
                    {
                        PropertyRef = nameof(Visual.Position),
                        From = new System.Drawing.PointF(x + xLine * config.DecorSize * 0.5f, halfHeight),
                        To = new System.Drawing.PointF(x, halfHeight),
                        StartTime = animationTimes.EdgeAnimStart,
                        EndTime = animationTimes.EdgeAnimEnd
                    },
                    new AnimateFloat
                    {
                        PropertyRef = nameof(Visual.Alpha),
                        From = 0.0f,
                        To = 1.0f,
                        StartTime = animationTimes.EdgeAnimStart,
                        EndTime = animationTimes.EdgeAnimEnd,
                        EasingFunction = Engine.Elements.Types.EasingFunction.Linear
                    }
                }
            };

            return result;
        }

        private Visual BuildContent(Config config, AnimationTimes animationTimes)
        {
            var halfWidth = config.Width / 2.0f;
            var halfHeight = config.Height / 2.0f;

            var contentWidth = config.Width - 2 * config.CornerThickness - 2 * config.InternalMargin - 2 * (config.ContentBorderThickness / 2.0f);
            var contentHeight = config.Height - 2 * config.CornerThickness - 2 * config.InternalMargin - 2 * (config.ContentBorderThickness / 2.0f);

            var result = new Rectangle
            {
                Position = new System.Drawing.PointF(halfWidth, halfHeight),
                Origin = new System.Drawing.PointF(contentWidth / 2.0f, contentHeight / 2.0f),
                Width = contentWidth,
                Height = contentHeight,
                Pen = new Pen
                {
                    Color = System.Drawing.Color.FromArgb(0xe0, 0x60, 0xeb, 0xf2),
                    Width = config.ContentBorderThickness
                },
                Brush = new SolidBrush
                {
                    Color = System.Drawing.Color.FromArgb(0x80, 0xba, 0xec, 0xef)
                },
                Animations =
                {
                    new AnimatePoint
                    {
                        PropertyRef = nameof(Rectangle.Origin),
                        From = new System.Drawing.PointF(0.0f, 0.0f),
                        StartTime = animationTimes.ContentStart,
                        EndTime = animationTimes.ContentEnd,
                        EasingFunction = Engine.Elements.Types.EasingFunction.QuadSlowDown
                    },
                    new AnimateFloat
                    {
                        PropertyRef = nameof(Rectangle.Width),
                        From = 0.0f,
                        StartTime = animationTimes.ContentStart,
                        EndTime = animationTimes.ContentEnd,
                        EasingFunction = Engine.Elements.Types.EasingFunction.QuadSlowDown
                    },
                    new AnimateFloat
                    {
                        PropertyRef = nameof(Rectangle.Height),
                        From = 0.0f,
                        StartTime = animationTimes.ContentStart,
                        EndTime = animationTimes.ContentEnd,
                        EasingFunction = Engine.Elements.Types.EasingFunction.QuadSlowDown
                    },
                    new AnimateFloat
                    {
                        PropertyRef = nameof(Rectangle.Alpha),
                        From = 0.0f,
                        To = 1.0f,
                        StartTime = animationTimes.ContentStart,
                        EndTime = animationTimes.ContentEnd,
                        EasingFunction = Engine.Elements.Types.EasingFunction.QuadSlowDown
                    }
                }
            };

            return result;
        }

        private Visual BuildHeaderBackground(Config config, AnimationTimes animationTimes)
        {
            float marginPlusHalfLine = config.InternalMargin + config.CornerThickness / 2.0f;

            var path = new Animator.Engine.Elements.Path
            {
                Definition =
                {
                    new MoveSegment
                    {
                        EndPoint = new System.Drawing.PointF(-config.InternalMargin, 
                            config.HeaderHeight + config.DecorSize - marginPlusHalfLine)
                    },
                    new VerticalLineSegment
                    {
                        Y = -config.InternalMargin
                    },
                    new HorizontalLineSegment
                    {
                        X = config.HeaderWidth + config.DecorSize - marginPlusHalfLine
                    },
                    new RelativeLineSegment
                    {
                        DeltaEndPoint = new System.Drawing.PointF(-(config.DecorSize - config.InternalMargin), -(config.DecorSize - config.InternalMargin))
                    },
                    new LineSegment
                    {
                        EndPoint = new System.Drawing.PointF(-config.DecorSize, -config.DecorSize)
                    },
                    new VerticalLineSegment
                    {
                        Y = config.HeaderHeight - config.CornerThickness / 2.0f
                    },
                    new CloseShapeSegment()
                },
                Brush = new Animator.Engine.Elements.SolidBrush
                {
                    Color = System.Drawing.Color.FromArgb(0x80, 0xba, 0xec, 0xef)
                },
                Animations =
                {
                    new AnimateFloat
                    {
                        PropertyRef = nameof(Visual.Alpha),
                        From = 0.0f,
                        To = 1.0f,
                        StartTime = animationTimes.HeaderShowStart,
                        EndTime = animationTimes.HeaderShowEnd,
                        EasingFunction = Engine.Elements.Types.EasingFunction.QuadSlowDown
                    }
                }
            };

            return path;
        }

        private Visual BuildHeaderLine(Config config, AnimationTimes animationTimes)
        {
            float marginPlusHalfLine = config.InternalMargin + config.CornerThickness / 2.0f;

            var path = new Animator.Engine.Elements.Path
            {
                Definition =
                {
                    new MoveSegment
                    {
                        EndPoint = new System.Drawing.PointF(-config.InternalMargin - config.CornerThickness / 2.0f,
                            config.HeaderHeight + config.DecorSize + config.HeaderLineHeight)
                    },
                    new RelativeVerticalLineSegment
                    {
                        DY = -(config.HeaderLineHeight)
                    },
                    new RelativeLineSegment
                    {
                        DeltaEndPoint = new System.Drawing.PointF(-config.DecorSize, -config.DecorSize)
                    },
                    new RelativeVerticalLineSegment
                    {
                        DY = - config.HeaderHeight - marginPlusHalfLine - config.DecorSize
                    },
                    new RelativeHorizontalLineSegment
                    {
                        DX = config.DecorSize + marginPlusHalfLine + config.HeaderWidth
                    },
                    new RelativeLineSegment
                    {
                        DeltaEndPoint = new System.Drawing.PointF(config.DecorSize, config.DecorSize)
                    },
                    new RelativeHorizontalLineSegment
                    {
                        DX = config.HeaderLineWidth
                    }
                },
                Pen = new Pen
                {
                    Color = System.Drawing.Color.FromArgb(0xe0, 0x60, 0xeb, 0xf2),
                    Width = config.CornerThickness
                },
                Animations =
                {
                    new AnimateFloat
                    {
                        PropertyRef = nameof(Animator.Engine.Elements.Path.CutTo),
                        From = 0.0f,
                        To = 1.0f,
                        StartTime = animationTimes.HeaderShowStart,
                        EndTime = animationTimes.HeaderShowEnd,
                        EasingFunction = Engine.Elements.Types.EasingFunction.QuadSlowDown
                    }
                }
            };

            return path;
        }

        private Visual BuildFooterBackground(Config config, AnimationTimes animationTimes)
        {
            float marginPlusHalfLine = config.InternalMargin + config.CornerThickness / 2.0f;

            var path = new Animator.Engine.Elements.Path
            {
                Definition =
                {
                    new MoveSegment
                    {
                        EndPoint = new System.Drawing.PointF(config.Width / 2.0f - config.FooterWidth / 2.0f - config.DecorSize + marginPlusHalfLine,
                            config.Height + config.InternalMargin)
                    },
                    new HorizontalLineSegment
                    {
                        X = config.Width / 2.0f + config.FooterWidth / 2.0f + config.DecorSize - marginPlusHalfLine
                    },
                    new RelativeLineSegment
                    {
                        DeltaEndPoint = new System.Drawing.PointF(-(config.DecorSize - config.InternalMargin), config.DecorSize - config.InternalMargin)
                    },
                    new RelativeHorizontalLineSegment
                    {
                        DX = -(config.FooterWidth - config.InternalMargin)
                    },                    
                    new CloseShapeSegment()
                },
                Brush = new Animator.Engine.Elements.SolidBrush
                {
                    Color = System.Drawing.Color.FromArgb(0x80, 0xba, 0xec, 0xef)
                },
                Animations =
                {
                    new AnimateFloat
                    {
                        PropertyRef = nameof(Visual.Alpha),
                        From = 0.0f,
                        To = 1.0f,
                        StartTime = animationTimes.HeaderShowStart,
                        EndTime = animationTimes.HeaderShowEnd,
                        EasingFunction = Engine.Elements.Types.EasingFunction.QuadSlowDown
                    }
                }
            };

            return path;
        }

        private Visual BuildFooterLine(Config config, AnimationTimes animationTimes)
        {
            float marginPlusHalfLine = config.InternalMargin + config.CornerThickness / 2.0f;

            var path = new Animator.Engine.Elements.Path
            {
                Definition =
                {
                    new MoveSegment
                    {
                        EndPoint = new System.Drawing.PointF(0.0f,
                            config.Height + config.InternalMargin + config.CornerThickness / 2.0f)
                    },
                    new HorizontalLineSegment
                    {
                        X = config.Width / 2 - config.FooterWidth / 2 - config.DecorSize
                    },
                    new RelativeLineSegment
                    {
                        DeltaEndPoint = new System.Drawing.PointF(config.DecorSize, config.DecorSize)
                    },
                    new RelativeHorizontalLineSegment
                    {
                        DX = config.FooterWidth
                    },
                    new RelativeLineSegment
                    {
                        DeltaEndPoint = new System.Drawing.PointF(config.DecorSize, -config.DecorSize)
                    },
                    new HorizontalLineSegment
                    {
                        X = config.Width
                    }
                },
                Pen = new Pen
                {
                    Color = System.Drawing.Color.FromArgb(0xe0, 0x60, 0xeb, 0xf2),
                    Width = config.CornerThickness
                },
                Animations =
                {
                    new AnimateFloat
                    {
                        PropertyRef = nameof(Animator.Engine.Elements.Path.CutTo),
                        From = 0.0f,
                        To = 1.0f,
                        StartTime = animationTimes.HeaderShowStart,
                        EndTime = animationTimes.HeaderShowEnd,
                        EasingFunction = Engine.Elements.Types.EasingFunction.QuadSlowDown
                    }
                }
            };

            return path;
        }

        public override ManagedObject Generate(XmlElement node)
        {
            Config config = ((XmlElement)node.ChildNodes[0]).Deserialize<Config>();
            AnimationTimes animationTimes = CalculateAnimationTimes(config);

            var result = new Layer();

            // Corners

            result.Items.Add(BuildCorner(config, animationTimes, 1.0f, 1.0f));
            result.Items.Add(BuildCorner(config, animationTimes, -1.0f, 1.0f));
            result.Items.Add(BuildCorner(config, animationTimes, 1.0f, -1.0f));
            result.Items.Add(BuildCorner(config, animationTimes, -1.0f, -1.0f));

            // Edges

            result.Items.Add(BuildHEdge(config, animationTimes, -1.0f));
            result.Items.Add(BuildHEdge(config, animationTimes, 1.0f));
            result.Items.Add(BuildVEdge(config, animationTimes, -1.0f));
            result.Items.Add(BuildVEdge(config, animationTimes, 1.0f));

            // Content

            result.Items.Add(BuildContent(config, animationTimes));

            // Header

            if (config.ShowHeader)
            {
                result.Items.Add(BuildHeaderLine(config, animationTimes));
                result.Items.Add(BuildHeaderBackground(config, animationTimes));
            }

            // Footer

            if (config.ShowFooter)
            {
                result.Items.Add(BuildFooterLine(config, animationTimes));
                result.Items.Add(BuildFooterBackground(config, animationTimes));
            }

            return result;
        }
    }
}
