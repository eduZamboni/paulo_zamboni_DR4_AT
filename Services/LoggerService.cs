using System;
using System.Collections.Generic;
using System.IO;

namespace AgenciaTurismo.Services
{
    public static class LoggerService
    {
        public static List<string> Memoria = new();

        public static void LogToConsole(string mensagem)
        {
            Console.WriteLine($"[Console] {mensagem}");
        }

        public static void LogToFile(string mensagem)
        {
            File.AppendAllText("log.txt", $"[Arquivo] {mensagem}{Environment.NewLine}");
        }

        public static void LogToMemory(string mensagem)
        {
            Memoria.Add($"[Memória] {mensagem}");
        }
    }
}