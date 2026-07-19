using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCaster : MonoBehaviour
{
    public Camera playerCamera;
    AudioSource beamAudioSource;
    public AudioClip beamSound;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        beamAudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
           Shot();
            beamAudioSource.PlayOneShot(beamSound);
        }
    }

    void Shot()
    {
        int distance = 50;
        Vector3 center = new Vector3(Screen.width / 2,Screen.height / 2, 0);
        Ray ray = playerCamera.ScreenPointToRay(center);
        RaycastHit hitinfo;

        if(Physics.Raycast(ray,out hitinfo,distance))
        {
            if (hitinfo.collider.tag == "Enemy")
            {
                hitinfo.collider.SendMessage("Damage");
            }
        }
    }

}
