using UnityEngine;
using HPTK.Models.Avatar;
using HPTK.Controllers.Avatar;
using HPTK.Controllers.Input;
using System.Collections;
using System.Collections.Generic;

public enum HandStatus
{
    ToInteraction,
    ToTrackedHand,
    Feedforward,
    TrackedVisible,
    TrackedInvisible
}
public class HandContainerFF : MonoBehaviour
{

    public HandStatus Status;
    public GameObject MasterParent;
    public GameObject SlaveParent;
    public OVRHand OVRHand;
    public OVRSkeleton Skeleton;
    public GameObject OVRHandPrefab;

    public HandModel MasterHandModel;
    public HandModel PhysicsHandModel;

    //The physics hand - slave. The slave follows the master i.e gets coordinates from it which in turn takes it from ovr hand
    public HandPhysicsModel PhysicsModel;

    //The master hand
    public InputModel InputModel;

    public InputController InputController;
    public HandPhysicsController PhysicsController;
    public ProxyHandModel ProxyHandModel;
    public bool RegisteredHand = false;
    public GameObject MeshObject;

    public void InitializeHand()
    {
        OVRHand = OVRHandPrefab.GetComponent<OVRHand>();

        Skeleton = OVRHandPrefab.GetComponent<OVRSkeleton>();
    }

    private void Awake()
    {
        MeshObject = SlaveParent.transform.GetChild(1).GetChild(0).gameObject;
        DisableMesh();
    }
    private void Start()
    {

    }

    public void EnableMesh()
    {
        Debug.Log("Enable mesh");
        MeshObject.SetActive(false);
    }

    public void DisableMesh()
    {
        Debug.Log("Disable mesh");
        MeshObject.SetActive(true);
    }

    public void SetMeshVisibility(bool visiblity)
    {
        Debug.Log("Set mesh visible");
        MeshObject.SetActive(visiblity);
    }

    public bool GetMeshVisibility()
    {
        return MeshObject.activeSelf;
    }
    private void Update()
    {
        if (!RegisteredHand && OVRManager.isHmdPresent)
        {
            InitializeHand();
        }
    }

    public void ChangeToGhostMaterial()
    {
        Material[] materials = MeshObject.GetComponent<SkinnedMeshRenderer>().sharedMaterials;
        materials[0] = HandPoolManager.GhostsBaseMaterial;
        materials[1] = HandPoolManager.GhostsMaterial;
         MeshObject.GetComponent<SkinnedMeshRenderer>().sharedMaterials = materials;
    }
    public void ChangeToSkinMaterial()
    {
        Material[] materials = MeshObject.GetComponent<SkinnedMeshRenderer>().sharedMaterials;
        materials[0] = HandPoolManager.SkinMaterial;
        materials[1] = null;
         MeshObject.GetComponent<SkinnedMeshRenderer>().sharedMaterials = materials;
    }
}
