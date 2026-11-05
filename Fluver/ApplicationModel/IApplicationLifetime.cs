namespace Fluver.ApplicationModel;

public interface IApplicationLifetime
{
    void Restart();
    void Exit();
}
