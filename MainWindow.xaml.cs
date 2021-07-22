using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SnakeGame
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int SnakeSquareSize = 10;
        int SnakeSpeed = 150;
        int Score = 0;
        bool locked = false;

        private List<Rectangle> snakeParts = new List<Rectangle>();
        Ellipse Food = new Ellipse();

        public enum SnakeDirection { Left, Right, Up, Down};
        private SnakeDirection snakeDirection = SnakeDirection.Right;

        private DispatcherTimer gameTimer = new DispatcherTimer();
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StartGame();
        }

        private void StartGame()
        {
            var p = new Point();
            p.X = Map.Width / 2;
            p.Y = Map.Height / 2;
            CreateTail(p);
            CreateFood();

            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Interval = TimeSpan.FromMilliseconds(SnakeSpeed);
            gameTimer.IsEnabled = true;
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            Move();
            locked = false;
        }

        private void Move()
        {
            var previous = new Point();
            previous.X = Canvas.GetLeft(snakeParts[0]);
            previous.Y = Canvas.GetTop(snakeParts[0]);
            var buffer = new Point();

            if(Canvas.GetLeft(snakeParts[0]) == Canvas.GetLeft(Food) && Canvas.GetTop(snakeParts[0]) == Canvas.GetTop(Food))
            {
                Eat();
            }

            if(snakeDirection == SnakeDirection.Right)
            {
                foreach (var part in snakeParts)
                    Map.Children.Remove(part);

                if(previous.X + SnakeSquareSize >= Map.Width)
                {
                    Canvas.SetLeft(snakeParts[0], 0);
                }
                else
                {
                    Canvas.SetLeft(snakeParts[0], previous.X + SnakeSquareSize);
                }

                for(int i = 1; i < snakeParts.Count; i++)
                {
                    buffer.X = Canvas.GetLeft(snakeParts[i]);
                    buffer.Y = Canvas.GetTop(snakeParts[i]);
                    Canvas.SetLeft(snakeParts[i], previous.X);
                    Canvas.SetTop(snakeParts[i], previous.Y);
                    previous.X = buffer.X;
                    previous.Y = buffer.Y;
                }

                foreach (var part in snakeParts)
                    Map.Children.Add(part);

            }else if(snakeDirection == SnakeDirection.Left)
            {
                foreach (var part in snakeParts)
                    Map.Children.Remove(part);

                if (previous.X - SnakeSquareSize < 0)
                {
                    Canvas.SetLeft(snakeParts[0], Map.Width - 10);
                }
                else
                {
                    Canvas.SetLeft(snakeParts[0], previous.X - SnakeSquareSize);
                }

                for (int i = 1; i < snakeParts.Count; i++)
                {
                    buffer.X = Canvas.GetLeft(snakeParts[i]);
                    buffer.Y = Canvas.GetTop(snakeParts[i]);
                    Canvas.SetLeft(snakeParts[i], previous.X);
                    Canvas.SetTop(snakeParts[i], previous.Y);
                    previous.X = buffer.X;
                    previous.Y = buffer.Y;
                }

                foreach (var part in snakeParts)
                    Map.Children.Add(part);
            }else if(snakeDirection == SnakeDirection.Up)
            {
                foreach (var part in snakeParts)
                    Map.Children.Remove(part);

                if (previous.Y - SnakeSquareSize < 0)
                {
                    Canvas.SetTop(snakeParts[0], Map.Height - 10);
                }
                else
                {
                    Canvas.SetTop(snakeParts[0], previous.Y - SnakeSquareSize);
                }

                for (int i = 1; i < snakeParts.Count; i++)
                {
                    buffer.X = Canvas.GetLeft(snakeParts[i]);
                    buffer.Y = Canvas.GetTop(snakeParts[i]);
                    Canvas.SetTop(snakeParts[i], previous.Y);
                    Canvas.SetLeft(snakeParts[i], previous.X);
                    previous.Y = buffer.Y;
                    previous.X = buffer.X;
                }

                foreach (var part in snakeParts)
                    Map.Children.Add(part);
            }else if(snakeDirection == SnakeDirection.Down)
            {
                foreach (var part in snakeParts)
                    Map.Children.Remove(part);

                if (previous.Y + SnakeSquareSize >= Map.Height)
                {
                    Canvas.SetTop(snakeParts[0], 0);
                }
                else
                {
                    Canvas.SetTop(snakeParts[0], previous.Y + SnakeSquareSize);
                }

                for (int i = 1; i < snakeParts.Count; i++)
                {
                    buffer.X = Canvas.GetLeft(snakeParts[i]);
                    buffer.Y = Canvas.GetTop(snakeParts[i]);
                    Canvas.SetTop(snakeParts[i], previous.Y);
                    Canvas.SetLeft(snakeParts[i], previous.X);
                    previous.Y = buffer.Y;
                    previous.X = buffer.X;
                }

                foreach (var part in snakeParts)
                    Map.Children.Add(part);
            }

            for (int i = 1; i < snakeParts.Count; i++)
            {
                if (Canvas.GetLeft(snakeParts[0]) == Canvas.GetLeft(snakeParts[i]) && Canvas.GetTop(snakeParts[0]) == Canvas.GetTop(snakeParts[i]))
                {
                    gameTimer.Stop();
                    var textBlock = new TextBlock()
                    {
                        Text = "Game Over!",
                        Width = 300,
                        Height = 72,
                        TextAlignment = TextAlignment.Center,
                        Foreground = Brushes.White,
                        FontSize = 48,
                    };
                    Canvas.SetTop(textBlock, (int)Map.Height / 2 - 36);
                    Canvas.SetLeft(textBlock, (int)Map.Width / 2 - 150);
                    Map.Children.Add(textBlock);
                }
            }
        }

        private void CreateTail(Point point)
        {
            var rect = new Rectangle()
            {
                Height = SnakeSquareSize,
                Width = SnakeSquareSize,
                Fill = Brushes.Green
            };
            Canvas.SetTop(rect, point.Y);
            Canvas.SetLeft(rect, point.X);
            snakeParts.Add(rect);
            Map.Children.Add(rect);
        }

        private void CreateFood()
        {
            var food = new Ellipse()
            {
                Height = 10,
                Width = 10,
                Fill = Brushes.Red
            };
            Food = food;
            var rndX = new Random();
            var rndY = new Random();
            Canvas.SetLeft(food, rndX.Next(((int)Map.Width - 10) / 10) * 10);
            Canvas.SetTop(food, rndY.Next(((int)Map.Height - 10) / 10) * 10);
            Map.Children.Add(food);
        }

        private void Eat()
        {
            Map.Children.Remove(Food);
            var p = new Point();
            p.X = Canvas.GetLeft(snakeParts[snakeParts.Count - 1]);
            p.Y = Canvas.GetTop(snakeParts[snakeParts.Count - 1]);
            if(snakeDirection == SnakeDirection.Up)
            {
                p.Y += SnakeSquareSize;
                CreateTail(p);
            }else if(snakeDirection == SnakeDirection.Down)
            {
                p.Y -= SnakeSquareSize;
                CreateTail(p);
            }else if(snakeDirection == SnakeDirection.Right)
            {
                p.X -= SnakeSquareSize;
                CreateTail(p);
            }else if(snakeDirection == SnakeDirection.Left)
            {
                p.X += SnakeSquareSize;
                CreateTail(p);
            }
            CreateFood();
            Score++;
            Window.Title = $"SnakeGame - Score: {Score}";
        }

        private void KeyMove(object sender, KeyEventArgs e)
        {
            if(locked != true)
            {
                if (e.Key == Key.Down)
                {
                    if (snakeDirection != SnakeDirection.Up)
                    {
                        snakeDirection = SnakeDirection.Down;
                    }
                }
                else if (e.Key == Key.Up)
                {
                    if (snakeDirection != SnakeDirection.Down)
                    {
                        snakeDirection = SnakeDirection.Up;
                    }
                }
                else if (e.Key == Key.Right)
                {
                    if (snakeDirection != SnakeDirection.Left)
                    {
                        snakeDirection = SnakeDirection.Right;
                    }
                }
                else if (e.Key == Key.Left)
                {
                    if (snakeDirection != SnakeDirection.Right)
                    {
                        snakeDirection = SnakeDirection.Left;
                    }
                }

                locked = true;
            }
        }
    }
}
