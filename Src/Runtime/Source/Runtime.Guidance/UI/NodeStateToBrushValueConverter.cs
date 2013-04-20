using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using NuPattern.Runtime.Guidance.Workflow;

namespace NuPattern.Runtime.Guidance.UI
{
    [ValueConversion(typeof(NodeState), typeof(System.Windows.Media.SolidColorBrush))]
    internal class NodeStateToBrushValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is NodeState)
            {
                var state = (NodeState)value;

                Entry tuple;
                Color color = Color.Black;
                if (StateColorMapping.TryGetValue(state, out tuple))
                {
                    color = (parameter == null) ? tuple.Fill : tuple.Stroke;
                }

                var brush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
                return brush;
            }

            return null;
        }

        private static readonly Dictionary<NodeState, Entry> StateColorMapping = new[]
        {
            new Entry { State = NodeState.Enabled,   Fill = Color.DarkGreen,  Stroke = Color.DarkGreen },
            new Entry { State = NodeState.Blocked,   Fill = Color.DarkRed,     Stroke = Color.DarkRed}, // Color.FromArgb(255, 64, 64)   --> Tomato
            new Entry { State = NodeState.Completed, Fill = Color.DarkSlateGray,  Stroke = Color.DarkSlateGray },
            new Entry { State = NodeState.Disabled,  Fill = Color.White,      Stroke = Color.DarkRed},
            new Entry { State = NodeState.Default,   Fill = Color.Black,   Stroke = Color.Black  },
        }.ToDictionary(t => t.State);

        private class Entry
        {
            public NodeState State { get; set; }
            public Color Fill { get; set; }
            public Color Stroke { get; set; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
