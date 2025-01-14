using Pic2PixelStylet.Pages;
using Stylet;
using StyletIoC;

namespace Pic2PixelStylet
{
    public class Bootstrapper : Bootstrapper<ShellViewModel>
    {
        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            // Configure the IoC container in here
            builder.Autobind();
        }

        protected override void Configure()
        {
            // Perform any other configuration before the application starts
            DbConnection.DbConnection.Db.CodeFirst.InitTables<DbConnection.PixelsHistory>();
        }
    }
}
