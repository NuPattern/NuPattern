using System;
using System.Linq;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using System.Collections.Generic;
using System.Drawing;


namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
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
                Color color = Color.DarkGray;
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
            new Entry { State = NodeState.Enabled,   Fill = Color.LimeGreen,  Stroke = Color.LimeGreen },
            new Entry { State = NodeState.Blocked,   Fill = Color.Red,     Stroke = Color.Red}, // Color.FromArgb(255, 64, 64)   --> Tomato
            new Entry { State = NodeState.Completed, Fill = Color.Gainsboro,  Stroke = Color.Gainsboro },
            new Entry { State = NodeState.Disabled,  Fill = Color.White,      Stroke = Color.Red},
            new Entry { State = NodeState.Default,   Fill = Color.DarkGray,   Stroke = Color.DarkGray  },
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
