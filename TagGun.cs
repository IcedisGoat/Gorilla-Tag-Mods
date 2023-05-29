private static LineRenderer pointer; // Declare LineRenderer variable

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
        if (Physics.Raycast(GorillaLocomotion.Player.Instance.rightHandTransform.position - GorillaLocomotion.Player.Instance.rightHandTransform.up, -GorillaLocomotion.Player.Instance.rightHandTransform.up, out raycastHit) && MenuPatch.pointer == null)
        {
            // Create LineRenderer if not already created
            if (pointer == null)
            {
                pointer = new GameObject("Pointer").AddComponent<LineRenderer>();
                pointer.material = new Material(Shader.Find("Sprites/Default"));
                pointer.startWidth = 0.05f;
                pointer.endWidth = 0.05f;
                pointer.startColor = Color.purple;
                pointer.endColor = Color.purple;
            }
        }
        else
        {
            // Destroy LineRenderer if no longer needed
            if (pointer != null)
            {
                GameObject.Destroy(pointer.gameObject);
                pointer = null;
            }
        }

        if (pointer != null)
        {
            pointer.transform.position = raycastHit.point;
            pointer.SetPosition(0, GorillaLocomotion.Player.Instance.rightHandTransform.position);
            pointer.SetPosition(1, raycastHit.point);

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
        if (pointer != null)
        {
            GameObject.Destroy(pointer.gameObject);
            pointer = null;
        }
    }
}
