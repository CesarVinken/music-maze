using Console;
using UnityEngine;

namespace UI
{
    public class ConsoleButton : MonoBehaviour
    {
        public void OpenConsole()
        {
            ConsoleContainer.Instance.ToggleConsole(ConsoleState.Large);
        }
    }
}