using System.Collections.Generic;

public class CommandInvoker
{
    private readonly Queue<ICommand> queue = new();

    public void Enqueue(ICommand cmd)
    {
        if (cmd != null) queue.Enqueue(cmd);
    }

    public void ExecuteAll()
    {
        while (queue.Count > 0)
            queue.Dequeue().Execute();
    }
}