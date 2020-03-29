using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forklift : MonoBehaviour
{
    public bool Ready = false;
    public bool Licensed = false;

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

    }

    // Destroy Vortex Studio mechanism
    void OnDisable()
    {

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
