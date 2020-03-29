using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtension
{
    // Depth first search
    public static Transform FindRecursively(this Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;
            var result = child.FindRecursively(name);
            if (result != null)
                return result;
        }
        return null;
    }
}

public class Forklift : MonoBehaviour
{
    public bool Ready = false;
    public bool Licensed = false;
    public ulong Mechanism = 0;
    public Dictionary<Transform, ulong> PartMapping = new Dictionary<Transform, ulong>();

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
            uint nodeHandlesCount = 128;
            ulong[] nodeHandles = new ulong[nodeHandlesCount];
            if (VxDLL.VortexGetGraphicsNodeHandles(Mechanism, nodeHandles, ref nodeHandlesCount))
            {
                string discoveredNodes = "";
                VortexGraphicNodeData nodeData = new VortexGraphicNodeData();
                for (uint i = 0; i < nodeHandlesCount && i < nodeHandles.Length; ++i)
                {
                    if (VxDLL.VortexGetGraphicNodeData(nodeHandles[i], ref nodeData))
                    {
                        // console output
                        if (discoveredNodes != "")
                        {
                            discoveredNodes += "\n";
                        }
                        string nodeName;
                        unsafe { nodeName = new string(nodeData.name); }
                        discoveredNodes += nodeName;

                        // mapping
                        Transform child = TransformExtension.FindRecursively(transform, nodeName);
                        if (child)
                        {
                            PartMapping[child] = nodeHandles[i];

                            // relative position
                            Vector3 newPosition = new Vector3();
                            unsafe
                            {
                                newPosition.x = (float)nodeData.position[0];
                                // swizzle Z and Y
                                newPosition.z = (float)nodeData.position[1];
                                newPosition.y = (float)nodeData.position[2];
                            }
                            transform.localPosition = newPosition;
                        }
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
            double[] position = new double[3];
            double[] scale = new double[3];
            double[] rotation = new double[4];
            Vector3 newPosition = new Vector3();
            foreach (var mapping in PartMapping)
            {
                if (mapping.Key)
                {
                    VxDLL.VortexGetParentTransform(mapping.Value, position, scale, rotation);
                    newPosition.x = (float)position[0];
                    // swizzle Z and Y
                    newPosition.z = (float)position[1];
                    newPosition.y = (float)position[2];
                    transform.position = newPosition;
                }
            }
        }
    }
}
