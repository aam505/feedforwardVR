//Shady
using UnityEngine;

[ExecuteInEditMode]
public class Reveal : MonoBehaviour
{
    [SerializeField] Material RevealMat;
    [SerializeField] Material DisappearMat;
    [SerializeField] Light SpotLight;

    [SerializeField] GameObject Sphere;
    void Update()
    {
        if (RevealMat && SpotLight)
        {
            //RevealMat.SetVector("MyLightPosition", SpotLight.transform.position);
            //RevealMat.SetVector("MyLightDirection", -SpotLight.transform.forward);
            //RevealMat.SetFloat("MyLightAngle", SpotLight.spotAngle+2);

            RevealMat.SetVector("MyLightPosition", Sphere.transform.position);
            RevealMat.SetVector("MyLightDirection", -Sphere.transform.forward);
            RevealMat.SetFloat("MyLightAngle", Sphere.GetComponent<SphereCollider>().radius+0.06f);
        }

        if (DisappearMat && SpotLight)
        {
            DisappearMat.SetVector("MyLightPosition", Sphere.transform.position);
            DisappearMat.SetVector("MyLightDirection", -Sphere.transform.forward);
            DisappearMat.SetFloat("MyLightAngle",  Sphere.GetComponent<SphereCollider>().radius);
        }
    }//Update() end
}//class end