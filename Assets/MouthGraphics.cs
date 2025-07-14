using System;
using UnityEngine;

public class MouthGraphics : MonoBehaviour
{
    public void ToungeOutAnimationStarted()
    {
        AudioManager.Instance.PlaySound("MouthOpen", transform.position, 1f);
    }
}
 