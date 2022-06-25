namespace MAction.SampleOnion.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                   .UseKestrel(o => { o.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(10000); });
                });
    }


}