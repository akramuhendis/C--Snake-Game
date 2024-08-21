namespace snake_game
{
    public partial class Form1 : Form
    {
        private const int tileSize = 20; // Her bir h�crenin (tile) boyutu
        private const int gridSize = 50; // Grid boyutu (50x50)
        private List<Point> snake = new List<Point>(); // Y�lan�n v�cut segmentlerini tutan liste
        private Point food; // Yiyecek i�in konum
        private int score = 0; // Oyuncunun puan�
        private bool gameOver = false; // Oyun bitti mi?
        private Direction direction = Direction.Right; // Y�lan�n hareket y�n�
        private Random random = new Random(); // Rastgele say� �reteci

        public Form1()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedDialog; // Pencere kenar �ubu�u yok
            this.MaximizeBox = false; // Pencereyi b�y�tme butonu yok
            this.StartPosition = FormStartPosition.CenterScreen; // Pencere ekran ortas�nda a��l�r
            this.Text = "Snake Game"; // Pencere ba�l���
            this.BackColor = Color.White; // Arka plan rengi
            this.Paint += Form1_Paint; // Paint olay�na Form1_Paint y�ntemini ekle
            this.KeyDown += Form1_KeyDown; // Klavye bas�m olay�na Form1_KeyDown y�ntemini ekle
            this.timer1.Interval = 200; // Zamanlay�c� intervali (ms cinsinden)
            this.timer1.Tick += Timer1_Tick; // Zamanlay�c� tetiklendi�inde Timer1_Tick y�ntemini �a��r
            this.Size = new Size(gridSize * tileSize + 16, gridSize * tileSize + 39); // Form boyutunu ayarla
            StartGame(); // Oyunu ba�lat
        }

        // Oyunu ba�lat
        private void StartGame()
        {
            snake.Clear(); // Y�lan�n segment listesini temizle
            snake.Add(new Point(10, 10)); // Y�lan�n ba�lang�� noktas�
            GenerateFood(); // Yiyecek �ret
            score = 0; // Puan� s�f�rla
            gameOver = false; // Oyun bitmedi
            direction = Direction.Right; // Y�lan�n ba�lang�� y�n�
            timer1.Start(); // Zamanlay�c�y� ba�lat
        }

        // Yeni yiyecek konumu �ret
        private void GenerateFood()
        {
            food = new Point(random.Next(0, gridSize), random.Next(0, gridSize)); // Rastgele bir yiyecek konumu se�
        }

        // Formun Paint olay�nda �a�r�l�r
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics; // �izim i�lemleri i�in Graphics nesnesini al

            if (!gameOver)
            {
                // Y�lan� �iz
                for (int i = 0; i < snake.Count; i++)
                {
                    Brush snakeColor = i == 0 ? Brushes.BlueViolet : Brushes.Violet; // Y�lan�n ba�� farkl� renkte
                    canvas.FillRectangle(snakeColor, new Rectangle(snake[i].X * tileSize, snake[i].Y * tileSize, tileSize, tileSize));
                }

                // Yiyece�i �iz
                canvas.FillRectangle(Brushes.Pink, new Rectangle(food.X * tileSize, food.Y * tileSize, tileSize, tileSize));
            }
            else
            {
                // Oyun bitti�inde ekranda "Game Over" mesaj�n� g�ster
                string gameOverText = "Game Over \nYour score is: " + score;
                canvas.DrawString(gameOverText, new Font("Arial", 20), Brushes.White, new PointF(Width / 2 - 100, Height / 2 - 50));
            }
        }

        // Klavye tu�lar�na bas�ld���nda �a�r�l�r
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (direction != Direction.Down)
                        direction = Direction.Up; // Y�lan yukar� hareket eder
                    break;
                case Keys.Down:
                    if (direction != Direction.Up)
                        direction = Direction.Down; // Y�lan a�a�� hareket eder
                    break;
                case Keys.Left:
                    if (direction != Direction.Right)
                        direction = Direction.Left; // Y�lan sola hareket eder
                    break;
                case Keys.Right:
                    if (direction != Direction.Left)
                        direction = Direction.Right; // Y�lan sa�a hareket eder
                    break;
            }
        }

        // Zamanlay�c� her tetiklendi�inde �a�r�l�r
        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (!gameOver)
            {
                // Y�lan�n t�m segmentlerini g�ncelle
                for (int i = snake.Count - 1; i >= 0; i--)
                {
                    if (i == 0)
                    {
                        // Y�lan�n ba��n� hareket ettir
                        switch (direction)
                        {
                            case Direction.Up:
                                snake[i] = new Point(snake[i].X, snake[i].Y - 1);
                                break;
                            case Direction.Down:
                                snake[i] = new Point(snake[i].X, snake[i].Y + 1);
                                break;
                            case Direction.Left:
                                snake[i] = new Point(snake[i].X - 1, snake[i].Y);
                                break;
                            case Direction.Right:
                                snake[i] = new Point(snake[i].X + 1, snake[i].Y);
                                break;
                        }

                        // Oyun bitti mi kontrol et
                        if (snake[i].X < 0 || snake[i].X >= gridSize || snake[i].Y < 0 || snake[i].Y >= gridSize)
                        {
                            EndGame(); // Ekran d���na ��karsa oyunu bitir
                            return;
                        }

                        // Kendine �arpma kontrol�
                        for (int j = 1; j < snake.Count; j++)
                        {
                            if (snake[i].Equals(snake[j]))
                            {
                                EndGame(); // Kendine �arpt�ysa oyunu bitir
                                return;
                            }
                        }

                        // Yiyece�e �arpma kontrol�
                        if (snake[i].Equals(food))
                        {
                            score++; // Puan� art�r
                            snake.Add(snake[snake.Count - 1]); // Y�lan�n boyutunu art�r
                            GenerateFood(); // Yeni yiyecek �ret
                        }
                    }
                    else
                    {
                        // Y�lan�n g�vde segmentlerini hareket ettir
                        snake[i] = snake[i - 1];
                    }
                }

                Invalidate(); // Formun yeniden �izilmesini sa�la
            }
        }

        // Oyunu bitir
        private void EndGame()
        {
            gameOver = true; // Oyunu bitir
            timer1.Stop(); // Zamanlay�c�y� durdur
            MessageBox.Show("Game Over! Your score is: " + score, "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information); // "Game Over" mesaj�n� g�ster

            // Yeni oyun oynamak isteyip istemedi�ini sor
            DialogResult result = MessageBox.Show("Would you like to play again?", "Play Again?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                StartGame(); // Yeni oyunu ba�lat
            }
            else
            {
                this.Close(); // Formu kapat
            }
        }

        // Y�n enumu
        enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }
    }
}
