using System.Text;
#pragma warning disable CS8600 // Преобразование литерала, допускающего значение NULL или возможного значения NULL в тип, не допускающий значение NULL.
#pragma warning disable CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.

namespace CryptorNew
{
    internal class Encryptor_base
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("Encryptor_base.cs");

            //вспомогательный метод для шифровки/дешифровки первым способом
            FreqDictGenerator(out List<char> freqsequence_list);

            //лист для шифровки/дешифроки вторым способом
            List<char> positions_list = PositionsListGenerator();

            Console.WriteLine("Шифровать или дешифровать? (E / D)");
            string choose = Convert.ToString(Console.ReadLine());

            //ШИФРОВКА
            if (choose == "E" || choose == "e")
            {
                Console.WriteLine("Введите шифруемое сообщение");
                string input_string = Console.ReadLine();
                string output_encrypted = "";
                Console.WriteLine("Выберите способ шифровки (1 / 2 / 3)");
                Console.WriteLine
                    (
                    $"1) поддерживает ограниченный набор символов, содержащийся в частотном словаре \n" +
                    $"2) поддерживает латиницу, кириллицу и распространенные спецсимволы \n" +
                    $"3) поддерживает ТОЛЬКО латиницу и спецсимволы(кодировка ASCII) \n"
                    );
                int encryption_method = Convert.ToInt32(Console.ReadLine());

                switch (encryption_method)
                {
                    #region Encryption, method 1
                    case 1://шифрование на основе частоты появления символов в исходном тексте, готово
                        Console.WriteLine("Зашифрованное сообщение:\n");
                        EncryptAs1(freqsequence_list, input_string, ref output_encrypted);
                        Console.WriteLine(output_encrypted);
                        break;
                    #endregion

                    #region Encryption, method 2
                    case 2: //порядковый номер символа в листе символов, готово                        
                        //Console.WriteLine("Ёмкость словаря: " + positions_list.Count);
                        Console.WriteLine("Зашифрованное сообщение:\n");
                        EncryptAs2(positions_list, input_string, out output_encrypted);
                        Console.WriteLine(output_encrypted);
                        break;
                    #endregion

                    #region Encryption, method 3
                    case 3://шифрование посредством вывода кодов символов + сдвиг + межсимвольный мусор
                           //генерация межсимвольного мусора
                        int seed = SeedGenerator();//диапазон кодов символов, доступных пользователю для ввода одной клавишей
                        //составление внутреннего seed с маркерами и вывод пользовательского seed
                        Console.WriteLine("Введите значение сдвига, число от 1 до 10: ");
                        int ascii_shift = Convert.ToInt32(Console.ReadLine());
                        string seed_string = InternalSeedBuilder(seed);
                        UserSeedOut(seed);
                        Console.WriteLine("Зашифрованное сообщение: \n");
                        string encrypted_symbols_string = EncryptAs3(input_string, ascii_shift, seed_string);
                        Console.WriteLine(encrypted_symbols_string);
                        break;
                    #endregion
                    default:
                        ErrorMessage();
                        break;
                }

            }
            else if (choose == "D" || choose == "d")
            {
                Console.WriteLine("Введите дешифруемое сообщение");
                string input_encrypted = Console.ReadLine();
                Console.WriteLine("Выберите способ дешифровки (1 / 2 / 3)");
                int decryption_method = int.Parse(Console.ReadLine());
                switch (decryption_method)
                {
                    #region Decryption, method 1

                    case 1:
                        string return_for_analysis;
                        Console.WriteLine("__________________________________________________________________________________________");
                        Console.WriteLine("Дешифрованное сообщение:");
                        return_for_analysis = DecryptAs1(freqsequence_list, input_encrypted);
                        Console.WriteLine("__________________________________________________________________________________________");
                        Console.WriteLine("Диаграмма, отражающая частоту вхождения символов в первичный текст:");
                        StringAnalyzer(input_encrypted);
                        Console.WriteLine("__________________________________________________________________________________________");
                        Console.WriteLine("Диаграмма, отражающая частоту вхождения символов в обработанный текст:");
                        StringAnalyzer(return_for_analysis);
                        break;
                    #endregion

                    #region Decryption, method 2
                    case 2:
                        Console.WriteLine("__________________________________________________________________________________________");
                        Console.WriteLine("Дешифрованное сообщение:");
                        return_for_analysis = DecryptAs2(positions_list, input_encrypted);
                        Console.WriteLine("__________________________________________________________________________________________");
                        Console.WriteLine("Диаграмма, отражающая частоту вхождения символов в первичный текст:");
                        StringAnalyzer(input_encrypted);
                        Console.WriteLine("__________________________________________________________________________________________");
                        Console.WriteLine("Диаграмма, отражающая частоту вхождения символов в обработанный текст:");
                        StringAnalyzer(return_for_analysis);
                        break;
                    #endregion

                    #region Decryption, method 3
                    case 3:
                        return_for_analysis = DecryptAs3(input_encrypted);
                        Console.WriteLine("__________________________________________________________________________________________");
                        Console.WriteLine("Диаграмма, отражающая частоту вхождения символов в первичный текст:");
                        StringAnalyzer(input_encrypted);
                        Console.WriteLine("__________________________________________________________________________________________");
                        Console.WriteLine("Диаграмма, отражающая частоту вхождения символов в обработанный текст:");
                        StringAnalyzer(return_for_analysis);
                        break;
                    #endregion
                    default:
                        ErrorMessage();
                        break;
                }
            }

