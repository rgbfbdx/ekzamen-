using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

[Serializable]
class DictionaryApp
{
    public string Type { get; set; }
    public Dictionary<string, List<string>> Words { get; set; } = new Dictionary<string, List<string>>();

    public void AddWord(string word, List<string> translations)
    {
        if (Words.ContainsKey(word))
            Words[word].AddRange(translations);
        else
            Words[word] = new List<string>(translations);
    }

    public void ReplaceWord(string oldWord, string newWord)
    {
        if (Words.ContainsKey(oldWord))
        {
            Words[newWord] = Words[oldWord];
            Words.Remove(oldWord);
        }
        else
            Console.WriteLine("Слово не знайдено");
    }

    public void ReplaceTranslation(string word, int index, string newTranslation)
    {
        if (Words.ContainsKey(word) && index >= 0 && index < Words[word].Count)
            Words[word][index] = newTranslation;
        else
            Console.WriteLine("Переклад не знайдено");
    }

    public void RemoveWord(string word)
    {
        Words.Remove(word);
    }

    public void RemoveTranslation(string word, int index)
    {
        if (Words.ContainsKey(word))
        {
            if (Words[word].Count > 1)
                Words[word].RemoveAt(index);
            else
                Console.WriteLine("Неможливо видалити останній переклад слова");
        }
        else
            Console.WriteLine("Слово не знайдено");
    }

    public List<string> GetTranslations(string word)
    {
        if (Words.ContainsKey(word))
            return Words[word];
        return null;
    }

    public void SaveToFile(string filename)
    {
        string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filename, json);
    }

    public static DictionaryApp LoadFromFile(string filename)
    {
        string json = File.ReadAllText(filename);
        return JsonSerializer.Deserialize<DictionaryApp>(json);
    }

    public void ExportWord(string word, string filename)
    {
        if (Words.ContainsKey(word))
        {
            string content = $"{word}: {string.Join(", ", Words[word])}";
            File.WriteAllText(filename, content);
        }
        else
            Console.WriteLine("Слово не знайдено");
    }
}

class Program
{
    static void Main()
    {
        DictionaryApp dict = null;
        while (true)
        {
            Console.WriteLine("\n--- Головне меню ---");
            Console.WriteLine("1. Створити словник");
            Console.WriteLine("2. Завантажити словник з файлу");
            Console.WriteLine("3. Додати слово");
            Console.WriteLine("4. Замінити слово або переклад");
            Console.WriteLine("5. Видалити слово або переклад");
            Console.WriteLine("6. Пошук перекладу слова");
            Console.WriteLine("7. Зберегти словник");
            Console.WriteLine("8. Експорт слова");
            Console.WriteLine("0. Вихід");
            Console.Write("Виберіть пункт: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Введіть тип словника: ");
                    string type = Console.ReadLine();
                    dict = new DictionaryApp { Type = type };
                    Console.WriteLine("Словник створено");
                    break;
                case "2":
                    Console.Write("Введіть ім'я файлу: ");
                    string loadFile = Console.ReadLine();
                    dict = DictionaryApp.LoadFromFile(loadFile);
                    Console.WriteLine("Словник завантажено");
                    break;
                case "3":
                    if (dict == null) { Console.WriteLine("Створіть або завантажте словник"); break; }
                    Console.Write("Слово: ");
                    string word = Console.ReadLine();
                    Console.Write("Переклади через кому: ");
                    string[] translations = Console.ReadLine().Split(',', StringSplitOptions.RemoveEmptyEntries);
                    dict.AddWord(word, new List<string>(translations));
                    Console.WriteLine("Слово додано");
                    break;
                case "4":
                    if (dict == null) { Console.WriteLine("Словник не завантажено"); break; }
                    Console.WriteLine("1. Замінити слово");
                    Console.WriteLine("2. Замінити переклад");
                    string subChoice = Console.ReadLine();
                    if (subChoice == "1")
                    {
                        Console.Write("Старе слово: ");
                        string oldWord = Console.ReadLine();
                        Console.Write("Нове слово: ");
                        string newWord = Console.ReadLine();
                        dict.ReplaceWord(oldWord, newWord);
                    }
                    else if (subChoice == "2")
                    {
                        Console.Write("Слово: ");
                        string w = Console.ReadLine();
                        Console.Write("Індекс перекладу (0 - перший): ");
                        int index = int.Parse(Console.ReadLine());
                        Console.Write("Новий переклад: ");
                        string newTrans = Console.ReadLine();
                        dict.ReplaceTranslation(w, index, newTrans);
                    }
                    break;
                case "5":
                    if (dict == null) { Console.WriteLine("Словник не завантажено"); break; }
                    Console.WriteLine("1. Видалити слово");
                    Console.WriteLine("2. Видалити переклад");
                    string delChoice = Console.ReadLine();
                    if (delChoice == "1")
                    {
                        Console.Write("Слово: ");
                        dict.RemoveWord(Console.ReadLine());
                    }
                    else if (delChoice == "2")
                    {
                        Console.Write("Слово: ");
                        string wordDel = Console.ReadLine();
                        Console.Write("Індекс перекладу (0 - перший): ");
                        int idxDel = int.Parse(Console.ReadLine());
                        dict.RemoveTranslation(wordDel, idxDel);
                    }
                    break;
                case "6":
                    if (dict == null) { Console.WriteLine("Словник не завантажено"); break; }
                    Console.Write("Слово для пошуку: ");
                    string searchWord = Console.ReadLine();
                    var tr = dict.GetTranslations(searchWord);
                    if (tr != null)
                        Console.WriteLine($"{searchWord}: {string.Join(", ", tr)}");
                    else
                        Console.WriteLine("Слово не знайдено");
                    break;
                case "7":
                    if (dict == null) { Console.WriteLine("Словник не завантажено"); break; }
                    Console.Write("Ім'я файлу для збереження: ");
                    dict.SaveToFile(Console.ReadLine());
                    Console.WriteLine("Словник збережено");
                    break;
                case "8":
                    if (dict == null) { Console.WriteLine("Словник не завантажено"); break; }
                    Console.Write("Слово для експорту: ");
                    string expWord = Console.ReadLine();
                    Console.Write("Ім'я файлу: ");
                    dict.ExportWord(expWord, Console.ReadLine());
                    Console.WriteLine("Експорт завершено");
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Невірний вибір");
                    break;
            }
        }
    }
}


