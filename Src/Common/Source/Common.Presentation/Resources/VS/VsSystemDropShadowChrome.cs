using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.PlatformUI;

namespace NuPattern.Common.Presentation.Vs
{
    /// <summary>
    /// The VS SystemDropShadowChrome control.
    /// </summary>
    /// <remarks>This class is a direct copy of the <see cref="SystemDropShadowChrome"/>, and is embedded here to avoid 
    /// referencing Microsoft.VisualStudio.Shell.1X.0 in any xaml files that use this class.</remarks>
    public sealed class VsSystemDropShadowChrome : Decorator
    {
        private Brush[] _brushes;
        private static Brush[] _commonBrushes;
        private static System.Windows.CornerRadius _commonCornerRadius;
        private static object _resourceAccess = new object();
        
        /// <summary>
        /// Gets the color property.
        /// </summary>
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(System.Windows.Media.Color), typeof(VsSystemDropShadowChrome), new FrameworkPropertyMetadata(System.Windows.Media.Color.FromArgb(0x71, 0, 0, 0), FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(VsSystemDropShadowChrome.ClearBrushes)));
        
        /// <summary>
        /// Gets the corner radius property.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(System.Windows.CornerRadius), typeof(VsSystemDropShadowChrome), new FrameworkPropertyMetadata(new System.Windows.CornerRadius(), FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(VsSystemDropShadowChrome.ClearBrushes)), new ValidateValueCallback(VsSystemDropShadowChrome.IsCornerRadiusValid));

        private static void ClearBrushes(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((VsSystemDropShadowChrome)o)._brushes = null;
        }

        private static Brush[] CreateBrushes(System.Windows.Media.Color c, System.Windows.CornerRadius cornerRadius)
        {
            GradientStopCollection stops2;
            GradientStopCollection stops3;
            GradientStopCollection stops4;
            GradientStopCollection stops5;
            Brush[] brushArray = new Brush[9];
            brushArray[4] = new SolidColorBrush(c);
            brushArray[4].Freeze();
            GradientStopCollection gradientStopCollection = CreateStops(c, 0.0);
            LinearGradientBrush brush = new LinearGradientBrush(gradientStopCollection, new Point(0.0, 1.0), new Point(0.0, 0.0));
            brush.Freeze();
            brushArray[1] = brush;
            LinearGradientBrush brush2 = new LinearGradientBrush(gradientStopCollection, new Point(1.0, 0.0), new Point(0.0, 0.0));
            brush2.Freeze();
            brushArray[3] = brush2;
            LinearGradientBrush brush3 = new LinearGradientBrush(gradientStopCollection, new Point(0.0, 0.0), new Point(1.0, 0.0));
            brush3.Freeze();
            brushArray[5] = brush3;
            LinearGradientBrush brush4 = new LinearGradientBrush(gradientStopCollection, new Point(0.0, 0.0), new Point(0.0, 1.0));
            brush4.Freeze();
            brushArray[7] = brush4;
            if (cornerRadius.TopLeft == 0.0)
            {
                stops2 = gradientStopCollection;
            }
            else
            {
                stops2 = CreateStops(c, cornerRadius.TopLeft);
            }
            RadialGradientBrush brush5 = new RadialGradientBrush(stops2)
            {
                RadiusX = 1.0,
                RadiusY = 1.0,
                Center = new Point(1.0, 1.0),
                GradientOrigin = new Point(1.0, 1.0)
            };
            brush5.Freeze();
            brushArray[0] = brush5;
            if (cornerRadius.TopRight == 0.0)
            {
                stops3 = gradientStopCollection;
            }
            else if (cornerRadius.TopRight == cornerRadius.TopLeft)
            {
                stops3 = stops2;
            }
            else
            {
                stops3 = CreateStops(c, cornerRadius.TopRight);
            }
            RadialGradientBrush brush6 = new RadialGradientBrush(stops3)
            {
                RadiusX = 1.0,
                RadiusY = 1.0,
                Center = new Point(0.0, 1.0),
                GradientOrigin = new Point(0.0, 1.0)
            };
            brush6.Freeze();
            brushArray[2] = brush6;
            if (cornerRadius.BottomLeft == 0.0)
            {
                stops4 = gradientStopCollection;
            }
            else if (cornerRadius.BottomLeft == cornerRadius.TopLeft)
            {
                stops4 = stops2;
            }
            else if (cornerRadius.BottomLeft == cornerRadius.TopRight)
            {
                stops4 = stops3;
            }
            else
            {
                stops4 = CreateStops(c, cornerRadius.BottomLeft);
            }
            RadialGradientBrush brush7 = new RadialGradientBrush(stops4)
            {
                RadiusX = 1.0,
                RadiusY = 1.0,
                Center = new Point(1.0, 0.0),
                GradientOrigin = new Point(1.0, 0.0)
            };
            brush7.Freeze();
            brushArray[6] = brush7;
            if (cornerRadius.BottomRight == 0.0)
            {
                stops5 = gradientStopCollection;
            }
            else if (cornerRadius.BottomRight == cornerRadius.TopLeft)
            {
                stops5 = stops2;
            }
            else if (cornerRadius.BottomRight == cornerRadius.TopRight)
            {
                stops5 = stops3;
            }
            else if (cornerRadius.BottomRight == cornerRadius.BottomLeft)
            {
                stops5 = stops4;
            }
            else
            {
                stops5 = CreateStops(c, cornerRadius.BottomRight);
            }
            RadialGradientBrush brush8 = new RadialGradientBrush(stops5)
            {
                RadiusX = 1.0,
                RadiusY = 1.0,
                Center = new Point(0.0, 0.0),
                GradientOrigin = new Point(0.0, 0.0)
            };
            brush8.Freeze();
            brushArray[8] = brush8;
            return brushArray;
        }

