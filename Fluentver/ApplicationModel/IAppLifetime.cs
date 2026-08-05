namespace Fluver.ApplicationModel;

public interface IAppLifetime
{
    void Restart();
    void Exit();
}
