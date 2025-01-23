using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class RobotController : MonoBehaviour
{
    int delay = 0, force = 1, consoleDelay = 10;
    public TMP_Text delayText, forceText, consoleDelayText;
    float consoleDelayTimer = 10;

    public ScrollRect console;
    public Transform consoleItemParent;
    public GameObject consoleItemPrefab, consoleImportantItemPrefab;

    enum Command
    {
        WALK_FRONT,
        WALK_BACK,
        WALK_RIGHT,
        WALK_LEFT,
        JUMP
    }
    Command command;
    List<Command> executedCommands = new();

    enum ActionState
    {
        NONE,
        MOVING
    }
    ActionState state;
    enum CommandState
    {
        NONE,
        WAITING
    }
    CommandState commandState;

    Rigidbody rb;

    int activeCoroutines = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //Console delay
        if (consoleDelayTimer > 0) consoleDelayTimer -= Time.deltaTime;
        else 
        {
            consoleDelayTimer = consoleDelay;
            ConsoleLog(executedCommands.Count() == 0 ? "Aucune commande effectuée" : $"Commandes effectuées : {executedCommands.Count()}", true);
            ConsoleLog($"État : {state}", true);
            ConsoleLog($"Commandes : {commandState}", true);

            executedCommands = new();
        }

        //States
        if (rb.linearVelocity.magnitude == 0) state = ActionState.NONE;
        else state = ActionState.MOVING;
        if (activeCoroutines == 0) commandState = CommandState.NONE;
        else commandState = CommandState.WAITING;
    }

    //Log a string into the visual console
    void ConsoleLog(string text, bool important = false)
    {
        GameObject item;
        if (important) item = Instantiate(consoleImportantItemPrefab, consoleItemParent);
        else item = Instantiate(consoleItemPrefab, consoleItemParent);
        item.GetComponentInChildren<TMP_Text>().text = text;
        
        Canvas.ForceUpdateCanvases();
        ResetConsoleScroll();
    }

    //Reset the console's scroll view
    void ResetConsoleScroll()
    {
        console.verticalNormalizedPosition = 0;
    }

    //Called when the DelaySlider's value is changed
    public void SetDelay(float value)
    {
        delay = (int)value;
        delayText.text = delay.ToString();
    }

    //Called when the ForceSlider's value is changed
    public void SetForce(float value)
    {
        force = (int)value;
        forceText.text = force.ToString();
    }

    //Called when the ConsoleDelaySlider's value is changed
    public void SetConsoleDelay(float value)
    {
        if (consoleDelay > value) consoleDelayTimer -= (float)consoleDelay - value;
        else consoleDelayTimer += value - (float)consoleDelay;

        consoleDelay = (int)value;
        consoleDelayText.text = consoleDelay.ToString();
    }

    //Called when the ActionDropdown's value is changed
    public void SetCommand(int option)
    {
        command = (Command)option;
        ConsoleLog($"Commande mise à jour : {(Command)option}");
    }

    //Called when pressing SubmitButton
    public void SubmitCommand()
    {
        StartCoroutine(ExecuteCommand(delay, force));
    }

    //Move the robot in a certain direction at a certain force
    void Walk(Vector3 direction, int f)
    {
        rb.AddForce(direction * f, ForceMode.Impulse);
    }

    //Make the robot jump at a certain force
    void Jump(int f)
    {
        rb.AddForce(new Vector3(0, 1 * f, 0), ForceMode.Impulse);
    }

    //Reset the robot's position and rotation
    public void Respawn()
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
        transform.position = new Vector3(0, 0.5f, 0);
        transform.rotation = new Quaternion(0, 0, 0, 0);
        rb.constraints = RigidbodyConstraints.None;

        ConsoleLog("Réapparu");
    }

    IEnumerator ExecuteCommand(int delay, int force)
    {        
        activeCoroutines++;

        if (delay != 0)
        {
            ConsoleLog($"Commande lancée : {command} ({delay}s)");
            yield return new WaitForSeconds(delay); //Delays the action if a delay was specified
        }

        ConsoleLog($"Commande exécutée : {command}");

        switch (command) //Executes different actions depending on the chosen action
        {
            case Command.WALK_FRONT:
                Walk(new Vector3(0, 0, -1), force);
                break;

            case Command.WALK_BACK:
                Walk(new Vector3(0, 0, 1), force);
                break;

            case Command.WALK_RIGHT:
                Walk(new Vector3(-1, 0, 0), force);
                break;

            case Command.WALK_LEFT:
                Walk(new Vector3(1, 0, 0), force);
                break;

            case Command.JUMP:
                Jump(force);
                break;
        }
        
        ConsoleLog($"Action effectuée : {command}");
        executedCommands.Add(command);

        activeCoroutines--;
    }
}