        private static GradientStopCollection CreateStops(System.Windows.Media.Color c, double cornerRadius)
        {
            double num = 1.0 / (cornerRadius + 5.0);
            GradientStopCollection stops = new GradientStopCollection {
                new GradientStop(c, (0.5 + cornerRadius) * num)
            };
            System.Windows.Media.Color color = c;
            color.A = (byte)(0.74336 * c.A);
            stops.Add(new GradientStop(color, (1.5 + cornerRadius) * num));
            color.A = (byte)(0.38053 * c.A);
            stops.Add(new GradientStop(color, (2.5 + cornerRadius) * num));
            color.A = (byte)(0.12389 * c.A);
            stops.Add(new GradientStop(color, (3.5 + cornerRadius) * num));
            color.A = (byte)(0.02654 * c.A);
            stops.Add(new GradientStop(color, (4.5 + cornerRadius) * num));
            color.A = 0;
            stops.Add(new GradientStop(color, (5.0 + cornerRadius) * num));
            stops.Freeze();
            return stops;
        }

        private Brush[] GetBrushes(System.Windows.Media.Color c, System.Windows.CornerRadius cornerRadius)
        {
            if (_commonBrushes == null)
            {
                lock (_resourceAccess)
                {
                    if (_commonBrushes == null)
                    {
                        _commonBrushes = CreateBrushes(c, cornerRadius);
                        _commonCornerRadius = cornerRadius;
                    }
                }
            }
            if ((c == ((SolidColorBrush)_commonBrushes[4]).Color) && (cornerRadius == _commonCornerRadius))
            {
                this._brushes = null;
                return _commonBrushes;
            }
            if (this._brushes == null)
            {
                this._brushes = CreateBrushes(c, cornerRadius);
            }
            return this._brushes;
        }

        private static bool IsCornerRadiusValid(object value)
        {
            System.Windows.CornerRadius radius = (System.Windows.CornerRadius)value;
            return ((((((radius.TopLeft >= 0.0) && (radius.TopRight >= 0.0)) && ((radius.BottomLeft >= 0.0) && (radius.BottomRight >= 0.0))) && ((!double.IsNaN(radius.TopLeft) && !double.IsNaN(radius.TopRight)) && (!double.IsNaN(radius.BottomLeft) && !double.IsNaN(radius.BottomRight)))) && ((!double.IsInfinity(radius.TopLeft) && !double.IsInfinity(radius.TopRight)) && !double.IsInfinity(radius.BottomLeft))) && !double.IsInfinity(radius.BottomRight));
        }

