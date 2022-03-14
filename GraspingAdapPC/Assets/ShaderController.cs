using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderController : MonoBehaviour
{
    public List<GameObject> FFWindows;
    public GameObject WindowFar;

    public List<Material> GhostMaterials;
    public List<Material> ActiveMaterials;
    [Range(0.1f, 64f)] public float m_VisDist = 6f;
    [Range(0f, 5f)] public float m_FadeWidth = 2f;

    [Header("Halo")]
    public bool m_HaloEdgeNoise = false;

    public GameObject ActiveWindow;


    public void SetupMaterials(List<Material> materials, bool inverse)
    {
        foreach (Material m in materials)
        {
            m.EnableKeyword("AWD_SPHERICAL");
            m.DisableKeyword("AWD_CUBIC");
            m.DisableKeyword("AWD_ROUND_XZ");
            m.SetFloat("_VisDist", m_VisDist);
            m.SetFloat("_FadeWidth", m_FadeWidth);
            if (inverse)
                m.EnableKeyword("AWD_INVERSE");
            else
                m.DisableKeyword("AWD_INVERSE");

        }
    }
    void Start()
    {
        SetupMaterials(GhostMaterials, false);
        SetupMaterials(ActiveMaterials, true);
    }

    // Update is called once per frame
    void Update()
    {
        bool HandActive = false;

        foreach (GameObject w in FFWindows)
            if (w.activeSelf)
            {
                HandActive = true;
                ActiveWindow = w;
                SetShaderPosition();

            }

        if (!HandActive)
        {
            ActiveWindow = WindowFar;
            SetShaderPosition();
        }
    }

    void SetShaderPosition()
    {
        foreach (Material m in GhostMaterials)
        {
            m.SetVector("_Pos", ActiveWindow.transform.position);
        }

        foreach (Material m in ActiveMaterials)
        {
            m.SetVector("_Pos", ActiveWindow.transform.position);
        }
    }

}
