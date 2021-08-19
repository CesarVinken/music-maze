using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Console
{
    public class Console : MonoBehaviour
    {
        public static Console Instance;
        public ConsoleState ConsoleState;

        public List<ConsoleCommand> Commands = new List<ConsoleCommand>();

        [SerializeField] private Text _inputText;
        [SerializeField] private Text _reportText;
        [SerializeField] private InputField _inputField;
        [SerializeField] private int _inputHistoryIndex = 0;

        private List<ConsoleLine> _consoleLines = new List<ConsoleLine>();
        private string _inputFieldText = "";

        void Awake()
        {
            Guard.CheckIsNull(_inputText, "InputText", gameObject);
            Guard.CheckIsNull(_reportText, "ReportText", gameObject);
            Guard.CheckIsNull(_inputField, "InputField", gameObject);

            Instance = this;
            ConsoleState = ConsoleState.Closed;

            RegisterCommands();
        }

        public void Update()
        {
            if (_inputField.isFocused)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    GetPreviousInput();
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    GetNextInput();
                }
            }
        }

        public void SetConsoleState(ConsoleState newConsoleState)
        {
            ConsoleState = newConsoleState;
            _inputField.ActivateInputField();
            _inputField.Select();
        }

        public void SetInputFieldText(string input)
        {
            if (ConsoleState == ConsoleState.Closed)
            {
                _inputField.text = _inputFieldText;
                return;
            }

            _inputFieldText = input;
            _inputField.text = _inputFieldText;
        }

        public void PublishInputText()
        {
            if (_inputField.text == "") return;

            string inputText = _inputText.text;

            PrintToReportText(inputText, true);

            _inputField.text = "";

            _inputField.ActivateInputField();
            _inputField.Select();

            if (inputText[0] == '$')
            {
                RunCommand(inputText);
            }
        }

        public void PrintToReportText(string text, bool isPlayerInput = false)
        {
            string consoleLine = "\n" + text;

            if (_consoleLines.Count > 30)
            {
                _consoleLines.RemoveAt(0);
            }

            _consoleLines.Add(new ConsoleLine(consoleLine, isPlayerInput));
            _inputHistoryIndex = _consoleLines.Count;

            string reportText = "";

            for (int i = 0; i < _consoleLines.Count; i++)
            {
                reportText = reportText + _consoleLines[i].Text;
            }

            _reportText.text = reportText;
        }

        public void GetPreviousInput()
        {
            if (_consoleLines.Count == 0) return;

            int localInputHistoryIndex = _inputHistoryIndex;

            for (int i = localInputHistoryIndex - 1; i >= 0; i--)
            {
                if (_consoleLines[i].IsPlayerInput)
                {
                    _inputHistoryIndex = i;
                    break;
                }
            }

            if (_inputHistoryIndex == localInputHistoryIndex)
            {
                for (int j = _consoleLines.Count - 1; j >= 0; j--)
                {
                    if (_consoleLines[j].IsPlayerInput)
                    {
                        _inputHistoryIndex = j;
                        break;
                    }
                }
            }

            if (_inputHistoryIndex == localInputHistoryIndex)
            {
                _inputField.text = "";
            }
            else
            {
                string previousReportText = _consoleLines[_inputHistoryIndex].Text;
                _inputField.text = previousReportText;
            }

        }

        public void GetNextInput()
        {
            if (_consoleLines.Count == 0) return;

            int localInputHistoryIndex = _inputHistoryIndex;

            for (int i = _inputHistoryIndex + 1; i < _consoleLines.Count; i++)
            {
                if (_consoleLines[i].IsPlayerInput)
                {
                    _inputHistoryIndex = i;
                    break;
                }
            }
            if (_inputHistoryIndex == localInputHistoryIndex)
            {
                for (int j = 0; j < _consoleLines.Count; j++)
                {
                    if (_consoleLines[j].IsPlayerInput)
                    {
                        _inputHistoryIndex = j;
                        break;
                    }
                }
            }

            if (_inputHistoryIndex == localInputHistoryIndex)
            {
                _inputField.text = "";
            }
            else
            {
                string nextReportText = _consoleLines[_inputHistoryIndex].Text;
                _inputField.text = nextReportText;
            }
        }

        public void RunCommand(string line)
        {
            string sanatisedLine = line.Substring(1);
            sanatisedLine.Trim();
            Logger.Log("Command input {0}", sanatisedLine);
            string[] arguments = sanatisedLine.Split(' ');
            List<string> sanatisedArguments = new List<string>();
            for (int i = 0; i < arguments.Length; i++)
            {
                if (arguments[i] == "") continue;
                sanatisedArguments.Add(arguments[i].ToLower());
            }

            string commandName = sanatisedArguments[0];
            sanatisedArguments.RemoveAt(0); // remove title from arguments

            ConsoleCommand Command = Commands.Find(j => j.Name == commandName);

            if (Command == null)
            {
                Logger.Warning("Could not find a command with the name {0}", commandName);
                PrintToReportText("Could not find a command with the name <color=" + ConsoleConfiguration.HighlightColour + ">" + commandName + "</color>");
                return;
            }

            Command.Execute(sanatisedArguments);
        }

        public void RegisterCommands()
        {
            Commands.Add(ConsoleCommand.AddCommand("add", 1, new AddCommand()));
            Commands.Add(ConsoleCommand.AddCommand("close", new CloseConsoleCommand()));
            Commands.Add(ConsoleCommand.AddCommand("configure", 2, 4, new ConfigureCommand()));
            Commands.Add(ConsoleCommand.AddCommand("delete", 1, 2, new DeleteCommand()));
            Commands.Add(ConsoleCommand.AddCommand("editor", 0, 1, new EditorCommand()));
            Commands.Add(ConsoleCommand.AddCommand("help", 0, 1, new HelpCommand()));
            Commands.Add(ConsoleCommand.AddCommand("info", 1, 2, new InfoCommand()));
            Commands.Add(ConsoleCommand.AddCommand("load", 1, 2, new LoadCommand()));
        }
    }
}
