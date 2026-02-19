using UnityEngine;

public class StartBootstrap : MonoBehaviour
{
    public CoreBootstrup _bootstrup;

    public void BootstrupStart()
    {
        _bootstrup.StartPuzzle();
    }
}
