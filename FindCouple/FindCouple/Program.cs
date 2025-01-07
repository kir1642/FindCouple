using System;
using SFML.Learning;
using SFML.System;
using SFML.Window;


class FindCouple : Game
{
    static string[] iconsName;
    static int[] iconId;

    static int[,] cards;
    static int cardCount = 20;
    static int cardWidth = 100;
    static int cardHeight = 100;

    static int countPerLine = 5;
    static int space = 40;
    static int leftOffset = 70;
    static int topOffset = 20;

    static int openCardAmount = 0;
    static int firstOpenCardIndex = -1;
    static int secondOpenCardIndex = -1;
    static int remainingCard = cardCount;

    static string click = LoadSound("click.ogg");
    static bool mainLoop = true;
    static bool end = false;
    static bool replay = false;

    static void LoadIcons()
    {
        iconsName = new string[11];
        iconsName[0] = LoadTexture("icon_close.png");
        for (int i = 1; i < iconsName.Length; i++)
        {
            iconsName[i] = LoadTexture("icon_" + (i).ToString() + ".png");
        }
    }

    static void Shuffle(int[] arr)
    {
        Random rand = new Random();

        for (int i = arr.Length-1; i > 0; i--)
        {
            int j = rand.Next(0, i + 1);

            int tmp = arr[j];
            arr[j] = arr[i];
            arr[i] = tmp;

        }
    }

    static void InitCard()
    {
        cards = new int[cardCount, 6];
        //result init
        iconId = new int[cards.GetLength(0)];
        int id = 0;
        for (int i = 0; i < iconId.Length; i++)
        {
            // для большего количества карт, но с повторением пар
/*            if (i % 2 == 0)
            {
                id = rnd.Next(1, 11);

            }*/

            // для фиксированного по количеству набора карт, но без повторения пар
            id = i + 1;
            if (i > (iconId.Length / 2) - 1) id = i - ((iconId.Length / 2) - 1);
            if (i == iconId.Length / 2) id = 1;

            iconId[i] = id;
        }

        // этот блок кода отлсеживает повторения при генерации ранд чисел
        // и пропускает итерацию, если такой id уже есть в массиве
        // но он некорректно работает
        // я не понял до конца почему
        /*        int j = 0;
                int pos = 0;
                bool flag = false;

                while (j < iconId.Length)
                {   
                    flag = false;
                    if (j % 2 == 0)
                    {
                        id = rnd.Next(1, 11);
                        foreach (int k in iconId)
                        {
                            if (id == iconId[k]) 
                            {
                                Console.WriteLine("sovpadevich"+ iconId[k]+ " "+id + " "+k);
                                flag = true;
                                pos = k;

                            }
                        }

                    }

                    if (flag)
                    {
                        Console.WriteLine("srabotal flag");
                        continue;
                    }

                    iconId[j] = id;
                    j++;
                    Console.WriteLine("iteration "+j);
                }*/



        Shuffle(iconId);
        Shuffle(iconId);
        Shuffle(iconId);
        Shuffle(iconId);

        for (int i = 0; i < cards.GetLength(0); i++)
        {
            cards[i, 0] = 0; // state
            cards[i, 1] = (i % countPerLine) * (cardWidth + space) + leftOffset; // posX
            cards[i, 2] = (i / countPerLine) * (cardHeight + space) + topOffset; // posY
            cards[i, 3] = cardWidth; // width
            cards[i, 4] = cardHeight; // height

            cards[i, 5] = iconId[i]; // id

        }
    }

    static void DrawCards()
    {
        for (int i = 0; i < cards.GetLength(0); i++)
        {
            if (cards[i,0] == 1) // open
            {
                DrawSprite(iconsName[cards[i, 5]], cards[i, 1], cards[i, 2]);
            }

            if (cards[i, 0] == 0) // close
            {
                DrawSprite(iconsName[0], cards[i, 1], cards[i, 2]);
            }
   
        }
    }

