using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

// Vortex Studio Foreign Function Interface (partial)
static class VxDLL
{
    public const int VortexNameSize = 64;
    public const string VortexContent = "C:/CM Labs/Vortex Studio Content 2020a";
    public const string VortexRoot = "C:/CM Labs/Vortex Studio 2020a";
    public const string VortexIntegration = "VortexIntegration.dll";

    [DllImport(VortexIntegration)]
    public static extern bool VortexHasValidLicense();

    [DllImport(VortexIntegration)]
    public static extern bool VortexCreateApplication(string setupDocument, string materialTable, string dataStore, string optionalLogFilepath, ulong terrainProviderInfo);

    [DllImport(VortexIntegration)]
    public static extern void VortexUpdateApplication();

    [DllImport(VortexIntegration)]
    public static extern ulong VortexLoadScene(string sceneFile);

    [DllImport(VortexIntegration)]
    public static extern bool VortexUnloadScene(ulong sceneHandle);

    [DllImport(VortexIntegration)]
    public static extern ulong VortexLoadMechanism(string mechanismFile, [In] double[] position, [In] double[] orientation);

    [DllImport(VortexIntegration)]
    public static extern bool VortexSetWorldTransform(ulong mechanismHandle, [In] double[] position, [In] double[] orientation);

    [DllImport(VortexIntegration)]
    public static extern bool VortexUnloadMechanism(ulong mechanismHandle);

    [DllImport(VortexIntegration)]
    public static extern bool VortexSetApplicationMode(int mode, bool waitForModeToBeApplied);

    [DllImport(VortexIntegration)]
    public static extern bool VortexPause(bool pause);

    [DllImport(VortexIntegration)]
    public static extern void VortexDestroyApplication();

    [DllImport(VortexIntegration)]
    public static extern bool VortexGetGraphicsNodeHandles(ulong objectHandle, [In, Out] ulong[] nodeHandleArray, ref uint nodeHandleCount);

    [DllImport(VortexIntegration)]
    public static extern bool VortexGetGraphicNodeData(ulong nodeHandle, ref VortexGraphicNodeData graphicNodeData);

    [DllImport(VortexIntegration)]
    public static extern void VortexGetParentTransform(ulong objectHandle, [In, Out] double[] translation, [In, Out] double[] scale, [In, Out] double[] rotationQuaternion);
}

unsafe struct VortexGraphicNodeData
{
    public fixed double position[3];
    public fixed double scale[3];
    public fixed double rotation[4];
    public fixed ulong parentNodeContentID[2];
    public fixed ulong contentID[2];
    public fixed char name[VxDLL.VortexNameSize];
    public bool hasGeometry;
    public bool hasConnection;
}

public class VortexApplication : MonoBehaviour
{
    public bool Ready = false;
    public bool Licensed = false;

    // Exactly one Vortex Studio Application must exist per project
    VortexApplication()
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

    // Create Vortex Studio application
    void OnEnable()
    {
        string previous = System.IO.Directory.GetCurrentDirectory();

        try
        {
            // this will allow the library to load
            System.IO.Directory.SetCurrentDirectory(VxDLL.VortexRoot + "/" + "bin");

            // create the application with a given setup document, optional physical materials and terrain provider
            Ready = VxDLL.VortexCreateApplication(VxDLL.VortexRoot + "/" + "resources/config/VortexIntegration.vxc", "", "", "", 0);
        }
        catch
        {
            // does nothing
        }

        System.IO.Directory.SetCurrentDirectory(previous);

        // switch to simulating
        VxDLL.VortexSetApplicationMode(0, true);
        VxDLL.VortexPause(false);
    }

    // Destroy Vortex Studio application
    void OnDisable()
    {
        if (Ready)
        {
            // switch to editing
            VxDLL.VortexPause(true);
            VxDLL.VortexSetApplicationMode(1, true);

            // unload and destroy
            VxDLL.VortexDestroyApplication();

            Ready = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // FixedUpdate is called on a fixed timestep
    void FixedUpdate()
    {
        if (Ready)
        {
            VxDLL.VortexUpdateApplication();
        }
    }
}
