using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ref : MonoBehaviour
{
    public PlayerBehaviour _PlayerBehaviour;
    public static PlayerBehaviour PlayerBehaviour { get { return Instance._PlayerBehaviour; } }

    public ManaSystem _ManaSystem;
    public static ManaSystem ManaSystem { get { return Instance._ManaSystem; } }

    public Movement _Movement;
    public static Movement Movement { get { return Instance._Movement; } }

    public LookPC _LookPC;
    public static LookPC LookPC { get { return Instance._LookPC; } }

    public Code _Code;
    public static Code Code { get { return Instance._Code; } }

    public UI _UI;
    public static UI UI { get { return Instance._UI; } }

    public EnemySpawner _EnemySpawner;
    public static EnemySpawner EnemySpawner { get { return Instance._EnemySpawner; } }

    public EnemyRadar _EnemyRadar;
    public static EnemyRadar EnemyRadar { get { return Instance._EnemyRadar; } }

    public AudioManager _AudioManager;
    public static AudioManager AudioManager { get { return Instance._AudioManager; } }

    public MapManager _MapManager;
    public static MapManager MapManager { get { return Instance._MapManager; } }

    public Tutorial _Tutorial;
    public static Tutorial Tutorial { get { return Instance._Tutorial; } }

    public MagicBook _MagicBook;
    public static MagicBook MagicBook { get { return Instance._MagicBook; } }

    public Inventory _Inventory;
    public static Inventory Inventory { get { return Instance._Inventory; } }

    public ManaUI _ManaUI;
    public static ManaUI ManaUI { get { return Instance._ManaUI; } }

    public SaveSystem _SaveSystem;
    public static SaveSystem SaveSystem { get { return Instance._SaveSystem; } }


    public static Ref Instance;

    private void OnEnable()
    {
        Instance = this;
    }


    public static void ActionAfterTime(float time, Action action)
    {
        Instance.StartCoroutine(ActionAfterTimeCor(time, action));
    }
    private static IEnumerator ActionAfterTimeCor(float time, Action action)
    {
        yield return new WaitForSecondsRealtime(time);
        action?.Invoke();
    }
}
