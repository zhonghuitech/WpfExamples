using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;

namespace WindowCapture.Framework
{
    public class ScreenCutAdorner : Adorner
    {
        private const double THUMB_SIZE = 15.0;
        private const double MINIMAL_SIZE = 20.0;
        private const double LINE_Size = 6.0;
        private readonly Thumb lc;
        private readonly Thumb tl;
        private readonly Thumb tc;
        private readonly Thumb tr;
        private readonly Thumb rc;
        private readonly Thumb br;
        private readonly Thumb bc;
        private readonly Thumb bl;
        private readonly VisualCollection visCollec;
        private readonly Canvas canvas;
        private readonly bool _isRatioScale;
        private readonly Size _scaleSize;

        public ScreenCutAdorner(UIElement adorned, bool isRatioScale = false, Size scaleSize = default(Size))
          : base(adorned)
        {
            this.canvas = ScreenCutAdorner.FindParent(adorned) as Canvas;
            this._isRatioScale = isRatioScale;
            this._scaleSize = scaleSize;
            this.visCollec = new VisualCollection((Visual)this);
            this.visCollec.Add((Visual)(this.lc = this.GetResizeThumb(Cursors.SizeWE, HorizontalAlignment.Left, VerticalAlignment.Center)));
            this.visCollec.Add((Visual)(this.tl = this.GetResizeThumb(Cursors.SizeNWSE, HorizontalAlignment.Left, VerticalAlignment.Top)));
            this.visCollec.Add((Visual)(this.tc = this.GetResizeThumb(Cursors.SizeNS, HorizontalAlignment.Center, VerticalAlignment.Top)));
            this.visCollec.Add((Visual)(this.tr = this.GetResizeThumb(Cursors.SizeNESW, HorizontalAlignment.Right, VerticalAlignment.Top)));
            this.visCollec.Add((Visual)(this.rc = this.GetResizeThumb(Cursors.SizeWE, HorizontalAlignment.Right, VerticalAlignment.Center)));
            this.visCollec.Add((Visual)(this.br = this.GetResizeThumb(Cursors.SizeNWSE, HorizontalAlignment.Right, VerticalAlignment.Bottom)));
            this.visCollec.Add((Visual)(this.bc = this.GetResizeThumb(Cursors.SizeNS, HorizontalAlignment.Center, VerticalAlignment.Bottom)));
            this.visCollec.Add((Visual)(this.bl = this.GetResizeThumb(Cursors.SizeNESW, HorizontalAlignment.Left, VerticalAlignment.Bottom)));
        }

        private static UIElement FindParent(UIElement element) => VisualTreeHelper.GetParent((DependencyObject)element) as UIElement;

        protected override int VisualChildrenCount => this.visCollec.Count;

        protected override Size ArrangeOverride(Size finalSize)
        {
            double num1 = 7.5;
            Size size = new Size(15.0, 15.0);
            Size renderSize;
            if (!this._isRatioScale)
            {
                Thumb lc = this.lc;
                double num2 = -num1;
                renderSize = this.AdornedElement.RenderSize;
                double num3 = renderSize.Height / 2.0 - num1;
                Rect finalRect1 = new Rect(new Point(num2, num3), size);
                lc.Arrange(finalRect1);
                Thumb tc = this.tc;
                renderSize = this.AdornedElement.RenderSize;
                Rect finalRect2 = new Rect(new Point(renderSize.Width / 2.0 - num1, -num1), size);
                tc.Arrange(finalRect2);
                Thumb rc = this.rc;
                renderSize = this.AdornedElement.RenderSize;
                double num4 = renderSize.Width - num1;
                renderSize = this.AdornedElement.RenderSize;
                double num5 = renderSize.Height / 2.0 - num1;
                Rect finalRect3 = new Rect(new Point(num4, num5), size);
                rc.Arrange(finalRect3);
                Thumb bc = this.bc;
                renderSize = this.AdornedElement.RenderSize;
                double num6 = renderSize.Width / 2.0 - num1;
                renderSize = this.AdornedElement.RenderSize;
                double num7 = renderSize.Height - num1;
                Rect finalRect4 = new Rect(new Point(num6, num7), size);
                bc.Arrange(finalRect4);
            }
            else
            {
                this.lc.Height = this.AdornedElement.RenderSize.Height;
                this.lc.Width = 6.0;
                this.lc.Arrange(new Rect(new Point(0.0, 0.0), new Size(this.lc.Width, this.lc.Height)));
                this.tc.Height = 6.0;
                this.tc.Width = this.AdornedElement.RenderSize.Width;
                this.tc.Arrange(new Rect(new Point(0.0, 0.0), new Size(this.tc.Width, this.tc.Height)));
                this.rc.Width = 6.0;
                this.rc.Height = this.AdornedElement.RenderSize.Height;
                this.rc.Arrange(new Rect(new Point(this.AdornedElement.RenderSize.Width - 6.0, 0.0), new Size(this.rc.Width, this.rc.Height)));
                this.bc.Height = 6.0;
                this.bc.Width = this.AdornedElement.RenderSize.Width;
                this.bc.Arrange(new Rect(new Point(0.0, this.AdornedElement.RenderSize.Height - 6.0), new Size(this.bc.Width, this.bc.Height)));
            }
            this.tl.Arrange(new Rect(new Point(-num1, -num1), size));
            Thumb tr = this.tr;
            renderSize = this.AdornedElement.RenderSize;
            Rect finalRect5 = new Rect(new Point(renderSize.Width - num1, -num1), size);
            tr.Arrange(finalRect5);
            Thumb br = this.br;
            renderSize = this.AdornedElement.RenderSize;
            double num8 = renderSize.Width - num1;
            renderSize = this.AdornedElement.RenderSize;
            double num9 = renderSize.Height - num1;
            Rect finalRect6 = new Rect(new Point(num8, num9), size);
            br.Arrange(finalRect6);
            Thumb bl = this.bl;
            double num10 = -num1;
            renderSize = this.AdornedElement.RenderSize;
            double num11 = renderSize.Height - num1;
            Rect finalRect7 = new Rect(new Point(num10, num11), size);
            bl.Arrange(finalRect7);
            return finalSize;
        }

