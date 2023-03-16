using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityButton : MonoBehaviour
{
    public void ToggleGravity()
    {
        try
        {
            ABGameWorld.Instance.SimulateRigidbody(!ABGameWorld.Instance.simulateRigidbody);
        }
        catch { }
    }
}
