using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forklift : MonoBehaviour
{
    public bool Ready = false;
    public bool Licensed = false;
    public ulong Mechanism = 0;

    // Vortex Studio Forklift
    Forklift()
    {
        string previous = System.IO.Directory.GetCurrentDirectory();

        try
        {
            // this will allow the library to load
            System.IO.Directory.SetCurrentDirectory(VxDLL.VortexRoot + "/" + "bin");

            // license check
            Licensed = VxDLL.VortexHasValidLicense();
        }
        catch
        {
            // does nothing
        }

        System.IO.Directory.SetCurrentDirectory(previous);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Create Vortex Studio mechanism
    void OnEnable()
    {
        Mechanism = VxDLL.VortexLoadMechanism(VxDLL.VortexContent + "/" + "Demo Scenes/Equipment/Forklift/Dynamic/Design/Forklift.vxmechanism", new double[3] { 0.0, 0.0, 0.0 }, new double[4] { 0.0, 0.0, 0.0, 1.0 });
    }

    // Destroy Vortex Studio mechanism
    void OnDisable()
    {
        VxDLL.VortexUnloadMechanism(Mechanism);
        Mechanism = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // FixedUpdate is called on a fixed timestep
    void FixedUpdate()
    {

    }
}
