using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBook : MonoBehaviour
{
    public GameObject Eye;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Eye.transform.LookAt(Ref.PlayerBehaviour.transform.position);
        //float x = Mathf.Clamp(Eye.transform.rotation.x, Eye.transform.rotation.x - 40, Eye.transform.rotation.x + 40);
        //float z = Mathf.Clamp(Eye.transform.rotation.z, Eye.transform.rotation.z - 40, Eye.transform.rotation.z + 40);
        //Eye.transform.rotation = Quaternion.Euler(x, Eye.transform.rotation.y, z);
    }
}
