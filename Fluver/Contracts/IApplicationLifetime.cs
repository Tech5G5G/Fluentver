namespace Fluver.Contracts;

public interface IApplicationLifetime
{
    void Restart();
    void Exit();
}
