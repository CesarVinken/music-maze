using System.Collections.Generic;

public class EditorCommand : CommandProcedure
{
    public override void Help()
    {
        Console.Instance.PrintToReportText("Access or leave the editor mode with the editor command.");
    }

    public override void Run(List<string> arguments)
    {
        if (arguments.Count == 0)
        {
            EditorManager.ToggleEditorMode();

            if (EditorManager.InEditor)
            {
                Console.Instance.PrintToReportText("Opened editor");
            }
            else
            {
                Console.Instance.PrintToReportText("Closed editor");
            }
            return;
        }

        if (arguments[0] == "open")
        {
            EditorManager.OpenEditor();
            Console.Instance.PrintToReportText("Opened editor");

        }
        else if (arguments[0] == "close")
        {
            EditorManager.CloseEditor();
            Console.Instance.PrintToReportText("Closed editor");
        }
        else
        {
            Console.Instance.PrintToReportText("Possible arguments for the editor command are 'open' and 'close'");
        }
    }
}
