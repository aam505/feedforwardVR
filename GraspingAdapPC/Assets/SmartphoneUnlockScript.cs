using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartphoneUnlockScript : MonoBehaviour
{
    public Material ScreenLocked;
    public Material ScreenUnlocked;
    public bool Pressed, Unlocking;
    Vector3 initPos;
    // Start is called before the first frame update
    void Start()
    {
        Pressed = false;
        Unlocking = false;
        initPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (Pressed && !Unlocking)
        {
            Pressed = false;
            StartCoroutine(UnlockPhone());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Equals("hands_coll:b_r_index3"))
        {
            Pressed = true;
        }
    }


    public IEnumerator UnlockPhone()
    {
        Unlocking = true;
        float InterpolationDuration = 0.4f;

        float elapsedTime = 0;
        float ratio = elapsedTime / InterpolationDuration;

        Vector3 offset = new Vector3(0.04f, 0, 0);
        Vector3 targetPost = initPos - offset;

        while (ratio < 1f)
        {
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / InterpolationDuration;

            this.transform.localPosition = Vector3.Lerp(initPos, targetPost, ratio);
            yield return null;
        }

        transform.GetComponent<SpriteRenderer>().enabled = false;
        Material[] mats = transform.parent.GetComponent<MeshRenderer>().sharedMaterials;
        mats[1] = ScreenUnlocked;
        transform.parent.GetComponent<MeshRenderer>().sharedMaterials = mats;

        Unlocking = false;

        StartCoroutine(LockPhone());
    }

    public IEnumerator LockPhone()
    {
        yield return new WaitForSeconds(3);

        this.transform.localPosition = initPos;
        transform.GetComponent<SpriteRenderer>().enabled = true;
        Material[] mats = transform.parent.GetComponent<MeshRenderer>().sharedMaterials;
        mats[1] = ScreenLocked;
        transform.parent.GetComponent<MeshRenderer>().sharedMaterials = mats;

    }


}
