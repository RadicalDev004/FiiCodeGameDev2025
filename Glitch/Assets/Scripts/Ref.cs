using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ref : MonoBehaviour
{
    public PlayerBehaviour _PlayerBehaviour;
    public static PlayerBehaviour PlayerBehaviour { get { return Instance._PlayerBehaviour; } }


    public Movement _Movement;
    public static Movement Movement { get { return Instance._Movement; } }


    public LookPC _LookPC;
    public static LookPC LookPC { get { return Instance._LookPC; } }


    public Code _Code;
    public static Code Code { get { return Instance._Code; } }


    public UI _UI;
    public static UI UI { get { return Instance._UI; } }


    public static Ref Instance;


    private void OnEnable()
    {
        Instance = this;
    }
}
