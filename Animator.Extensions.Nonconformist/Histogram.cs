using Animator.Engine.Base;
using Animator.Engine.Elements;
using Animator.Extensions.Nonconformist.Models.Histogram;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Animator.Extensions.Nonconformist
{
    public class Histogram : BaseGenerator
    {
        // Private types ------------------------------------------------------
        private sealed record SeriesInfo(float MinValue, 
            float MaxValue, 
            int SeriesCount, 
            int PointCount);

        private sealed record SeriesMetrics(System.Drawing.RectangleF YLabelArea,
            System.Drawing.RectangleF XLabelArea,
            System.Drawing.RectangleF ChartArea,
            System.Drawing.PointF YLineStart,
            System.Drawing.PointF YLineEnd,
            float YLineWidth,
            System.Drawing.PointF XLineStart,
            System.Drawing.PointF XLineEnd,
            float XLineWidth
            );

        // Private methods ----------------------------------------------------

        private Config DeserializeConfig(XmlNode node)
        {
            var configNode = node.ChildNodes[0];
            XmlSerializer serializer = new XmlSerializer(typeof(Models.Histogram.Config));
            var reader = new StringReader($"{configNode.OuterXml}");
            Models.Histogram.Config config = (Models.Histogram.Config)serializer.Deserialize(reader);
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

            float dataSpan = scaleMax - scaleMin;
            int scale10thPower = (int)Math.Ceiling(Math.Log10(dataSpan)) - 1;

            float zeroY = scaleMax / (scaleMax - scaleMin) * chartAreaHeight;

            return new SeriesMetrics(new System.Drawing.RectangleF(0, 0, yLabelsAreaWidth, config.Height),
                new System.Drawing.RectangleF(yLabelsAreaWidth + yLineWidth, config.Height - xLabelsHeight, chartAreaWidth, xLabelsHeight),
                new System.Drawing.RectangleF(yLabelsAreaWidth + yLineWidth, 0, chartAreaWidth, chartAreaHeight),
                new System.Drawing.PointF(yLabelsAreaWidth + yLineWidth / 2, 0),
                new System.Drawing.PointF(yLabelsAreaWidth + yLineWidth / 2, chartAreaHeight),
                yLineWidth,
                new System.Drawing.PointF(yLabelsAreaWidth + yLineWidth / 2, zeroY),
                new System.Drawing.PointF(chartAreaWidth, zeroY),
                xLineWidth);
        }

        // Public methods -----------------------------------------------------

        public override ManagedObject Generate(XmlNode node)
        {
            if (node.ChildNodes.Count != 1)
                throw new InvalidOperationException("Histogram node should have only one child: Config!");

            Config config = DeserializeConfig(node);
            SeriesMetrics metrics = BuildSeriesMetrics(config);            

            var result = new Layer();

            // Render lines

            var yLine = new Line
            {
                Start = metrics.YLineStart,
                End = metrics.YLineEnd,
                Pen = new Pen
                {
                    Color = System.Drawing.Color.FromArgb(0x60, 0xEB, 0xF2),
                    Width = metrics.YLineWidth
                }
            };
            result.Items.Add(yLine);

            var xLine = new Line
            {
                Start = metrics.XLineStart,
                End = metrics.XLineEnd,
                Pen = new Pen
                {
                    Color = System.Drawing.Color.FromArgb(0x60, 0xEB, 0xF2),
                    Width = metrics.XLineWidth
                }
            };
            result.Items.Add(xLine);

            return result;
        }

    }
}
