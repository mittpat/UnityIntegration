using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forklift : MonoBehaviour
{
    public bool Ready = false;
    public bool Licensed = false;
    public ulong Mechanism = 0;
    public ulong Body = 0;
    public double[] BodyPosition = new double[3];
    public double[] BodyScale = new double[3];
    public double[] BodyRotation = new double[4];

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

        // Vortex node discovery
        if (Mechanism != 0)
        {
            uint nodeHandlesCount = 32;
            ulong[] nodeHandles = new ulong[nodeHandlesCount];
            if (VxDLL.VortexGetGraphicsNodeHandles(Mechanism, nodeHandles, ref nodeHandlesCount))
            {
                string discoveredNodes = "";
                VortexGraphicNodeData nodeData = new VortexGraphicNodeData();
                for (uint i = 0; i < nodeHandlesCount && i < nodeHandles.Length; ++i)
                {
                    if (VxDLL.VortexGetGraphicNodeData(nodeHandles[i], ref nodeData))
                    {
                        if (discoveredNodes != "")
                        {
                            discoveredNodes += "\n";
                        }
                        unsafe { discoveredNodes += new string(nodeData.name); }
                    }
                }
                if (nodeHandlesCount > 0)
                {
                    Debug.Log("Discovered Vortex Nodes:\n" + discoveredNodes);
                }
            }
        }
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
        // Vortex node mapping
        if (Mechanism != 0)
        {
            uint nodeHandlesCount = 32;
            ulong[] nodeHandles = new ulong[nodeHandlesCount];
            if (VxDLL.VortexGetGraphicsNodeHandles(Mechanism, nodeHandles, ref nodeHandlesCount))
            {
                VortexGraphicNodeData nodeData = new VortexGraphicNodeData();
                for (uint i = 0; i < nodeHandlesCount && i < nodeHandles.Length; ++i)
                {
                    if (VxDLL.VortexGetGraphicNodeData(nodeHandles[i], ref nodeData))
                    {
                        string nodeName = "";
                        unsafe { nodeName = new string(nodeData.name); }
                        if (nodeName == "Body")
                        {
                            VxDLL.VortexGetParentTransform(nodeHandles[i], BodyPosition, BodyScale, BodyRotation);
                        }
                    }
                }
            }
        }
    }
}
