
namespace Microsoft.VisualStudio.Patterning.Extensibility.Serialization.Json
{
    internal delegate TResult MethodCall<T, TResult>(T target, params object[] args);
}
