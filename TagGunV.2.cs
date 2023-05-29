    private static LineRenderer tagGun; // Declare LineRenderer variable

    private static void ProcessTagGun()
    {
        bool trig = false;
        bool grip = false;
        List<InputDevice> list = new List<InputDevice>();
        InputDevices.GetDevices(list);
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right, list);
        list[0].TryGetFeatureValue(CommonUsages.triggerButton, out trig);
        list[0].TryGetFeatureValue(CommonUsages.gripButton, out grip);
        if (grip)
        {
            RaycastHit raycastHit;
            if (Physics.Raycast(GorillaLocomotion.Player.Instance.rightHandTransform.position - GorillaLocomotion.Player.Instance.rightHandTransform.up, -GorillaLocomotion.Player.Instance.rightHandTransform.up, out raycastHit) && tagGun == null)
            {
                // Create LineRenderer if not already created
                if (tagGun == null)
                {
                    tagGun = new GameObject("TagGun").AddComponent<LineRenderer>();
                    tagGun.material = new Material(Shader.Find("Sprites/Default"));
                    tagGun.startWidth = 0.05f;
                    tagGun.endWidth = 0.05f;
                    tagGun.startColor = Color.magenta; // Use Color.magenta for purple
                    tagGun.endColor = Color.magenta; // Use Color.magenta for purple
                }
            }
            else
            {
                // Destroy LineRenderer if no longer needed
                if (tagGun != null)
                {
                    GameObject.Destroy(tagGun.gameObject);
                    tagGun = null;
                }
            }

            if (tagGun != null)
            {
                tagGun.transform.position = raycastHit.point;
                tagGun.SetPosition(0, GorillaLocomotion.Player.Instance.rightHandTransform.position);
                tagGun.SetPosition(1, raycastHit.point);

                Photon.Realtime.Player owner = raycastHit.collider.GetComponentInParent<PhotonView>().Owner;
                if (trig)
                {
                    GorillaTagger.Instance.myVRRig.enabled = false;
                    GorillaTagger.Instance.myVRRig.transform.position = MenuPatch.FindVRRigForPlayer(owner).transform.position;
                    PhotonView.Get(GorillaGameManager.instance.GetComponent<GorillaGameManager>()).RPC("ReportTagRPC", RpcTarget.MasterClient, new object[]
                    {
                    owner
                    });
                    GorillaTagger.Instance.myVRRig.enabled = true;
                }
            }
        }
        else
        {
            GorillaTagger.Instance.myVRRig.enabled = true;
            if (tagGun != null)
            {
                GameObject.Destroy(tagGun.gameObject);
                tagGun = null;
            }
        }
    }
