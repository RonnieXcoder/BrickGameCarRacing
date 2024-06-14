using ABI.Windows.Foundation;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.System;
using Windows.UI.StartScreen;

// If you enjoy this project, you can support it by making a donation!
// Donation link: https://buymeacoffee.com/_ronniexcoder
// You can also visit my YouTube channel for more content: https://www.youtube.com/@ronniexcoder

namespace BrickGameCarRacing
{
   
    public sealed partial class MainWindow : Window
    {
        private const int TileSize = 40;
        private const int GridWidth = 10;
        private const int GridHeight = 20;
        private bool isGameInProgress = false;
        private int Score = 0;
        private DispatcherTimer timer;
        private MediaPlayer SoundPlayer = new MediaPlayer();
        public enum Direction
        {
            Left,
            Right
        }

        private const int StartPosition = -5;
        private const int LeftPosition = 2;
        private const int RightPosition = 5;

        private int GuardrailX4 = 4;
        private int GuardrailX8 = 8;
        private int GuardrailX14 = 14;
        private int GuardrailX18 = 18;
        private int GuardrailX3 = 3;
        private int GuardrailX9 = 9;
        private int GuardrailX13 = 13;
        private int GuardrailX19 = 19;

        private Direction currentDirection = Direction.Left;

        private int YPositionCar1 = -1;
        private int YPositionCar2 = -10;
        private int YPositionCar3 = -19;
        private int XPositionCar1 = 2;
        private int XPositionCar2 = 2;
        private int XPositionCar3 = 2;