class User
{
    public string Login { get; set; }
    public string Password { get; set; }
    public DateTime BirthDate { get; set; }
    public Dictionary<string, int> QuizScores { get; set; } = new Dictionary<string, int>();
}

[Serializable]
class Question
{
    public string Text { get; set; }
    public List<string> Options { get; set; } = new List<string>();
    public List<int> CorrectAnswers { get; set; } = new List<int>();
}

[Serializable]
class Quiz
{
    public string Name { get; set; }
    public List<Question> Questions { get; set; } = new List<Question>();
}

class Program
{
    static List<User> Users = new List<User>();
    static List<Quiz> Quizzes = new List<Quiz>();
    static User CurrentUser = null;

    static void Main()
    {
        LoadUsers();
        LoadQuizzes();
        AuthMenu();
    }

    static void AuthMenu()
    {
        while (true)
        {
            Console.WriteLine("1. Вхід\n2. Реєстрація\n0. Вихід");
            string choice = Console.ReadLine();
            if (choice == "1") Login(); 
            else if (choice == "2") Register();
            else if (choice == "0") return;
        }
    }

    static void Login()
    {
        Console.Write("Логін: ");
        string login = Console.ReadLine();
        Console.Write("Пароль: ");
        string pass = Console.ReadLine();
        CurrentUser = Users.FirstOrDefault(u => u.Login == login && u.Password == pass);
        if (CurrentUser != null) UserMenu();
        else Console.WriteLine("Невірний логін або пароль");
    }

    static void Register()
    {
        Console.Write("Логін: ");
        string login = Console.ReadLine();
        if (Users.Any(u => u.Login == login)) { Console.WriteLine("Логін існує"); return; }
        Console.Write("Пароль: ");
        string pass = Console.ReadLine();
        Console.Write("Дата народження (ДД.ММ.ГГГГ): ");
        DateTime bd = DateTime.Parse(Console.ReadLine());
        User newUser = new User { Login = login, Password = pass, BirthDate = bd };
        Users.Add(newUser);
        SaveUsers();
        Console.WriteLine("Реєстрація успішна");
    }

