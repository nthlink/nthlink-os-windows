using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace nthLink.Wpf.Views
{
    /// <summary>
    /// ThemeEditorPage.xaml 的互動邏輯
    /// </summary>
    public partial class ThemeEditorPage : UserControl
    {
        private readonly ResourceDictionary? resourceDictionary;

        public ThemeEditorPage()
        {
            InitializeComponent();

            foreach (ResourceDictionary item in Application.Current.Resources.MergedDictionaries)
            {
                if (item.Source.OriginalString.Contains("dynamictheme", StringComparison.OrdinalIgnoreCase))
                {
                    this.resourceDictionary = item;
                    break;
                }
            }

            if (this.resourceDictionary != null)
            {
                if (this.resourceDictionary.Contains("BubbleInfoMargin") &&
                    this.resourceDictionary["BubbleInfoMargin"] is Thickness bubbleInfoMargin)
                {
                    PART_BubbleInfoMarginSlider.SetCurrentValue(Slider.ValueProperty, bubbleInfoMargin.Top);
                }

                if (this.resourceDictionary.Contains("BubbleInfoWidth") &&
                    this.resourceDictionary["BubbleInfoWidth"] is double bubbleInfoWidth)
                {
                    PART_BubbleInfoWidthSlider.SetCurrentValue(Slider.ValueProperty, bubbleInfoWidth);
                }

                if (this.resourceDictionary.Contains("BubbleInfoHeight") &&
                    this.resourceDictionary["BubbleInfoHeight"] is double bubbleInfoHeight)
                {
                    PART_BubbleInfoHeightSlider.SetCurrentValue(Slider.ValueProperty, bubbleInfoHeight);
                }

                if (this.resourceDictionary.Contains("BubbleInfoCornerRadius") &&
                    this.resourceDictionary["BubbleInfoCornerRadius"] is CornerRadius bubbleInfoCornerRadius)
                {
                    PART_BubbleInfoCornerRadiusSlider.SetCurrentValue(Slider.ValueProperty, bubbleInfoCornerRadius.TopLeft);
                }

                if (this.resourceDictionary.Contains("BubbleInfoArrowWidth") &&
                    this.resourceDictionary["BubbleInfoArrowWidth"] is double bubbleInfoArrowWidth)
                {
                    PART_BubbleInfoArrowWidthSlider.SetCurrentValue(Slider.ValueProperty, bubbleInfoArrowWidth);
                }

                if (this.resourceDictionary.Contains("BubbleInfoArrowHeight") &&
                    this.resourceDictionary["BubbleInfoArrowHeight"] is double bubbleInfoArrowHeight)
                {
                    PART_BubbleInfoArrowHeightSlider.SetCurrentValue(Slider.ValueProperty, bubbleInfoArrowHeight);
                }

                if (this.resourceDictionary.Contains("BubbleInfoFontSize") &&
                    this.resourceDictionary["BubbleInfoFontSize"] is double bubbleInfoFontSize)
                {
                    PART_BubbleInfoFontSizeSlider.SetCurrentValue(Slider.ValueProperty, bubbleInfoFontSize);
                }

                if (this.resourceDictionary.Contains("BubbleInfoFontForeground") &&
                    this.resourceDictionary["BubbleInfoFontForeground"] is SolidColorBrush bubbleInfoFontForeground)
                {
                    PART_BubbleInfoFontForegroundRedSlider.SetCurrentValue(Slider.ValueProperty, (double)bubbleInfoFontForeground.Color.R);
                    PART_BubbleInfoFontForegroundGreenSlider.SetCurrentValue(Slider.ValueProperty, (double)bubbleInfoFontForeground.Color.G);
                    PART_BubbleInfoFontForegroundBlueSlider.SetCurrentValue(Slider.ValueProperty, (double)bubbleInfoFontForeground.Color.B);
                }

                if (this.resourceDictionary.Contains("BubbleInfoFontWeight") &&
                    this.resourceDictionary["BubbleInfoFontWeight"] is FontWeight bubbleInfoFontWeight)
                {
                    PART_BubbleInfoFontWeightCheckBox.SetCurrentValue(CheckBox.IsCheckedProperty, bubbleInfoFontWeight == FontWeights.SemiBold);
                }
            }
        }

        protected void PART_BubbleInfoMarginSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.resourceDictionary != null)
            {
                if (this.resourceDictionary.Contains("BubbleInfoMargin"))
                {
                    this.resourceDictionary["BubbleInfoMargin"] = new Thickness(0, PART_BubbleInfoMarginSlider.Value, 0, 0);
                }
            }
        }

        protected void PART_BubbleInfoWidthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.resourceDictionary != null)
            {
                if (this.resourceDictionary.Contains("BubbleInfoWidth"))
                {
                    this.resourceDictionary["BubbleInfoWidth"] = PART_BubbleInfoWidthSlider.Value;
                }
            }
        }

        protected void PART_BubbleInfoHeightSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.resourceDictionary != null)
            {
                if (this.resourceDictionary.Contains("BubbleInfoHeight"))
                {
                    this.resourceDictionary["BubbleInfoHeight"] = PART_BubbleInfoHeightSlider.Value;
                }
            }
        }

        protected void PART_BubbleInfoCornerRadiusSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.resourceDictionary != null)
            {
                if (this.resourceDictionary.Contains("BubbleInfoCornerRadius"))
                {
                    this.resourceDictionary["BubbleInfoCornerRadius"] = new CornerRadius(PART_BubbleInfoCornerRadiusSlider.Value);
                }
            }
        }

        protected void PART_BubbleInfoArrowWidthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.resourceDictionary != null)
            {
                if (this.resourceDictionary.Contains("BubbleInfoArrowWidth"))
                {
                    this.resourceDictionary["BubbleInfoArrowWidth"] = PART_BubbleInfoArrowWidthSlider.Value;
                }
            }
        }

        protected void PART_BubbleInfoArrowHeightSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.resourceDictionary != null)
            {
                if (this.resourceDictionary.Contains("BubbleInfoArrowHeight"))
                {
                    this.resourceDictionary["BubbleInfoArrowHeight"] = PART_BubbleInfoArrowHeightSlider.Value;
                }
            }
        }

        protected void PART_BubbleInfoFontSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.resourceDictionary != null)
            {
                if (this.resourceDictionary.Contains("BubbleInfoFontSize"))
                {
                    this.resourceDictionary["BubbleInfoFontSize"] = PART_BubbleInfoFontSizeSlider.Value;
                }
            }
        }

        protected void PART_BubbleInfoFontForegroundSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.resourceDictionary != null)
            {
                if (this.resourceDictionary.Contains("BubbleInfoFontForeground"))
                {
                    this.resourceDictionary["BubbleInfoFontForeground"] =
                        new SolidColorBrush(Color.FromRgb((byte)PART_BubbleInfoFontForegroundRedSlider.Value,
                        (byte)PART_BubbleInfoFontForegroundGreenSlider.Value,
                        (byte)PART_BubbleInfoFontForegroundBlueSlider.Value));
                }
            }
        }

        protected void PART_BubbleInfoFontWeightCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (this.resourceDictionary != null)
            {
                if (this.resourceDictionary.Contains("BubbleInfoFontWeight"))
                {
                    this.resourceDictionary["BubbleInfoFontWeight"] = FontWeights.SemiBold;
                }
            }
        }

        protected void PART_BubbleInfoFontWeightCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (this.resourceDictionary != null)
            {
                if (this.resourceDictionary.Contains("BubbleInfoFontWeight"))
                {
                    this.resourceDictionary["BubbleInfoFontWeight"] = FontWeights.Normal;
                }
            }
        }
    }
}
