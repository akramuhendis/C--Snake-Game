namespace snake_game
{
    public partial class Form1 : Form
    {
        private const int tileSize = 20; // Her bir hücrenin (tile) boyutu
        private const int gridSize = 50; // Grid boyutu (50x50)
        private List<Point> snake = new List<Point>(); // Yýlanýn vücut segmentlerini tutan liste
        private Point food; // Yiyecek için konum
        private int score = 0; // Oyuncunun puaný
        private bool gameOver = false; // Oyun bitti mi?
        private Direction direction = Direction.Right; // Yýlanýn hareket yönü
        private Random random = new Random(); // Rastgele sayý üreteci

        public Form1()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedDialog; // Pencere kenar çubuðu yok
            this.MaximizeBox = false; // Pencereyi büyütme butonu yok
            this.StartPosition = FormStartPosition.CenterScreen; // Pencere ekran ortasýnda açýlýr
            this.Text = "Snake Game"; // Pencere baþlýðý
            this.BackColor = Color.White; // Arka plan rengi
            this.Paint += Form1_Paint; // Paint olayýna Form1_Paint yöntemini ekle
            this.KeyDown += Form1_KeyDown; // Klavye basým olayýna Form1_KeyDown yöntemini ekle
            this.timer1.Interval = 200; // Zamanlayýcý intervali (ms cinsinden)
            this.timer1.Tick += Timer1_Tick; // Zamanlayýcý tetiklendiðinde Timer1_Tick yöntemini çaðýr
            this.Size = new Size(gridSize * tileSize + 16, gridSize * tileSize + 39); // Form boyutunu ayarla
            StartGame(); // Oyunu baþlat
        }

        // Oyunu baþlat
        private void StartGame()
        {
            snake.Clear(); // Yýlanýn segment listesini temizle
            snake.Add(new Point(10, 10)); // Yýlanýn baþlangýç noktasý
            GenerateFood(); // Yiyecek üret
            score = 0; // Puaný sýfýrla
            gameOver = false; // Oyun bitmedi
            direction = Direction.Right; // Yýlanýn baþlangýç yönü
            timer1.Start(); // Zamanlayýcýyý baþlat
        }

        // Yeni yiyecek konumu üret
        private void GenerateFood()
        {
            food = new Point(random.Next(0, gridSize), random.Next(0, gridSize)); // Rastgele bir yiyecek konumu seç
        }

        // Formun Paint olayýnda çaðrýlýr
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics; // Çizim iþlemleri için Graphics nesnesini al

            if (!gameOver)
            {
                // Yýlaný çiz
                for (int i = 0; i < snake.Count; i++)
                {
                    Brush snakeColor = i == 0 ? Brushes.BlueViolet : Brushes.Violet; // Yýlanýn baþý farklý renkte
                    canvas.FillRectangle(snakeColor, new Rectangle(snake[i].X * tileSize, snake[i].Y * tileSize, tileSize, tileSize));
                }

                // Yiyeceði çiz
                canvas.FillRectangle(Brushes.Pink, new Rectangle(food.X * tileSize, food.Y * tileSize, tileSize, tileSize));
            }
            else
            {
                // Oyun bittiðinde ekranda "Game Over" mesajýný göster
                string gameOverText = "Game Over \nYour score is: " + score;
                canvas.DrawString(gameOverText, new Font("Arial", 20), Brushes.White, new PointF(Width / 2 - 100, Height / 2 - 50));
            }
        }

        // Klavye tuþlarýna basýldýðýnda çaðrýlýr
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (direction != Direction.Down)
                        direction = Direction.Up; // Yýlan yukarý hareket eder
                    break;
                case Keys.Down:
                    if (direction != Direction.Up)
                        direction = Direction.Down; // Yýlan aþaðý hareket eder
                    break;
                case Keys.Left:
                    if (direction != Direction.Right)
                        direction = Direction.Left; // Yýlan sola hareket eder
                    break;
                case Keys.Right:
                    if (direction != Direction.Left)
                        direction = Direction.Right; // Yýlan saða hareket eder
                    break;
            }
        }

        // Zamanlayýcý her tetiklendiðinde çaðrýlýr
        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (!gameOver)
            {
                // Yýlanýn tüm segmentlerini güncelle
                for (int i = snake.Count - 1; i >= 0; i--)
                {
                    if (i == 0)
                    {
                        // Yýlanýn baþýný hareket ettir
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
                            EndGame(); // Ekran dýþýna çýkarsa oyunu bitir
                            return;
                        }

                        // Kendine çarpma kontrolü
                        for (int j = 1; j < snake.Count; j++)
                        {
                            if (snake[i].Equals(snake[j]))
                            {
                                EndGame(); // Kendine çarptýysa oyunu bitir
                                return;
                            }
                        }

                        // Yiyeceðe çarpma kontrolü
                        if (snake[i].Equals(food))
                        {
                            score++; // Puaný artýr
                            snake.Add(snake[snake.Count - 1]); // Yýlanýn boyutunu artýr
                            GenerateFood(); // Yeni yiyecek üret
                        }
                    }
                    else
                    {
                        // Yýlanýn gövde segmentlerini hareket ettir
                        snake[i] = snake[i - 1];
                    }
                }

                Invalidate(); // Formun yeniden çizilmesini saðla
            }
        }

        // Oyunu bitir
        private void EndGame()
        {
            gameOver = true; // Oyunu bitir
            timer1.Stop(); // Zamanlayýcýyý durdur
            MessageBox.Show("Game Over! Your score is: " + score, "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information); // "Game Over" mesajýný göster

            // Yeni oyun oynamak isteyip istemediðini sor
            DialogResult result = MessageBox.Show("Would you like to play again?", "Play Again?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                StartGame(); // Yeni oyunu baþlat
            }
            else
            {
                this.Close(); // Formu kapat
            }
        }

        // Yön enumu
        enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }
    }
}