        Random random = new Random();
        public MainWindow()
        {
            this.InitializeComponent();

            int randomValue = random.Next(2);
            if (randomValue == 0) XPositionCar1 = 2; else XPositionCar1 = 5;

            randomValue = random.Next(2);
            if (randomValue == 0) XPositionCar2 = 2; else XPositionCar2 = 5;

            randomValue = random.Next(2);
            if (randomValue == 0) XPositionCar3 = 2; else XPositionCar3 = 5;

            InitializeTimer();

        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {

            GameCanvas.Children.Clear();

            DrawGrid();
            DrawObstacles();

            DrawRoadGuardrail();

            if (isGameInProgress)
            {
                CheckCollision();
                Score += 1;
                GameScore.Text = "Score: " + Score.ToString();

            }
        }

        private void DrawGrid()
        {

            for (int x = 0; x < GridWidth; x++)
            {
                for (int y = 0; y < GridHeight; y++)
                {


                    Rectangle rect = new Rectangle
                    {
                        Width = TileSize,
                        Height = TileSize,
                        Stroke = new SolidColorBrush(Colors.Beige),
                        StrokeThickness = 1
                    };

                    Rectangle innerRect1 = new Rectangle
                    {
                        Width = TileSize - 5,
                        Height = TileSize - 5,
                        Stroke = new SolidColorBrush(Colors.LightGray),
                        StrokeThickness = 5
                    };

                    Rectangle innerRect2 = new Rectangle
                    {
                        Width = TileSize - 25,
                        Height = TileSize - 25,
                        Fill = new SolidColorBrush(Colors.LightGray)
                    };

                    if (currentDirection == Direction.Left)   //Draw player car
                    {
                        if (((x == 2 || x == 4) && y == 19) ||
                            ((x == 2 || x == 3 || x == 4) && y == 17) ||
                            (x == 3 && (y == 18 || y == 16)))
                        {
                            innerRect1.Stroke = new SolidColorBrush(Colors.Black);
                            innerRect2.Fill = new SolidColorBrush(Colors.Black);
                        }
                    }
                    else
                    {
                        if (((x == 5 || x == 7) && y == 19) ||
                            ((x == 5 || x == 6 || x == 7) && y == 17) ||
                            (x == 6 && (y == 18 || y == 16)))
                        {
                            innerRect1.Stroke = new SolidColorBrush(Colors.Black);
                            innerRect2.Fill = new SolidColorBrush(Colors.Black);
                        }
                    }


                    Canvas.SetLeft(rect, x * TileSize);
                    Canvas.SetTop(rect, y * TileSize);

                    Canvas.SetLeft(innerRect1, x * TileSize + 2.5);
                    Canvas.SetTop(innerRect1, y * TileSize + 2.5);

                    Canvas.SetLeft(innerRect2, x * TileSize + 12.5);
                    Canvas.SetTop(innerRect2, y * TileSize + 12.5);

                    GameCanvas.Children.Add(rect);
                    GameCanvas.Children.Add(innerRect1);
                    GameCanvas.Children.Add(innerRect2);


                }

            }

        }

        private void DrawCar(int x, int y)
        {
            Rectangle innerRect1 = new Rectangle
            {
                Width = TileSize - 5,
                Height = TileSize - 5,
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 5
            };

            Rectangle innerRect2 = new Rectangle
            {
                Width = TileSize - 25,
                Height = TileSize - 25,
                Fill = new SolidColorBrush(Colors.Black)
            };


            Canvas.SetLeft(innerRect1, x * TileSize + 2.5);
            Canvas.SetTop(innerRect1, y * TileSize + 2.5);

            Canvas.SetLeft(innerRect2, x * TileSize + 12.5);
            Canvas.SetTop(innerRect2, y * TileSize + 12.5);

            GameCanvas.Children.Add(innerRect1);
            GameCanvas.Children.Add(innerRect2);
        }

        private void DrawObstacles()
        {
            if (isGameInProgress)
            {
                YPositionCar1++;
                YPositionCar2++;
                YPositionCar3++;
            }

            if (YPositionCar1 > GridHeight + 1)
            {
                YPositionCar1 = StartPosition;

                int randomValue = random.Next(2);

                if (randomValue == 0) XPositionCar1 = LeftPosition; else XPositionCar1 = RightPosition;

            }
            if (YPositionCar2 > GridHeight + 1)
            {
                YPositionCar2 = StartPosition;

                int randomValue = random.Next(2);

                if (randomValue == 0) XPositionCar2 = LeftPosition; else XPositionCar2 = RightPosition;

            }

            if (YPositionCar3 > GridHeight + 1)
            {
                YPositionCar3 = StartPosition;

                int randomValue = random.Next(2);

                if (randomValue == 0) XPositionCar3 = LeftPosition; else XPositionCar3 = RightPosition;

            }


            if ((YPositionCar1 < GridHeight) && (YPositionCar1 >= 0))
                DrawCar(XPositionCar1 + 1, YPositionCar1);

            if ((YPositionCar1 - 1 < GridHeight) && (YPositionCar1 - 1 >= 0))
            {
                DrawCar(XPositionCar1 + 1, YPositionCar1 - 1);
                DrawCar(XPositionCar1, YPositionCar1 - 1);
                DrawCar(XPositionCar1 + 2, YPositionCar1 - 1);
            }
            if ((YPositionCar1 - 2 < GridHeight) && (YPositionCar1 - 2 >= 0))
                DrawCar(XPositionCar1 + 1, YPositionCar1 - 2);

            if ((YPositionCar1 - 3 < GridHeight) && (YPositionCar1 - 3 >= 0))
            {
                DrawCar(XPositionCar1, YPositionCar1 - 3);
                DrawCar(XPositionCar1 + 2, YPositionCar1 - 3);
            }



            if ((YPositionCar2 < GridHeight) && (YPositionCar2 >= 0))
                DrawCar(XPositionCar2 + 1, YPositionCar2);

            if ((YPositionCar2 - 1 < GridHeight) && (YPositionCar2 - 1 >= 0))
            {
                DrawCar(XPositionCar2 + 1, YPositionCar2 - 1);
                DrawCar(XPositionCar2, YPositionCar2 - 1);
                DrawCar(XPositionCar2 + 2, YPositionCar2 - 1);
            }
            if ((YPositionCar2 - 2 < GridHeight) && (YPositionCar2 - 2 >= 0))
                DrawCar(XPositionCar2 + 1, YPositionCar2 - 2);

            if ((YPositionCar2 - 3 < GridHeight) && (YPositionCar2 - 3 >= 0))
            {
                DrawCar(XPositionCar2, YPositionCar2 - 3);
                DrawCar(XPositionCar2 + 2, YPositionCar2 - 3);
            }



            if ((YPositionCar3 < GridHeight) && (YPositionCar3 >= 0))
                DrawCar(XPositionCar3 + 1, YPositionCar3);

            if ((YPositionCar3 - 1 < GridHeight) && (YPositionCar3 - 1 >= 0))
            {
                DrawCar(XPositionCar3 + 1, YPositionCar3 - 1);
                DrawCar(XPositionCar3, YPositionCar3 - 1);
                DrawCar(XPositionCar3 + 2, YPositionCar3 - 1);
            }
            if ((YPositionCar3 - 2 < GridHeight) && (YPositionCar3 - 2 >= 0))
                DrawCar(XPositionCar3 + 1, YPositionCar3 - 2);

            if ((YPositionCar3 - 3 < GridHeight) && (YPositionCar3 - 3 >= 0))
            {
                DrawCar(XPositionCar3, YPositionCar3 - 3);
                DrawCar(XPositionCar3 + 2, YPositionCar3 - 3);
            }

        }

        private void DrawRoadGuardrail()
        {


            for (int x = 0; x < GridWidth; x++)
            {
                for (int y = 0; y < GridHeight; y++)
                {


                    if (y == GuardrailX4 || y == GuardrailX8 ||
                        y == GuardrailX14 || y == GuardrailX18 ||
                        y == GuardrailX3 || y == GuardrailX9 ||
                        y == GuardrailX13 || y == GuardrailX19)
                    { continue; }



                    if (x == 0 || x == 9)
                    {


                        Rectangle rect = new Rectangle
                        {
                            Width = TileSize,
                            Height = TileSize,
                            Stroke = new SolidColorBrush(Colors.Black),
                            StrokeThickness = 1
                        };

                        Rectangle innerRect1 = new Rectangle
                        {
                            Width = TileSize - 5,
                            Height = TileSize - 5,
                            Stroke = new SolidColorBrush(Colors.Black),
                            StrokeThickness = 5
                        };

                        Rectangle innerRect2 = new Rectangle
                        {
                            Width = TileSize - 25,
                            Height = TileSize - 25,
                            Fill = new SolidColorBrush(Colors.Black)
                        };



                        Canvas.SetLeft(rect, x * TileSize);
                        Canvas.SetTop(rect, y * TileSize);

                        Canvas.SetLeft(innerRect1, x * TileSize + 2.5);
                        Canvas.SetTop(innerRect1, y * TileSize + 2.5);

                        Canvas.SetLeft(innerRect2, x * TileSize + 12.5);
                        Canvas.SetTop(innerRect2, y * TileSize + 12.5);

                        GameCanvas.Children.Add(rect);
                        GameCanvas.Children.Add(innerRect1);
                        GameCanvas.Children.Add(innerRect2);
                    }
                }


            }

            if (isGameInProgress)
            {
                GuardrailX4 = (GuardrailX4 > 18) ? 0 : GuardrailX4 + 1;
                GuardrailX8 = (GuardrailX8 > 18) ? 0 : GuardrailX8 + 1;
                GuardrailX14 = (GuardrailX14 > 18) ? 0 : GuardrailX14 + 1;
                GuardrailX18 = (GuardrailX18 > 18) ? 0 : GuardrailX18 + 1;
                GuardrailX3 = (GuardrailX3 > 18) ? 0 : GuardrailX3 + 1;
                GuardrailX9 = (GuardrailX9 > 18) ? 0 : GuardrailX9 + 1;
                GuardrailX13 = (GuardrailX13 > 18) ? 0 : GuardrailX13 + 1;
                GuardrailX19 = (GuardrailX19 > 18) ? 0 : GuardrailX19 + 1;
            }


        }

        private void CheckCollision()
        {
            if (isGameInProgress)
            {
                if (currentDirection == Direction.Left)
                {
                    if ((YPositionCar1 >= 16 && YPositionCar1 < 20 && XPositionCar1 == LeftPosition) ||
                        (YPositionCar2 >= 16 && YPositionCar2 < 20 && XPositionCar2 == LeftPosition) ||
                        (YPositionCar3 >= 16 && YPositionCar3 < 20 && XPositionCar3 == LeftPosition)) GameOver();

                }
                else
                {
                    if ((YPositionCar1 >= 16 && YPositionCar1 < 20 && XPositionCar1 == RightPosition) ||
                        (YPositionCar2 >= 16 && YPositionCar2 < 20 && XPositionCar2 == RightPosition) ||
                        (YPositionCar3 >= 16 && YPositionCar3 < 20 && XPositionCar3 == RightPosition)) GameOver();
                }
            }
        }

        private async void GameOver()
        {
            isGameInProgress = false;
            SoundPlayer.Pause();
            SoundPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/game-over.mp3"));
            SoundPlayer.IsLoopingEnabled = false;
            SoundPlayer.Volume = 0.5;
            SoundPlayer.Play();
            ButtonStart.Content = "Start";
            await CollisionAnimation();
            YPositionCar1 = -1;
            YPositionCar2 = -10;
            YPositionCar3 = -19;
            Score = 0;
            GameScore.Text = "Score: 0";
        }


        private async Task CollisionAnimation()
        {


            Rectangle innerRect1 = new Rectangle
            {
                Width = TileSize * 3 - 5,
                Height = TileSize * 4 - 5,
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 5
            };

            Rectangle innerRect2 = new Rectangle
            {
                Width = TileSize * 3 - 25,
                Height = TileSize * 4 - 25,
                Fill = new SolidColorBrush(Colors.Black)
            };

            if (currentDirection == Direction.Left)
            {
                Canvas.SetLeft(innerRect1, LeftPosition * TileSize + 2.5);
                Canvas.SetTop(innerRect1, 16 * TileSize + 2.5);

                Canvas.SetLeft(innerRect2, LeftPosition * TileSize + 12.5);
                Canvas.SetTop(innerRect2, 16 * TileSize + 12.5);
            }
            else
            {
                Canvas.SetLeft(innerRect1, RightPosition * TileSize + 2.5);
                Canvas.SetTop(innerRect1, 16 * TileSize + 2.5);

                Canvas.SetLeft(innerRect2, RightPosition * TileSize + 12.5);
                Canvas.SetTop(innerRect2, 16 * TileSize + 12.5);
            }

            for (int i = 0; i < 3; i++)
            {
                GameCanvas.Children.Add(innerRect1);
                GameCanvas.Children.Add(innerRect2);

                await Task.Delay(TimeSpan.FromSeconds(1));

                GameCanvas.Children.Remove(innerRect1);
                GameCanvas.Children.Remove(innerRect2);
            }



        }

        private void GameCanvas_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (!isGameInProgress) return;

            switch (e.Key)
            {
                case VirtualKey.A:
                    currentDirection = Direction.Left;
                    break;
                case VirtualKey.D:
                    currentDirection = Direction.Right;
                    break;
            }
        }

        private void GameCanvas_LostFocus(object sender, RoutedEventArgs e)
        {
            GameCanvas.Focus(FocusState.Pointer);
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            if (!isGameInProgress)
            {
                isGameInProgress = true;
                ButtonStart.Content = "Stop";
                SoundPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/car-sound.mp3"));
                SoundPlayer.Volume = 0.2;
                SoundPlayer.IsLoopingEnabled = true;
                SoundPlayer.Play();
            }
            else
            {
                isGameInProgress = false;
                ButtonStart.Content = "Start";
                SoundPlayer.Pause();
            }
        }

        // If you enjoy this project, you can support it by making a donation!
        // Donation link: https://buymeacoffee.com/_ronniexcoder
        // You can also visit my YouTube channel for more content: https://www.youtube.com/@ronniexcoder
    
   }
}
