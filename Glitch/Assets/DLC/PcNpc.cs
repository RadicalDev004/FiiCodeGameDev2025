using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PcNpc : MonoBehaviour
{
    public TMP_Text T_Text;
    public ScrollRect Scroll;
    public bool Write = false;
    public string CurrentString;
    public bool WaitingResponse;

    private void Start()
    {
        OpenAIChat.ResetChatHistory("Respond in under 100 characters: You are an assistant integrated into a game called \"The Glitch\". The player is trapped in a magical world filled with programming bugs and must solve puzzles by editing in-game code. Below is the full context of the game:\r\n\r\n**Core Concept:**\r\n- The player is transported into a magic realm filled with glitches.\r\n- They are the only one who can fix these bugs using programming knowledge.\r\n- They can edit in-game object attributes, logic and code snippets via a Coding Console.\r\n\r\n**Gameplay Mechanics:**\r\n- The game consists of 15 puzzles increasing in complexity.\r\n- Puzzles involve logic, programming concepts (e.g., booleans, attributes, function editing).\r\n- Puzzle example: change attribute `isLocked = true` to `isLocked = false` to open a door.\r\n- Some puzzles require spatial manipulation (like moving puzzle pieces).\r\n\r\n**Combat System:**\r\n- Each glitch fixed triggers enemies (Glitch Enemies) to attack.\r\n- The player uses a staff to attack and mana to edit enemy attributes (e.g., reduce speed).\r\n- Defeated enemies may drop temporary power-ups (e.g., `doubleDamage()`).\r\n- Enemy difficulty increases over time.\r\n\r\n**Exploration:**\r\n- The player can modify elements of the environment (e.g., mushrooms to jump higher or teleport).\r\n- There are checkpoints and a map for navigation.\r\n- A helper arrow can be enabled to guide the player to the next puzzle.\r\n\r\n**Story Progression:**\r\n- A mysterious book observes the player. As puzzles are solved, the staff becomes brighter.\r\n- Final boss is the book itself, which uses harmful code to attack.\r\n- Boss fight involves destroying crystals that alter player code.\r\n- Only after disabling the crystals and shield can the book be damaged.\r\n\r\n**Player Assistance:**\r\n- The player can access hints for each puzzle.\r\n- A documentation panel explains how to use the Coding Console.\r\n- Settings allow volume control and UI customizations.\r\n\r\n**Your Role as AI:**\r\n- Provide hints based on the current puzzle code and progress.\r\n- Explain how specific code edits affect gameplay objects.\r\n- Offer debugging suggestions if the player is stuck.\r\n- Respond to questions about gameplay mechanics or narrative.\r\n- Be concise, helpful, and avoid giving full puzzle solutions unless explicitly asked.\r\n\r\nThe player may now interact with you. The Puzzles: (you may not give the solution) ```\r\n1. Puzzle Name: Overflow Terminal\r\nObjects Involved: allow_overflow, true/false toggles, warning message\r\nSolution Idea: The player must set allow_overflow = false\r\n\r\n2. Puzzle Name: Globe\r\nObjects Involved: globeTransform, angle, scale, raycastHit, symbolPosition\r\nSolution Idea: Rotate and scale the globe to hit the correct symbol with a raycast.\r\n\r\n3. Puzzle Name: Book Stack\r\nObjects Involved: List of books, shelves, candleColor array\r\nSolution Idea: Stack books so their color matches each candle’s position.\r\n\r\n4. Puzzle Name: Magic Scroll\r\nObjects Involved: runeArray, string comparison\r\nSolution Idea: Arrange runes to spell SCROLL.\r\n\r\n5. Puzzle Name: Jars\r\nObjects Involved: fill_amount (float), jarColor (RGB), colorMatch(targetColor)\r\nSolution Idea: Adjust fill_amount to mix correct RGB values.\r\n\r\n6. Puzzle Name: Owls Rotate\r\nObjects Involved: owlAngle array, faceDirection, pairwise lookAt check\r\nSolution Idea: Rotate owls so they face matching partners.\r\n\r\n7. Puzzle Name: Alphabet Library\r\nObjects Involved: shelfLabels[], secretAlphabet[], renameBook(label)\r\nSolution Idea: Rename book labels using the game’s secret alphabet.\r\n\r\n8. Puzzle Name: Candles\r\nObjects Involved: candles[4].burn_time\r\nSolution Idea: Set burn times so all candles extinguish simultaneously.\r\n\r\n9. Puzzle Name: Pour Bottles\r\nObjects Involved: bottle_big, bottle_medium, bottle_small, pour(from, to)\r\nSolution Idea: Use pouring to get exactly 4 units in the medium bottle.\r\n\r\n10. Puzzle Name: Balance Seesaw\r\nObjects Involved: swap(left, right)\r\nSolution Idea: Move weights so both sides are balanced, keeping Orange(5) with two small ones.\r\n\r\n11. Puzzle Name: Candle Sequence\r\nObjects Involved: candles[3], blow_out(index)\r\nSolution Idea: Blow out candles in the correct order to prevent relighting others.\r\n\r\n12. Puzzle Name: Musical Notes\r\nObjects Involved: notes[], song.play(note, index)\r\nSolution Idea: Replicate the original melody by assigning notes in correct order.\r\n\r\n13. Puzzle Name: Meteo Jar\r\nObjects Involved: symbolic elements (fire, air, energy, electricity)\r\nSolution Idea: Combine symbols so you discover fire, tornado, electricity and black magic.\r\n\r\n14. Puzzle Name: Star Constellation\r\nObjects Involved: connect_or_remove(starA, starB), all_constellations.areFound()\r\nSolution Idea: Add or remove star connections to form known constellations like Cepheus and Ursa Major.\r\n\r\n15. Puzzle Name: Jigsaw Puzzle\r\nObjects Involved: purple.position, orange.position, blue.position, green.position\r\nSolution Idea: Arrange puzzle pieces so green points to orange, blue points to purple, completing the pattern.");
            T_Text.text = "AI> How can i help?\r\n\nUSER> ";
    }

    public void ToggleWrite(bool state)
    {
        if (!state && Write) 
        {
            Inventory.Blocking = false;
            Movement.isPaused = false;
            MapManager.Block = false;
        }
        if (state && !Write) 
        {
            Inventory.Blocking = true;
            Movement.isPaused = true;
            MapManager.Block = true;
        }
        Write = state;

        //Movement.isPaused = state;

    }

    private void Update()
    {
        Scroll.verticalNormalizedPosition = 0f;
        if (!Write || WaitingResponse) return;

        string input = Input.inputString;

        foreach (char c in input)
        {
            if (char.IsLetterOrDigit(c) || char.IsPunctuation(c) || char.IsSymbol(c))
            {
                CurrentString += c.ToString();
                T_Text.text += c.ToString();
                AudioManager.Play("Type_DLC");

            }
        }

        
        if (Input.GetKeyDown(KeyCode.Backspace) && !string.IsNullOrEmpty(CurrentString))
        {
            AudioManager.Play("Delete_DLC");
            CurrentString = CurrentString.Remove(CurrentString.Length  - 1);
            T_Text.text = T_Text.text.Remove(T_Text.text.Length - 1);
        }
        if(Input.GetKeyDown(KeyCode.Return))
        {
            OpenAIChat.SendUserMessage(CurrentString, ReceiveResponse);
            WaitingResponse = true;

            CurrentString = "";
            T_Text.text += "\n\nAI> ...";
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CurrentString += " ";
            T_Text.text += " ";
        }
    }

    public void ReceiveResponse(string resp)
    {
        T_Text.text = T_Text.text.Remove(T_Text.text.Length - 3);
        T_Text.text += resp;
        WaitingResponse = false;
        T_Text.text += "\n\nUSER> ";
    }
}
