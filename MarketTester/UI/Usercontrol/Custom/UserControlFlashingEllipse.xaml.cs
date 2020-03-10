using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MarketTester.UI.Usercontrol.Custom
{
    /// <summary>
    /// Interaction logic for UserControlFlashingEllipse.xaml
    /// </summary>
    public partial class UserControlFlashingEllipse : UserControl, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private double foregroundWidth;
        public double ForegroundWidth
        {
            get { return foregroundWidth; }
            set
            {
                foregroundWidth = value;
                AnimationWidthMin = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ForegroundWidth)));
            }
        }
        private double foregroundHeight;
        public double ForegroundHeight
        {
            get { return foregroundHeight; }
            set
            {
                foregroundHeight = value;
                AnimationHeightMin = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ForegroundHeight)));
            }
        }

        private SolidColorBrush backgroundColor;
        public SolidColorBrush BackgroundColor
        {
            get => backgroundColor;
            set
            {
                backgroundColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(backgroundColor)));
            }
        }
        private SolidColorBrush foregroundColor;
        public SolidColorBrush ForegroundColor
        {
            get => foregroundColor;
            set
            {
                foregroundColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ForegroundColor)));
            }
        }
        private double animationWidthMin;
        public double AnimationWidthMin
        {
            get => animationWidthMin;
            set
            {
                animationWidthMin = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AnimationWidthMin)));
            }
        }
        private double animationWidthMax;
        public double AnimationWidthMax
        {
            get => animationWidthMax;
            set
            {
                animationWidthMax = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AnimationWidthMax)));
            }
        }

        private double animationHeightMin;
        public double AnimationHeightMin
        {
            get => animationHeightMin;
            set
            {
                animationHeightMin = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AnimationHeightMin)));
            }
        }

        private double animationHeightMax;
        public double AnimationHeightMax
        {
            get => animationHeightMax;
            set
            {
                animationHeightMax = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AnimationHeightMax)));
            }
        }
        private double backgroundOpacity;
        public double BackgroundOpacity
        {
            get
            {
                return backgroundOpacity;
            }
            set
            {
                if (value <= 1 && value >= 0)
                {
                    backgroundOpacity = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BackgroundOpacity)));
                }
            }
        }
        private double foregroundOpacity;
        public double ForegroundOpacity
        {
            get
            {
                return foregroundOpacity;
            }
            set
            {
                if (value <= 1 && value >= 0)
                {
                    foregroundOpacity = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ForegroundOpacity)));
                }
            }
        }
        private double animationMultiplier;
        private Duration duration;

        public double AnimationMultiplier
        {
            get
            {
                return animationMultiplier;
            }
            set
            {
                animationMultiplier = value;
                AnimationWidthMax = ForegroundWidth * value;
                AnimationHeightMax = ForegroundHeight * value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AnimationMultiplier)));
            }
        }
        public Duration Duration 
        { 
            get => duration;
            set
            {
                duration = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Duration)));
            }
        }
        public double Blur
        {
            get { return Math.Pow(AnimationMultiplier,-1) * AnimationWidthMin; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="width">Foreground ellipse width</param>
        /// <param name="height">Foreground ellipse height</param>
        /// <param name="animationDuration">animation duration in milliseconds</param>
        /// <param name="animationSpan">The ratio of the maximum width that the background flash can get to the width of the actual ellipse</param>
        public UserControlFlashingEllipse(double width, double height, int animationDuration, double animationMultiplier, SolidColorBrush foreground, SolidColorBrush background)
        {
            ForegroundWidth = width;
            ForegroundHeight = height;
            ForegroundColor = foreground;
            BackgroundColor = background;
            AnimationWidthMin = width;
            AnimationWidthMax = width * animationMultiplier;
            AnimationHeightMin = height;
            AnimationHeightMax = height * animationMultiplier;
            BackgroundOpacity = 1;
            ForegroundOpacity = 1;
            AnimationMultiplier = animationMultiplier;
            Duration = new Duration(new TimeSpan(0, 0, 0, 0, animationDuration));
            InitializeComponent();
        }


    }
}