        /// <summary>
        /// Renders the control.
        /// </summary>
        /// <param name="drawingContext"></param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            System.Windows.CornerRadius cornerRadius = this.CornerRadius;
            Rect rect = new Rect(new Point(5.0, 5.0), new Size(base.RenderSize.Width, base.RenderSize.Height));
            System.Windows.Media.Color c = this.Color;
            if (((rect.Width > 0.0) && (rect.Height > 0.0)) && (c.A > 0))
            {
                double width = (rect.Right - rect.Left) - 10.0;
                double height = (rect.Bottom - rect.Top) - 10.0;
                double num3 = Math.Min((double)(width * 0.5), (double)(height * 0.5));
                cornerRadius.TopLeft = Math.Min(cornerRadius.TopLeft, num3);
                cornerRadius.TopRight = Math.Min(cornerRadius.TopRight, num3);
                cornerRadius.BottomLeft = Math.Min(cornerRadius.BottomLeft, num3);
                cornerRadius.BottomRight = Math.Min(cornerRadius.BottomRight, num3);
                Brush[] brushes = this.GetBrushes(c, cornerRadius);
                double num4 = rect.Top + 5.0;
                double num5 = rect.Left + 5.0;
                double num6 = rect.Right - 5.0;
                double num7 = rect.Bottom - 5.0;
                double[] guidelinesX = new double[] { num5, num5 + cornerRadius.TopLeft, num6 - cornerRadius.TopRight, num5 + cornerRadius.BottomLeft, num6 - cornerRadius.BottomRight, num6 };
                double[] guidelinesY = new double[] { num4, num4 + cornerRadius.TopLeft, num4 + cornerRadius.TopRight, num7 - cornerRadius.BottomLeft, num7 - cornerRadius.BottomRight, num7 };
                drawingContext.PushGuidelineSet(new GuidelineSet(guidelinesX, guidelinesY));
                cornerRadius.TopLeft += 5.0;
                cornerRadius.TopRight += 5.0;
                cornerRadius.BottomLeft += 5.0;
                cornerRadius.BottomRight += 5.0;
                Rect rectangle = new Rect(rect.Left, rect.Top, cornerRadius.TopLeft, cornerRadius.TopLeft);
                drawingContext.DrawRectangle(brushes[0], null, rectangle);
                double num8 = guidelinesX[2] - guidelinesX[1];
                if (num8 > 0.0)
                {
                    Rect rect3 = new Rect(guidelinesX[1], rect.Top, num8, 5.0);
                    drawingContext.DrawRectangle(brushes[1], null, rect3);
                }
                Rect rect4 = new Rect(guidelinesX[2], rect.Top, cornerRadius.TopRight, cornerRadius.TopRight);
                drawingContext.DrawRectangle(brushes[2], null, rect4);
                double num9 = guidelinesY[3] - guidelinesY[1];
                if (num9 > 0.0)
                {
                    Rect rect5 = new Rect(rect.Left, guidelinesY[1], 5.0, num9);
                    drawingContext.DrawRectangle(brushes[3], null, rect5);
                }
                double num10 = guidelinesY[4] - guidelinesY[2];
                if (num10 > 0.0)
                {
                    Rect rect6 = new Rect(guidelinesX[5], guidelinesY[2], 5.0, num10);
                    drawingContext.DrawRectangle(brushes[5], null, rect6);
                }
                Rect rect7 = new Rect(rect.Left, guidelinesY[3], cornerRadius.BottomLeft, cornerRadius.BottomLeft);
                drawingContext.DrawRectangle(brushes[6], null, rect7);
                double num11 = guidelinesX[4] - guidelinesX[3];
                if (num11 > 0.0)
                {
                    Rect rect8 = new Rect(guidelinesX[3], guidelinesY[5], num11, 5.0);
                    drawingContext.DrawRectangle(brushes[7], null, rect8);
                }
                Rect rect9 = new Rect(guidelinesX[4], guidelinesY[4], cornerRadius.BottomRight, cornerRadius.BottomRight);
                drawingContext.DrawRectangle(brushes[8], null, rect9);
                if (((cornerRadius.TopLeft == 5.0) && (cornerRadius.TopLeft == cornerRadius.TopRight)) && ((cornerRadius.TopLeft == cornerRadius.BottomLeft) && (cornerRadius.TopLeft == cornerRadius.BottomRight)))
                {
                    Rect rect10 = new Rect(guidelinesX[0], guidelinesY[0], width, height);
                    drawingContext.DrawRectangle(brushes[4], null, rect10);
                }
                else
                {
                    PathFigure figure = new PathFigure();
                    if (cornerRadius.TopLeft > 5.0)
                    {
                        figure.StartPoint = new Point(guidelinesX[1], guidelinesY[0]);
                        figure.Segments.Add(new LineSegment(new Point(guidelinesX[1], guidelinesY[1]), true));
                        figure.Segments.Add(new LineSegment(new Point(guidelinesX[0], guidelinesY[1]), true));
                    }
                    else
                    {
                        figure.StartPoint = new Point(guidelinesX[0], guidelinesY[0]);
                    }
                    if (cornerRadius.BottomLeft > 5.0)
                    {
                        figure.Segments.Add(new LineSegment(new Point(guidelinesX[0], guidelinesY[3]), true));
                        figure.Segments.Add(new LineSegment(new Point(guidelinesX[3], guidelinesY[3]), true));
                        figure.Segments.Add(new LineSegment(new Point(guidelinesX[3], guidelinesY[5]), true));
                    }
                    else
                    {
                        figure.Segments.Add(new LineSegment(new Point(guidelinesX[0], guidelinesY[5]), true));
                    }
                    if (cornerRadius.BottomRight > 5.0)
                    {
                        figure.Segments.Add(new LineSegment(new Point(guidelinesX[4], guidelinesY[5]), true));
                        figure.Segments.Add(new LineSegment(new Point(guidelinesX[4], guidelinesY[4]), true));
                        figure.Segments.Add(new LineSegment(new Point(guidelinesX[5], guidelinesY[4]), true));
                    }
                    else
                    {
                        figure.Segments.Add(new LineSegment(new Point(guidelinesX[5], guidelinesY[5]), true));
                    }
                    if (cornerRadius.TopRight > 5.0)
                    {
                        figure.Segments.Add(new LineSegment(new Point(guidelinesX[5], guidelinesY[2]), true));
                        figure.Segments.Add(new LineSegment(new Point(guidelinesX[2], guidelinesY[2]), true));
                        figure.Segments.Add(new LineSegment(new Point(guidelinesX[2], guidelinesY[0]), true));
                    }
                    else
                    {
                        figure.Segments.Add(new LineSegment(new Point(guidelinesX[5], guidelinesY[0]), true));
                    }
                    figure.IsClosed = true;
                    figure.Freeze();
                    PathGeometry geometry = new PathGeometry
                    {
                        Figures = { figure }
                    };
                    geometry.Freeze();
                    drawingContext.DrawGeometry(brushes[4], null, geometry);
                }
                drawingContext.Pop();
            }
        }

        /// <summary>
        /// Gets or sets the color of the control.
        /// </summary>
        public System.Windows.Media.Color Color
        {
            get
            {
                return (System.Windows.Media.Color)base.GetValue(ColorProperty);
            }
            set
            {
                base.SetValue(ColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the radius of the corner.
        /// </summary>
        public System.Windows.CornerRadius CornerRadius
        {
            get
            {
                return (System.Windows.CornerRadius)base.GetValue(CornerRadiusProperty);
            }
            set
            {
                base.SetValue(CornerRadiusProperty, value);
            }
        }
    }
}