        private void Resize(FrameworkElement frameworkElement)
        {
            Size renderSize;
            if (double.IsNaN(frameworkElement.Width))
            {
                FrameworkElement frameworkElement1 = frameworkElement;
                renderSize = frameworkElement.RenderSize;
                double width = renderSize.Width;
                frameworkElement1.Width = width;
            }
            if (!double.IsNaN(frameworkElement.Height))
                return;
            FrameworkElement frameworkElement2 = frameworkElement;
            renderSize = frameworkElement.RenderSize;
            double height = renderSize.Height;
            frameworkElement2.Height = height;
        }

        private Thumb GetResizeThumb(Cursor cur, HorizontalAlignment hor, VerticalAlignment ver)
        {
            Thumb thumb = new Thumb();
            if (this._isRatioScale && (hor == HorizontalAlignment.Left && ver == VerticalAlignment.Center || hor == HorizontalAlignment.Center && ver == VerticalAlignment.Top || hor == HorizontalAlignment.Right && ver == VerticalAlignment.Center || hor == HorizontalAlignment.Center && ver == VerticalAlignment.Bottom))
            {
                Thumb thumb1 = new Thumb();
                thumb1.HorizontalAlignment = hor == HorizontalAlignment.Center ? HorizontalAlignment.Stretch : hor;
                thumb1.VerticalAlignment = ver == VerticalAlignment.Center ? VerticalAlignment.Stretch : ver;
                thumb1.Cursor = cur;
                ControlTemplate controlTemplate = new ControlTemplate(typeof(Thumb));
                controlTemplate.VisualTree = this.GetFactoryRectangle();
                thumb1.Template = controlTemplate;
                thumb = thumb1;
            }
            else
            {
                Thumb thumb2 = new Thumb();
                thumb2.Width = 15.0;
                thumb2.Height = 15.0;
                thumb2.HorizontalAlignment = hor;
                thumb2.VerticalAlignment = ver;
                thumb2.Cursor = cur;
                ControlTemplate controlTemplate = new ControlTemplate(typeof(Thumb));
                controlTemplate.VisualTree = this.GetFactory((Brush)new SolidColorBrush(Colors.White));
                thumb2.Template = controlTemplate;
                thumb = thumb2;
            }
            if (!double.IsNaN(this.canvas.Width))
            {
                double width = this.canvas.Width;
            }
            else
            {
                double actualWidth = this.canvas.ActualWidth;
            }
            if (!double.IsNaN(this.canvas.Height))
            {
                double height = this.canvas.Height;
            }
            else
            {
                double actualHeight = this.canvas.ActualHeight;
            }
            thumb.DragDelta += (DragDeltaEventHandler)((s, e) =>
            {
                if (!(this.AdornedElement is FrameworkElement adornedElement2))
                    return;
                this.Resize(adornedElement2);
                switch (thumb.VerticalAlignment)
                {
                    case VerticalAlignment.Top:
                        if (adornedElement2.Height - e.VerticalChange > 20.0)
                        {
                            double newHeight = adornedElement2.Height - e.VerticalChange;
                            double top = Canvas.GetTop((UIElement)adornedElement2);
                            if (newHeight > 0.0 && top + e.VerticalChange >= 0.0)
                            {
                                adornedElement2.Height = newHeight;
                                Canvas.SetTop((UIElement)adornedElement2, top + e.VerticalChange);
                                if (this._isRatioScale)
                                {
                                    this.ScaleWidth(thumb, adornedElement2, newHeight);
                                    break;
                                }
                                break;
                            }
                            break;
                        }
                        break;
                    case VerticalAlignment.Bottom:
                        if (adornedElement2.Height + e.VerticalChange > 20.0)
                        {
                            double newHeight = adornedElement2.Height + e.VerticalChange;
                            double num = Canvas.GetTop((UIElement)adornedElement2) + newHeight;
                            if (newHeight > 0.0 && num <= this.canvas.ActualHeight)
                            {
                                adornedElement2.Height = newHeight;
                                if (this._isRatioScale)
                                {
                                    this.ScaleWidth(thumb, adornedElement2, newHeight);
                                    break;
                                }
                                break;
                            }
                            break;
                        }
                        break;
                }
                switch (thumb.HorizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        if (adornedElement2.Width - e.HorizontalChange > 20.0)
                        {
                            double newWidth = adornedElement2.Width - e.HorizontalChange;
                            double left = Canvas.GetLeft((UIElement)adornedElement2);
                            if (newWidth > 0.0 && left + e.HorizontalChange >= 0.0)
                            {
                                adornedElement2.Width = newWidth;
                                Canvas.SetLeft((UIElement)adornedElement2, left + e.HorizontalChange);
                                if (this._isRatioScale)
                                {
                                    this.ScaleHeight(thumb, adornedElement2, newWidth);
                                    break;
                                }
                                break;
                            }
                            break;
                        }
                        break;
                    case HorizontalAlignment.Right:
                        if (adornedElement2.Width + e.HorizontalChange > 20.0)
                        {
                            double newWidth = adornedElement2.Width + e.HorizontalChange;
                            double num = Canvas.GetLeft((UIElement)adornedElement2) + newWidth;
                            if (newWidth > 0.0 && num <= this.canvas.ActualWidth)
                            {
                                adornedElement2.Width = newWidth;
                                if (this._isRatioScale)
                                {
                                    this.ScaleHeight(thumb, adornedElement2, newWidth);
                                    break;
                                }
                                break;
                            }
                            break;
                        }
                        break;
                }
                e.Handled = true;
            });
            return thumb;
        }

