using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Threading;
using System.Text.Json;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;


namespace Server
{

    public class Program

    {
        ///	<summary>
        ///	���������� ������ � ������� �������� ������ �����/������
        ///	</summary>

        public static List<RegData> RegDatas = new List<RegData>();

        ///	<summary>
        ///	���������� ������ � ������� �������� ��� ������� ���������
        ///	</summary>

        public static List<message> Messages = new List<message>();

        ///	<summary>
        ///	������� ������������� (� ����/�� � ����)
        ///	</summary>
        public static List<string> OnlineUsers = new List<string>();

        ///	<summary>
        ///	������� ������������� � ������� ���������� ������� ������
        ///	</summary>

        public static Dictionary<string, DateTime> OnlineUsersTimeout = new Dictionary<string, DateTime>();

        private static string Url = "http://localhost:5000";

        ///	<summary>
        ///	����� �����
        ///	</summary>

        ///	<param name="args">�������� ���������</param> 
        public static void Main(string[] args)

        {

            JsonWorker.Load(); string IP; string port;

            if (args.Length > 0)

            {

                IP = args[0];

                port = args[1];
            }

            else

            {

                Console.Write("Enter IP(or press enter or default):"); IP = Console.ReadLine();

                if (!string.IsNullOrEmpty(IP))
                {

                    Console.Write("Enter port:");

                    port = Console.ReadLine();
                    Url = $"http://{IP}:{port}";

                }

            }

            CreateHostBuilder(args).Build().Run();

        }

        ///	<summary>

        ///	�������� ���-������� ��� ��������� ��������
        ///	</summary>
        ///	<param name="args">�������� ���������</param>

        public static IHostBuilder CreateHostBuilder(string[] args)
        {

            return Host.CreateDefaultBuilder(args)

            .ConfigureWebHostDefaults(webBuilder =>
            {

                webBuilder.UseStartup<Startup>();

                webBuilder.UseUrls(Url);

            });
        }

    }

}
namespace Server

{

    ///	<summary>

    ///	����� ��� �������� ������ �����/������
    ///	</summary>

    public class RegData

    {
        ///	<summary>
        ///	����� ���������� ��������� ������������
        ///	</summary>
        public string Username { get; set; }

        ///	<summary>
        ///	������ - ��������� ��� �������
        ///	</summary>

        public string Password { get; set; }

    }

}



namespace Server
{

    internal class JsonWorker

    {
        ///	<summary>
        ///	���� ��� ���������� ��������� � ���� json �������
        ///	</summary>
        private const string MessagesPath = @"messages.json";

        private const string RegDataPath = @"regData.json";

        ///	<summary>
        ///	������� ���������� � ����
        ///	</summary>

        ///	<param name="messages">������ ���������</param> 
        public static async void Save(List<message> messages)

        {

            var NumberOfRetries = 3; var DelayOnRetry = 1000;

            for (var i = 1; i <= NumberOfRetries; ++i)

                try

                {

                    await using var streamWriter = new StreamWriter(MessagesPath);

                    await streamWriter.WriteAsync(JsonConvert.SerializeObject(messages)); break;

                }
                catch (IOException e) when (i <= NumberOfRetries)

                {

                    Thread.Sleep(DelayOnRetry);
                }

        }

        ///	<summary>
        ///	������� ���������� � ����
        ///	</summary>

        ///	<param name="regDatas">������ ������ �����/������</param> 
        public static async void Save(List<RegData> regDatas)

        {
            await using var streamWriter = new StreamWriter(RegDataPath);
            await streamWriter.WriteAsync(JsonConvert.SerializeObject(regDatas));

        }

        ///	<summary>
        ///	������� �������� ��������� �� �����, ���������������� �� json � List <Message> ������
        ///	</summary>
        public static async void Load()

        {

            if (!File.Exists(MessagesPath))
                return;

            try

            {

                using var streamReader = new StreamReader(MessagesPath); 
                Program.Messages = JsonConvert.DeserializeObject<List<message>>(await streamReader.ReadToEndAsync());

            }
            catch (Exception)

            {

            }

            if (!File.Exists(RegDataPath))

                return;

            try
            {

                using var streamReader = new StreamReader(RegDataPath); Program.RegDatas = JsonConvert.DeserializeObject<List<RegData>>(await streamReader.ReadToEndAsync());
            }

            catch (Exception)
            {

            }

        }
    }

}