            else
            {
                ErrorMessage();
            }
        }

        #region First encryptor methods
        private static char[] EncryptAs1(List<char> freqsequence_list, string input_string, ref string output_encrypted)
        {
            char[] inputChar_arr = input_string.ToCharArray();
            inputChar_arr = input_string.ToCharArray();
            for (int x = 0; x < input_string.Length; x++)
            {
                int index_of = freqsequence_list.IndexOf(inputChar_arr[x]);
                output_encrypted = output_encrypted + index_of + " ";
            }

            return inputChar_arr;
        }
        #endregion
        #region Second encryptor methods
        private static void EncryptAs2(List<char> dictionary, string input_string, out string output_encrypted)
        {
            char[] inputChar_arr = input_string.ToCharArray();
            output_encrypted = "";
            for (int x = 0; x < input_string.Length; x++)
            {
                int index_of = dictionary.IndexOf(inputChar_arr[x]);
                output_encrypted = output_encrypted + index_of + " ";
            }
        }
        #endregion
        #region Third encryptor methods
        private static string EncryptAs3(string input_string, int ascii_shift, string seed_string)
        {
            byte[] ascii_bytes = Encoding.ASCII.GetBytes(input_string);//получение байтов символов в кодировке ASCII (латиница)
            string encrypted_symbols_string = "";
            foreach (int ascii_bytes_of_element in ascii_bytes)
            {
                string encrypted_symbol = (ascii_bytes_of_element - ascii_shift + seed_string);//байтовое значение символа-сдвиг+добавление подстроки seed
                encrypted_symbols_string += encrypted_symbol;
            }

            return encrypted_symbols_string;
        }
        #endregion

        #region First decryptor methods
        private static string DecryptAs1(List<char> freqsequence_list, string input_encrypted)
        {
            string input_index_str = input_encrypted;
            string input_index_int = input_index_str.Replace(",", string.Empty);
            int _ind = 0;
            int[] inputIndex_arr = new int[_ind];
            string return_for_analysis = "";

            inputIndex_arr = input_index_int.Split(' ').Select(int.Parse).ToArray();

            for (int i = 0; i < inputIndex_arr.Length; i++)//повторяется, пока не закончатся символы в строке
            {
                for (int j = 0; j < freqsequence_list.Count; j++)//перебирает все символы в словаре
                {

                    if (inputIndex_arr[i] == j)
                    {
                        return_for_analysis += Convert.ToString(freqsequence_list[j]);
                        Console.Write(freqsequence_list[j]);
                    }
                }
            }
            Console.WriteLine();
            return return_for_analysis;
        }

        #endregion
        #region Second decryptor methods
        private static string DecryptAs2(List<char> dictionary, string input_encrypted)
        {
            string return_for_analysis = "";

            string input_index_str = input_encrypted;
            string input_index_int = input_index_str.Replace(",", string.Empty);

            int _ind = 0;
            int[] inputIndex_arr = new int[_ind];

            inputIndex_arr = input_index_int.Split(' ').Select(int.Parse).ToArray();

            for (int i = 0; i < inputIndex_arr.Length; i++)//повторяется, пока не закончатся символы в строке
            {
                for (int j = 0; j < dictionary.Count; j++)//перебирает все символы в словаре
                {

                    if (inputIndex_arr[i] == j)
                    {
                        return_for_analysis += Convert.ToString(dictionary[j]);
                        Console.Write(dictionary[j]);

                    }
                }
            }
            Console.WriteLine();
            return return_for_analysis;
        }
        #endregion
        #region Third decryptor methods
        private static string DecryptAs3(string input_encrypted)
        {
            string return_for_analysis = "";
            Console.WriteLine("Введите seed, один символ:");
            string seed_string = DecryptionSeedBuilder();

            Console.WriteLine("Введите значение сдвига");
            int ascii_shift = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("__________________________________________________________________________________________");
            Console.WriteLine("Дешифрованное сообщение:");
            string input_encrypted_without_seed = input_encrypted.Replace(seed_string, " ");
            //
            List<int> int_chars_list = new();
            string[] output_arr = input_encrypted_without_seed.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in output_arr)
            {
                if (!int.TryParse(s, out int temp))
                {
                    throw new Exception("Wrong argument!");
                }

                else
                {
                    int_chars_list.Add(temp);
                }
            }
            foreach (int i in int_chars_list)
            {
                return_for_analysis += Convert.ToString((char)(i + ascii_shift));
                Console.Write((char)(i + ascii_shift));
            }
            Console.WriteLine();
            return return_for_analysis;

        }
        #endregion

        private static void FreqDictGenerator(out List<char> freqsequence_list)
        {
            string freqsequence_string = AbbcccStringBuilder();//построение частотной строки
            Dictionary<char, int> freqdict = SymbolRepeat_Counter(freqsequence_string);//пересчет вхождений каждого символа в частотную строку
            //перемещение ключей(символов) в порядке увеличения числа вхождений в массив(теперь кол-во вхождений элемента == индекс этого же элемента в массиве)
            freqsequence_list = FreqSequenceListBuilder(freqdict);
        }

        private static List<char> FreqSequenceListBuilder(Dictionary<char, int> freqdict)
        {
            List<char> freqsequence_list = new()
            {
                (char)10060//заполнение нулевого индекса листа, подгонка индексации к частоте вхождения
            };
            freqsequence_list = freqdict.Keys.ToList();
            return freqsequence_list;
        }

        private static Dictionary<char, int> SymbolRepeat_Counter(string freqsequence_string)
        {
            char alphabet_char;
            Dictionary<char, int> freqdict = new();
            foreach (char ch in freqsequence_string)
            {
                alphabet_char = ch;
                if (freqdict.ContainsKey(alphabet_char))
                    freqdict[alphabet_char]++;
                else
                    freqdict.Add(alphabet_char, 1);
            }

            return freqdict;
        }


        private static string DecryptionSeedBuilder()
        {
            char seed_char = Convert.ToChar(Console.ReadLine());    //введенный символ конвертится из string в char
            int seed_char_int = seed_char;                     //далее извлекается код символа
            string seed_string = "!-" + seed_char_int + "-!";       //и строится полноценная строка с маркерами начала\конца seed'a
            return seed_string;
        }

        private static string InternalSeedBuilder(int seed)//построение внутреннего seed для построения итоговой строки
        {
            return "!-" + seed.ToString() + "-!";
        }

        private static List<char> PositionsListGenerator()//создание листа символов для дальнейшего вывода индексов символов
        {
            return new()
            {
                't',
                'h',
                'e',
                'q',
                'u',
                'i',
                'c',
                'k',
                'b',
                'r',
                'o',
                'w',
                'n',
                'f',
                'x',
                'j',
                'm',
                'p',
                's',
                'v',
                'l',
                'a',
                'z',
                'y',
                'd',
                'g',
                'T',
                'H',
                'E',
                'Q',
                'U',
                'I',
                'C',
                'K',
                'B',
                'R',
                'O',
                'W',
                'N',
                'F',
                'X',
                'J',
                'M',
                'P',
                'S',
                'V',
                'L',
                'A',
                'Z',
                'Y',
                'D',
                'G',
                'с',
                'ъ',
                'е',
                'ш',
                'ь',
                'ж',
                'щ',
                'ё',
                'э',
                'т',
                'и',
                'х',
                'м',
                'я',
                'г',
                'к',
                'ф',
                'р',
                'а',
                'н',
                'ц',
                'у',
                'з',
                'б',
                'л',
                'о',
                'д',
                'в',
                'ы',
                'п',
                'й',
                'ч',
                'ю',
                'С',
                'Ъ',
                'Е',
                'Ш',
                'Ь',
                'Ж',
                'Щ',
                'Ё',
                'Э',
                'Т',
                'И',
                'Х',
                'М',
                'Я',
                'Г',
                'К',
                'Ф',
                'Р',
                'А',
                'Н',
                'Ц',
                'У',
                'З',
                'Б',
                'Л',
                'О',
                'Д',
                'В',
                'Ы',
                'П',
                'Й',
                'Ч',
                'Ю',
                ' ',
                ',',
                '!',
                '.',
                '?',
                '-',
                '_',
                '1',
                '2',
                '3',
                '4',
                '5',
                '6',
                '7',
                '8',
                '9',
                '0'
            };
        }

        private static string AbbcccStringBuilder()//создание строки повторяющихся символов для дальнейшего частотного анализа (способ №1)
        {
            string symbol_string = "";
            int repeat = 0;
            for (int symbol = 32; symbol <= 126; symbol++, repeat++)//с 32 по 126 символ(латиница+символы)
            {
                for (int i = repeat; i >= 0; i--) symbol_string = symbol_string.Insert(symbol_string.Length, Convert.ToString(Convert.ToChar(symbol)));
            }
            //пропуск неподдерживаемых консолью символов
            for (int symbol = 1040; symbol <= 1103; symbol++, repeat++)//с 1040 по 1103 символ(кириллица, оба регистра)
            {
                for (int i = repeat; i >= 0; i--) symbol_string = symbol_string.Insert(symbol_string.Length, Convert.ToString(Convert.ToChar(symbol)));
            }

            return symbol_string;
        }

        private static void ErrorMessage()//вывод сообщения об ошибке для случая ввода некорректного значения
        {
            Console.Write("Error. Unexpected symbol");
        }

        private static void UserSeedOut(int seed)//вывод пользовательского seed(для копирования)
        {
            Console.WriteLine("Ваш seed: " + (char)seed);
        }

        private static int SeedGenerator()//генератор численного seed
        {
            Random random = new();
            int seed = random.Next(33, 125);//диапазон кодов символов, доступных пользователю для ввода одной клавишей
            return seed;
        }

        private static void StringAnalyzer(string parsed_string)
        {
            string analyzer_dict = "!\"#$%&'()*+,-./0123456789:;<=>?@[\\]^_`{|}~" +
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ" +
                "абвгдежзийклмнопрстуфхцчшщъыьэюяАБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
            Dictionary<char, int> dic = new();
            foreach (char ch in analyzer_dict)
                dic.Add(ch, 0);
            foreach (char ch in parsed_string)
            {
                if (analyzer_dict.Contains(ch.ToString()))
                    dic[ch]++;
            }
            foreach (var pair in dic)
                if (pair.Value > 0)
                    Console.WriteLine("{0} {1}", pair.Key, string.Concat(Enumerable.Repeat("𝩋", pair.Value)));
        }

    }
}