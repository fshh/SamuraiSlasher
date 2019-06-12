using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaintainGlobalRotation : MonoBehaviour
{
    public Quaternion constantRotation = Quaternion.identity;

    // Reset this gameObject's global rotation to the specified constant each frame
    void Update()
    {
        transform.rotation = constantRotation;
    }
}
