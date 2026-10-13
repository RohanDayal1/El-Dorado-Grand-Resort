using System;
    class Program
    {
        static void Main(string[] args)
        {
            new DatabaseInitializer().ResetDatabase();
            Menu.Start();
        }
    }
