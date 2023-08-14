using Animator.Engine.Base;
using Animator.Engine.Base.Persistence.Types;
using Animator.Engine.Elements;
using Animator.Engine.Elements.Types;
using Animator.Extensions.Nonconformist.Models.Chart;
using Animator.Extensions.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Animator.Extensions.Nonconformist
{
    public class Chart : BaseGenerator
    {
        // Private types ------------------------------------------------------
        private sealed record SeriesInfo(float MinValue, 
            float MaxValue, 
            int SeriesCount, 
            int PointCount);

        private sealed record LineDefinition(System.Drawing.PointF Start,
            System.Drawing.PointF End,
            System.Drawing.Color Color,
            float Width);

        private sealed record AnimationTimes(TimeSpan AnimationStart, 
            TimeSpan AnimationEnd, 
            TimeSpan FadeDuration, 
            List<TimeSpan> SeriesSwitchTimes);

        private sealed record SeriesMetrics(System.Drawing.RectangleF YLabelArea,
            System.Drawing.RectangleF XLabelArea,
            System.Drawing.RectangleF ChartArea,
            System.Drawing.PointF YLineStart,
            System.Drawing.PointF YLineEnd,
            float YLineWidth,
            System.Drawing.PointF XLineStart,
            System.Drawing.PointF XLineEnd,
            float XLineWidth,
            float ScaleMin,
            float ScaleMax,
            int pointCount,
            int seriesCount,
            float barWidth)
        {
            public float GetYForValue(float value) =>            
                ((ScaleMax - value) / (ScaleMax - ScaleMin)) * ChartArea.Height + ChartArea.Top;

            public float GetXForPoint(int point) =>
                ChartArea.Left + ((point + 0.5f) / pointCount) * ChartArea.Width;

            public float GetLeftForBar(int point, int series) =>
                ChartArea.Left + ((point + 0.5f) / pointCount) * ChartArea.Width - (barWidth / 2) + ((float)series / seriesCount) * (barWidth / seriesCount);
        }

        // Private constants --------------------------------------------------

        private const EasingFunction EasingFunctionIn = EasingFunction.QuadSlowDown;
        private const EasingFunction EasingFunctionOut = EasingFunction.QuadSpeedUp;

        // Private methods ----------------------------------------------------

        private Config DeserializeConfig(XmlNode node)
        {
            var configNode = node.ChildNodes[0];
            XmlSerializer serializer = new XmlSerializer(typeof(Models.Chart.Config));
            var reader = new StringReader($"{configNode.OuterXml}");
            Models.Chart.Config config = (Models.Chart.Config)serializer.Deserialize(reader);
            return config;
        }

        private SeriesInfo GetSeriesInfo(Config config)
        {
            if (config.Data.Series.Any() && config.Data.Series[0].Points.Any())
            {
                float minValue = config.Data.Series
                    .SelectMany(s => s.Points)
                    .Select(p => p.Value)
                    .Min();

                float maxValue = config.Data.Series
                    .SelectMany(s => s.Points)
                    .Select(p => p.Value)
                    .Max();

                int seriesCount = config.Data.Series.Count;

                int pointCount = config.Data.Series
                    .Select(s => s.Points.Count)
                    .Max();

                return new SeriesInfo(minValue, maxValue, seriesCount, pointCount);
            }
            else
            {
                return new SeriesInfo(0, 1, 0, 0);
            }
        }

        private SeriesMetrics BuildSeriesMetrics(Config config)
        {
            if (config.Axis.YAxis.LabelAreaWidth >= config.Width)
                throw new InvalidOperationException("Area for Y labels exceeds width of the whole chart!");
            if (config.Axis.XAxis.LabelAreaHeight >= config.Height)
                throw new InvalidOperationException("Area for X labels exceeds height of the whole chart!");
            
            SeriesInfo seriesInfo = GetSeriesInfo(config);

            // Collecting data

            float yLabelsAreaWidth = config.Axis.YAxis.LabelAreaWidth;
            float yLineWidth = config.LineWidth;
            float chartAreaWidth = config.Width - (yLabelsAreaWidth + yLineWidth);

            float xLabelsHeight = config.Axis.XAxis.LabelAreaHeight;
            float xLineWidth = config.LineWidth;
            float chartAreaHeight = config.Height - (xLabelsHeight + xLineWidth);

            float scaleMin = Math.Min(0, seriesInfo.MinValue);
            float scaleMax = Math.Max(0, seriesInfo.MaxValue);

            if (scaleMin == scaleMax)
                scaleMax = scaleMin + 1;

            if (config.Axis.YAxis.Scale > 1.0f)            
            {
                float currentDataSpan = scaleMax - scaleMin;
                float additional = ((config.Axis.YAxis.Scale - 1.0f) * currentDataSpan) / 2.0f;

                scaleMin -= additional;
                scaleMax += additional;
            }

            float zeroY = scaleMax / (scaleMax - scaleMin) * chartAreaHeight;

            float dataSpan = scaleMax - scaleMin;
            int scale10thPower = (int)Math.Ceiling(Math.Log10(dataSpan)) - 1;

            float barWidth = (chartAreaWidth / seriesInfo.PointCount) * (Math.Max(0.0f, Math.Min(1.0f, config.Axis.XAxis.BarScale)));

            return new SeriesMetrics(new System.Drawing.RectangleF(0, 0, yLabelsAreaWidth, config.Height),
                new System.Drawing.RectangleF(yLabelsAreaWidth + yLineWidth, config.Height - xLabelsHeight, chartAreaWidth, xLabelsHeight),
                new System.Drawing.RectangleF(yLabelsAreaWidth + yLineWidth, 0, chartAreaWidth, chartAreaHeight),
                new System.Drawing.PointF(yLabelsAreaWidth + yLineWidth / 2, 0),
                new System.Drawing.PointF(yLabelsAreaWidth + yLineWidth / 2, chartAreaHeight),
                yLineWidth,
                new System.Drawing.PointF(yLabelsAreaWidth + yLineWidth / 2, zeroY),
                new System.Drawing.PointF(config.Width, zeroY),
                xLineWidth,
                scaleMin,
                scaleMax,
                seriesInfo.PointCount,
                seriesInfo.SeriesCount,
                barWidth);
        }

        private Engine.Elements.Path BuildAxisLine(LineDefinition lineDefinition, AnimationTimes animationTimes)
        {
            return new Animator.Engine.Elements.Path
            {
                Pen = new Pen
                {
                    Color = lineDefinition.Color,
                    Width = lineDefinition.Width
                },
                Definition =
                {
                    new MoveSegment
                    {
                        EndPoint = lineDefinition.Start
                    },
                    new LineSegment
                    {
                        EndPoint = lineDefinition.End
                    }
                },
                Animations =
                {
                    new Storyboard
                    {
                        Keyframes =
                        {
                            new For
                            {
                                PropertyRef = nameof(Engine.Elements.Path.CutFrom),
                                Keyframes =
                                {
                                    new FloatKeyframe
                                    {
                                        Time = animationTimes.AnimationStart,
                                        Value = 0.0f
                                    },
                                    new FloatKeyframe
                                    {
                                        Time = animationTimes.AnimationEnd - animationTimes.FadeDuration,
                                        Value = 0.0f,
                                        EasingFunction = Engine.Elements.Types.EasingFunction.Linear,
                                    },
                                    new FloatKeyframe
                                    {
                                        Time = animationTimes.AnimationEnd,
                                        Value = 1.0f,
                                        EasingFunction = EasingFunctionOut
                                    }
                                }
                            },

                            new For
                            {
                                PropertyRef = nameof(Engine.Elements.Path.CutTo),
                                Keyframes =
                                {
                                    new FloatKeyframe
                                    {
                                        Time = animationTimes.AnimationStart,
                                        Value = 0.0f
                                    },
                                    new FloatKeyframe
                                    {
                                        Time = animationTimes.AnimationStart + animationTimes.FadeDuration,
                                        Value = 1.0f,
                                        EasingFunction = EasingFunctionIn
                                    },
                                    new FloatKeyframe
                                    {
                                        Time = animationTimes.AnimationEnd,
                                        Value = 1.0f,
                                        EasingFunction = Engine.Elements.Types.EasingFunction.Linear,
                                    }
                                }
                            }
                        }
                    }
                }

            };
        }

        // Public methods -----------------------------------------------------

        public override ManagedObject Generate(XmlNode node)
        {
            if (node.ChildNodes.Count != 1)
                throw new InvalidOperationException("Histogram node should have only one child: Config!");

            Config config = DeserializeConfig(node);
            SeriesMetrics metrics = BuildSeriesMetrics(config);

            // Calculate animation times
            
            TimeSpan animationStart = (TimeSpan)TypeSerialization.Deserialize(config.Animation.AnimationStart, typeof(TimeSpan));
            TimeSpan animationEnd = (TimeSpan)TypeSerialization.Deserialize(config.Animation.AnimationEnd, typeof(TimeSpan));
            TimeSpan fadeDuration = (TimeSpan)TypeSerialization.Deserialize(config.Animation.FadeDuration, typeof(TimeSpan));

            List<TimeSpan> seriesSwitchTimes = config.Animation.SeriesSwitchTimes
                .Select(s => (TimeSpan)TypeSerialization.Deserialize(s, typeof(TimeSpan)))
                .ToList();

            // Sanitize animation times

            if (animationStart + fadeDuration >= animationEnd - fadeDuration)
                throw new InvalidOperationException("Show animation exceeds animation end time");

            for (int i = 0; i < seriesSwitchTimes.Count; i++)
            {
                if (i < seriesSwitchTimes.Count - 1)
                {
                    if (seriesSwitchTimes[i] + fadeDuration >= seriesSwitchTimes[i + 1])
                        throw new InvalidOperationException($"Series switch {i} interferes with series switch {i + 1}");

                    if (seriesSwitchTimes[i] >= seriesSwitchTimes[i + 1])
                        throw new InvalidOperationException($"Series switch {i} happens after or at series switch {i + 1}");
                }

                if (seriesSwitchTimes[i] + fadeDuration >= animationEnd - fadeDuration)
                    throw new InvalidOperationException($"Series switch {i} exceeds animation end time");
            }

            int maxSwitches = config.Data.Series
                .SelectMany(s => s.Points.Select(p => p.NextValues.Count))
                .Max();

            if (maxSwitches > seriesSwitchTimes.Count)
                throw new InvalidOperationException($"There are not enough series switch times defined ({seriesSwitchTimes.Count} defined, {maxSwitches} required)");

            var animationTimes = new AnimationTimes(animationStart, animationEnd, fadeDuration, seriesSwitchTimes);

            // Render

            var result = new Layer();

            // Render series

            for (int i = 0; i < config.Data.Series.Count; i++)
            {
                var series = config.Data.Series[i];

                switch (config.Data.Series[i].Type)
                {
                    case Types.SeriesType.Bar:

                        for (int j = 0; j < series.Points.Count; j++)
                        {
                            float xFrom = metrics.GetLeftForBar(j, i) + config.LineWidth / 2.0f;
                            float width = metrics.barWidth / config.Data.Series.Count - config.LineWidth;

                            float yFrom, yTo;
                            if (series.Points[j].Value >= 0)
                            {
                                yFrom = metrics.GetYForValue(series.Points[j].Value);
                                yTo = metrics.GetYForValue(0);
                            }
                            else
                            {
                                yFrom = metrics.GetYForValue(0);
                                yTo = metrics.GetYForValue(series.Points[j].Value);
                            }

                            var borderColor = (System.Drawing.Color)TypeSerialization.Deserialize(series.Color, typeof(System.Drawing.Color));
                            var fillColor = borderColor.BrightenBy(0.2f);

                            var bar = new Rectangle
                            {
                                Position = new System.Drawing.PointF(xFrom, yFrom),
                                Width = width,
                                Height = yTo - yFrom,
                                Pen = new Pen
                                {
                                    Color = borderColor,
                                    Width = config.LineWidth
                                },
                                Brush = new LinearGradientBrush
                                {
                                    Color1 = fillColor.BrightenBy(0.1f),
                                    Color2 = fillColor.DarkenBy(0.1f),
                                    Point1 = new System.Drawing.PointF(0.0f, 0.0f),
                                    Point2 = new System.Drawing.PointF(0.0f, yTo - yFrom)
                                },
                                Name = $"Series_{i}_Bar_{j}"
                            };

                            result.Items.Add(bar);

                            var innerBar = new Rectangle
                            {
                                Position = new System.Drawing.PointF(xFrom + 2 * config.LineWidth, yFrom + 2 * config.LineWidth),
                                Width = width - 4 * config.LineWidth,
                                Height = yTo - yFrom - 4 * config.LineWidth,
                                Brush = new SolidBrush
                                {
                                    Color = System.Drawing.Color.FromArgb(128, 255, 255, 255)
                                },
                                Name = $"Series_{i}_BarInner_{j}"
                            };

                            result.Items.Add(innerBar);
                        }

                        break;
                    case Types.SeriesType.Line:
                        break;
                    default:
                        throw new InvalidOperationException("Unsupported series type!");
                }
            }

            // Render lines

            var yLineColor = (System.Drawing.Color)TypeSerialization.Deserialize(config.Axis.YAxis.Color, typeof(System.Drawing.Color));
            var yLine = BuildAxisLine(new LineDefinition(metrics.YLineStart, metrics.YLineEnd, yLineColor, config.LineWidth),
                animationTimes);
            result.Items.Add(yLine);
            
            var xLineColor = (System.Drawing.Color)TypeSerialization.Deserialize(config.Axis.XAxis.Color, typeof(System.Drawing.Color));
            var xLine = BuildAxisLine(new LineDefinition(metrics.XLineStart, metrics.XLineEnd, xLineColor, config.LineWidth),
                animationTimes);
            result.Items.Add(xLine);

            return result;
        }
    }
}
