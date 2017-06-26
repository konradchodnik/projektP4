using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using System.Windows.Media.Animation;
using System.Windows.Threading;


namespace konrachchodnik_projektp4_gierka
{
    public partial class MainWindow : Window
    {
       
        Random random = new Random();
       
        //licznik shurikenTimer
        DispatcherTimer shurikenTimer = new DispatcherTimer();
        //licznik targetTimer
        DispatcherTimer targetTimer = new DispatcherTimer();
   
        //balonik - dotkniecie - domyslnie jest nieschwytany
        bool BalonikKolizja = false;
        //licznik shurikenow
        int shuriken = 0;
        //licznik diamentow
        int diamenty = 0;





        public MainWindow()
        {
            InitializeComponent();

            gameOverText.Text = "Włącz Gre - Start \n" +
                                 "Sterowanie Myszka\n" +
                                 "Kliknij w balonik\n" +
                                 "Zbierz 10 diamentów \n"
                                 ;

            //zdarzenie Tick gdy interwal czasowy zostanie osiagniety
            shurikenTimer.Tick += shurikenTimer_Tick;
            //predkosc
            shurikenTimer.Interval = TimeSpan.FromSeconds(3);

            targetTimer.Tick += targetTimer_Tick;
            //predkosc timera, paska czasu na dole
            targetTimer.Interval += TimeSpan.FromSeconds(.5);

        }

        //licznik progres bar i czas rozgrywki
        void targetTimer_Tick(object sender, EventArgs e)
        {
            //progresbar postepowanie razem z licznikiem
            progressBar.Value += 5;
            //if jesli wartosc progressBar bedzie wieksza lub rowna maksymalnej to koncz gre
            if (progressBar.Value >= progressBar.Maximum)
                endTheGame();
        }


        //metoda konczaca gre
        private void endTheGame()
        {


            if (!playArea.Children.Contains(gameOverText))
            {
                //zatrzymanie licznika shurikenTimer
                shurikenTimer.Stop();
                //zatrzymanie licznika targetTimer
                targetTimer.Stop();

                //wyzerowanie trafienia - ustawienie globalnej na false
                BalonikKolizja = false;

                //wyswietlenie przycisku start
                startButton.Visibility = Visibility.Visible;
                //text na koniec
                gameOverText.Text = "Koniec Gry";
                playArea.Children.Add(gameOverText);
                //wyzerowanie licznika
                diamenty = 0;
                //wyzerowanie speeda
                shurikenTimer.Interval = TimeSpan.FromSeconds(2);
                targetTimer.Interval += TimeSpan.FromSeconds(.5);
            }
        }

        //metoda obsluga zdarzen timer
        //dodanie wrogow do obszaru gry
        private void shurikenTimer_Tick(object sender, EventArgs e)
        {
            addEnemy();

        }

        //Metoda Start - Klikniecie 
        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            //uruchomienie gry
            startGame();
            //licznik przeciwn ikow na 1
            shuriken = 0;
  
            LicznikShuriken.Text = shuriken.ToString();
     
            //diamenty = 1;

            //licznik diamentow
            LicznikDiamentow.Text = diamenty.ToString();

            if (diamenty >= 10)
            {
                shurikenTimer.Interval = TimeSpan.FromSeconds(1.7);
                targetTimer.Interval += TimeSpan.FromSeconds(.3);
            }
            if (diamenty >= 20)
            {
                shurikenTimer.Interval = TimeSpan.FromSeconds(0.7);
                targetTimer.Interval += TimeSpan.FromSeconds(.2);
            }
            if (diamenty >= 30)
            {
                shurikenTimer.Interval = TimeSpan.FromSeconds(0.5);
                targetTimer.Interval += TimeSpan.FromSeconds(.1);
            }
        }

        //metoda startujaca
        private void startGame()
        {
            //balonik czyli StackPanel - ustwione na true - kolizje, trawienia
            balonik.IsHitTestVisible = true;

            //zmienna logiczna bool na false - wlaczenie gry jest ustawionie trafienie na false
            BalonikKolizja = false;

            //wyzerowany licznik progressBar
            progressBar.Value = 0;

            //znikniecie, ustawienei na collapsed przycisku Start
            startButton.Visibility = Visibility.Collapsed;

            //kolekcja  playArea.Children dodany jest obiekt StackPanel(czlowiek) i Rectangle(docelowy portal)
            playArea.Children.Clear();
            playArea.Children.Add(target);
            playArea.Children.Add(balonik);
            //obiekt enemy ustawiony na start
            shurikenTimer.Start();
            //obiekt diament ustawiony na start
            targetTimer.Start();
            //ustawienie na zero licznika wrogow
            shuriken = 0;



        }

        //metoda dodania shurikena
        private void addEnemy()
        {
            //tworze nowy obiekt typu ContentControl
            ContentControl enemy = new ContentControl();

            
            //tutaj jest uzycie obiekt.Szablon = chyba nazwa nadana jako szablon
            //ze slownika Resources pobierany jest obiekt ControlTemplate
            //Slownik Resources zwraca obiekt typu <object, object> a zatem wartosc odwolania 
            //Resources["EnemyTemplate"] jest rzutowaniem w dol na konkretny typ ControlTemplate;
            //za pomoca slowa kluczowego as
            enemy.Template = Resources["ShurikenSzablon"] as ControlTemplate;

      
            //statyczne metody canwas = "(Canvas.Left)" i "(Canvas.Top)"
            AnimateEnemy(enemy, 0, playArea.ActualWidth - 100, "(Canvas.Left)");
           
            //text.Text = myInt.ToString();
            AnimateEnemy(enemy, random.Next((int)playArea.ActualHeight - 100),
                random.Next((int)playArea.ActualHeight - 100), "(Canvas.Top)");

            //Dodaje utworzona przed chwila kontrolke wroga do kolekcji 

            playArea.Children.Add(enemy);

            //dodanie do licznika i rzutowanie na stringi
            shuriken++;
            LicznikShuriken.Text = shuriken.ToString();

            enemy.MouseEnter += enemy_MouseEnter;
        }

