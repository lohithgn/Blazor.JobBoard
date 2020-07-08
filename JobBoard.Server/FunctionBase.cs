using System;

namespace JobBoard.Server
{
    public abstract class FunctionBase
    {
        protected FunctionBase()
        {
            DatabaseName = Environment.GetEnvironmentVariable("DatabaseName");
            ContainerName = Environment.GetEnvironmentVariable("ContainerName");    
        }

        protected string DatabaseName { get; private set; }
        protected string ContainerName { get; private set; }
    }
}
