using System.Diagnostics;

namespace TowardAgarioStepOne
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        private bool initialized;
        WorldModel model;
        WorldDrawable draw;
        Timer timer;

        public MainPage()
        {
            InitializeComponent();

            initialized = false;
        }

        /// <summary>
        ///    Called when the window is resized.  
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            Debug.WriteLine($"OnSizeAllocated {width} {height}");

            if (!initialized)
            {
                initialized = true;
                InitializeGameLogic();
            }
        }
        private void InitializeGameLogic()
        {
            model = new();
            draw = new(model);
            
            //change last timer parameter to change framerate
            timer = new(new TimerCallback(GameStep), null, 0, 33);

            PlaySurface.Drawable = draw;
        }

        private void GameStep(object state)
        {
            draw.Model.AdvanceGameOneStep();

            PlaySurface.Invalidate();

            Dispatcher.Dispatch(() =>
            {
                CircleCenter.Text = $"{draw.Model.X}, {draw.Model.Y}";
                Direction.Text = $"{draw.Model.GetDirection()}";
            });
        }
    }
}