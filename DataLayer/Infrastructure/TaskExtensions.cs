namespace DataLayer.Infrastructure;

public static class TaskExtensions
{
    public static async Task<bool> TryAsync<T>(this Task<T> task)
    {
        try
        {
            await task;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static async Task<bool> TryAsync(this Task task)
    {
        try
        {
            await task;
            return true;
        }
        catch
        {
            return false;
        }
    }
}