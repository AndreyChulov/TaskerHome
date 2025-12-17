using System;
using System.Numerics;
using Avalonia.Controls;
using Avalonia.Controls.Skia;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using TaskerHome.ControlViewModels;
using TaskerHome.Core.Renderers;

namespace TaskerHome.Controls
{
    public partial class BackgroundModel : UserControl
    {
        private readonly CubeRenderer _cubeRenderer = new();
        private readonly DispatcherTimer _timer;
        private readonly SKCanvasControl _canvasControl;
        private double _canvasControlWidth;
        private double _canvasControlHeight;
        private float _canvasControlAspectRatio;

        public BackgroundModel()
        {
            InitializeComponent();
            
            DataContext = new BackgroundModelViewModel();
            
            _canvasControl = this.FindControl<SKCanvasControl>("BackgroundCanvasControl") ?? 
                                throw new NullReferenceException("BackgroundCanvasControl not found");
            
            OnSizeChanged(this, new SizeChangedEventArgs(null, null));
            
            _timer = new DispatcherTimer 
            { 
                Interval = TimeSpan.FromMilliseconds(16) 
            };
            _timer.Tick += (s, e) => _canvasControl.InvalidateVisual();
            _timer.Start();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void OnDraw(object? sender, SKCanvasEventArgs e)
        {
            var canvas = e.Canvas;
            
            canvas.Clear(SKColors.Black);
            
            // Установка камеры
            var camera = Matrix4x4.CreateLookAt(
                new Vector3(0, 0, 5),
                Vector3.Zero,
                new Vector3(0, 1, 0));

            // Проекция
            var projection = Matrix4x4.CreatePerspectiveFieldOfView(
                MathF.PI / 4, 
                _canvasControlAspectRatio, 
                0.1f, 
                100f);

            // Рендер куба
            _cubeRenderer.Render(canvas, camera, projection);
        }

        private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
        {
            var size = e.NewSize;
            _canvasControlWidth = size.Width;
            _canvasControlHeight = size.Height;
            _canvasControlAspectRatio = (float)size.AspectRatio;
        }
    }
}