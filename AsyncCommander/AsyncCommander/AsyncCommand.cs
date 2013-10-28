using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncCommander
{
    public interface Command
    {
        void execute();
        void undo();

        void executeAsynchronously();
        void undoAsynchronously();
    }

    public abstract class AbstractCommand : Command
    {
        public abstract void execute();
        public abstract void undo();

        private delegate void ExecuteCommandFunctionDelegate();
        private ExecuteCommandFunctionDelegate executeCommandFunctionDelegate;

        public void executeAsynchronously()
        {
            executeCommandFunctionDelegate = new ExecuteCommandFunctionDelegate(this.execute);
            executeCommandFunctionDelegate.BeginInvoke(null, null);
        }

        private delegate void UndoCommandFunctionDelegate();
        private UndoCommandFunctionDelegate undoCommandFunctionDelegate;

        public void undoAsynchronously()
        {
            undoCommandFunctionDelegate = new UndoCommandFunctionDelegate(this.undo);
            undoCommandFunctionDelegate.BeginInvoke(null, null);
        }

    }

    public class MacroCommand : AbstractCommand
    {

        private List<Command> commands;
        public bool executeAllAsynchronously { get; set; }
        public bool undoAllAsynchronously { get; set; }

        public MacroCommand(bool executeAllAsynchronously = true, bool undoAllAsynchronously = true)
        {
            this.executeAllAsynchronously = executeAllAsynchronously;
            this.undoAllAsynchronously = undoAllAsynchronously;
            commands = new List<Command>();
        }

        public MacroCommand(Command command, params Command[] additionalCommands)
        {
            commands = new List<Command>();

            if (command != null)
            {
                commands.Add(command);
            }

            if (additionalCommands != null && additionalCommands.Length > 0)
            {
                foreach (Command additionalCommand in additionalCommands)
                {
                    if (additionalCommand != null)
                    {
                        commands.Add(additionalCommand);
                    }
                }
            }
        }

        public void addCommand(Command command)
        {
            commands = commands == null ? new List<Command>() : commands;
            commands.Add(command);
        }

        public void removeCommand(Command command)
        {
            if (commands != null)
            {
                commands.Remove(command);
            }
        }

        public override void execute()
        {
            if (commands != null && commands.Count > 0)
            {
                foreach (Command command in commands)
                {
                    if (executeAllAsynchronously)
                    {
                        command.executeAsynchronously();
                    }
                    else
                    {
                        command.execute();
                    }
                }
            }
        }

        public override void undo()
        {
            if (commands != null && commands.Count > 0)
            {
                foreach (Command command in commands)
                {
                    if (undoAllAsynchronously)
                    {
                        command.undoAsynchronously();
                    }
                    else
                    {
                        command.undo();
                    }
                }
            }
        }
    }

}
