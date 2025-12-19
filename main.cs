using System;

namespace DesignPatternsDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Singleton ===");
            Logger.Instance.Log("Запуск програми");
            Logger.Instance.Log("Singleton працює");

            Console.WriteLine("\n=== Decorator ===");
            ICoffee coffee = new SimpleCoffee();
            coffee = new MilkDecorator(coffee);
            coffee = new SugarDecorator(coffee);
            Console.WriteLine($"{coffee.GetDescription()} | Ціна: {coffee.GetCost()} грн");

            Console.WriteLine("\n=== Strategy ===");
            CompressionContext context = new CompressionContext();

            context.SetStrategy(new ZipCompressionStrategy());
            context.CreateArchive("data.txt");

            context.SetStrategy(new RarCompressionStrategy());
            context.CreateArchive("data.txt");

            Console.ReadLine();
        }
    }

    // ======================================================
    // 1. CREATIONAL PATTERN — SINGLETON
    // ======================================================
    public class Logger
    {
        private static Logger _instance;
        private static readonly object _lock = new object();

        private Logger() { }

        public static Logger Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new Logger();
                    return _instance;
                }
            }
        }

        public void Log(string message)
        {
            Console.WriteLine($"[LOG]: {message}");
        }
    }

    // ======================================================
    // 2. STRUCTURAL PATTERN — DECORATOR
    // ======================================================
    public interface ICoffee
    {
        string GetDescription();
        double GetCost();
    }

    public class SimpleCoffee : ICoffee
    {
        public string GetDescription() => "Кава";
        public double GetCost() => 20.0;
    }

    public abstract class CoffeeDecorator : ICoffee
    {
        protected ICoffee _coffee;

        protected CoffeeDecorator(ICoffee coffee)
        {
            _coffee = coffee;
        }

        public virtual string GetDescription() => _coffee.GetDescription();
        public virtual double GetCost() => _coffee.GetCost();
    }

    public class MilkDecorator : CoffeeDecorator
    {
        public MilkDecorator(ICoffee coffee) : base(coffee) { }

        public override string GetDescription() =>
            _coffee.GetDescription() + ", молоко";

        public override double GetCost() =>
            _coffee.GetCost() + 5.0;
    }

    public class SugarDecorator : CoffeeDecorator
    {
        public SugarDecorator(ICoffee coffee) : base(coffee) { }

        public override string GetDescription() =>
            _coffee.GetDescription() + ", цукор";

        public override double GetCost() =>
            _coffee.GetCost() + 2.0;
    }

    // ======================================================
    // 3. BEHAVIORAL PATTERN — STRATEGY
    // ======================================================
    public interface ICompressionStrategy
    {
        void Compress(string fileName);
    }

    public class ZipCompressionStrategy : ICompressionStrategy
    {
        public void Compress(string fileName)
        {
            Console.WriteLine($"Стиснення {fileName} у формат ZIP");
        }
    }

    public class RarCompressionStrategy : ICompressionStrategy
    {
        public void Compress(string fileName)
        {
            Console.WriteLine($"Стиснення {fileName} у формат RAR");
        }
    }

    public class CompressionContext
    {
        private ICompressionStrategy _strategy;

        public void SetStrategy(ICompressionStrategy strategy)
        {
            _strategy = strategy;
        }

        public void CreateArchive(string fileName)
        {
            if (_strategy == null)
                throw new InvalidOperationException("Стратегія не задана");

            _strategy.Compress(fileName);
        }
    }
}