    static int GetIndexCardByMousePosition()
    {
        for (int i = 0; i < cards.GetLength(0); i++)
        {
            if (MouseX >= cards[i,1] && MouseX <= cards[i,1] + cards[i,3] && MouseY >= cards[i, 2] && MouseY <= cards[i, 2] + cards[i, 4])
            {
                return i;
            }
        }

        return -1;
    }

    static void SetStateToAllCards(int state)
    {
        for (int i = 0; i < cards.GetLength(0); i++)
        {
            cards[i, 0] = state;
        }
    }

    static void CheckOpenCards()
    {
        if (openCardAmount == 2)
        {
            if (cards[firstOpenCardIndex, 5] == cards[secondOpenCardIndex, 5])
            {
                cards[firstOpenCardIndex, 0] = -1;
                cards[secondOpenCardIndex, 0] = -1;

                remainingCard -= 2;
            }
            else
            {
                cards[firstOpenCardIndex, 0] = 0;
                cards[secondOpenCardIndex, 0] = 0;
            }

            firstOpenCardIndex = -1;
            secondOpenCardIndex = -1;
            openCardAmount = 0;

            Delay(1000);
        }
    }

    static void ClickCardDetector()
    {
        //Console.WriteLine(GetIndexCardByMousePosition());

        if (GetMouseButtonDown(0) == true)
        {
            int index = GetIndexCardByMousePosition();
            if (index != -1) PlaySound(click, 80);

            if (index != -1 && index != firstOpenCardIndex)
            {
                cards[index, 0] = 1;
                openCardAmount++;

                if (openCardAmount == 1) firstOpenCardIndex = index;
                if (openCardAmount == 2) secondOpenCardIndex = index;
            }
        }
    }

    static void Main(string[] args)
    {


        LoadIcons();

        InitWindow(800, 600, "FindCouple");

        SetFont("caviar-dreams.ttf");

        InitCard();

        for (int i = 0;i < cards.GetLength(0); i++)
        {
            Console.Write(cards[i, 5]+" ");
        }

        SetStateToAllCards(1);
        
        DrawCards();
        DisplayWindow();
        Delay(4000); // пауза для запоминания

        SetStateToAllCards(0);

        Clock clock = new Clock();
        Time time = new Time();
        string timeStr = "";

        while (true)
        {   
            
            if (replay)
            {
                //InitCard();
                Shuffle(iconId);
                for (int i = 0; i < cards.GetLength(0); i++)
                {
                    cards[i, 5] = iconId[i];
                }

                    ClearWindow();
                for (int i = 0; i < cards.GetLength(0); i++)
                {
                    Console.Write(cards[i, 5] + " ");
                }

                SetStateToAllCards(1);

                DrawCards();
                DisplayWindow();
                Delay(4000); // пауза для запоминания

                SetStateToAllCards(0);
                replay = false;
                mainLoop = true;
                remainingCard = cardCount;
                clock.Restart();
            }
            
            if (mainLoop)
            {

                DispatchEvents();
                if (GetKeyDown(Keyboard.Key.Escape) == true) break;

                if (remainingCard == 0 )
                {
                    mainLoop = false;
                    end = true;
                }

                CheckOpenCards();

                ClickCardDetector();

                ClearWindow();

                DrawCards();

                time = clock.ElapsedTime;
                timeStr = (time.AsSeconds()).ToString();
                //timeStr = timeStr.Substring(0, timeStr.Length - 4); выдает ошибки на тестах, 
                Console.WriteLine(timeStr);

            }

            if (end)
            {
                DispatchEvents();
                Console.WriteLine("Выиграл");
                ClearWindow();
                SetFillColor(255,255, 255);
                DrawText(180, 150, "Поздравляю! \nТы нашел все пары\n" + 
                    "за "+ timeStr +" секунд!\n" +
                    "Пробел - новая игра\n" +
                    "Esc - выход", 48);

                //DisplayWindow();
                //Delay(7000);
                if (GetKeyDown(Keyboard.Key.Space) == true)
                { 
                    replay = true; 
                    end = false;
                }
                if (GetKeyDown(Keyboard.Key.Escape) == true) break;

            }

            DisplayWindow();

            Delay(1);
        }

    }
}

