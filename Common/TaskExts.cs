using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Dnsk.Common;

public static class TaskExts
{
    public static Task<T> Task<T>(this T obj) => System.Threading.Tasks.Task.FromResult(obj);
}