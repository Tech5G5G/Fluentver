namespace Fluver.Helpers;

/// <summary>Provides methods to assign values using the try statement.</summary>
public static class AssignerHelper
{
    /// <summary>Tries to return <paramref name="operation"/>.</summary>
    /// <typeparam name="T">The type of the operation to return.</typeparam>
    /// <param name="operation">The operation to try and execute.</param>
    /// <returns><paramref name="operation"/>, but if an exception is caught, the <see langword="default"/> of <typeparamref name="T"/>.</returns>
    public static T TryAssign<T>(Func<T> operation)
    {
        try
        {
            return operation.Invoke();
        }
        catch
        {
            return default;
        }
    }
    /// <summary>Tries to return <paramref name="operation"/>.</summary>
    /// <typeparam name="T">The type of the operation to return.</typeparam>
    /// <param name="operation">The operation to try and execute.</param>
    /// <param name="altOperation">The alternative operation to execute if <paramref name="operation"/> fails.</param>
    /// <returns><paramref name="operation"/>, but if an exception is caught, <paramref name="altOperation"/>.</returns>
    public static T TryAssign<T>(Func<T> operation, Func<T> altOperation)
    {
        try
        {
            return operation.Invoke();
        }
        catch
        {
            return altOperation.Invoke();
        }
    }
    /// <summary>Tries to return <paramref name="operation"/>, but catches <typeparamref name="E"/> if thrown.</summary>
    /// <typeparam name="T">The type of the operation to return.</typeparam>
    /// <typeparam name="E">The <see cref="Exception"/> to catch when executing <paramref name="operation"/>.</typeparam>
    /// <param name="operation">The operation to try and execute.</param>
    /// <param name="altOperation">The alternative operation to execute if <paramref name="operation"/> throws <typeparamref name="E"/>.</param>
    /// <returns><paramref name="operation"/>, but if <typeparamref name="E"/> is caught, <paramref name="altOperation"/>.</returns>
    public static T TryAssign<T, E>(Func<T> operation, Func<T> altOperation) where E : Exception
    {
        try
        {
            return operation.Invoke();
        }
        catch (E)
        {
            return altOperation.Invoke();
        }
    }

    /// <summary>Tries to return <paramref name="operation"/>.</summary>
    /// <typeparam name="T">The type of the operation to return.</typeparam>
    /// <param name="operation">The operation to try and execute.</param>
    /// <returns><paramref name="operation"/>, but if an exception is caught, the <see langword="default"/> of <typeparamref name="T"/>, asynchronously.</returns>
    public async static Task<T> TryAssignAsync<T>(Func<Task<T>> operation)
    {
        try
        {
            return await operation.Invoke();
        }
        catch
        {
            return default;
        }
    }
    /// <summary>Tries to return <paramref name="operation"/>.</summary>
    /// <typeparam name="T">The type of the operation to return.</typeparam>
    /// <param name="operation">The operation to try and execute.</param>
    /// <param name="altOperation">The alternative operation to execute if <paramref name="operation"/> fails.</param>
    /// <returns><paramref name="operation"/>, but if an exception is caught, <paramref name="altOperation"/>, asynchronously.</returns>
    public async static Task<T> TryAssignAsync<T>(Func<Task<T>> operation, Func<Task<T>> altOperation)
    {
        try
        {
            return await operation.Invoke();
        }
        catch
        {
            return await altOperation.Invoke();
        }
    }
    /// <summary>Tries to return <paramref name="operation"/>, but catches <typeparamref name="E"/> if thrown.</summary>
    /// <typeparam name="T">The type of the operation to return.</typeparam>
    /// <typeparam name="E">The <see cref="Exception"/> to catch when executing <paramref name="operation"/>.</typeparam>
    /// <param name="operation">The operation to try and execute.</param>
    /// <param name="altOperation">The alternative operation to execute if <paramref name="operation"/> throws <typeparamref name="E"/>.</param>
    /// <returns><paramref name="operation"/>, but if <typeparamref name="E"/> is caught, <paramref name="altOperation"/>, asynchronously.</returns>
    public async static Task<T> TryAssignAsync<T, E>(Func<Task<T>> operation, Func<Task<T>> altOperation) where E : Exception
    {
        try
        {
            return await operation.Invoke();
        }
        catch (E)
        {
            return await altOperation.Invoke();
        }
    }
}
