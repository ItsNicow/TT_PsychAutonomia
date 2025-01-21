using UnityEngine;
using TMPro;
using System.Collections;

public class RobotController : MonoBehaviour
{
    public TMP_InputField delayInputField;

    enum Command
    {
        WALK_FRONT,
        WALK_BACK,
        WALK_RIGHT,
        WALK_LEFT,
        JUMP
    }
    Command command;

    public void SubmitCommand()
    {
        int delay = (delayInputField.text == "") ? 0 : int.Parse(delayInputField.text);
        StartCoroutine(ExecuteCommand(delay));
    }

    public void SetCommand(int option)
    {
        command = (Command)option;
    }

    IEnumerator ExecuteCommand(int delay)
    {
        if (delay != 0) yield return new WaitForSeconds(delay);

        switch (command)
        {
            case Command.WALK_FRONT:

                break;

            case Command.WALK_BACK:

                break;

            case Command.WALK_RIGHT:

                break;

            case Command.WALK_LEFT:

                break;

            case Command.JUMP:

                break;
        }
    }
}