        private void ScaleWidth(Thumb thumb, FrameworkElement element, double newHeight)
        {
            if ((!this._isRatioScale || thumb.VerticalAlignment == VerticalAlignment.Top) && (thumb.HorizontalAlignment == HorizontalAlignment.Left || thumb.HorizontalAlignment == HorizontalAlignment.Right) || this._scaleSize.IsEmpty || this._scaleSize.Width <= double.MinValue || this._scaleSize.Height <= double.MinValue)
                return;
            double num1 = this._scaleSize.Width * newHeight;
            double num2 = Canvas.GetLeft((UIElement)element) + num1;
            if (num1 <= 0.0 || num2 > this.canvas.ActualWidth)
                return;
            element.Width = num1;
        }

        private void ScaleHeight(Thumb thumb, FrameworkElement element, double newWidth)
        {
            if ((!this._isRatioScale || thumb.VerticalAlignment == VerticalAlignment.Top) && (thumb.HorizontalAlignment == HorizontalAlignment.Left || thumb.HorizontalAlignment == HorizontalAlignment.Right) || this._scaleSize.IsEmpty || this._scaleSize.Width <= double.MinValue || this._scaleSize.Height <= double.MinValue)
                return;
            double num1 = newWidth / this._scaleSize.Width;
            double num2 = Canvas.GetTop((UIElement)element) + num1;
            if (num1 <= 0.0 || num2 > this.canvas.ActualHeight)
                return;
            element.Height = num1;
        }

        private FrameworkElementFactory GetFactoryRectangle()
        {
            FrameworkElementFactory factoryRectangle = new FrameworkElementFactory(typeof(Rectangle));
            factoryRectangle.SetValue(Shape.FillProperty, (object)Brushes.Transparent);
            return factoryRectangle;
        }

        private FrameworkElementFactory GetFactory(Brush back)
        {
            FrameworkElementFactory factory = new FrameworkElementFactory(typeof(Ellipse));
            factory.SetValue(Shape.FillProperty, (object)back);
            factory.SetValue(Shape.StrokeProperty, (object)ControlsHelper.PrimaryNormalBrush);
            factory.SetValue(Shape.StrokeThicknessProperty, (object)2.0);
            return factory;
        }

        protected override Visual GetVisualChild(int index) => this.visCollec[index];
    }
}