    static void UserMenu()
    {
        while (true)
        {
            Console.WriteLine("\n1. Старт нової вікторини\n2. Результати минулих вікторин\n3. Топ-20\n4. Налаштування\n0. Вихід");
            string choice = Console.ReadLine();
            if (choice == "1") StartQuizMenu();
            else if (choice == "2") ShowPastResults();
            else if (choice == "3") ShowTop20();
            else if (choice == "4") SettingsMenu();
            else if (choice == "0") { CurrentUser = null; break; }
        }
    }

    static void StartQuizMenu()
    {
        Console.WriteLine("Доступні вікторини:");
        for (int i = 0; i < Quizzes.Count; i++)
            Console.WriteLine($"{i}. {Quizzes[i].Name}");
        Console.WriteLine("Виберіть індекс вікторини або -1 для змішаної:");
        int idx = int.Parse(Console.ReadLine());
        List<Question> questions;
        if (idx == -1) questions = Quizzes.SelectMany(q => q.Questions).OrderBy(x => Guid.NewGuid()).Take(20).ToList();
        else questions = Quizzes[idx].Questions.Take(20).ToList();
        int score = RunQuiz(questions);
        Console.WriteLine($"Правильних відповідей: {score}");
        string quizName = idx == -1 ? "Змішана" : Quizzes[idx].Name;
        if (CurrentUser.QuizScores.ContainsKey(quizName)) CurrentUser.QuizScores[quizName] = score;
        else CurrentUser.QuizScores.Add(quizName, score);
        SaveUsers();
    }

    static int RunQuiz(List<Question> questions)
    {
        int score = 0;
        for (int i = 0; i < questions.Count; i++)
        {
            Question q = questions[i];
            Console.WriteLine($"\nПитання {i + 1}: {q.Text}");
            for (int j = 0; j < q.Options.Count; j++)
                Console.WriteLine($"{j}. {q.Options[j]}");
            Console.Write("Виберіть індекси правильних відповідей через кому: ");
            var ans = Console.ReadLine().Split(',').Select(x => int.Parse(x.Trim())).OrderBy(x => x).ToList();
            if (ans.SequenceEqual(q.CorrectAnswers.OrderBy(x => x))) score++;
        }
        return score;
    }

    static void ShowPastResults()
    {
        foreach (var kvp in CurrentUser.QuizScores)
            Console.WriteLine($"{kvp.Key}: {kvp.Value} правильних відповідей");
    }

    static void ShowTop20()
    {
        Console.WriteLine("Доступні вікторини:");
        for (int i = 0; i < Quizzes.Count; i++)
            Console.WriteLine($"{i}. {Quizzes[i].Name}");
        int idx = int.Parse(Console.ReadLine());
        string quizName = Quizzes[idx].Name;
        var top = Users.Where(u => u.QuizScores.ContainsKey(quizName))
                       .OrderByDescending(u => u.QuizScores[quizName])
                       .Take(20);
        foreach (var u in top)
            Console.WriteLine($"{u.Login}: {u.QuizScores[quizName]}");
    }

    static void SettingsMenu()
    {
        Console.WriteLine("1. Змінити пароль\n2. Змінити дату народження");
        string choice = Console.ReadLine();
        if (choice == "1")
        {
            Console.Write("Новий пароль: ");
            CurrentUser.Password = Console.ReadLine();
        }
        else if (choice == "2")
        {
            Console.Write("Нова дата народження (ДД.ММ.ГГГГ): ");
            CurrentUser.BirthDate = DateTime.Parse(Console.ReadLine());
        }
        SaveUsers();
    }

    static void LoadUsers()
    {
        if (File.Exists("users.json"))
        {
            string json = File.ReadAllText("users.json");
            Users = JsonSerializer.Deserialize<List<User>>(json);
        }
    }

    static void SaveUsers()
    {
        string json = JsonSerializer.Serialize(Users, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("users.json", json);
    }

    static void LoadQuizzes()
    {
        if (File.Exists("quizzes.json"))
        {
            string json = File.ReadAllText("quizzes.json");
            Quizzes = JsonSerializer.Deserialize<List<Quiz>>(json);
        }
        else
        {
            Quizzes = new List<Quiz>();
        }
    }
}