        //metoda dotkniecia bohatera przez wroga
        void enemy_MouseEnter(object sender, MouseEventArgs e)
        {
            if (BalonikKolizja)
                endTheGame();
        }

   
        private void AnimateEnemy(ContentControl enemy, double from, double to, string propertyToAnimate)
        {
            Storyboard storyboard = new Storyboard() { AutoReverse = true, RepeatBehavior = RepeatBehavior.Forever };
            DoubleAnimation animation = new DoubleAnimation()
            {
                From = from,
                To = to,
                Duration = new Duration(TimeSpan.FromSeconds(random.Next(2, 5))),
            };
            Storyboard.SetTarget(animation, enemy);
            Storyboard.SetTargetProperty(animation, new PropertyPath(propertyToAnimate));
            storyboard.Children.Add(animation);
            storyboard.Begin();


        }

        //metoda progresbar
        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        //metoda jak balonik zbierze diament
        private void balonik_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (shurikenTimer.IsEnabled)
            {
                BalonikKolizja = true;
                balonik.IsHitTestVisible = false;
            }
        }

        //clear metoda 
        private void metodaClear()
        {
            playArea.Children.Clear();


        }

        //metoda, gdy bohater zbierze diamenta
        private void target_MouseEnter(object sender, MouseEventArgs e)
        {
            if (targetTimer.IsEnabled && BalonikKolizja)
            {
                progressBar.Value = 0;
                diamenty++;
                LicznikDiamentow.Text = diamenty.ToString();

                Canvas.SetLeft(target, random.Next(100, (int)playArea.ActualWidth - 100));
                Canvas.SetTop(target, random.Next(100, (int)playArea.ActualHeight - 100));
                Canvas.SetLeft(balonik, random.Next(100, (int)playArea.ActualWidth - 100));
                Canvas.SetTop(balonik, random.Next(100, (int)playArea.ActualHeight - 100));
                BalonikKolizja = false;
                balonik.IsHitTestVisible = true;

                if (diamenty == 10)
                {
                    //zatrzymanie licznika shurikenTimer
                    shurikenTimer.Stop();
                    //zatrzymanie licznika targetTimer
                    targetTimer.Stop();

                    //wyzerowanie trafienia - ustawienie globalnej na false
                    BalonikKolizja = false;

                    //wyswietlenie przycisku start
                    startButton.Visibility = Visibility.Visible;
                    //text na koniec

                    playArea.Children.Clear();
                    playArea.Children.Add(target);
                    playArea.Children.Add(balonik);

                    gameOverText.Text = "Poziom 2";
                    playArea.Children.Add(gameOverText);

                }
                if (diamenty == 20)
                {
                    //zatrzymanie licznika shurikenTimer
                    shurikenTimer.Stop();
                    //zatrzymanie licznika targetTimer
                    targetTimer.Stop();

                    //wyzerowanie trafienia - ustawienie globalnej na false
                    BalonikKolizja = false;

                    //wyswietlenie przycisku start
                    startButton.Visibility = Visibility.Visible;
                    //text na koniec

                    playArea.Children.Clear();
                    playArea.Children.Add(target);
                    playArea.Children.Add(balonik);

                    gameOverText.Text = "Poziom 3";
                    playArea.Children.Add(gameOverText);

                }
                if (diamenty == 30)
                {
                    //zatrzymanie licznika shurikenTimer
                    shurikenTimer.Stop();
                    //zatrzymanie licznika targetTimer
                    targetTimer.Stop();

                    //wyzerowanie trafienia - ustawienie globalnej na false
                    BalonikKolizja = false;

                    //wyswietlenie przycisku start
                    startButton.Visibility = Visibility.Visible;
                    //text na koniec

                    playArea.Children.Clear();
                    playArea.Children.Add(target);
                    playArea.Children.Add(balonik);

                    gameOverText.Text = "Poziom Master";
                    playArea.Children.Add(gameOverText);

                }
            }
        }

        //metoda po kliknieciu myszka w bohatera
        private void playArea_MouseMove(object sender, MouseEventArgs e)
        {
            if (BalonikKolizja)
            {

                Point pointerPosition = e.GetPosition(null);
                Point relativePosition = grid.TransformToVisual(playArea).Transform(pointerPosition);
                //prowadzenie postaci myszka
                if ((Math.Abs(relativePosition.X - Canvas.GetLeft(balonik)) > balonik.ActualWidth * 3) ||
                    (Math.Abs(relativePosition.Y - Canvas.GetTop(balonik)) > balonik.ActualHeight * 3))
                {
                    BalonikKolizja = false;
                    balonik.IsHitTestVisible = true;
                }
                else
                {
                    Canvas.SetLeft(balonik, relativePosition.X - balonik.ActualWidth / 2);
                    Canvas.SetTop(balonik, relativePosition.Y - balonik.ActualHeight / 2);
                }
            }
        }

        //metoda jak juz nie jest klikniety bohater 
        private void playArea_MouseLeave(object sender, MouseEventArgs e)
        {
            if (BalonikKolizja)
                endTheGame();
        }
    }
}
