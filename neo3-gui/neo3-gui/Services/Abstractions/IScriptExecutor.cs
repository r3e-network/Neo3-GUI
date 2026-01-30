using Neo.VM;

namespace Neo.Services.Abstractions
{
    /// <summary>
    /// Script execution service interface
    /// </summary>
    public interface IScriptExecutor
    {
        ApplicationEngine Execute(byte[] script);
        bool IsSuccess(ApplicationEngine engine);
    }
}
