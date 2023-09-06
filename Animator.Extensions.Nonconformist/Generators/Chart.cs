using Animator.Engine.Base;
using Animator.Engine.Base.Persistence.Types;
using Animator.Engine.Elements;
using Animator.Engine.Elements.Types;
using Animator.Engine.Utils;
using Animator.Extensions.Nonconformist.Models.Chart;
using Animator.Extensions.Nonconformist.Types.Chart;
using Animator.Extensions.Utils.Extensions;
using Animator.Extensions.Utils.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Animator.Extensions.Nonconformist.Generators
{
    public class Chart : BaseGenerator
    {
        // Private types ------------------------------------------------------
        private sealed record SeriesInfo(float MinValue,
            float MaxValue,
            int BarSeriesCount,
            int SeriesCount,
            int PointCount);

        private sealed record LineDefinition(System.Drawing.PointF Start,
            System.Drawing.PointF End,
            System.Drawing.Color Color,
            float Width);

        private sealed record AnimationTimes(TimeSpan Start,
            TimeSpan End,
            TimeSpan FadeDuration,
            List<TimeSpan> SeriesSwitchTimes);

        private sealed record SeriesMetrics(System.Drawing.RectangleF HeaderArea,
            System.Drawing.RectangleF YLabelArea,
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
            int PointCount,
            int SeriesCount,
            int BarSeriesCount,
            float BarWidth,
            float ScaleStep)
        {
            public float GetYForValue(float value) =>
                (ScaleMax - value) / (ScaleMax - ScaleMin) * ChartArea.Height + ChartArea.Top;

            public float GetXForPoint(int point) =>
                ChartArea.Left + (point + 0.5f) / PointCount * ChartArea.Width;

            public float GetLeftForBar(int point, int barSeries) =>
                ChartArea.Left + (point + 0.5f) / PointCount * ChartArea.Width - BarWidth / 2 + (float)barSeries / BarSeriesCount * (BarWidth / BarSeriesCount);
        }

        // Private constants --------------------------------------------------

        private const EasingFunction EasingFunctionIn = EasingFunction.CubicSlowDown;
        private const EasingFunction EasingFunctionInOut = EasingFunction.CubicBoth;
        private const EasingFunction EasingFunctionOut = EasingFunction.CubicSpeedUp;

        // Private methods ----------------------------------------------------

        private static AnimationTimes CalculateAnimationTimes(Config config)
        {
            // Calculate animation times

            TimeSpan animationStart = (TimeSpan)TypeSerialization.Deserialize(config.Animation.Start, typeof(TimeSpan));
            TimeSpan animationEnd = (TimeSpan)TypeSerialization.Deserialize(config.Animation.End, typeof(TimeSpan));
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

            int maxSwitches = new[] { 0 }
                .Concat(config.Data.Series
                    .SelectMany(s => s.Points
                        .Select(p => p.NextValues.Count)))
                .Max();

            if (maxSwitches > seriesSwitchTimes.Count)
                throw new InvalidOperationException($"There are not enough series switch times defined ({seriesSwitchTimes.Count} defined, {maxSwitches} required)");

            var animationTimes = new AnimationTimes(animationStart, animationEnd, fadeDuration, seriesSwitchTimes);
            return animationTimes;
        }

        private Visual BuildAxisLine(LineDefinition lineDefinition, AnimationTimes animationTimes)
        {
            return new Engine.Elements.Path
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
                                        Time = animationTimes.Start,
                                        Value = 0.0f
                                    },
                                    new FloatKeyframe
                                    {
                                        Time = animationTimes.End - animationTimes.FadeDuration,
                                        Value = 0.0f,
                                        EasingFunction = EasingFunction.Linear,
                                    },
                                    new FloatKeyframe
                                    {
                                        Time = animationTimes.End,
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
                                        Time = animationTimes.Start,
                                        Value = 0.0f
                                    },
                                    new FloatKeyframe
                                    {
                                        Time = animationTimes.Start + animationTimes.FadeDuration,
                                        Value = 1.0f,
                                        EasingFunction = EasingFunctionIn
                                    },
                                    new FloatKeyframe
                                    {
                                        Time = animationTimes.End,
                                        Value = 1.0f,
                                        EasingFunction = EasingFunction.Linear,
                                    }
                                }
                            }
                        }
                    }
                }

            };
        }

        private Visual BuildBar(int pointIndex, int seriesIndex, Config config, SeriesMetrics metrics, AnimationTimes animationTimes)
        {
            var series = config.Data.Series[seriesIndex];
            var point = series.Points[pointIndex];

            // BarSeriesIndex represents the index of these series among bar-only series.
            var barSeriesIndex = config.Data.Series.Take(seriesIndex).Count(s => s.Type == SeriesType.Bar);
            float xFrom = metrics.GetLeftForBar(pointIndex, barSeriesIndex);
            float width = metrics.BarWidth / metrics.BarSeriesCount;

            float yZero = metrics.GetYForValue(0);

            var barColor = (System.Drawing.Color)TypeSerialization.Deserialize(series.Color, typeof(System.Drawing.Color));
            var altColor = (System.Drawing.Color)TypeSerialization.Deserialize(series.AltColor, typeof(System.Drawing.Color));

            var bar = new Engine.Elements.Path
            {
                Name = $"Series_{seriesIndex}_Bar_{pointIndex}",
                Brush = new SolidBrush
                {
                    Color = barColor
                }
            };
            BuildGlobalAlphaAnimation(bar.Animations, animationTimes);

            var s1 = new MoveSegment
            {
                EndPoint = new System.Drawing.PointF(xFrom, yZero)
            };
            bar.Definition.Add(s1);

            var s2 = new LineSegment
            {
                EndPoint = new System.Drawing.PointF(xFrom + width, yZero)
            };
            bar.Definition.Add(s2);

            var s3 = new LineSegment
            {
                EndPoint = new System.Drawing.PointF(xFrom + width, yZero), // Will be animated anyway                
            };
            BuildPointAnimations(xFrom + width, point, s3.Animations, nameof(LineSegment.EndPoint), config, metrics, animationTimes);
            bar.Definition.Add(s3);

            var s4 = new LineSegment
            {
                EndPoint = new System.Drawing.PointF(xFrom, yZero) // Will be animated anyway
            };
            BuildPointAnimations(xFrom, point, s4.Animations, nameof(LineSegment.EndPoint), config, metrics, animationTimes);
            bar.Definition.Add(s4);

            var s5 = new CloseShapeSegment();
            bar.Definition.Add(s5);

            // Frame for end animation

            var yLast = metrics.GetYForValue(point.NextValues.Any() ? point.NextValues.Last() : point.Value);

            return bar;
        }

        private void BuildGlobalAlphaAnimation(ManagedCollection<Engine.Elements.Animation> animations,
            AnimationTimes animationTimes)
        {
            var storyboard = new Storyboard
            {
                Keyframes =
                {
                    new For
                    {
                        PropertyRef = nameof(Visual.Alpha),
                        Keyframes =
                        {
                            new FloatKeyframe
                            {
                                Time = animationTimes.Start,
                                Value = 0.0f
                            },
                            new FloatKeyframe
                            {
                                Time = animationTimes.Start + animationTimes.FadeDuration,
                                Value = 1.0f,
                                EasingFunction = EasingFunctionIn
                            },
                            new FloatKeyframe
                            {
                                Time = animationTimes.End - animationTimes.FadeDuration,
                                Value = 1.0f,
                                EasingFunction = EasingFunction.Linear
                            },
                            new FloatKeyframe
                            {
                                Time = animationTimes.End,
                                Value = 0.0f,
                                EasingFunction = EasingFunctionOut
                            }
                        }
                    }
                }
            };

            animations.Add(storyboard);
        }

        private void BuildGlobalCutAnimation(ManagedCollection<Engine.Elements.Animation> animations,
            AnimationTimes animationTimes)
        {
            var storyboard = new Storyboard
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
                                Time = animationTimes.End - animationTimes.FadeDuration,
                                Value = 0.0f,
                                EasingFunction = EasingFunction.Linear
                            },
                            new FloatKeyframe
                            {
                                Time = animationTimes.End,
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
                                Time = animationTimes.Start,
                                Value = 0.0f
                            },
                            new FloatKeyframe
                            {
                                Time = animationTimes.Start + animationTimes.FadeDuration,
                                Value = 1.0f,
                                EasingFunction = EasingFunctionIn
                            }
                        }
                    }
                }
            };

            animations.Add(storyboard);
        }

        private Visual BuildLineChart(int seriesIndex, Config config, SeriesMetrics metrics, AnimationTimes animationTimes)
        {
            var series = config.Data.Series[seriesIndex];

            var color = (System.Drawing.Color)TypeSerialization.Deserialize(series.Color, typeof(System.Drawing.Color));

            var path = new Engine.Elements.Path
            {
                Pen = new Pen
                {
                    Width = series.LineWidth,
                    Color = color
                }
            };
            BuildGlobalCutAnimation(path.Animations, animationTimes);

            if (series.Points.Count < 2)
                return path;

            // Initial moveto

            var x = metrics.GetXForPoint(0);
            var y = metrics.GetYForValue(0);
            var m1 = new MoveSegment
            {
                EndPoint = new System.Drawing.PointF(x, y)
            };
            BuildPointAnimations(x, series.Points[0], m1.Animations, nameof(MoveSegment.EndPoint), config, metrics, animationTimes, false, false);
            path.Definition.Add(m1);

            // Subsequent lineto-s

            for (int i = 1; i < series.Points.Count; i++)
            {
                var point = series.Points[i];

                x = metrics.GetXForPoint(i);
                y = metrics.GetYForValue(point.Value);

                var l = new LineSegment
                {
                    EndPoint = new System.Drawing.PointF(x, y)
                };
                BuildPointAnimations(x, point, l.Animations, nameof(LineSegment.EndPoint), config, metrics, animationTimes, false, false);
                path.Definition.Add(l);
            }

            return path;
        }

        private void BuildPointAnimations(float x,
            Point point,
            ManagedCollection<Engine.Elements.Animation> animations,
            string pointPropertyName,
            Config config,
            SeriesMetrics metrics,
            AnimationTimes animationTimes,
            bool first = true,
            bool last = true)
        {
            var storyboard = new Storyboard();
            var @for = new For
            {
                PropertyRef = pointPropertyName
            };
            storyboard.Keyframes.Add(@for);

            float yZero = metrics.GetYForValue(0);
            float yCurrent = metrics.GetYForValue(point.Value);

            // Two keyframes for initial reveal of the bar

            if (first)
            {
                @for.Keyframes.Add(new PointKeyframe
                {
                    Time = animationTimes.Start,
                    Value = new System.Drawing.PointF(x, yZero)
                });

                @for.Keyframes.Add(new PointKeyframe
                {
                    Time = animationTimes.Start + animationTimes.FadeDuration,
                    Value = new System.Drawing.PointF(x, yCurrent),
                    EasingFunction = EasingFunctionIn
                });
            }

            // Two keyframes for every next value

            for (int i = 0; i < point.NextValues.Count; i++)
            {
                float yPrevious = yCurrent;
                yCurrent = metrics.GetYForValue(point.NextValues[i]);
                TimeSpan time = (TimeSpan)TypeSerialization.Deserialize(config.Animation.SeriesSwitchTimes[i], typeof(TimeSpan));

                @for.Keyframes.Add(new PointKeyframe
                {
                    Time = time,
                    Value = new System.Drawing.PointF(x, yPrevious),
                    EasingFunction = EasingFunction.Linear
                });

                @for.Keyframes.Add(new PointKeyframe
                {
                    Time = time + animationTimes.FadeDuration,
                    Value = new System.Drawing.PointF(x, yCurrent),
                    EasingFunction = EasingFunctionInOut
                });
            }

            // Two keyframes for final fade

            if (last)
            {
                @for.Keyframes.Add(new PointKeyframe
                {
                    Time = animationTimes.End - animationTimes.FadeDuration,
                    Value = new System.Drawing.PointF(x, yCurrent),
                    EasingFunction = EasingFunction.Linear
                });

                @for.Keyframes.Add(new PointKeyframe
                {
                    Time = animationTimes.End,
                    Value = new System.Drawing.PointF(x, yZero),
                    EasingFunction = EasingFunctionOut
                });
            }

            animations.Add(storyboard);
        }

        private SeriesMetrics BuildSeriesMetrics(Config config)
        {
            if (config.Axis.YAxis.Labels.AreaWidth >= config.Width)
                throw new InvalidOperationException("Chart is not wide enough to fit all parts!");
            if (config.Axis.XAxis.Labels.AreaHeight + config.Header.Height >= config.Height)
                throw new InvalidOperationException("Chart is not high enough to fit all parts!");

            SeriesInfo seriesInfo = GetSeriesInfo(config);

            // Collecting data

            float headerHeight;
            if (config.Header.Show)
                headerHeight = config.Header.Height;
            else
                headerHeight = 0;

            System.Drawing.RectangleF headerArea = new System.Drawing.RectangleF(0, 0, config.Width, headerHeight);

            float xLabelsHeight = config.Axis.XAxis.Labels.AreaHeight;
            float xLineWidth = config.LineWidth;
            float yLabelsAreaWidth = config.Axis.YAxis.Labels.AreaWidth;
            float yLineWidth = config.LineWidth;
            float chartAreaWidth = config.Width - (yLabelsAreaWidth + yLineWidth);
            float chartAreaHeight = config.Height - headerHeight - (xLabelsHeight + xLineWidth);

            System.Drawing.RectangleF chartArea = new System.Drawing.RectangleF(yLabelsAreaWidth + yLineWidth, headerArea.Bottom, chartAreaWidth, chartAreaHeight);

            System.Drawing.RectangleF xLabelArea = new System.Drawing.RectangleF(chartArea.Left, config.Height - xLabelsHeight, chartArea.Width, xLabelsHeight);
            System.Drawing.RectangleF yLabelArea = new System.Drawing.RectangleF(0, chartArea.Top, yLabelsAreaWidth, chartArea.Height);

            float scaleMin = Math.Min(0, seriesInfo.MinValue);
            float scaleMax = Math.Max(0, seriesInfo.MaxValue);

            if (scaleMin == scaleMax)
                scaleMax = scaleMin + 1;

            if (config.Axis.YAxis.Scale > 1.0f)
            {
                float currentDataSpan = scaleMax - scaleMin;
                float additional = (config.Axis.YAxis.Scale - 1.0f) * currentDataSpan / 2.0f;

                scaleMin -= additional;
                scaleMax += additional;
            }

            float zeroY = chartArea.Top + scaleMax / (scaleMax - scaleMin) * chartAreaHeight;

            System.Drawing.PointF yLineStart = new System.Drawing.PointF(yLabelsAreaWidth + yLineWidth / 2, chartArea.Top);
            System.Drawing.PointF yLineEnd = new System.Drawing.PointF(yLabelsAreaWidth + yLineWidth / 2, chartArea.Bottom);
            System.Drawing.PointF xLineStart = new System.Drawing.PointF(chartArea.Left, zeroY);
            System.Drawing.PointF xLineEnd = new System.Drawing.PointF(chartArea.Right, zeroY);

            float dataSpan = scaleMax - scaleMin;

            int scale10thPower = (int)Math.Ceiling(Math.Log10(dataSpan)) - 1;
            float scaleStep = (float)Math.Pow(10, scale10thPower);

            float barWidth = chartAreaWidth / seriesInfo.PointCount * Math.Max(0.0f, Math.Min(1.0f, config.Axis.XAxis.BarScale));

            return new SeriesMetrics(headerArea,
                yLabelArea,
                xLabelArea,
                chartArea,
                yLineStart,
                yLineEnd,
                yLineWidth,
                xLineStart,
                xLineEnd,
                xLineWidth,
                scaleMin,
                scaleMax,
                seriesInfo.PointCount,
                seriesInfo.SeriesCount,
                seriesInfo.BarSeriesCount,
                barWidth,
                scaleStep);
        }

        private Visual CreateBackground(Config config, SeriesMetrics metrics, AnimationTimes animationTimes)
        {
            var backgroundColor = (System.Drawing.Color)TypeSerialization.Deserialize(config.ChartBackgroundColor, typeof(System.Drawing.Color));

            var result = new Rectangle
            {
                Position = new System.Drawing.PointF(metrics.ChartArea.X, metrics.ChartArea.Y),
                Width = metrics.ChartArea.Width,
                Height = metrics.ChartArea.Height,
                Brush = new SolidBrush
                {
                    Color = backgroundColor
                },
            };

            BuildGlobalAlphaAnimation(result.Animations, animationTimes);

            return result;
        }

        private Layer CreateMainLayer(Config config, AnimationTimes animationTimes)
        {
            return new Layer
            {
                Origin = new System.Drawing.PointF(config.Width / 2, config.Height / 2),
                Position = new System.Drawing.PointF(config.Width / 2, config.Height / 2),
                Animations =
                {
                    new Storyboard
                    {
                        Keyframes =
                        {
                            new For
                            {
                                PropertyRef = "Scale",
                                Keyframes =
                                {
                                    new PointKeyframe
                                    {
                                        Time = animationTimes.Start,
                                        Value = new System.Drawing.PointF(1.25f, 1.25f)
                                    },
                                    new PointKeyframe
                                    {
                                        Time = animationTimes.Start + animationTimes.FadeDuration,
                                        Value = config.Animation.SlowZoom ? new System.Drawing.PointF(1.05f, 1.05f) : new System.Drawing.PointF(1.0f, 1.0f),
                                        EasingFunction = EasingFunctionIn
                                    },
                                    new PointKeyframe
                                    {
                                        Time = animationTimes.End - animationTimes.FadeDuration,
                                        Value = config.Animation.SlowZoom ? new System.Drawing.PointF(0.95f, 0.95f) : new System.Drawing.PointF(1.0f, 1.0f),
                                        EasingFunction = EasingFunction.Linear
                                    },
                                    new PointKeyframe
                                    {
                                        Time = animationTimes.End,
                                        Value = new System.Drawing.PointF(0.75f, 0.75f),
                                        EasingFunction = EasingFunctionOut
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }

        private SeriesInfo GetSeriesInfo(Config config)
        {
            if (config.Data.Series.Any() && config.Data.Series[0].Points.Any())
            {
                float minValue = config.Data.Series
                    .SelectMany(s => s.Points)
                    .SelectMany(p => new[] { p.Value }.Concat(p.NextValues))
                    .Min();

                float maxValue = config.Data.Series
                    .SelectMany(s => s.Points)
                    .SelectMany(p => new[] { p.Value }.Concat(p.NextValues))
                    .Max();

                int seriesCount = config.Data.Series.Count;

                int barSeriesCount = config.Data.Series.Count(s => s.Type == SeriesType.Bar);

                int pointCount = config.Data.Series
                    .Select(s => s.Points.Count)
                    .Max();

                return new SeriesInfo(minValue, maxValue, barSeriesCount, seriesCount, pointCount);
            }
            else
            {
                return new SeriesInfo(0, 1, 0, 0, 0);
            }
        }

        private void RenderHeader(Config config, SeriesMetrics metrics, AnimationTimes animationTimes, Layer result)
        {
            List<string> headers = new[] { config.Header.Text }
                .Concat(config.Header.NextHeader)
                .ToList();

            List<TimeSpan> times = new[] { animationTimes.Start }
                .Concat(animationTimes.SeriesSwitchTimes)
                .Concat(new[] { animationTimes.End - animationTimes.FadeDuration / 2})
                .ToList();

            float x = metrics.HeaderArea.Left + metrics.HeaderArea.Width / 2;
            float y = metrics.HeaderArea.Top + metrics.HeaderArea.Height / 2;
            float xDelta = config.Width / 15.0f;
            float yDelta = config.Height / 10.0f;

            var color = (System.Drawing.Color)TypeSerialization.Deserialize(config.Header.Color, typeof(System.Drawing.Color));

            for (int i = 0; i < Math.Min(headers.Count, times.Count - 1); i++)
            {
                var header = new Label
                {
                    Position = new System.Drawing.PointF(x, y),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontFamily = config.Header.FontFamily,
                    FontSize = config.Header.FontSize,
                    Bold = config.Header.Bold,
                    Italic = config.Header.Italic,
                    Underline = config.Header.Underline,
                    Text = headers[i],
                    Brush = new SolidBrush
                    {
                        Color = color
                    },
                    Animations =
                    {
                        new Storyboard
                        {
                            Keyframes =
                            {
                                new For
                                {
                                    PropertyRef = nameof(Label.Alpha),
                                    Keyframes =
                                    {
                                        new FloatKeyframe
                                        {
                                            Time = times[i],
                                            Value = 0
                                        },
                                        new FloatKeyframe
                                        {
                                            Time = times[i] + animationTimes.FadeDuration,
                                            Value = 1,
                                            EasingFunction = EasingFunctionIn
                                        },
                                        new FloatKeyframe
                                        {
                                            Time = times[i + 1] - animationTimes.FadeDuration / 2,
                                            Value = 1,
                                            EasingFunction = EasingFunction.Linear
                                        },
                                        new FloatKeyframe
                                        {
                                            Time = times[i + 1] + animationTimes.FadeDuration / 2,
                                            Value = 0,
                                            EasingFunction = EasingFunctionOut
                                        }
                                    }
                                },
                                new For
                                {
                                    PropertyRef = nameof(Label.Position),
                                    Keyframes =
                                    {
                                        new PointKeyframe
                                        {
                                            Time = times[i],
                                            Value = new System.Drawing.PointF(x, y - yDelta)
                                        },
                                        new PointKeyframe
                                        {
                                            Time = times[i] + animationTimes.FadeDuration,
                                            Value = new System.Drawing.PointF(x, y),
                                            EasingFunction = EasingFunctionIn
                                        },
                                        new PointKeyframe
                                        {
                                            Time = times[i + 1] - animationTimes.FadeDuration / 2,
                                            Value = new System.Drawing.PointF(x, y),
                                            EasingFunction = EasingFunction.Linear
                                        },
                                        new PointKeyframe
                                        {
                                            Time = times[i + 1] + animationTimes.FadeDuration / 2,
                                            Value = new System.Drawing.PointF(x, y + yDelta),
                                            EasingFunction = EasingFunctionOut
                                        }
                                    }
                                }
                            }
                        }
                    }
                };

                result.Items.Add(header);
            }
        }

        private void RenderAxis(Config config, SeriesMetrics metrics, AnimationTimes animationTimes, Layer result)
        {
            var yLineColor = (System.Drawing.Color)TypeSerialization.Deserialize(config.Axis.YAxis.Color, typeof(System.Drawing.Color));
            var yLine = BuildAxisLine(new LineDefinition(metrics.YLineStart, metrics.YLineEnd, yLineColor, config.LineWidth),
                animationTimes);
            result.Items.Add(yLine);

            var xLineColor = (System.Drawing.Color)TypeSerialization.Deserialize(config.Axis.XAxis.Color, typeof(System.Drawing.Color));
            var xLine = BuildAxisLine(new LineDefinition(metrics.XLineStart, metrics.XLineEnd, xLineColor, config.LineWidth),
                animationTimes);
            result.Items.Add(xLine);
        }

        private void RenderXAxisLabels(Config config, SeriesMetrics metrics, AnimationTimes animationTimes, Layer result)
        {
            if (config.Axis.XAxis.Labels.Show)
            {
                var maxLabels = Math.Min(metrics.PointCount, config.Axis.XAxis.Labels.Labels.Count);

                System.Drawing.Color labelColor = (System.Drawing.Color)TypeSerialization.Deserialize(config.Axis.XAxis.Labels.Color, typeof(System.Drawing.Color));

                for (int i = 0; i < maxLabels; i++)
                {
                    var x = metrics.GetXForPoint(i);
                    var y = metrics.XLabelArea.Top + config.Axis.XAxis.Labels.Margin;

                    var label = new Label
                    {
                        Position = new System.Drawing.PointF(x, y),
                        Text = config.Axis.XAxis.Labels.Labels[i],
                        HorizontalAlignment = config.Axis.XAxis.Labels.HorizontalAlignment,
                        VerticalAlignment = config.Axis.XAxis.Labels.VerticalAlignment,
                        FontFamily = config.Axis.XAxis.Labels.FontFamily,
                        FontSize = config.Axis.XAxis.Labels.FontSize,
                        Bold = config.Axis.XAxis.Labels.Bold,
                        Italic = config.Axis.XAxis.Labels.Italic,
                        Underline = config.Axis.XAxis.Labels.Underline,
                        Brush = new SolidBrush
                        {
                            Color = labelColor
                        }
                    };

                    if (!Math.Abs(config.Axis.XAxis.Labels.Angle).IsZero())
                        label.Rotation = config.Axis.XAxis.Labels.Angle;

                    BuildGlobalAlphaAnimation(label.Animations, animationTimes);
                    result.Items.Add(label);
                }
            }
        }

        private void RenderYAxisLabels(Config config, SeriesMetrics metrics, AnimationTimes animationTimes, Layer result)
        {
            Label CreateLabel(float x, float y, float current, System.Drawing.Color labelColor)
            {
                string text;

                switch (config.Axis.YAxis.Labels.Rounding)
                {
                    case RoundingType.None:
                        text = string.Format(config.Axis.YAxis.Labels.LabelFormat, current.ToString(config.Axis.YAxis.Labels.NumericFormat));
                        break;
                    case RoundingType.SI:
                        text = string.Format(config.Axis.YAxis.Labels.LabelFormat, current.ToSIString(config.Axis.YAxis.Labels.NumericFormat));
                        break;
                    case RoundingType.Money:
                        text = string.Format(config.Axis.YAxis.Labels.LabelFormat, current.ToMoneyString(config.Axis.YAxis.Labels.NumericFormat));
                        break;
                    default:
                        throw new InvalidOperationException("Unsupported rounding!");
                }

                var label = new Label
                {
                    Position = new System.Drawing.PointF(x, y),
                    Text = text,
                    HorizontalAlignment = config.Axis.YAxis.Labels.HorizontalAlignment,
                    VerticalAlignment = config.Axis.YAxis.Labels.VerticalAlignment,
                    FontFamily = config.Axis.YAxis.Labels.FontFamily,
                    FontSize = config.Axis.YAxis.Labels.FontSize,
                    Bold = config.Axis.YAxis.Labels.Bold,
                    Italic = config.Axis.YAxis.Labels.Italic,
                    Underline = config.Axis.YAxis.Labels.Underline,
                    Brush = new SolidBrush
                    {
                        Color = labelColor
                    }
                };

                if (!Math.Abs(config.Axis.YAxis.Labels.Angle).IsZero())
                    label.Rotation = config.Axis.YAxis.Labels.Angle;

                BuildGlobalAlphaAnimation(label.Animations, animationTimes);

                return label;
            }

            if (config.Axis.YAxis.Labels.Show)
            {
                float current = 0.0f;

                float y = metrics.GetYForValue(current);
                float x = metrics.YLabelArea.Right - config.Axis.YAxis.Labels.Margin;

                System.Drawing.Color labelColor = (System.Drawing.Color)TypeSerialization.Deserialize(config.Axis.YAxis.Labels.Color, typeof(System.Drawing.Color));

                while (y > metrics.ChartArea.Top)
                {
                    var label = CreateLabel(x, y, current, labelColor);
                    result.Items.Add(label);

                    current += metrics.ScaleStep * config.Axis.YAxis.Labels.Skip;
                    y = metrics.GetYForValue(current);
                }

                current = 0.0f - metrics.ScaleStep * config.Axis.YAxis.Labels.Skip;
                y = metrics.GetYForValue(current);

                while (y < metrics.ChartArea.Bottom)
                {
                    var label = CreateLabel(x, y, current, labelColor);
                    result.Items.Add(label);

                    current -= metrics.ScaleStep * config.Axis.YAxis.Labels.Skip;
                    y = metrics.GetYForValue(current);
                }
            }
        }

        // Public methods -----------------------------------------------------

        public override ManagedObject Generate(XmlElement node)
        {
            if (node.ChildNodes.Count != 1)
                throw new InvalidOperationException("Histogram node should have only one child: Config!");

            Config config = ((XmlElement)node.ChildNodes[0]).Deserialize<Config>();
            SeriesMetrics metrics = BuildSeriesMetrics(config);
            AnimationTimes animationTimes = CalculateAnimationTimes(config);

            Layer result = CreateMainLayer(config, animationTimes);

            // Render background

            var backgroundColor = (System.Drawing.Color)TypeSerialization.Deserialize(config.ChartBackgroundColor, typeof(System.Drawing.Color));
            if (backgroundColor.A > 0)
            {
                Visual background = CreateBackground(config, metrics, animationTimes);
                result.Items.Add(background);
            }

            // Render series

            for (int seriesIndex = 0; seriesIndex < config.Data.Series.Count; seriesIndex++)
            {
                var series = config.Data.Series[seriesIndex];

                switch (config.Data.Series[seriesIndex].Type)
                {
                    case SeriesType.Bar:
                        for (int pointIndex = 0; pointIndex < series.Points.Count; pointIndex++)
                            result.Items.Add(BuildBar(pointIndex, seriesIndex, config, metrics, animationTimes));
                        break;
                    case SeriesType.Line:
                        result.Items.Add(BuildLineChart(seriesIndex, config, metrics, animationTimes));
                        break;
                    default:
                        throw new InvalidOperationException("Unsupported series type!");
                }
            }

            // Render labels

            RenderXAxisLabels(config, metrics, animationTimes, result);

            RenderYAxisLabels(config, metrics, animationTimes, result);

            // Render lines

            RenderAxis(config, metrics, animationTimes, result);

            // Render header

            RenderHeader(config, metrics, animationTimes, result);

            return result;
        }
    }
}
