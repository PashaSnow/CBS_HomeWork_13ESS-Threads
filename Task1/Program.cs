﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Task1
{
    class Program
    {
        static void Main(string[] args)
        {
            int height = 30, width = 80;
            Console.SetWindowSize(width, height);

            for (int i = 0; i < width-1 ; i++)
            {
                var writer = new ColomnWriter();
                ParameterizedThreadStart parameterizedThread = new ParameterizedThreadStart(writer.WriteColomn);
                new Thread(parameterizedThread).Start(height);
            }
        }
    }


    /// <summary>
    /// Какой массив будем добавлять?
    /// </summary>
    enum MatrixInfo
    {
        blackArray,
        charArray
    }

    /// <summary>
    /// Цвет консоли
    /// </summary>
    enum Color
    {
        White,
        Green,
        DarkGreen
    }

    /// <summary>
    /// Как выводим массив
    /// </summary>
    enum FillConsole
    {
        Start,
        Normal
    }

    class ColomnWriter
    {
        //Позиционирование
        static int xPosition = 0; // hack: не забувай
        int consoleHeight;
        int nonStaticPosition = 0;

        // разделяет потоки
        static object thredFlag = new object();

        // К массивам и их обслуге
        Random rnd = new Random(Guid.NewGuid().GetHashCode());

        char[][] symbolStream = new char[1][]; // масив который хранит все массивы в потоке. Это массивы символов и массив пустых значений один за одным
        string symbols = "123456789aAbBcCdDeEfFgGhiIjJkKlLmMnN";

        static MatrixInfo choseArray;
        Color color = Color.White;


        public ColomnWriter()
        {
            nonStaticPosition = xPosition++;                   // новая колонка 
            choseArray = MatrixInfo.charArray;
            // сперва символьный массив
            symbolStream = new char[2][]; // первый массив чисел, второв пустой массив (что бы сохранить растаяние) // todo: исправить за не надобностю длину массива
            symbolStream[0] = new char[new Random(Guid.NewGuid().GetHashCode()).Next(6, 10)];
            symbolStream[1] = new char[new Random(Guid.NewGuid().GetHashCode()).Next(5, 9)]; // todo: возможно убрать за не надобности
            Console.CursorVisible = false;
            InitArray(symbolStream[0]);
            InitArray(symbolStream[1]);

        }

        public void WriteColomn(object height)
        {
            Thread.Sleep(new Random(Guid.NewGuid().GetHashCode()).Next(100, 1000));
            FillConsole mode = FillConsole.Start;
            consoleHeight = (int)height;   // высота консоли
            int factLength = 1; // фактическая выведеная в консоль длина массива
            int realyLengt = SumLength(symbolStream); // вся длина массива
            int setter = 0;
            int stepIndex = 0; // индекc каждого прохода (этот параметер преследует setter)
            int firstLength = symbolStream[0][0]; // для режима нормал, переопределить в режиме mode = FillConsole.Normal

            // первый раз прокатит
            FillStream(consoleHeight, ref realyLengt);
            char[] steam = GetCharSteam();
            int index = 0; // индекс массмва steam в бесконечном цикле while
            Console.CursorVisible = false; // скрытный курсор
            while (true)
            {
                if (consoleHeight < realyLengt)
                {
                    FillStream(consoleHeight, ref realyLengt);
                }
                // Console будет разделяемым ресурсом между всема потоками
                lock (thredFlag)
                {
                    // если массив только заполняет колонку в консоли todo: курсор всегда проходит только в низ
                    if (mode == FillConsole.Start)
                    {
                        for (int iterator = factLength; iterator != -1; iterator--)
                        {
                            Console.CursorTop = setter;
                            WriteSymbol(ref steam[iterator]);

                            // todo: гедо условие для индекса stepIndex
                            if (setter < factLength)
                            {
                                index++;
                                setter++;
                            }
                            else
                            {
                                setter = 0;
                            }
                            if (index == steam.Length - 1)
                            {
                                index = 0;
                            }
                        }

                        if (factLength < consoleHeight)
                            factLength++;

                        // выход в нормальный режим
                        if (Console.CursorTop == consoleHeight - 1)
                        {
                            mode = FillConsole.Normal;
                        }
                    }

                    // когда массив заполнил всю строку
                    if (mode == FillConsole.Normal)
                    {
                        for (int iterator = factLength; iterator != -1; iterator--)
                        {
                            Console.CursorTop = setter;
                            WriteSymbol(ref steam[iterator]);  // todo: тут ексепшены

                            // todo: гедо условие для индекса stepIndex
                            if (setter < factLength)
                            {
                                setter++;
                            }
                            else
                            {
                                setter = 0;
                            }
                        }

                        // встановить курсор
                        if (setter < consoleHeight)
                        {
                            Console.CursorTop = setter;
                            setter++;
                        }
                        else
                            setter = 0;
                    }
                    //Thread.Sleep(new Random(Guid.NewGuid().GetHashCode()).Next(15, 50));
                }
            }
        }

        /// <summary>
        /// Заполняет всю длину потока символов
        /// </summary>
        /// <param name="consoleHeight">высота консоли</param>
        /// <param name="realyLengt">длина массивов</param>
        private void FillStream(int consoleHeight, ref int realyLengt)
        {
            while (consoleHeight + 7 > realyLengt)
            {
                AddNewArray(); // добавит массив символов
                AddNewArray(); // на этот раз добавит массив пустых значений, что бы замкнуть повторяющюю цепочку todo: убрать (возможно будет постоянно менятся после переопредиления массива
                realyLengt = SumLength(symbolStream);
            }
        }


        /// <summary>
        /// Длинна всех масивов в потоке
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        private int SumLength(char[][] array)
        {
            int sum = 0;
            for (int i = 0; i < array.Length; i++)
            {
                sum += array[i].Length;
            }
            return sum;
        }

        /// <summary>s
        /// Добавляет новый массив в цепочку массивов
        /// </summary>
        private void AddNewArray()
        {
            char[][] bufArray = new char[symbolStream.Length + 1][]; // последний массив будет новым

            // copy values
            for (int index = 0; index < symbolStream.Length; index++)
            {
                bufArray[index] = new char[symbolStream[index].Length];
                Array.Copy(symbolStream[index], bufArray[index], symbolStream[index].Length);
            }
            bufArray[symbolStream.Length] = new char[new Random(Guid.NewGuid().GetHashCode()).Next(4, 8)];
            InitArray(bufArray[symbolStream.Length]);

            symbolStream = bufArray;
        }


        /// <summary>
        /// Возвращает одномерный массив вместо зубчастого
        /// </summary>
        /// <returns></returns>
        private char[] GetCharSteam()
        {
            int length = SumLength(symbolStream);
            char[] bufSteam = new char[length];
            int bufCounter = 0;
            while (bufCounter < length)
            {
                for (int array = 0; array < symbolStream.Length; array++)
                {
                    for (int index = 0; index < symbolStream[array].Length; index++)
                    {
                        bufSteam[bufCounter] = symbolStream[array][index];
                        bufCounter++;
                    }
                }
            }
            return bufSteam;
        }

        private char GetChar()
        {
            return symbols[new Random(Guid.NewGuid().GetHashCode()).Next(0, symbols.Length - 1)];
        }

        /// <summary>
        /// Заполнить рандомными символами, может не работать
        /// </summary>
        /// <param name="emptyArray"></param>
        private char[] InitArray(char[] emptyArray)
        {
            switch (choseArray)
            {
                case MatrixInfo.charArray:
                    for (int i = 0; i < emptyArray.Length; i++)
                    {
                        emptyArray[i] = GetChar(); // new Random(Guid.NewGuid().GetHashCode()).Next(0, symbols.Length - 1) ; // rnd.Next(0, symbols.Length - 1)
                    }
                    choseArray = MatrixInfo.blackArray;
                    break;
                case MatrixInfo.blackArray:
                    for (int i = 0; i < emptyArray.Length; i++)
                    {
                        emptyArray[i] = ' ';
                    }
                    choseArray = MatrixInfo.charArray;
                    break;
            }
            return emptyArray;
        }


        /// <summary>
        /// Вывод символа в консоль
        /// </summary>
        /// <param name="symbol">аргумент который выведется на консоль</param>
        /// <param name="index">аргумент который представляет индекс массива, по его значению определяем цвет: 
        /// 0 - белый
        /// 1 - зеленый
        /// больше 1 - темно зеленый
        /// </param>
        private void WriteSymbol(ref char symbol)
        {

            switch (color)
            {
                case Color.White:
                    Console.ForegroundColor = ConsoleColor.White;
                    if (symbol != ' ')
                        color = Color.Green;
                    break;
                case Color.Green:
                    Console.ForegroundColor = ConsoleColor.Green;
                    color = Color.DarkGreen;
                    break;
                case Color.DarkGreen:
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    break;
            }



            Console.CursorLeft = nonStaticPosition;
            Console.Write(symbol);

            // если это не пустой масив, то получить новое значение
            if (symbol != ' ')
            {
                symbol = GetChar();
                //Thread.Sleep(10);
                //Console.CursorLeft = nonStaticPosition;
                //Console.Write(symbol);
                //Thread.Sleep(70); // hack: тут Thread.Sleep
            }
            else
            {
                color = Color.White;
            }
        }
    }
}