using UnityEngine;

public class ManualInputManager : MonoBehaviour
{
    private LayerMask targetLayer;

    private void Awake()
    {
        targetLayer = LayerMask.GetMask("MapTiles");
    }

    void Update()
    {
        CastRaycast();  
    }

    private void CastRaycast()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayer))
            {
                if(hit.collider.GetComponent<MapTile>() is MapTile tile)
                {
                    tile.OnRaycastHit();
                }
            }
        }
    }
}
