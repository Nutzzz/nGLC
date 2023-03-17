using System.Data;
using Terminal.Gui;
using GLC;

Application.Init();

try
{
    GLCView view = new();
    Application.Run(view);
}
finally
{
    Application.Shutdown();
}