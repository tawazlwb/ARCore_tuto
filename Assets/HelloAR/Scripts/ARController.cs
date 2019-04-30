using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GoogleARCore;
using GoogleARCore.Examples.Common;

#if UNITY_EDITOR
using input = GoogleARCore.InstantPreviewInput;
#endif
public class ARController : MonoBehaviour
{
    // We will fill this list with the planes that ARCore detected in the current frame 
    private List<DetectedPlane> detectedPlanes = new List<DetectedPlane>();

    public GameObject GridPrefab;
    public GameObject Portal;
    public GameObject ARCamera;

    // Start is called before the first frame update
    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    // Update is called once per frame
    void Update()
    {
        // Check ARCore session status
        if(Session.Status != SessionStatus.Tracking)
        {
            return;
        }

        // The following function wil fill detectedPlanes with the planes that ARCore detected in the current frame
        Session.GetTrackables<DetectedPlane>(detectedPlanes, TrackableQueryFilter.New);

        // Instanciate a Grid for each detected plane in detectedPlanes
        for (int i=0; i< detectedPlanes.Count; i++)
        {
            GameObject grid = Instantiate(GridPrefab, Vector3.zero, Quaternion.identity, transform);
            
            // This function will set the position of the grid and and modify the verteces of the attached mesh
            grid.GetComponent<GridVisualiser>().Initialize(detectedPlanes[i]);

        }

        // Check if the user touches the screen
        Touch touch;
        if(Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }

        // Let's now check if the user touched any of the detected planes
        TrackableHit hit;
        if(Frame.Raycast(touch.position.x, touch.position.y, TrackableHitFlags.PlaneWithinPolygon, out hit)) {
            // Let's now place the portal on top of tracked plane that we touched
            // Enable the portal
            Portal.SetActive(true);

            // Create a new Anchor
            Anchor anchor = hit.Trackable.CreateAnchor(hit.Pose);

            // Set the position of the portal to be the same as the hit postion
            Portal.transform.position = hit.Pose.position;
            Portal.transform.rotation = hit.Pose.rotation;

            // We want the portal to face the portal camera
            Vector3 cameraPosition = ARCamera.transform.position;

            // The portal should only rotate around the Y axis 
            cameraPosition.y = hit.Pose.position.y;

            // Rotate the portal to face the camera
            Portal.transform.LookAt(cameraPosition, Portal.transform.up);

            // ARCore will keep understanding the world and update the anchor accordingly to hence we neeed to attach our portal to the anchor
            Portal.transform.parent = anchor.transform;
        }
    }
}
