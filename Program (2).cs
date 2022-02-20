using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Операционные_системы._Практика_1
{
    internal class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class User
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Company { get; set; }
    }

    internal class Program
    {
        private static async Task Main(string[] args)
        {
            CheckDriveInfo(); //1. Вывести информацию в консоль о логических дисках, именах, метке тома, размере типе файловой системы.

            WorkWithFile(); //2. Работа с файлами

            await WorkWithJson(); //3. Работа с форматом JSON

            WorkWithXml(); //4. Работа с форматом XML

            WorkWithZip(); //5. Создание zip архива, добавление туда файла, определение размера архива

            Console.ReadLine();
        }

        private static void WorkWithZip()
        {
            var sourceFolder = "test/"; // исходная папка
            var file = "test/file.txt";
            if (!Directory.Exists(sourceFolder)) Directory.CreateDirectory(sourceFolder);

            using (var fStream = new FileStream(file, FileMode.OpenOrCreate))
            {
                var array = Encoding.Default.GetBytes("testText");
                fStream.Write(array, 0, array.Length);
            }

            var zipFile = "test.zip"; // сжатый файл
            var targetFolder = "newtest/"; // папка, куда распаковывается файл

            ZipFile.CreateFromDirectory(sourceFolder, zipFile);
            Console.WriteLine($"Папка {sourceFolder} архивирована в файл {zipFile}");
            ZipFile.ExtractToDirectory(zipFile, targetFolder);

            Console.WriteLine($"Файл {zipFile} распакован в папку {targetFolder}");
            Directory.Delete(sourceFolder, true);
            Directory.Delete(targetFolder, true);
            File.Delete(zipFile);
        }

        private static void WorkWithXml()
        {
            var users = new List<User>();

            if (!File.Exists("users.xml"))
                File.Create("users.xml");
            users.Add(new User
            {
                Age = 19,
                Company = "Microsoft",
                Name = "Stive"
            });

            var tmpUser = new User();
            Console.Write("Age> ");
            tmpUser.Age = int.Parse(Console.ReadLine());
            Console.Write("Company> ");
            tmpUser.Company = Console.ReadLine();
            Console.Write("Name> ");
            tmpUser.Name = Console.ReadLine();
            foreach (var u in users)
                Console.WriteLine($"{u.Name} ({u.Company}) - {u.Age}");

            users.Add(tmpUser);

            var xmlSerializer = new XmlSerializer(users.GetType());
            using (var stream = new StreamWriter("users.xml"))
            {
                xmlSerializer.Serialize(stream, users);
            }

            using (var stream = new StreamReader("users.xml"))
            {
                Console.WriteLine(stream.ReadToEnd());
            }

            File.Delete("users.xml");
        }

        private static async Task WorkWithJson()
        {
            using (var fs = new FileStream("user.json", FileMode.OpenOrCreate))
            {
                var tom = new Person {Name = "Tom", Age = 35};
                await JsonSerializer.SerializeAsync(fs, tom);
                Console.WriteLine("Data has been saved to file");
            }

            // чтение данных
            using (var fs = new FileStream("user.json", FileMode.OpenOrCreate))
            {
                var restoredPerson = await JsonSerializer.DeserializeAsync<Person>(fs);
                Console.WriteLine($"Name: {restoredPerson.Name}  Age: {restoredPerson.Age}");
            }
        }

        private static void WorkWithFile()
        {
            Console.Write("Введите строку для записи в файл> ");
            var text = Console.ReadLine();

            using (var fStream = new FileStream("note.txt", FileMode.OpenOrCreate))
            {
                var array = Encoding.Default.GetBytes(text);
                fStream.Write(array, 0, array.Length);
                Console.WriteLine("Текст записан в файл");
            }

            // чтение из файла
            using (var fStream = File.OpenRead("note.txt"))
            {
                var array = new byte[fStream.Length];
                fStream.Read(array, 0, array.Length);
                var textFromFile = Encoding.Default.GetString(array);
                Console.WriteLine($"Текст из файла: {textFromFile}");
            }

            File.Delete("note.txt");
            Console.WriteLine();
        }

        private static void CheckDriveInfo()
        {
            var drives = DriveInfo.GetDrives();

            foreach (var drive in drives)
            {
                Console.WriteLine($"Название: {drive.Name}");
                Console.WriteLine($"Тип: {drive.DriveType}");
                if (drive.IsReady)
                {
                    Console.WriteLine($"Объем диска: {drive.TotalSize}");
                    Console.WriteLine($"Свободное пространство: {drive.TotalFreeSpace}");
                    Console.WriteLine($"Метка: {drive.VolumeLabel}");
                    Console.WriteLine($"Формат диска: {drive.DriveFormat}");
                }

                Console.WriteLine();
            }
        }
    }
}