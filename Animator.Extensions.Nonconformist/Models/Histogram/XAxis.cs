﻿using System;
using System.Xml.Serialization;

namespace Animator.Extensions.Nonconformist.Models.Histogram
{
    [XmlRoot]
    public class XAxis
    {
        [XmlAttribute]
        public bool ShowLabels { get; set; } = true;

        [XmlAttribute]
        public float LabelAreaHeight { get; set; } = 100.0f;

        [XmlElement("Label")]
        public List<string> Labels { get; set; } = new();
    }
}